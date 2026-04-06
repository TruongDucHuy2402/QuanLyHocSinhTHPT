using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Models;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HocSinhService _hocSinhService;

        /// <summary>
        /// ✅ Fix: Sử dụng DI để inject HocSinhService thay vì tạo manual
        /// </summary>
        public DashboardController(HocSinhService hocSinhService)
        {
            _hocSinhService = hocSinhService;
        }

        private bool CheckLogin()
        {
            return HttpContext.Session.GetString("TenDangNhap") != null;
        }

        private void SetViewBagData()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro") ?? "HOCSINH";
            var tenDangNhap = HttpContext.Session.GetString("TenDangNhap");
            var danhSachQuyenStr = HttpContext.Session.GetString("DanhSachQuyen") ?? "";
            var danhSachQuyen = string.IsNullOrEmpty(danhSachQuyenStr) 
                ? new List<string>() 
                : danhSachQuyenStr.Split(",").ToList();

            ViewBag.TenDangNhap = tenDangNhap;
            ViewBag.VaiTro = vaiTro;
            ViewBag.DanhSachQuyen = danhSachQuyen;
        }

        private void LoadStatistics()
        {
            try
            {
                var danhSachHocSinh = _hocSinhService.GetDanhSachHocSinh();
                ViewBag.TongHocSinh = danhSachHocSinh.Count;
                ViewBag.SoLop = danhSachHocSinh.Select(hs => hs.TenLop).Distinct().Count();
                ViewBag.SoNam = danhSachHocSinh.Where(hs => hs.GioiTinh == "Nam").Count();
                ViewBag.SoNu = danhSachHocSinh.Where(hs => hs.GioiTinh == "Nữ").Count();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi khi tải dữ liệu: {ex.Message}";
            }
        }

        // GET: /Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            LoadStatistics();
            return View();
        }

        // GET: /Dashboard/Students
        [HttpGet]
        public IActionResult Students()
        {
            if (!CheckLogin()) return RedirectToAction("Index", "Login");
            SetViewBagData();
            try
            {
                var danhSachHocSinh = _hocSinhService.GetDanhSachHocSinh();
                return View(danhSachHocSinh);
            }
            catch (Exception ex)
            {
                return Content($"LỖI THẬT: {ex.GetType().Name}\n\n{ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // GET: /Dashboard/Classes
        [HttpGet]
        public IActionResult Classes()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            try
            {
                var danhSachLop = _hocSinhService.GetDanhSachLopHoc();
                return View(danhSachLop);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View(new List<LopHoc>());
            }
        }

        // GET: /Dashboard/AddStudent
        [HttpGet]
        public IActionResult AddStudent()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH" && 
                HttpContext.Session.GetString("VaiTro") != "GVCN"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            return View();
        }

        // GET: /Dashboard/EditStudent
        [HttpGet]
        public IActionResult EditStudent()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH" && 
                HttpContext.Session.GetString("VaiTro") != "GVCN"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            return View();
        }

        // GET: /Dashboard/Permissions
        [HttpGet]
        public IActionResult Permissions()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            return View();
        }

        // GET: /Dashboard/Reports
        [HttpGet]
        public IActionResult Reports()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            return View();
        }

        // GET: /Dashboard/Scores
        [HttpGet]
        public IActionResult Scores(int? maLop = null, int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();

            try
            {
                // Nếu không có maLop, lấy từ session người dùng hiện tại
                if (!maLop.HasValue)
                {
                    var maMaHsStr = HttpContext.Session.GetString("MaHS");
                    System.Diagnostics.Debug.WriteLine($"📌 Session MaHS: {maMaHsStr}");
                    
                    if (!string.IsNullOrEmpty(maMaHsStr) && int.TryParse(maMaHsStr, out int maHS))
                    {
                        var student = _hocSinhService.GetHocSinhById(maHS);
                        maLop = student?.MaLop ?? 1;
                        System.Diagnostics.Debug.WriteLine($"📌 Student MaHS={maHS}, MaLop={maLop}");
                    }
                    else
                    {
                        maLop = 1; // Mặc định
                        System.Diagnostics.Debug.WriteLine($"📌 No MaHS in session, using default MaLop=1");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"📌 Getting scores for: MaLop={maLop}, HocKy={hocKy}, NamHoc={namHoc}");

                // Lấy danh sách điểm trung bình theo môn
                var diemTheoMon = _hocSinhService.GetDiemTBTheoMon(maLop.Value, hocKy, namHoc);
                System.Diagnostics.Debug.WriteLine($"📌 Found {diemTheoMon.Count} subjects with scores");
                
                // Lấy danh sách học sinh để hiển thị thống kê
                var danhSachHocSinh = _hocSinhService.GetHocSinhTheoLop(maLop.Value);
                System.Diagnostics.Debug.WriteLine($"📌 Found {danhSachHocSinh.Count} students in class");

                ViewBag.DiemTheoMon = diemTheoMon;
                ViewBag.DanhSachHocSinh = danhSachHocSinh;
                ViewBag.MaLop = maLop;
                ViewBag.HocKy = hocKy;
                ViewBag.NamHoc = namHoc;
                ViewBag.TongHocSinh = danhSachHocSinh.Count;
                ViewBag.TongMon = diemTheoMon.Count;
                ViewBag.DiemTBChung = diemTheoMon.Count > 0 
                    ? diemTheoMon.Average(d => d.DiemTB_Mon ?? 0).ToString("F2") 
                    : "0.00";
                
                // Debug info for diagnosis
                ViewBag.DebugMaLop = maLop.Value;
                ViewBag.DebugHocKy = hocKy;
                ViewBag.DebugNamHoc = namHoc;

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Scores Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View();
            }
        }

        // GET: /Dashboard/ScoresBySubject
        [HttpGet]
        public IActionResult ScoresBySubject(int maMon, string loaiDiem = "", int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();

            try
            {
                // Lấy MaLop từ session hoặc mặc định
                int maLop = 1;
                var maMaHsStr = HttpContext.Session.GetString("MaHS");
                if (!string.IsNullOrEmpty(maMaHsStr) && int.TryParse(maMaHsStr, out int maHS))
                {
                    var student = _hocSinhService.GetHocSinhById(maHS);
                    maLop = student?.MaLop ?? 1;
                }

                // Lấy điểm theo môn
                var diemTheoMon = _hocSinhService.GetDiemTBTheoMon(maLop, hocKy, namHoc);
                var diemMon = diemTheoMon.FirstOrDefault(d => d.MaMon == maMon);

                ViewBag.DiemTheoMon = diemMon != null ? new List<dynamic> { diemMon } : new List<dynamic>();
                ViewBag.DanhSachMon = diemTheoMon;
                ViewBag.MaMon = maMon;
                ViewBag.LoaiDiem = loaiDiem;
                ViewBag.HocKy = hocKy;
                ViewBag.NamHoc = namHoc;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View();
            }
        }

        // GET: /Dashboard/TestScores - DEBUG ONLY
        [HttpGet]
        public IActionResult TestScores()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            var diagnostics = _hocSinhService.GetDiemDiagnostics();
            return Json(diagnostics);
        }
    }
}

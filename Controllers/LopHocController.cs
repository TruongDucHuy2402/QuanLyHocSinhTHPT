using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Models;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class LopHocController : Controller
    {
        private readonly HocSinhService _hocSinhService;

        public LopHocController(HocSinhService hocSinhService)
        {
            _hocSinhService = hocSinhService;
        }

        // ── Helper: Check Login ──────────────────────────
        private bool CheckLogin() =>
            HttpContext.Session.GetString("TenDangNhap") != null;

        private void SetViewBagUser()
        {
            ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
            ViewBag.Quyen = HttpContext.Session.GetString("Quyen");
            ViewBag.VaiTro = HttpContext.Session.GetString("VaiTro");
        }

        // GET: /LopHoc
        [HttpGet]
        public IActionResult Index()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagUser();
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

        // GET: /LopHoc/Details/{maLop}
        [HttpGet]
        public IActionResult Details(int maLop)
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagUser();
            try
            {
                var lopHoc = _hocSinhService.GetDanhSachLopHoc()
                    .FirstOrDefault(l => l.MaLop == maLop);
                
                if (lopHoc == null)
                {
                    return NotFound("Không tìm thấy lớp học");
                }

                // Lấy danh sách HS của lớp
                var danhSachHS = _hocSinhService.GetHocSinhTheoLop(maLop);
                
                ViewBag.LopHoc = lopHoc;
                ViewBag.DanhSachHS = danhSachHS;
                ViewBag.MaLop = maLop;

                return View(danhSachHS);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }

        // GET: /LopHoc/Scores/{maLop}
        [HttpGet]
        public IActionResult Scores(int maLop, int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "ADMIN" && 
                HttpContext.Session.GetString("VaiTro") != "BGH" &&
                HttpContext.Session.GetString("VaiTro") != "GVCN"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagUser();
            try
            {
                // Lấy thông tin lớp
                var lopHoc = _hocSinhService.GetDanhSachLopHoc()
                    .FirstOrDefault(l => l.MaLop == maLop);

                if (lopHoc == null)
                {
                    return NotFound("Không tìm thấy lớp học");
                }

                System.Diagnostics.Debug.WriteLine($"📌 Getting scores for class: MaLop={maLop}, HocKy={hocKy}, NamHoc={namHoc}");

                // Lấy điểm trung bình theo môn
                var diemTheoMon = _hocSinhService.GetDiemTBTheoMon(maLop, hocKy, namHoc);
                System.Diagnostics.Debug.WriteLine($"📌 Found {diemTheoMon.Count} subjects with scores");

                // Lấy danh sách HS
                var danhSachHS = _hocSinhService.GetHocSinhTheoLop(maLop);

                ViewBag.LopHoc = lopHoc;
                ViewBag.DiemTheoMon = diemTheoMon;
                ViewBag.DanhSachHS = danhSachHS;
                ViewBag.MaLop = maLop;
                ViewBag.HocKy = hocKy;
                ViewBag.NamHoc = namHoc;
                ViewBag.TongHocSinh = danhSachHS.Count;
                ViewBag.TongMon = diemTheoMon.Count;
                ViewBag.DiemTBChung = diemTheoMon.Count > 0
                    ? diemTheoMon.Average(d => d.DiemTB_Mon ?? 0).ToString("F2")
                    : "0.00";

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Scores Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View();
            }
        }

        // GET: /LopHoc/Students/{maLop}
        [HttpGet]
        public IActionResult Students(int maLop)
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagUser();
            try
            {
                var lopHoc = _hocSinhService.GetDanhSachLopHoc()
                    .FirstOrDefault(l => l.MaLop == maLop);

                if (lopHoc == null)
                {
                    return NotFound("Không tìm thấy lớp học");
                }

                var danhSachHS = _hocSinhService.GetHocSinhTheoLop(maLop);

                ViewBag.LopHoc = lopHoc;
                ViewBag.MaLop = maLop;
                ViewBag.TongHocSinh = danhSachHS.Count;

                return View(danhSachHS);
            }
            catch (Exception ex)
            {
                return Content($"Lỗi: {ex.Message}");
            }
        }

        // GET: /LopHoc/Statistics/{maLop}
        [HttpGet]
        public IActionResult Statistics(int maLop, int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagUser();
            try
            {
                var lopHoc = _hocSinhService.GetDanhSachLopHoc()
                    .FirstOrDefault(l => l.MaLop == maLop);

                if (lopHoc == null)
                {
                    return NotFound("Không tìm thấy lớp học");
                }

                var diemTheoMon = _hocSinhService.GetDiemTBTheoMon(maLop, hocKy, namHoc);
                var danhSachHS = _hocSinhService.GetHocSinhTheoLop(maLop);

                // Xử lý dữ liệu thống kê
                var statistics = new Dictionary<string, object>
                {
                    { "MaLop", maLop },
                    { "TenLop", lopHoc.TenLop ?? "" },
                    { "TongHS", danhSachHS.Count },
                    { "SoNam", lopHoc.SoNam },
                    { "SoNu", lopHoc.SoNu },
                    { "TongMon", diemTheoMon.Count },
                    { "DiemTBChung", diemTheoMon.Count > 0 ? diemTheoMon.Average(d => d.DiemTB_Mon ?? 0) : 0 },
                    { "DiemTheoMon", diemTheoMon }
                };

                return Json(statistics);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}

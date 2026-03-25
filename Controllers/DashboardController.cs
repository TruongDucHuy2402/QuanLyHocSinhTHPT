using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HocSinhService _hocSinhService;
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = configuration.GetConnectionString("OracleDB");
            _hocSinhService = new HocSinhService(connectionString);
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
        public IActionResult Scores()
        {
            if (!CheckLogin() || (HttpContext.Session.GetString("VaiTro") != "HOCSINH" && 
                HttpContext.Session.GetString("VaiTro") != "PHUHUYNH"))
            {
                return RedirectToAction("Index", "Login");
            }

            SetViewBagData();
            return View();
        }
    }
}

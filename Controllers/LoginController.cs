using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Models;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;

        public LoginController(string connectionString)
        {
            _userService = new UserService(connectionString);
        }

        // GET: /Login
        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra xem user đã đăng nhập chưa
            if (HttpContext.Session.GetString("TenDangNhap") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: /Login/DangNhap
        [HttpPost]
        public IActionResult DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu";
                return View("Index");
            }

            var user = _userService.Login(tenDangNhap, matKhau);
            if (user != null)
            {
                // Lưu session
                HttpContext.Session.SetString("MaND", user.MaND);
                HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
                HttpContext.Session.SetString("VaiTro", user.VaiTro);
                HttpContext.Session.SetString("Email", user.Email);
                
                // Lưu danh sách quyền (dạng chuỗi, ngăn cách bởi dấu phẩy)
                string danhSachQuyenJson = string.Join(",", user.DanhSachQuyen);
                HttpContext.Session.SetString("DanhSachQuyen", danhSachQuyenJson);

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác";
            return View("Index");
        }

        // GET: /Login/DangXuat
        [HttpGet]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}

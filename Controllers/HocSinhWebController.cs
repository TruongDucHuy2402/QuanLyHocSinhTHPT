using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class HocSinhWebController : Controller
    {
        private readonly HocSinhService _service;

        public HocSinhWebController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OracleDB");
            _service = new HocSinhService(connectionString);
        }

        private bool CheckLogin()
        {
            return HttpContext.Session.GetString("TenDangNhap") != null;
        }

        // GET: /HocSinhWeb
        public IActionResult Index()
        {
            if (!CheckLogin())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var danhSachHocSinh = _service.GetDanhSachHocSinh();
                ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
                ViewBag.Quyen = HttpContext.Session.GetString("Quyen");
                return View(danhSachHocSinh);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View(new List<QuanLyHocSinhTHPT.Models.HocSinh>());
            }
        }

        // GET: /HocSinhWeb/GetStudentsList - Return HTML table for dashboard
        public IActionResult GetStudentsList()
        {
            if (!CheckLogin())
            {
                return Unauthorized();
            }

            try
            {
                var danhSachHocSinh = _service.GetDanhSachHocSinh();
                
                var html = @"<div class=""table-container"">
                    <table>
                        <thead>
                            <tr>
                                <th>Mã HS</th>
                                <th>Họ Tên</th>
                                <th>Lớp</th>
                                <th>Giới Tính</th>
                                <th>Năm Sinh</th>
                                <th>Hành Vi</th>
                            </tr>
                        </thead>
                        <tbody>";

                foreach (var hs in danhSachHocSinh)
                {
                    var gioiTinhText = hs.GioiTinh == "N" ? "Nam" : "Nữ";
                    var hanhKiemClass = hs.HanhKiem == "T" ? "badge-good" : hs.HanhKiem == "K" ? "badge-notice" : "badge-bad";
                    var hanhKiemText = hs.HanhKiem == "T" ? "Tốt" : hs.HanhKiem == "K" ? "Khá" : "Chưa Đạt";
                    var namSinh = hs.NgaySinh.HasValue ? hs.NgaySinh.Value.Year.ToString() : "N/A";

                    html += $@"
                        <tr>
                            <td>{hs.MaHS}</td>
                            <td>{hs.HoTen}</td>
                            <td>{hs.TenLop}</td>
                            <td>{gioiTinhText}</td>
                            <td>{namSinh}</td>
                            <td><span class=""badge {hanhKiemClass}"">{hanhKiemText}</span></td>
                        </tr>";
                }

                html += @"
                        </tbody>
                    </table>
                </div>";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<p style='color: #d32f2f; text-align: center;'>Lỗi: {ex.Message}</p>", "text/html");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Models;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    public class HocSinhWebController : Controller
    {
        private readonly HocSinhService _service;

        // ✅ Sửa: inject HocSinhService thay vì tạo thủ công
        public HocSinhWebController(HocSinhService service)
        {
            _service = service;
        }

        // ── Helper kiểm tra đăng nhập ───────────────────
        private bool CheckLogin() =>
            HttpContext.Session.GetString("TenDangNhap") != null;

        private void SetViewBagUser()
        {
            ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
            ViewBag.Quyen = HttpContext.Session.GetString("Quyen");
        }

        // ════════════════════════════════════════════════
        // CODE CŨ — GIỮ NGUYÊN
        // ════════════════════════════════════════════════

        // GET: /HocSinhWeb
        public IActionResult Index()
        {
            if (!CheckLogin())
                return RedirectToAction("Index", "Login");

            try
            {
                SetViewBagUser();
                var danhSachHocSinh = _service.GetDanhSachHocSinh();
                return View(danhSachHocSinh);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View(new List<HocSinh>());
            }
        }

        // GET: /HocSinhWeb/GetStudentsList
        public IActionResult GetStudentsList()
        {
            if (!CheckLogin())
                return Unauthorized();

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
                                <th>Hạnh Kiểm</th>
                            </tr>
                        </thead>
                        <tbody>";

                foreach (var hs in danhSachHocSinh)
                {
                    var gioiTinhText   = hs.GioiTinh == "N" ? "Nam" : "Nữ";
                    var hanhKiemClass  = hs.HanhKiem == "Tốt"  ? "badge-good"
                                      : hs.HanhKiem == "Khá"  ? "badge-notice"
                                      : "badge-bad";
                    var hanhKiemText   = hs.HanhKiem ?? "Chưa xếp loại";
                    var namSinh        = hs.NgaySinh.HasValue
                                      ? hs.NgaySinh.Value.Year.ToString()
                                      : "N/A";

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

                html += @"</tbody></table></div>";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return Content(
                    $"<p style='color:#d32f2f;text-align:center'>Lỗi: {ex.Message}</p>",
                    "text/html");
            }
        }

        // ════════════════════════════════════════════════
        // CODE MỚI — Sprint 1
        // ════════════════════════════════════════════════

        // GET: /HocSinhWeb/BangDiem?maLop=101&maMon=1&hocKy=1&namHoc=2024-2025
        public IActionResult BangDiem(int maLop = 101, int maMon = 1,
                                      int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin())
                return RedirectToAction("Index", "Login");

            try
            {
                SetViewBagUser();
                ViewBag.MaLop  = maLop;
                ViewBag.MaMon  = maMon;
                ViewBag.HocKy  = hocKy;
                ViewBag.NamHoc = namHoc;

                var data = _service.GetBangDiemLop(maLop, maMon, hocKy, namHoc);
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View(new List<BangDiem>());
            }
        }

        // GET: /HocSinhWeb/TongHop?tenLop=10A1&hocKy=1&namHoc=2024-2025
        public IActionResult TongHop(string tenLop = "10A1",
                                     int hocKy = 1, string namHoc = "2024-2025")
        {
            if (!CheckLogin())
                return RedirectToAction("Index", "Login");

            try
            {
                SetViewBagUser();
                ViewBag.TenLop = tenLop;
                ViewBag.HocKy  = hocKy;
                ViewBag.NamHoc = namHoc;

                var data = _service.GetTongHopHocKy(tenLop, hocKy, namHoc);
                return View(data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Có lỗi: {ex.Message}";
                return View(new List<DiemTBResult>());
            }
        }

        // GET: /HocSinhWeb/NhapDiem
        public IActionResult NhapDiem()
        {
            if (!CheckLogin())
                return RedirectToAction("Index", "Login");

            SetViewBagUser();
            return View(new NhapDiemRequest());
        }

        // POST: /HocSinhWeb/NhapDiem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NhapDiem(NhapDiemRequest req)
        {
            if (!CheckLogin())
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                SetViewBagUser();
                TempData["Error"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại";
                return View(req);
            }

            var (success, message) = _service.NhapDiem(req);

            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;

            return RedirectToAction("NhapDiem");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using QuanLyHocSinhTHPT.Models;
using QuanLyHocSinhTHPT.Services;

namespace QuanLyHocSinhTHPT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HocSinhController : ControllerBase
    {
        private readonly HocSinhService _service;

        public HocSinhController(HocSinhService service)
        {
            _service = service;
        }

        // ════════════════════════════════════════════════
        // CODE CŨ — GIỮ NGUYÊN
        // ════════════════════════════════════════════════

        // GET /api/hocsinh
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var data = _service.GetDanhSachHocSinh();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/hocsinh/lop/101
        [HttpGet("lop/{maLop}")]
        public IActionResult GetByLop(int maLop)
        {
            try
            {
                var data = _service.GetHocSinhTheoLop(maLop);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/hocsinh/1
        [HttpGet("{maHS}")]
        public IActionResult GetById(int maHS)
        {
            try
            {
                var data = _service.GetHocSinhById(maHS);
                if (data == null)
                    return NotFound(new { message = "Không tìm thấy học sinh" });
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ════════════════════════════════════════════════
        // CODE MỚI — Sprint 1
        // ════════════════════════════════════════════════

        // GET /api/hocsinh/diemtbmon?maHS=1&maMon=1&hocKy=1&namHoc=2024-2025
        [HttpGet("diemtbmon")]
        public IActionResult GetDiemTBMon(int maHS, int maMon, int hocKy, string namHoc = "2024-2025")
        {
            try
            {
                var diemTB = _service.TinhDiemTBMon(maHS, maMon, hocKy, namHoc);
                return Ok(new { maHS, maMon, hocKy, namHoc, diemTB });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/hocsinh/diemtbhk?maHS=1&hocKy=1&namHoc=2024-2025
        [HttpGet("diemtbhk")]
        public IActionResult GetDiemTBHK(int maHS, int hocKy, string namHoc = "2024-2025")
        {
            try
            {
                var diemTB  = _service.TinhDiemTBHK(maHS, hocKy, namHoc);
                var xepLoai = diemTB.HasValue
                    ? _service.XepLoaiHocLuc(diemTB.Value)
                    : "Chưa xếp loại";
                return Ok(new { maHS, hocKy, namHoc, diemTB, xepLoai });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/hocsinh/bangdiem?maLop=101&maMon=1&hocKy=1&namHoc=2024-2025
        [HttpGet("bangdiem")]
        public IActionResult GetBangDiem(int maLop, int maMon,
                                         int hocKy, string namHoc = "2024-2025")
        {
            try
            {
                var data = _service.GetBangDiemLop(maLop, maMon, hocKy, namHoc);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/hocsinh/tonghop?tenLop=10A1&hocKy=1&namHoc=2024-2025
        [HttpGet("tonghop")]
        public IActionResult GetTongHop(string tenLop, int hocKy,
                                        string namHoc = "2024-2025")
        {
            try
            {
                var data = _service.GetTongHopHocKy(tenLop, hocKy, namHoc);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/hocsinh/nhapdiem
        // Body JSON: { "maGV":1, "maHS":1, "maMon":1, "maLD":1,
        //              "hocKy":1, "namHoc":"2024-2025", "soDiem":9.5 }
        [HttpPost("nhapdiem")]
        public IActionResult NhapDiem([FromBody] NhapDiemRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var (success, message) = _service.NhapDiem(req);
                if (success)
                    return Ok(new { success = true, message });
                else
                    return BadRequest(new { success = false, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
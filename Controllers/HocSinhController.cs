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

        public HocSinhController(HocSinhService service)  // ✅ thay string bằng HocSinhService
        {
            _service = service;
        }

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
    }
}
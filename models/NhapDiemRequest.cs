using System.ComponentModel.DataAnnotations;

namespace QuanLyHocSinhTHPT.Models
{
    public class NhapDiemRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã giáo viên phải lớn hơn 0")]
        public int MaGV { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã học sinh phải lớn hơn 0")]
        public int MaHS { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã môn phải lớn hơn 0")]
        public int MaMon { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã loại điểm phải lớn hơn 0")]
        public int MaLD { get; set; }

        [Range(1, 2, ErrorMessage = "Học kỳ chỉ được là 1 hoặc 2")]
        public int HocKy { get; set; }

        [Required(ErrorMessage = "Năm học không được để trống")]
        public string NamHoc { get; set; } = "2024-2025";

        [Range(0, 10, ErrorMessage = "Số điểm phải từ 0 đến 10")]
        public float SoDiem { get; set; }
    }
}
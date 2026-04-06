namespace QuanLyHocSinhTHPT.Models
{
    public class NhapDiemRequest
    {
        public int MaGV { get; set; }
        public int MaHS { get; set; }
        public int MaMon { get; set; }
        public int MaLD { get; set; }
        public int HocKy { get; set; }
        public string NamHoc { get; set; } = "2024-2025";
        public float SoDiem { get; set; }
    }
}
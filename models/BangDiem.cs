namespace QuanLyHocSinhTHPT.Models
{
    public class BangDiem
    {
        public int MaHS { get; set; }
        public string? TenHocSinh { get; set; }
        public string? TenLop { get; set; }
        public string? TenMon { get; set; }
        public string? LoaiDiem { get; set; }
        public float? HeSo { get; set; }
        public int HocKy { get; set; }
        public string NamHoc { get; set; } = "";
        public float SoDiem { get; set; }
        public float? DiemTB_Mon { get; set; }
    }
}
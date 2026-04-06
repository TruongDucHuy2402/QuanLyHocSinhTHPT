namespace QuanLyHocSinhTHPT.Models
{
    public class DiemMonTBResult
    {
        public int MaMon { get; set; }
        public string? TenMon { get; set; }
        public float? DiemTB_Mon { get; set; }
        public string? XepLoai { get; set; }  // Giỏi, Khá, Trung Bình, Yếu
        public int SoHocSinhDat { get; set; }
        public int TongHocSinh { get; set; }
        public float TiLeDat => TongHocSinh > 0 ? (SoHocSinhDat * 100f / TongHocSinh) : 0;
    }
}

namespace QuanLyHocSinhTHPT.Models
{
    public class DiemChiTietMonResult
    {
        public int MaHS { get; set; }
        public string HoTenHocSinh { get; set; } = "";
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";

        public float? Diem15Phut { get; set; }
        public float? Diem1Tiet { get; set; }
        public float? DiemHocKy { get; set; }
        public float? DiemTBMon { get; set; }
    }
}
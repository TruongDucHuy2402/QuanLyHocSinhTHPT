namespace QuanLyHocSinhTHPT.Models
{
    public class User
    {
        public string MaND { get; set; } // VARCHAR2(50)
        public string TenDangNhap { get; set; } // VARCHAR2(100)
        public string MatKhau { get; set; } // VARCHAR2(255)
        public string Email { get; set; } // VARCHAR2(100)
        public string VaiTro { get; set; } // VARCHAR2(50) - ADMIN, BGH, GVCN, GVBOMON, HOCSINH, PHUHUYNH
        public int? MaGV { get; set; } // Foreign Key to GIAO_VIEN
        public int? MaHS { get; set; } // Foreign Key to HOC_SINH
        public List<string> DanhSachQuyen { get; set; } = new List<string>(); // List quyền từ PHAN_QUYEN_NGUOI_DUNG
    }
}

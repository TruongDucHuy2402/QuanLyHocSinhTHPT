using Oracle.ManagedDataAccess.Client;
using QuanLyHocSinhTHPT.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyHocSinhTHPT.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Kiểm tra mật khẩu (hỗ trợ cả plain text và hash)
        private bool VerifyPassword(string password, string storedPassword)
        {
            // Trường hợp 1: Mật khẩu trong DB là plain text
            if (password.Equals(storedPassword))
                return true;

            // Trường hợp 2: Mật khẩu trong DB đã được mã hóa
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(storedPassword);
        }

        // Lấy danh sách quyền của người dùng
        private List<string> GetQuyenList(string maNguoiDung)
        {
            var quyenList = new List<string>();
            try
            {
                using var conn = new OracleConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT qh.MaQuyen, qh.TenQuyen
                    FROM PHAN_QUYEN_NGUOI_DUNG pqnd
                    JOIN QUYEN_HAN qh ON pqnd.MaQuyen = qh.MaQuyen
                    WHERE pqnd.MaND = :maNguoiDung";

                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("maNguoiDung", OracleDbType.Varchar2).Value = maNguoiDung;

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    quyenList.Add(reader["MaQuyen"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy quyền: {ex.Message}");
            }
            return quyenList;
        }

        // Đăng nhập
        public User? Login(string tenDangNhap, string matKhau)
        {
            try
            {
                using var conn = new OracleConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT MaND, TenDangNhap, MatKhau, Email, VaiTro, MaGV_ND, MaHS_ND
                    FROM NGUOI_DUNG
                    WHERE TenDangNhap = :tenDangNhap";

                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("tenDangNhap", OracleDbType.Varchar2).Value = tenDangNhap;

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var matKhauLuu = reader["MatKhau"].ToString();
                    if (VerifyPassword(matKhau, matKhauLuu))
                    {
                        var maNguoiDung = reader["MaND"].ToString();
                        return new User
                        {
                            MaND = maNguoiDung,
                            TenDangNhap = reader["TenDangNhap"].ToString(),
                            MatKhau = matKhauLuu,
                            Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                            VaiTro = reader["VaiTro"].ToString(),
                            MaGV = reader["MaGV_ND"] == DBNull.Value ? null : Convert.ToInt32(reader["MaGV_ND"]),
                            MaHS = reader["MaHS_ND"] == DBNull.Value ? null : Convert.ToInt32(reader["MaHS_ND"]),
                            DanhSachQuyen = GetQuyenList(maNguoiDung)
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng nhập: {ex.Message}");
                return null;
            }
        }

        // Lấy user theo MaND
        public User? GetUserById(string maNguoiDung)
        {
            try
            {
                using var conn = new OracleConnection(_connectionString);
                conn.Open();

                string sql = @"
                    SELECT MaND, TenDangNhap, MatKhau, Email, VaiTro, MaGV_ND, MaHS_ND
                    FROM NGUOI_DUNG
                    WHERE MaND = :maNguoiDung";

                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("maNguoiDung", OracleDbType.Varchar2).Value = maNguoiDung;

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        MaND = maNguoiDung,
                        TenDangNhap = reader["TenDangNhap"].ToString(),
                        MatKhau = reader["MatKhau"].ToString(),
                        Email = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString(),
                        VaiTro = reader["VaiTro"].ToString(),
                        MaGV = reader["MaGV_ND"] == DBNull.Value ? null : Convert.ToInt32(reader["MaGV_ND"]),
                        MaHS = reader["MaHS_ND"] == DBNull.Value ? null : Convert.ToInt32(reader["MaHS_ND"]),
                        DanhSachQuyen = GetQuyenList(maNguoiDung)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lấy user: {ex.Message}");
                return null;
            }
        }
    }
}

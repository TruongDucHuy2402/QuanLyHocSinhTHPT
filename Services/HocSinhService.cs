using Oracle.ManagedDataAccess.Client;
using QuanLyHocSinhTHPT.Models;

namespace QuanLyHocSinhTHPT.Services
{
    public class HocSinhService
    {
        private readonly string _connectionString;

        public HocSinhService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Lấy tất cả học sinh
        public List<HocSinh> GetDanhSachHocSinh()
        {
            var result = new List<HocSinh>();

            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT hs.MaHS, hs.HoTen, hs.NgaySinh, hs.GioiTinh,
                       hs.DiaChi, hs.SDT, hs.MaLop, lh.TenLop,
                       hs.DiemTB, hs.HanhKiem
                FROM HOC_SINH hs
                JOIN LOP_HOC lh ON hs.MaLop = lh.MaLop
                ORDER BY lh.TenLop, hs.HoTen";

            using var cmd = new OracleCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new HocSinh
                {
                    MaHS     = Convert.ToInt32(reader["MaHS"]),
                    HoTen    = reader["HoTen"].ToString(),
                    NgaySinh = reader["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(reader["NgaySinh"]),
                    GioiTinh = reader["GioiTinh"].ToString(),
                    DiaChi   = reader["DiaChi"].ToString(),
                    SDT      = reader["SDT"] == DBNull.Value ? null : Convert.ToInt64(reader["SDT"]),
                    MaLop    = Convert.ToInt32(reader["MaLop"]),
                    TenLop   = reader["TenLop"].ToString(),
                    DiemTB   = reader["DiemTB"] == DBNull.Value ? null : Convert.ToDouble(reader["DiemTB"]),
                    HanhKiem = reader["HanhKiem"].ToString(),
                });
            }
            return result;
        }

        // Lấy học sinh theo lớp
        public List<HocSinh> GetHocSinhTheoLop(int maLop)
        {
            var result = new List<HocSinh>();

            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT hs.MaHS, hs.HoTen, hs.NgaySinh, hs.GioiTinh,
                       hs.DiaChi, hs.SDT, hs.MaLop, lh.TenLop,
                       hs.DiemTB, hs.HanhKiem
                FROM HOC_SINH hs
                JOIN LOP_HOC lh ON hs.MaLop = lh.MaLop
                WHERE hs.MaLop = :maLop
                ORDER BY hs.HoTen";

            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("maLop", OracleDbType.Int32).Value = maLop;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new HocSinh
                {
                    MaHS     = Convert.ToInt32(reader["MaHS"]),
                    HoTen    = reader["HoTen"].ToString(),
                    NgaySinh = reader["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(reader["NgaySinh"]),
                    GioiTinh = reader["GioiTinh"].ToString(),
                    DiaChi   = reader["DiaChi"].ToString(),
                    SDT      = reader["SDT"] == DBNull.Value ? null : Convert.ToInt64(reader["SDT"]),
                    MaLop    = Convert.ToInt32(reader["MaLop"]),
                    TenLop   = reader["TenLop"].ToString(),
                    DiemTB   = reader["DiemTB"] == DBNull.Value ? null : Convert.ToDouble(reader["DiemTB"]),
                    HanhKiem = reader["HanhKiem"] == DBNull.Value ? null : reader["HanhKiem"].ToString(),
// Hiện tại thiếu null check cho HanhKiem — nếu DB trả về DBNull mà gọi .ToString() thì OK
// nhưng nếu Oracle trả về kiểu khác thì có thể crash
                });
            }
            return result;
        }

        // Lấy 1 học sinh theo mã
        public HocSinh? GetHocSinhById(int maHS)
        {
            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT hs.MaHS, hs.HoTen, hs.NgaySinh, hs.GioiTinh,
                       hs.DiaChi, hs.SDT, hs.MaLop, lh.TenLop,
                       hs.DiemTB, hs.HanhKiem
                FROM HOC_SINH hs
                JOIN LOP_HOC lh ON hs.MaLop = lh.MaLop
                WHERE hs.MaHS = :maHS";

            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("maHS", OracleDbType.Int32).Value = maHS;

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new HocSinh
                {
                    MaHS     = Convert.ToInt32(reader["MaHS"]),
                    HoTen    = reader["HoTen"].ToString(),
                    NgaySinh = reader["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(reader["NgaySinh"]),
                    GioiTinh = reader["GioiTinh"].ToString(),
                    DiaChi   = reader["DiaChi"].ToString(),
                    SDT      = reader["SDT"] == DBNull.Value ? null : Convert.ToInt64(reader["SDT"]),
                    MaLop    = Convert.ToInt32(reader["MaLop"]),
                    TenLop   = reader["TenLop"].ToString(),
                    DiemTB   = reader["DiemTB"] == DBNull.Value ? null : Convert.ToDouble(reader["DiemTB"]),
                    HanhKiem = reader["HanhKiem"] == DBNull.Value ? null : reader["HanhKiem"].ToString(),
// Hiện tại thiếu null check cho HanhKiem — nếu DB trả về DBNull mà gọi .ToString() thì OK
// nhưng nếu Oracle trả về kiểu khác thì có thể crash
                };
            }
            return null;
        }
    }
}
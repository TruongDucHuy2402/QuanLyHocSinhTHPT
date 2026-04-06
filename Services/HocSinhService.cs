using Oracle.ManagedDataAccess.Client;
using QuanLyHocSinhTHPT.Models;
using Microsoft.Extensions.Configuration;

namespace QuanLyHocSinhTHPT.Services
{
    public class HocSinhService
    {
        private readonly string _connectionString;

        // ── Sửa constructor để nhận IConfiguration ──────
        public HocSinhService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb")!;
        }

        // ════════════════════════════════════════════════
        // CODE CŨ — GIỮ NGUYÊN, chỉ fix null check
        // ════════════════════════════════════════════════

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
                result.Add(MapHocSinh(reader));
            }
            return result;
        }

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
                result.Add(MapHocSinh(reader));
            }
            return result;
        }

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
                return MapHocSinh(reader);
            }
            return null;
        }

        // Helper tránh lặp code mapping ─────────────────
        private static HocSinh MapHocSinh(OracleDataReader r) => new()
        {
            MaHS     = Convert.ToInt32(r["MaHS"]),
            HoTen    = r["HoTen"]?.ToString() ?? "",
            NgaySinh = r["NgaySinh"] == DBNull.Value ? null : Convert.ToDateTime(r["NgaySinh"]),
            GioiTinh = r["GioiTinh"]?.ToString() ?? "",
            DiaChi   = r["DiaChi"]?.ToString() ?? "",
            SDT      = r["SDT"] == DBNull.Value ? null : Convert.ToInt64(r["SDT"]),
            MaLop    = r["MaLop"] == DBNull.Value ? null : Convert.ToInt32(r["MaLop"]),
            TenLop   = r["TenLop"]?.ToString() ?? "",
            DiemTB   = r["DiemTB"] == DBNull.Value ? null : Convert.ToDouble(r["DiemTB"]),
            HanhKiem = r["HanhKiem"] == DBNull.Value ? "" : r["HanhKiem"]?.ToString() ?? ""
        };

        // ════════════════════════════════════════════════
        // CODE MỚI — Sprint 1
        // ════════════════════════════════════════════════

        // ── 1. FN_TINH_DIEM_TB_MON ──────────────────────
        public float? TinhDiemTBMon(int maHS, int maMon, int hocKy, string namHoc)
        {
            using var conn = new OracleConnection(_connectionString);
            conn.Open();
            using var cmd = new OracleCommand(
                "SELECT FN_TINH_DIEM_TB_MON(:maHS,:maMon,:hocKy,:namHoc) FROM DUAL", conn);

            cmd.Parameters.Add("maHS",   OracleDbType.Int32).Value    = maHS;
            cmd.Parameters.Add("maMon",  OracleDbType.Int32).Value    = maMon;
            cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
            cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

            var result = cmd.ExecuteScalar();
            return result == DBNull.Value || result == null
                ? null
                : Convert.ToSingle(result);
        }

        // ── 2. FN_TINH_DIEM_TB_HK ───────────────────────
        public float? TinhDiemTBHK(int maHS, int hocKy, string namHoc)
        {
            using var conn = new OracleConnection(_connectionString);
            conn.Open();
            using var cmd = new OracleCommand(
                "SELECT FN_TINH_DIEM_TB_HK(:maHS,:hocKy,:namHoc) FROM DUAL", conn);

            cmd.Parameters.Add("maHS",   OracleDbType.Int32).Value    = maHS;
            cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
            cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

            var result = cmd.ExecuteScalar();
            return result == DBNull.Value || result == null
                ? null
                : Convert.ToSingle(result);
        }

        // ── 3. FN_XEP_LOAI_HOC_LUC ──────────────────────
        public string XepLoaiHocLuc(float diemTB)
        {
            using var conn = new OracleConnection(_connectionString);
            conn.Open();
            using var cmd = new OracleCommand(
                "SELECT FN_XEP_LOAI_HOC_LUC(:diem) FROM DUAL", conn);

            cmd.Parameters.Add("diem", OracleDbType.BinaryFloat).Value = diemTB;

            return cmd.ExecuteScalar()?.ToString() ?? "Chưa xếp loại";
        }

        // ── 4. V_BANG_DIEM_LOP ──────────────────────────
        public List<BangDiem> GetBangDiemLop(int maLop, int maMon, int hocKy, string namHoc)
        {
            var list = new List<BangDiem>();
            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT MaHS, TenHocSinh, TenLop, TenMon,
                       LoaiDiem, HeSo, HocKy, NamHoc, SoDiem, DiemTB_Mon
                FROM V_BANG_DIEM_LOP
                WHERE MaLop  = :maLop
                  AND MaMon  = :maMon
                  AND HocKy  = :hocKy
                  AND NamHoc = :namHoc
                ORDER BY TenHocSinh, HeSo";

            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("maLop",  OracleDbType.Int32).Value    = maLop;
            cmd.Parameters.Add("maMon",  OracleDbType.Int32).Value    = maMon;
            cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
            cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BangDiem
                {
                    MaHS       = Convert.ToInt32(reader["MaHS"]),
                    TenHocSinh = reader["TenHocSinh"]?.ToString(),
                    TenLop     = reader["TenLop"]?.ToString(),
                    TenMon     = reader["TenMon"]?.ToString(),
                    LoaiDiem   = reader["LoaiDiem"]?.ToString(),
                    HeSo       = reader["HeSo"] == DBNull.Value ? null : Convert.ToSingle(reader["HeSo"]),
                    HocKy      = Convert.ToInt32(reader["HocKy"]),
                    NamHoc     = reader["NamHoc"]?.ToString() ?? "",
                    SoDiem     = Convert.ToSingle(reader["SoDiem"]),
                    DiemTB_Mon = reader["DiemTB_Mon"] == DBNull.Value ? null : Convert.ToSingle(reader["DiemTB_Mon"])
                });
            }
            return list;
        }

        // ── 5. V_TONG_HOP_HOC_KY ────────────────────────
        public List<DiemTBResult> GetTongHopHocKy(string tenLop, int hocKy, string namHoc)
        {
            var list = new List<DiemTBResult>();
            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT MaHS, TenHocSinh, TenLop, HocKy, NamHoc,
                       DiemTB_HK, XepLoai_HocLuc, HanhKiem
                FROM V_TONG_HOP_HOC_KY
                WHERE TenLop = :tenLop
                  AND HocKy  = :hocKy
                  AND NamHoc = :namHoc";

            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("tenLop", OracleDbType.Varchar2).Value = tenLop;
            cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
            cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DiemTBResult
                {
                    MaHS           = Convert.ToInt32(reader["MaHS"]),
                    TenHocSinh     = reader["TenHocSinh"]?.ToString(),
                    TenLop         = reader["TenLop"]?.ToString(),
                    HocKy          = Convert.ToInt32(reader["HocKy"]),
                    NamHoc         = reader["NamHoc"]?.ToString(),
                    DiemTB_HK      = reader["DiemTB_HK"] == DBNull.Value ? null : Convert.ToSingle(reader["DiemTB_HK"]),
                    XepLoai_HocLuc = reader["XepLoai_HocLuc"]?.ToString(),
                    HanhKiem       = reader["HanhKiem"] == DBNull.Value ? null : reader["HanhKiem"]?.ToString()
                });
            }
            return list;
        }

        // ── 6. SP_NHAP_DIEM ─────────────────────────────
        public (bool success, string message) NhapDiem(NhapDiemRequest req)
        {
            try
            {
                using var conn = new OracleConnection(_connectionString);
                conn.Open();
                using var cmd = new OracleCommand("SP_NHAP_DIEM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("p_MaGV",   OracleDbType.Int32).Value       = req.MaGV;
                cmd.Parameters.Add("p_MaHS",   OracleDbType.Int32).Value       = req.MaHS;
                cmd.Parameters.Add("p_MaMon",  OracleDbType.Int32).Value       = req.MaMon;
                cmd.Parameters.Add("p_MaLD",   OracleDbType.Int32).Value       = req.MaLD;
                cmd.Parameters.Add("p_HocKy",  OracleDbType.Int32).Value       = req.HocKy;
                cmd.Parameters.Add("p_NamHoc", OracleDbType.Varchar2).Value    = req.NamHoc;
                cmd.Parameters.Add("p_SoDiem", OracleDbType.BinaryFloat).Value = req.SoDiem;

                cmd.ExecuteNonQuery();
                return (true, "Nhập điểm thành công");
            }
            catch (OracleException ex)
            {
                // Bắt lỗi RAISE_APPLICATION_ERROR từ Oracle procedure
                return (false, ex.Message);
            }
        }

        // ── 7. GET DIEM TB THEO MON (AVERAGE BY SUBJECT) ─
        public List<DiemMonTBResult> GetDiemTBTheoMon(int maLop, int hocKy, string namHoc)
        {
            var list = new List<DiemMonTBResult>();
            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            // Try query từ V_BANG_DIEM_LOP view first
            try
            {
                string sql = @"
                    SELECT 
                        MaMon,
                        TenMon,
                        ROUND(AVG(DiemTB_Mon), 2) as DiemTB_Mon,
                        COUNT(DISTINCT MaHS) as TongHocSinh,
                        COUNT(DISTINCT CASE WHEN DiemTB_Mon >= 8 THEN MaHS ELSE NULL END) as SoHocSinhDat
                    FROM V_BANG_DIEM_LOP
                    WHERE MaLop  = :maLop
                      AND HocKy  = :hocKy
                      AND NamHoc = :namHoc
                    GROUP BY MaMon, TenMon
                    ORDER BY TenMon";

                using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("maLop",  OracleDbType.Int32).Value    = maLop;
                cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
                cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var diemTB = reader["DiemTB_Mon"] == DBNull.Value ? (float?)null : (float?)Convert.ToSingle(reader["DiemTB_Mon"]);
                    var xepLoai = ClassifyScore(diemTB);

                    list.Add(new DiemMonTBResult
                    {
                        MaMon         = Convert.ToInt32(reader["MaMon"]),
                        TenMon        = reader["TenMon"]?.ToString(),
                        DiemTB_Mon    = diemTB,
                        XepLoai       = xepLoai,
                        SoHocSinhDat  = Convert.ToInt32(reader["SoHocSinhDat"]),
                        TongHocSinh   = Convert.ToInt32(reader["TongHocSinh"])
                    });
                }
                
                // Nếu lấy được dữ liệu từ view, return
                if (list.Count > 0)
                    return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ V_BANG_DIEM_LOP Error: {ex.Message}");
                // Continue to try direct query
            }

            // Fallback: Query trực tiếp từ các bảng cơ bản
            try
            {
                string sqlDirect = @"
                    WITH DiemTheoHS AS (
                        SELECT 
                            m.MaMon,
                            m.TenMon,
                            bd.MaHS,
                            ROUND(AVG(bd.SoDiem), 2) as DiemTrungBinh
                        FROM BANG_DIEM bd
                        INNER JOIN MON_HOC m ON bd.MaMon = m.MaMon
                        INNER JOIN HOC_SINH hs ON bd.MaHS = hs.MaHS
                        WHERE hs.MaLop = :maLop
                          AND bd.HocKy = :hocKy
                          AND bd.NamHoc = :namHoc
                        GROUP BY m.MaMon, m.TenMon, bd.MaHS
                    )
                    SELECT 
                        MaMon,
                        TenMon,
                        COUNT(DISTINCT MaHS) as TongHocSinh,
                        ROUND(AVG(DiemTrungBinh), 2) as DiemTB_Mon,
                        COUNT(DISTINCT CASE WHEN DiemTrungBinh >= 8 THEN MaHS ELSE NULL END) as SoHocSinhDat
                    FROM DiemTheoHS
                    GROUP BY MaMon, TenMon
                    ORDER BY TenMon";

                using var cmd = new OracleCommand(sqlDirect, conn);
                cmd.Parameters.Add("maLop",  OracleDbType.Int32).Value    = maLop;
                cmd.Parameters.Add("hocKy",  OracleDbType.Int32).Value    = hocKy;
                cmd.Parameters.Add("namHoc", OracleDbType.Varchar2).Value = namHoc;

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var diemTB = reader["DiemTB_Mon"] == DBNull.Value ? (float?)null : (float?)Convert.ToSingle(reader["DiemTB_Mon"]);
                    var xepLoai = ClassifyScore(diemTB);

                    list.Add(new DiemMonTBResult
                    {
                        MaMon         = Convert.ToInt32(reader["MaMon"]),
                        TenMon        = reader["TenMon"]?.ToString(),
                        DiemTB_Mon    = diemTB,
                        XepLoai       = xepLoai,
                        SoHocSinhDat  = reader["SoHocSinhDat"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoHocSinhDat"]),
                        TongHocSinh   = Convert.ToInt32(reader["TongHocSinh"])
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Direct Query Error: {ex.Message}");
            }

            return list;
        }

        // ── 9. GET ALL CLASSES (DANH SÁCH LỚP HỌC) ──────
        public List<LopHoc> GetDanhSachLopHoc()
        {
            var list = new List<LopHoc>();
            using var conn = new OracleConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT 
                    lh.MaLop,
                    lh.TenLop,
                    lh.Khoi,
                    COUNT(DISTINCT hs.MaHS) as SoHocSinh,
                    SUM(CASE WHEN hs.GioiTinh = 'Nam' THEN 1 ELSE 0 END) as SoNam,
                    SUM(CASE WHEN hs.GioiTinh = 'Nu' THEN 1 ELSE 0 END) as SoNu
                FROM LOP_HOC lh
                LEFT JOIN HOC_SINH hs ON lh.MaLop = hs.MaLop
                GROUP BY lh.MaLop, lh.TenLop, lh.Khoi
                ORDER BY lh.TenLop";

            using var cmd = new OracleCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new LopHoc
                {
                    MaLop = Convert.ToInt32(reader["MaLop"]),
                    TenLop = reader["TenLop"]?.ToString() ?? "",
                    Khoi = reader["Khoi"]?.ToString() ?? "",
                    SoHocSinh = reader["SoHocSinh"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoHocSinh"]),
                    SoNam = reader["SoNam"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoNam"]),
                    SoNu = reader["SoNu"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SoNu"])
                });
            }
            return list;
        }

        // ── Helper: Classify Score ──────────────────────
        private static string ClassifyScore(float? diem)
        {
            if (diem == null) return "Chưa xếp loại";
            return diem >= 8 ? "Giỏi"
                : diem >= 6.5 ? "Khá"
                : diem >= 5 ? "Trung Bình"
                : "Yếu";
        }

        // ── 8. DEBUG: Get all data from BANG_DIEM ─────────
        public Dictionary<string, object> GetDiemDiagnostics()
        {
            var result = new Dictionary<string, object>();
            
            try
            {
                using var conn = new OracleConnection(_connectionString);
                conn.Open();

                // Count total records in BANG_DIEM
                using var cmd = new OracleCommand("SELECT COUNT(*) FROM BANG_DIEM", conn);
                result["TotalBangDiem"] = cmd.ExecuteScalar();

                // Group by HocKy, NamHoc
                using var cmd2 = new OracleCommand(@"
                    SELECT HocKy, NamHoc, COUNT(*) as SoLuong 
                    FROM BANG_DIEM 
                    GROUP BY HocKy, NamHoc 
                    ORDER BY NamHoc DESC, HocKy", conn);
                
                var hkData = new List<string>();
                using var reader = cmd2.ExecuteReader();
                while (reader.Read())
                {
                    var hk = reader["HocKy"];
                    var nh = reader["NamHoc"];
                    var cnt = reader["SoLuong"];
                    hkData.Add($"HK{hk}/{nh}: {cnt} records");
                }
                result["HocKyData"] = hkData;

                // Count by class
                using var cmd3 = new OracleCommand(@"
                    SELECT hs.MaLop, lh.TenLop, COUNT(DISTINCT hs.MaHS) as SoHS
                    FROM BANG_DIEM bd
                    INNER JOIN HOC_SINH hs ON bd.MaHS = hs.MaHS
                    LEFT JOIN LOP_HOC lh ON hs.MaLop = lh.MaLop
                    GROUP BY hs.MaLop, lh.TenLop
                    ORDER BY hs.MaLop", conn);
                
                var lopData = new List<string>();
                using var reader3 = cmd3.ExecuteReader();
                while (reader3.Read())
                {
                    var maLop = reader3["MaLop"];
                    var tenLop = reader3["TenLop"] ?? "N/A";
                    var soHS = reader3["SoHS"];
                    lopData.Add($"Lớp {maLop} ({tenLop}): {soHS} HS");
                }
                result["LopData"] = lopData;
            }
            catch (Exception ex)
            {
                result["Error"] = ex.Message;
            }

            return result;
        }
    }
}
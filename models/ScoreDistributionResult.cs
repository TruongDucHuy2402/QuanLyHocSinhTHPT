namespace QuanLyHocSinhTHPT.Models
{
    public class ScoreDistributionResult
    {
        public float MaxScore { get; set; }
        public float MinScore { get; set; }
        public float AvgScore { get; set; }
        
        public int SoGioi { get; set; }      // >= 8.0
        public int SoKha { get; set; }       // 6.5 - 7.9
        public int SoTrungBinh { get; set; } // 5.0 - 6.4
        public int SoYeu { get; set; }       // < 5.0
        public int TongHocSinh { get; set; }
        
        public float TiLeGioi => TongHocSinh > 0 ? (SoGioi * 100f / TongHocSinh) : 0;
        public float TiLeKha => TongHocSinh > 0 ? (SoKha * 100f / TongHocSinh) : 0;
        public float TiLeTrungBinh => TongHocSinh > 0 ? (SoTrungBinh * 100f / TongHocSinh) : 0;
        public float TiLeYeu => TongHocSinh > 0 ? (SoYeu * 100f / TongHocSinh) : 0;
    }
}

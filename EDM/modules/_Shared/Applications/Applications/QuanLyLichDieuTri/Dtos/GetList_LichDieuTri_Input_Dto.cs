namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class GetList_LichDieuTri_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}
namespace Applications.QuanLyLichHen.Dtos
{
    public class GetList_LichHen_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}
namespace Applications.QuanLyPhieuKham.Dtos
{
    public class GetList_PhieuKham_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}
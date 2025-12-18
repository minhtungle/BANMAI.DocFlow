namespace Applications.QuanLyNhaCungCap.Dtos
{
    public class GetList_NhaCungCap_Input_Dto
    {
        public string Loai { get; set; } = "all";
        public LocThongTinDto LocThongTin { get; set; }
    }
}
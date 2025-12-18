using Applications.QuanLyNguoiDung.Models;
using EDM_DB;
using Public.Models;
using System.Collections.Generic;

namespace Applications.QuanLyNguoiDung.Dtos
{
    public class DisplayModal_CRUD_NguoiDung_Output_Dto
    {
        public string Loai { get; set; }
        public tbNguoiDungExtend NguoiDung { get; set; }
        public List<tbKieuNguoiDung> KieuNguoiDungs { get; set; }
        public List<tbCoCauToChuc> CoCauToChucs { get; set; }

        public List<Tree<tbCoCauToChuc>> CoCauToChucs_TREE { get; set; }
    }
}
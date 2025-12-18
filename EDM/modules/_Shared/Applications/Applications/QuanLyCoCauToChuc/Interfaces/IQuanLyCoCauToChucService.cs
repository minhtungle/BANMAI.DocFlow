using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.QuanLyCoCauToChuc.Interfaces
{
    public interface IQuanLyCoCauToChucService
    {
        Task<Tree<tbCoCauToChuc>> Get_CoCauToChucs_Tree(Guid idCoCau);
        Task XuLy_TenCoCauToChuc(
            List<Tree<tbCoCauToChuc>> coCaus_IN,
            List<tbCoCauToChuc> coCaus_OUT,
            string mucLuc = "",
            string khoangCachDauDong = "",
            bool kichHoat = true);
    }
}

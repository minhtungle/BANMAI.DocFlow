using Applications.QuanLyDonVi.Dtos;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyDonVi.Interfaces
{
    public interface IQuanLyDonViService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<List<tbDonViSuDung>> GetDonVis(
            string loai = "all",
            List<Guid> idDonVis = null,
            LocThongTinDto locThongTin = null);
        Task<bool> IsExisted_DonVi(tbDonViSuDung donVi);
        Task Create_DonVi(tbDonViSuDung donVi, HttpPostedFileBase logo);
        Task Update_DonVi(tbDonViSuDung donVi, HttpPostedFileBase logo);
        Task Delete_DonVi(List<Guid> idDonVis);

    }
}

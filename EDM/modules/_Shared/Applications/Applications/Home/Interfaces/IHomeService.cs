using Applications.Home.Dtos;
using Applications.QuanLyBenhNhan.Dtos;
using Applications.QuanLyBenhNhan.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.Home.Interfaces
{
    public interface IHomeService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_Output_Dto> Index();
        Task<tbDonViSuDung> GetDonViSuDung();
        Task<Get_ChucNangs_Output_Dto> GetDonViSuDung(Get_ChucNangs_Input_Dto input);
    }
}
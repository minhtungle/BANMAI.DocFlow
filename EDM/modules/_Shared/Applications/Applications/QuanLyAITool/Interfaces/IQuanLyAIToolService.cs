using Applications.QuanLyAITool.Dtos;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyAITool.Interfaces
{
    public interface IQuanLyAIToolService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<List<tbAITool>> GetAITools(
            string loai = "all",
            List<Guid> idAITools = null,
            LocThongTinDto locThongTin = null);
        Task<bool> IsExisted_AITool(tbAITool aiTool);
        Task Create_AITool(tbAITool aiTool);
        Task Update_AITool(tbAITool aiTool);
        Task Delete_AITool(List<Guid> idAITools);
        Task<string> WorkWithAITool(WorkWithAITool_Input_Dto input);
    }
}
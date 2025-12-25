
using Applications.QuanLyNhaCungCap.Excel.Dtos;
using Applications.QuanLyNhaCungCap.Models;
using ClosedXML.Excel;
using Public.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Excel.Interfaces {
    public interface IExcelNhaCungCapExcelService
    {
        List<tbNhaCungCapExtend> ReadImportData(HttpPostedFileBase[] files);
        Task<ValidateImportData_Output_Dto> ValidateImportData(List<tbNhaCungCapExtend> input);
        Task<XLWorkbook> GenerateErrorFile(List<tbNhaCungCapExtend> input);
        Task<XLWorkbook> GenerateTemplateFile(List<tbNhaCungCapExtend> input);
        List<Tree<tbNhaCungCapExtend>> SapXepChaCon_Tree(List<tbNhaCungCapExtend> input);
        Task LuuTreeAsync(List<Tree<tbNhaCungCapExtend>> trees);
    }
}

using Applications.QuanLyTaiKhoan.Dtos;
using Applications.QuanLyTaiKhoan.Interfaces;
using Applications.QuanLyTaiKhoan.Services;
using Applications.SocialApi.Dtos;
using EDM_DB;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyTaiKhoan.Controllers
{
    [CustomAuthorize]
    public class QuanLyTaiKhoanController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyTaiKhoan";
        public readonly IQuanLyTaiKhoanAppService _quanLyTaiKhoanAppService;
        public QuanLyTaiKhoanController(IQuanLyTaiKhoanAppService quanLyTaiKhoanAppService)
        {
            _quanLyTaiKhoanAppService = quanLyTaiKhoanAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyTaiKhoanAppService.Index_OutPut();
            return View($"{VIEW_PATH}/taikhoan.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_TaiKhoan()
        {
            var data = await _quanLyTaiKhoanAppService.GetTaiKhoans(loai: "all");
            var output = new GetList_TaiKhoan_Output_Dto
            {
                TaiKhoans = data.ToList(),
                ThaoTacs = _quanLyTaiKhoanAppService.GetThaoTacs(maChucNang: "QuanLyTaiKhoan"),
            };
            return PartialView($"{VIEW_PATH}/taikhoan-getList.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_TaiKhoan(
            DisplayModel_CRUD_TaiKhoan_Input_Dto input)
        {
            var output = await _quanLyTaiKhoanAppService.DisplayModel_CRUD_TaiKhoan_Ouput(input: input);
            
            return PartialView($"{VIEW_PATH}/taikhoan-crud/taikhoan-crud.cshtml", output);
        }
        public async Task<ActionResult> chonNenTang(Guid input)
        {
            var oupput = await _quanLyTaiKhoanAppService.ChonNenTang(
                input: input);
            return PartialView($"{VIEW_PATH}/taikhoan-crud/tab-thongtin-yeucaunentang.cshtml", oupput);
        }
        [HttpPost]
        public async Task<JsonResult> getUserAndPages(GetSocialInfo_Input_Dto input)
        {
            try
            {
                var output = await _quanLyTaiKhoanAppService.GetUserAndPages(
                    input: input);

                var html = Public.Handle.RenderViewToString(
                    controller: this,
                    viewName: $"{VIEW_PATH}/taikhoan-crud/form-userandpages.cshtml",
                    model: output);

                return Json(new
                {
                    status = "success",
                    mess = "Kết nối thành công",
                    html,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = ex.ToString(),
                    html = "",
                });
            }
        }
        [HttpPost]
        public async Task<JsonResult> getPageInfo(GetSocialInfo_Input_Dto input)
        {
            try
            {
                var output = await _quanLyTaiKhoanAppService.GetPageInfo(
                   input: input);

                var html = Public.Handle.RenderViewToString(
                    controller: this,
                    viewName: $"{VIEW_PATH}/taikhoan-crud/form-pageinfo.cshtml",
                    model: output);

                return Json(new
                {
                    status = "success",
                    mess = "Kết nối thành công",
                    html,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = ex.ToString(),
                    html = "",
                });
            }
         ;
        }
        [HttpPost]
        public async Task<ActionResult> create_TaiKhoan()
        {
            try
            {
                var taiKhoan_NEW = JsonConvert.DeserializeObject<tbTaiKhoan>(Request.Form["taiKhoan"]);
                if (taiKhoan_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isValid = await _quanLyTaiKhoanAppService.Is_ValidToSave(taiKhoan: taiKhoan_NEW);
                if (!isValid.Item1)
                    return Json(new { status = "error", mess = isValid.Item2 }, JsonRequestBehavior.AllowGet);

                taiKhoan_NEW.Name = isValid.Item3;
                await _quanLyTaiKhoanAppService.Create_TaiKhoan(taiKhoan: taiKhoan_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_TaiKhoan()
        {
            try
            {
                var taiKhoan_NEW = JsonConvert.DeserializeObject<tbTaiKhoan>(Request.Form["taiKhoan"]);
                if (taiKhoan_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isValid = await _quanLyTaiKhoanAppService.Is_ValidToSave(taiKhoan: taiKhoan_NEW);
                if (!isValid.Item1)
                    return Json(new { status = "error", mess = isValid.Item2 }, JsonRequestBehavior.AllowGet);

                taiKhoan_NEW.Name = isValid.Item3;
                await _quanLyTaiKhoanAppService.Update_TaiKhoan(taiKhoan: taiKhoan_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> delete_TaiKhoan()
        {
            try
            {
                var idTaiKhoans_NEW = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idTaiKhoans"]);
                if (idTaiKhoans_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyTaiKhoanAppService.Delete_TaiKhoan(idTaiKhoans: idTaiKhoans_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
    }
}
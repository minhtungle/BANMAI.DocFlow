using Applications.QuanLyKieuNguoiDung.Dtos;
using Applications.QuanLyKieuNguoiDung.Interfaces;
using Applications.QuanLyKieuNguoiDung.Models;
using EDM_DB;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyKieuNguoiDung.Controllers
{
    [CustomAuthorize]
    public class QuanLyKieuNguoiDungController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/QuanLyKieuNguoiDung";
        public readonly IQuanLyKieuNguoiDungService _quanLyKieuNguoiDungService;
        #endregion
        public ActionResult Index()
        {
            var output = _quanLyKieuNguoiDungService.Index_OutPut();
            return View($"{VIEW_PATH}/kieunguoidung.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> getList(LocThongTinDto input)
        {
            var kieuNguoiDungs = await _quanLyKieuNguoiDungService.Get_KieuNguoiDungs(loai: "all", locThongTin: input);
            var thaoTacs = _quanLyKieuNguoiDungService.GetThaoTacs(maChucNang: "QuanLyKieuNguoiDung");
            var output = new GetList_KieuNguoiDung_Output_Dto
            {
                KieuNguoiDungs = kieuNguoiDungs,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/kieunguoidung-getList.cshtml", output);
        }
        #region CRUD
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD(DisplayModal_CRUD_KieuNguoiDung_Input_Dto input)
        {
            var output = await _quanLyKieuNguoiDungService.DisplayModal_CRUD_KieuNguoiDung_Ouput(input: input);

            return PartialView($"{VIEW_PATH}/kieunguoidung-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_KieuNguoiDung()
        {
            try
            {
                var kieuNguoiDung_NEW = JsonConvert.DeserializeObject<tbKieuNguoiDung>(Request.Form["kieuNguoiDung"]);
                if (kieuNguoiDung_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyKieuNguoiDungService.IsExisted_KieuNguoiDung(
                    kieuNguoiDung: kieuNguoiDung_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyKieuNguoiDungService.Create_KieuNguoiDung(kieuNguoiDung: kieuNguoiDung_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
 ;
        }
        [HttpPost]
        public async Task<ActionResult> update_KieuNguoiDung()
        {
            try
            {
                var kieuNguoiDung_NEW = JsonConvert.DeserializeObject<tbKieuNguoiDung>(Request.Form["kieuNguoiDung"]);
                if (kieuNguoiDung_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyKieuNguoiDungService.IsExisted_KieuNguoiDung(
                    kieuNguoiDung: kieuNguoiDung_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyKieuNguoiDungService.Update_KieuNguoiDung(kieuNguoiDung: kieuNguoiDung_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_KieuNguoiDung()
        {
            try
            {
                var idKieuNguoiDungs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idKieuNguoiDungs"]);
                var idKieuNguoiDung_THAYTHE = Guid.Parse(Request.Form["idKieuNguoiDung_THAYTHE"]);
                if (idKieuNguoiDungs == null || idKieuNguoiDungs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyKieuNguoiDungService.Delete_KieuNguoiDungs(
                    idKieuNguoiDungs: idKieuNguoiDungs,
                    idKieuNguoiDung_THAYTHE: idKieuNguoiDung_THAYTHE);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
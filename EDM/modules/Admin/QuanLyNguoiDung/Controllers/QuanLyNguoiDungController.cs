using Applications.QuanLyNguoiDung.Models;
using Applications.QuanLyNguoiDung.Dtos;
using Applications.QuanLyNguoiDung.Interfaces;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyNguoiDung.Controllers
{

    [CustomAuthorize]
    public class QuanLyNguoiDungController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/UserAccount";
         public readonly IQuanLyNguoiDungService _quanLyNguoiDungService;
        #endregion

        public QuanLyNguoiDungController(IQuanLyNguoiDungService quanLyNguoiDungService)
        {
            _quanLyNguoiDungService = quanLyNguoiDungService;
        }

        public ActionResult Index()
        {
            var output = _quanLyNguoiDungService.Index_OutPut();
            return View($"{VIEW_PATH}/nguoidung.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> getList(LocThongTinDto input)
        {
            var nguoiDungs = await _quanLyNguoiDungService.Get_NguoiDungs(loai: "all", locThongTin: input);
            var thaoTacs = _quanLyNguoiDungService.GetThaoTacs(maChucNang: "QuanLyNguoiDung");
            var output = new GetList_NguoiDung_Output_Dto
            {
                NguoiDungs = nguoiDungs,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/nguoidung-getList.cshtml", output);
        }

        #region CRUD
        [HttpPost]
        public async Task<bool> capNhatNguoiDungHoatDong(Guid idNguoiDung)
        {
            var ouput = await _quanLyNguoiDungService.CapNhatNguoiDungHoatDong(idNguoiDung);
            return ouput;
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD(DisplayModal_CRUD_NguoiDung_Input_Dto input)
        {
            var output = await _quanLyNguoiDungService.DisplayModal_CRUD_NguoiDung(input: input);

            return PartialView($"{VIEW_PATH}/nguoidung-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_NguoiDung()
        {
            try
            {
                var nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(Request.Form["nguoiDung"]);
                if (nguoiDung_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyNguoiDungService.IsExisted_NguoiDung(
                    nguoiDung: nguoiDung_NEW.NguoiDung);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNguoiDungService.Create_NguoiDung(nguoiDung: nguoiDung_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
 ;
        }
        [HttpPost]
        public async Task<ActionResult> update_NguoiDung()
        {
            try
            {
                var nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(Request.Form["nguoiDung"]);
                if (nguoiDung_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyNguoiDungService.IsExisted_NguoiDung(
                    nguoiDung: nguoiDung_NEW.NguoiDung);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNguoiDungService.Update_NguoiDung(nguoiDung: nguoiDung_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<ActionResult> update_MatKhau()
        {
            try
            {
                var nguoiDung_NEW = JsonConvert.DeserializeObject<tbNguoiDungExtend>(Request.Form["nguoiDung"]);
                if (nguoiDung_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyNguoiDungService.Update_MatKhau(nguoiDung: nguoiDung_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_NguoiDung()
        {
            try
            {
                var idNguoiDungs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idNguoiDungs"]);
                if (idNguoiDungs == null || idNguoiDungs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyNguoiDungService.Delete_NguoiDungs(idNguoiDungs: idNguoiDungs);

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
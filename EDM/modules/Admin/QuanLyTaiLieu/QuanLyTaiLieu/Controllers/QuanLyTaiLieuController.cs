using Applications.QuanLyTaiLieu.Dtos;
using Applications.QuanLyTaiLieu.Interfaces;
using Applications.QuanLyTaiLieu.Models;
using Newtonsoft.Json;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyTaiLieu.Controllers
{
    [CustomAuthorize]
    public class QuanLyTaiLieuController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyTaiLieu";
        private readonly IQuanLyTaiLieuService _quanLyTaiLieuService;
        #endregion

        public QuanLyTaiLieuController(
            IQuanLyTaiLieuService quanLyTaiLieuService
            )
        {
            _quanLyTaiLieuService = quanLyTaiLieuService;
        }

        public async Task<ActionResult> Index(Index_Input_Dto input)
        {
            var output = await _quanLyTaiLieuService.Index(input: input);
            return View($"{VIEW_PATH}/tailieu.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> getList_TaiLieu(GetList_TaiLieu_Input_Dto input)
        {
            var taiLieus = await _quanLyTaiLieuService.Get_TaiLieus(input: input);
            var thaoTacs = _quanLyTaiLieuService.GetThaoTacs(maChucNang: "QuanLyTaiLieu");
            var output = new GetList_TaiLieu_Output_Dto
            {
                TaiLieus = taiLieus,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/tailieu-getList.cshtml", output);
        }

        #region Tài liệu
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_TaiLieu(DisplayModal_CRUD_TaiLieu_Input_Dto input)
        {
            var output = await _quanLyTaiLieuService.DisplayModal_CRUD_TaiLieu(input: input);
            return PartialView($"{VIEW_PATH}/tailieu-crud.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> create_TaiLieu(HttpPostedFileBase[] files)
        {
            try
            {
                var taiLieu_NEWs = JsonConvert.DeserializeObject<List<tbTaiLieuExtend>>(Request.Form["taiLieus"]);
                if (taiLieu_NEWs == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                //var isExisted = await _quanLyTaiLieuService.IsExisted_TaiLieu(taiLieu: taiLieu_NEWs.TaiLieu);
                //if (isExisted)
                //    return Json(new { status = "error", mess = "Tài liệu đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyTaiLieuService.Create_TaiLieu(taiLieus: taiLieu_NEWs, files: files);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> update_TaiLieu()
        {
            try
            {
                var taiLieu_NEW = JsonConvert.DeserializeObject<tbTaiLieuExtend>(Request.Form["taiLieu"]);
                if (taiLieu_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyTaiLieuService.IsExisted_TaiLieu(taiLieu_NEW.TaiLieu);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tài liệu đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyTaiLieuService.Update_TaiLieu(taiLieu: taiLieu_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> delete_TaiLieus()
        {
            try
            {
                var idTaiLieus = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idTaiLieus"]);
                if (idTaiLieus == null || idTaiLieus.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                await _quanLyTaiLieuService.Delete_TaiLieus(idTaiLieus: idTaiLieus);

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

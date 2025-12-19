using Applications.QuanLyTruongHoc.Dtos;
using Applications.QuanLyTruongHoc.Interfaces;
using Applications.QuanLyTruongHoc.Models;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyTruongHoc.Controllers
{
    [CustomAuthorize]
    public class QuanLyTruongHocController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyTruongHoc";
        public readonly IQuanLyTruongHocService _quanLyTruongHocService;
        #endregion

        public QuanLyTruongHocController(
            IQuanLyTruongHocService quanLyTruongHocService
            )
        {
            _quanLyTruongHocService = quanLyTruongHocService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyTruongHocService.Index();
            return View($"{VIEW_PATH}/truonghoc.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> getList_TruongHoc(GetList_TruongHoc_Input_Dto input)
        {
            var truongHocs = await _quanLyTruongHocService.Get_TruongHocs(input: input);
            var thaoTacs = _quanLyTruongHocService.GetThaoTacs(maChucNang: "QuanLyTruongHoc");
            var output = new GetList_TruongHoc_Output_Dto
            {
                TruongHocs = truongHocs,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/truonghoc-getList.cshtml", output);
        }

        #region Trường học
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_TruongHoc(DisplayModal_CRUD_TruongHoc_Input_Dto input)
        {
            var output = await _quanLyTruongHocService.DisplayModal_CRUD_TruongHoc(input: input);
            return PartialView($"{VIEW_PATH}/truonghoc-crud.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> create_TruongHoc()
        {
            try
            {
                var truongHoc_NEW = JsonConvert.DeserializeObject<tbTruongHocExtend>(Request.Form["truongHoc"]);
                if (truongHoc_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyTruongHocService.IsExisted_TruongHoc(
                    truongHoc: truongHoc_NEW.TruongHoc);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên trường đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyTruongHocService.Create_TruongHoc(truongHoc: truongHoc_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> update_TruongHoc()
        {
            try
            {
                var truongHoc_NEW = JsonConvert.DeserializeObject<tbTruongHocExtend>(Request.Form["truongHoc"]);
                if (truongHoc_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyTruongHocService.IsExisted_TruongHoc(
                    truongHoc: truongHoc_NEW.TruongHoc);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên trường đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyTruongHocService.Update_TruongHoc(truongHoc: truongHoc_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> delete_TruongHocs()
        {
            try
            {
                var idTruongHocs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idTruongHocs"]);
                if (idTruongHocs == null || idTruongHocs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                await _quanLyTruongHocService.Delete_TruongHocs(idTruongHocs: idTruongHocs);

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
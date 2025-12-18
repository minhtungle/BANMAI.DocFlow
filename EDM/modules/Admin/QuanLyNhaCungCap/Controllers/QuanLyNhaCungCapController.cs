using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyNhaCungCap.Controllers
{
    [CustomAuthorize]
    public class QuanLyNhaCungCapController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyNhaCungCap";
        public readonly IQuanLyNhaCungCapService _quanLyNhaCungCapService;
        #endregion

        public QuanLyNhaCungCapController(
            IQuanLyNhaCungCapService quanLyNhaCungCapService
            )
        {
            _quanLyNhaCungCapService = quanLyNhaCungCapService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyNhaCungCapService.Index();
            return View($"{VIEW_PATH}/nhacungcap.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> getList_NhaCungCap(GetList_NhaCungCap_Input_Dto input)
        {
            var nhaCungCaps = await _quanLyNhaCungCapService.Get_NhaCungCaps(input: input);
            var thaoTacs = _quanLyNhaCungCapService.GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
            var output = new GetList_NhaCungCap_Output_Dto
            {
                NhaCungCaps = nhaCungCaps,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/nhacungcap-getList.cshtml", output);
        }

        #region Nhà cung cấp
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_NhaCungCap(DisplayModal_CRUD_NhaCungCap_Input_Dto input)
        {
            var output = await _quanLyNhaCungCapService.DisplayModal_CRUD_NhaCungCap(input: input);

            return PartialView($"{VIEW_PATH}/nhacungcap-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_NhaCungCap()
        {
            try
            {
                var nhaCungCap_NEW = JsonConvert.DeserializeObject<tbNhaCungCapExtend>(Request.Form["nhaCungCap"]);
                if (nhaCungCap_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyNhaCungCapService.IsExisted_NhaCungCap(
                    nhaCungCap: nhaCungCap_NEW.NhaCungCap);
                if (isExisted)
                    return Json(new { status = "error", mess = "Mã đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNhaCungCapService.Create_NhaCungCap(nhaCungCap: nhaCungCap_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_NhaCungCap()
        {
            try
            {
                var nhaCungCap_NEW = JsonConvert.DeserializeObject<tbNhaCungCapExtend>(Request.Form["nhaCungCap"]);
                if (nhaCungCap_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyNhaCungCapService.IsExisted_NhaCungCap(
                    nhaCungCap: nhaCungCap_NEW.NhaCungCap);
                if (isExisted)
                    return Json(new { status = "error", mess = "Mã đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNhaCungCapService.Update_NhaCungCap(nhaCungCap: nhaCungCap_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_NhaCungCap()
        {
            try
            {
                var idNhaCungCaps = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idNhaCungCaps"]);
                if (idNhaCungCaps == null || idNhaCungCaps.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyNhaCungCapService.Delete_NhaCungCaps(idNhaCungCaps: idNhaCungCaps);

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
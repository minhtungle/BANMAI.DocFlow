using Applications.QuanLyLichHen.Dtos;
using Applications.QuanLyLichHen.Interfaces;
using Applications.QuanLyLichHen.Models;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyLichHen.Controllers
{
    [CustomAuthorize]
    public class QuanLyLichHenController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_QuanLyHoSoLichHen/QuanLyLichHen";
        public readonly IQuanLyLichHenService _quanLyLichHenService;
        #endregion

        public QuanLyLichHenController(IQuanLyLichHenService quanLyLichHenService)
        {
            _quanLyLichHenService = quanLyLichHenService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyLichHenService.Index();
            return View($"{VIEW_PATH}/lichhen.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> getList_LichHen(GetList_LichHen_Input_Dto input)
        {
            var lichHens = await _quanLyLichHenService.Get_LichHens(input: input);
            var thaoTacs = _quanLyLichHenService.GetThaoTacs(maChucNang: "QuanLyLichHen");
            var output = new GetList_LichHen_Output_Dto
            {
                LichHens = lichHens,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/lichhen-getList.cshtml", output);
        }

        #region CRUD
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_LichHen(DisplayModal_CRUD_LichHen_Input_Dto input)
        {
            var output = await _quanLyLichHenService.DisplayModal_CRUD_LichHen(input: input);

            return PartialView($"{VIEW_PATH}/lichhen-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_LichHen()
        {
            try
            {
                var lichHen_NEW = JsonConvert.DeserializeObject<tbLichHenExtend>(Request.Form["lichHen"]);
                if (lichHen_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyLichHenService.Create_LichHen(lichHen: lichHen_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
 ;
        }
        [HttpPost]
        public async Task<ActionResult> update_LichHen()
        {
            try
            {
                var lichHen_NEW = JsonConvert.DeserializeObject<tbLichHenExtend>(Request.Form["lichHen"]);
                if (lichHen_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyLichHenService.Update_LichHen(lichHen: lichHen_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_LichHen()
        {
            try
            {
                var idLichHens = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idLichHens"]);
                if (idLichHens == null || idLichHens.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyLichHenService.Delete_LichHens(idLichHens: idLichHens);

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
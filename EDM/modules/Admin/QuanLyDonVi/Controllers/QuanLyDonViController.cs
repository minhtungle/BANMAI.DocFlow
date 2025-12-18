using Applications.QuanLyDonVi.Dtos;
using Applications.QuanLyDonVi.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyDonVi.Controllers
{
    [CustomAuthorize]
    public class QuanLyDonViController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyDonVi";
        public readonly IQuanLyDonViService _quanLyDonViService;
        public QuanLyDonViController(IQuanLyDonViService quanLyDonViAppService)
        {
            _quanLyDonViService = quanLyDonViAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyDonViService.Index_OutPut();
            return View($"{VIEW_PATH}/donvi.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_DonVi()
        {
            var data = await _quanLyDonViService.GetDonVis(loai: "all");
            var output = new GetList_DonVi_Output_Dto
            {
                DonVis = data.ToList(),
                ThaoTacs = _quanLyDonViService.GetThaoTacs(maChucNang: "QuanLyDonVi"),
            };
            return PartialView($"{VIEW_PATH}/donvi-getList.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_DonVi(DisplayModel_CRUD_DonVi_Input_Dto input)
        {
            var donVi = await _quanLyDonViService.GetDonVis(loai: "single", idDonVis: new List<Guid> { input.IdDonVi });

            var output = new DisplayModel_CRUD_DonVi_Output_Dto
            {
                Loai = input.Loai,
                DonVi = donVi.FirstOrDefault() ?? new tbDonViSuDung(),
            };
            return PartialView($"{VIEW_PATH}/donvi-crud.cshtml", output);
        }

        [HttpPost]
        public async Task<ActionResult> create_DonVi(HttpPostedFileBase logo)
        {
            try
            {
                var donVi_NEW = JsonConvert.DeserializeObject<tbDonViSuDung>(Request.Form["donVi"]);
                if (donVi_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyDonViService.IsExisted_DonVi(donVi: donVi_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyDonViService.Create_DonVi(donVi: donVi_NEW, logo: logo);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_DonVi(HttpPostedFileBase logo)
        {
            try
            {
                var donVi_NEW = JsonConvert.DeserializeObject<tbDonViSuDung>(Request.Form["donVi"]);
                if (donVi_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyDonViService.IsExisted_DonVi(donVi: donVi_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyDonViService.Update_DonVi(donVi: donVi_NEW, logo: logo);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> delete_DonVi()
        {
            try
            {
                var idDonVis_NEW = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idDonVis"]);
                if (idDonVis_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyDonViService.Delete_DonVi(idDonVis: idDonVis_NEW);
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
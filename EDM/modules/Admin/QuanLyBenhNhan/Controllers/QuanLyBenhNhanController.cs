using Applications.QuanLyBenhNhan.Dtos;
using Applications.QuanLyBenhNhan.Interfaces;
using Applications.QuanLyBenhNhan.Models;
using Applications.QuanLyLichDieuTri.Dtos;
using Applications.QuanLyLichDieuTri.Interfaces;
using Applications.QuanLyLichDieuTri.Models;
using Applications.QuanLyLichHen.Dtos;
using Applications.QuanLyLichHen.Interfaces;
using Applications.QuanLyLichHen.Models;
using Applications.QuanLyPhieuKham.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Enums;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyBenhNhan.Controllers
{
    [CustomAuthorize]
    public class QuanLyBenhNhanController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_QuanLyHoSoBenhNhan/QuanLyBenhNhan";
        public readonly IQuanLyBenhNhanService _quanLyBenhNhanService;
        public readonly IQuanLyLichHenService _quanLyLichHenService;
        public readonly IQuanLyPhieuKhamService _quanLyPhieuKhamService;
        public readonly IQuanLyLichDieuTriService _quanLyLichDieuTriService;
        #endregion

        public QuanLyBenhNhanController(
            IQuanLyBenhNhanService quanLyBenhNhanService,
            IQuanLyLichHenService quanLyLichHenService,
            IQuanLyPhieuKhamService quanLyPhieuKhamService,
             IQuanLyLichDieuTriService quanLyLichDieuTriService
            )
        {
            _quanLyBenhNhanService = quanLyBenhNhanService;
            _quanLyLichHenService = quanLyLichHenService;
            _quanLyPhieuKhamService = quanLyPhieuKhamService;
            _quanLyLichDieuTriService = quanLyLichDieuTriService;
        }

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyBenhNhanService.Index();
            return View($"{VIEW_PATH}/benhnhan.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> getList_BenhNhan(GetList_BenhNhan_Input_Dto input)
        {
            var benhNhans = await _quanLyBenhNhanService.Get_BenhNhans(input: input);
            var thaoTacs = _quanLyBenhNhanService.GetThaoTacs(maChucNang: "QuanLyBenhNhan");
            var output = new GetList_BenhNhan_Output_Dto
            {
                BenhNhans = benhNhans,
                ThaoTacs = thaoTacs,
            };
            return PartialView($"{VIEW_PATH}/benhnhan-getList.cshtml", output);
        }

        #region Bệnh nhân
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_BenhNhan(DisplayModal_CRUD_BenhNhan_Input_Dto input)
        {
            var output = await _quanLyBenhNhanService.DisplayModal_CRUD_BenhNhan(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_BenhNhan()
        {
            try
            {
                var benhNhan_NEW = JsonConvert.DeserializeObject<tbBenhNhanExtend>(Request.Form["benhNhan"]);
                if (benhNhan_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyBenhNhanService.IsExisted_BenhNhan(
                    benhNhan: benhNhan_NEW.BenhNhan);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyBenhNhanService.Create_BenhNhan(benhNhan: benhNhan_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_BenhNhan()
        {
            try
            {
                var benhNhan_NEW = JsonConvert.DeserializeObject<tbBenhNhanExtend>(Request.Form["benhNhan"]);
                if (benhNhan_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyBenhNhanService.IsExisted_BenhNhan(
                    benhNhan: benhNhan_NEW.BenhNhan);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyBenhNhanService.Update_BenhNhan(benhNhan: benhNhan_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_BenhNhan()
        {
            try
            {
                var idBenhNhans = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idBenhNhans"]);
                if (idBenhNhans == null || idBenhNhans.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyBenhNhanService.Delete_BenhNhans(idBenhNhans: idBenhNhans);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Lịch hẹn
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_LichHen(DisplayModal_CRUD_LichHen_Input_Dto input)
        {
            var output = await _quanLyLichHenService.DisplayModal_CRUD_LichHen(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-lichhen/lichhen-crud.cshtml", output);
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

        #region Phiếu khám
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_PhieuKham(DisplayModal_CRUD_LichHen_Input_Dto input)
        {
            var output = await _quanLyLichHenService.DisplayModal_CRUD_LichHen(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-phieukham/phieukham-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_PhieuKham()
        {
            try
            {
                var phieuKham_NEW = JsonConvert.DeserializeObject<tbLichHenExtend>(Request.Form["phieuKham"]);
                if (phieuKham_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyLichHenService.Create_LichHen(lichHen: phieuKham_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_PhieuKham()
        {
            try
            {
                var phieuKham_NEW = JsonConvert.DeserializeObject<tbLichHenExtend>(Request.Form["phieuKham"]);
                if (phieuKham_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                await _quanLyLichHenService.Update_LichHen(lichHen: phieuKham_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_PhieuKham()
        {
            try
            {
                var idPhieuKhams = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idPhieuKhams"]);
                if (idPhieuKhams == null || idPhieuKhams.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyLichHenService.Delete_LichHens(idLichHens: idPhieuKhams);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Lịch điều trị
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_LichDieuTri(DisplayModal_CRUD_LichDieuTri_Input_Dto input)
        {
            var output = await _quanLyLichDieuTriService.DisplayModal_CRUD_LichDieuTri(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-phieukham/lichdieutri-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_ChonRang(DisplayModal_ChonRang_Input_Dto input)
        {
            var output = await _quanLyLichDieuTriService.DisplayModal_ChonRang(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-phieukham/tientrinhdieutri-chonrang.cshtml", output);
        }
        [HttpPost]
        public async Task<JsonResult> chonThuThuat(Guid idThuThuat)
        {
            var output = await _quanLyLichDieuTriService.ChonThuThuat(idThuThuat: idThuThuat);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> themDong_TienTrinhDieuTri(DisplayModal_CRUD_LichDieuTri_Input_Dto input)
        {
            var output = await _quanLyLichDieuTriService.DisplayModal_CRUD_LichDieuTri(input: input);

            return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-phieukham/tientrinhdieutri-themdong.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_LichDieuTri()
        {
            try
            {
                var lichDieuTri_NEW = JsonConvert.DeserializeObject<tbLichDieuTriExtend>(Request.Form["lichDieuTri"]);
                if (lichDieuTri_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyLichDieuTriService.IsExisted_LichDieuTri(
                    lichDieuTri: lichDieuTri_NEW.LichDieuTri);
                if (isExisted)
                {
                    var ngayDieuTri = lichDieuTri_NEW.LichDieuTri.NgayDieuTri == null
                        ? ClockHelper.UtcNow
                        : lichDieuTri_NEW.LichDieuTri.NgayDieuTri;
                    return Json(new
                    {
                        status = "error",
                        mess = $"Lịch điều trị ngày {DateHelper.DateToString(date: ngayDieuTri, format: "dd/MM/yyyy")} đã tồn tại"
                    }, JsonRequestBehavior.AllowGet);
                };

                await _quanLyLichDieuTriService.Create_LichDieuTri(lichDieuTri: lichDieuTri_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
         ;
        }
        [HttpPost]
        public async Task<ActionResult> update_LichDieuTri()
        {
            try
            {
                var lichDieuTri_NEW = JsonConvert.DeserializeObject<tbLichDieuTriExtend>(Request.Form["lichDieuTri"]);
                if (lichDieuTri_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyLichDieuTriService.IsExisted_LichDieuTri(lichDieuTri: lichDieuTri_NEW.LichDieuTri);
                if (isExisted)
                {
                    var ngayDieuTri = lichDieuTri_NEW.LichDieuTri.NgayDieuTri == null
                        ? ClockHelper.UtcNow
                        : lichDieuTri_NEW.LichDieuTri.NgayDieuTri;
                    return Json(new
                    {
                        status = "error",
                        mess = $"Lịch điều trị ngày {DateHelper.DateToString(date: ngayDieuTri, format: "dd/MM/yyyy")} đã tồn tại"
                    }, JsonRequestBehavior.AllowGet);
                }
                ;

                await _quanLyLichDieuTriService.Update_LichDieuTri(lichDieuTri: lichDieuTri_NEW);

                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_LichDieuTri()
        {
            try
            {
                var idLichDieuTris = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idLichDieuTris"]);
                if (idLichDieuTris == null || idLichDieuTris.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyLichDieuTriService.Delete_LichDieuTris(idLichDieuTris: idLichDieuTris);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Xem chi tiết
        public async Task<ActionResult> xemChiTiet_BenhNhan(Guid idBenhNhan)
        {
            var output = await _quanLyBenhNhanService.XemChiTiet_BenhNhan(idBenhNhan: idBenhNhan);

            return View($"{VIEW_PATH}/benhnhan-xemchitiet/benhnhan-xemchitiet.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> showTab_BenhNhan(ShowTab_BenhNhan_Input_Dto input)
        {
            if (input.TabName == "thongtincoban")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "lichhen")
            {
                var output = await _quanLyBenhNhanService.ShowTab_LichHen_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "phieukham")
            {
                var output = await _quanLyBenhNhanService.ShowTab_PhieuKham_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "lichchamsoc")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "donthuoc")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "thuvienanh")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "xuongvattu")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "thanhtoan")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "bieumau")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            if (input.TabName == "lichsuthaotac")
            {
                var output = await _quanLyBenhNhanService.ShowTab_ThongTinCoBan_BenhNhan(idBenhNhan: input.IdBenhNhan);
                return PartialView($"{VIEW_PATH}/benhnhan-xemchitiet/tab-{input.TabName}/tab-{input.TabName}.cshtml", output);
            }
            return null;
        }
        #endregion
    }
}
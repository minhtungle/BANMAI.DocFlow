using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Excel.Dtos;
using Applications.QuanLyNhaCungCap.Excel.Interfaces;
using Applications.QuanLyNhaCungCap.Excel.Services;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using Applications.QuanLyNhaCungCap.Validations;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using Newtonsoft.Json;
using Public.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyNhaCungCap.Controllers {
    [CustomAuthorize]
    public class QuanLyNhaCungCapController : Controller {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyNhaCungCap";
        public readonly IQuanLyNhaCungCapService _quanLyNhaCungCapService;
        private readonly IExcelNhaCungCapExcelService _excelNhaCungCapExcelService;

        private readonly IsExistedNhaCungCapValidation _isExistedNhaCungCapValidation;
        private readonly IsValidNhaCungCapValidation _isValidNhaCungCapValidation;
        #endregion

        public QuanLyNhaCungCapController(
            IQuanLyNhaCungCapService quanLyNhaCungCapService,
            IExcelNhaCungCapExcelService excelNhaCungCapExcelService,

            IsExistedNhaCungCapValidation isExistedNhaCungCapValidation,
            IsValidNhaCungCapValidation isValidNhaCungCapValidation
            ) {
            _quanLyNhaCungCapService = quanLyNhaCungCapService;
            _excelNhaCungCapExcelService = excelNhaCungCapExcelService;
            _isExistedNhaCungCapValidation = isExistedNhaCungCapValidation;
            _isValidNhaCungCapValidation = isValidNhaCungCapValidation;
        }

        public async Task<ActionResult> Index() {
            var output = await _quanLyNhaCungCapService.Index();
            return View($"{VIEW_PATH}/nhacungcap.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> getList_NhaCungCap(GetList_NhaCungCap_Input_Dto input) {
            var nhaCungCaps = await _quanLyNhaCungCapService.Get_NhaCungCaps(input: input);
            var thaoTacs = _quanLyNhaCungCapService.GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
            var output = new GetList_NhaCungCap_Output_Dto {
                NhaCungCaps = nhaCungCaps,
                ThaoTacs = thaoTacs,
            };

            return PartialView($"{VIEW_PATH}/nhacungcap/nhacungcap-getList.cshtml", output);
        }

        #region Nhà cung cấp
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_NhaCungCap(DisplayModal_CRUD_NhaCungCap_Input_Dto input) {
            var html = Public.Handles.Handle.RenderViewToString(
              controller: this,
              viewName: $"{VIEW_PATH}/nhacungcap/nhacungcap-crud/nhacungcap-crud.cshtml",
              model: input);
            return Json(new {
                html,
                output = input
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> addBanGhi_Modal_CRUD(AddBanGhi_Modal_CRUD_Input_Dto input) {
            var output = await _quanLyNhaCungCapService.AddBanGhi_Modal_CRUD_Output(input: input);

            output.LoaiView = "row";
            var html_nhacungcap_row = Public.Handles.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/nhacungcap/nhacungcap-crud/form-themnhacungcap.cshtml",
                model: output);

            output.LoaiView = "read";
            var html_nhacungcap_read = Public.Handles.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/nhacungcap/nhacungcap-crud/form-themnhacungcap.cshtml",
                model: output);

            return Json(new {
                status = (output.Data.NhaCungCaps != null && output.Data.NhaCungCaps.Count > 0),
                Loai = output.Data.Loai,
                html_nhacungcap_row,
                html_nhacungcap_read
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_NhaCungCap() {
            try {
                var nhaCungCap_NEWs = JsonConvert.DeserializeObject<List<tbNhaCungCapExtend>>(Request.Form["nhaCungCaps"]);
                if (nhaCungCap_NEWs == null || nhaCungCap_NEWs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                //var validationResult = await _isExistedNhaCungCapValidation.IsExisted(
                //    nhaCungCaps: nhaCungCap_NEWs);
                //if (validationResult.NhaCungCap_KhongHopLe.Count != 0)
                //    return Json(new {
                //        status = "error",
                //        mess = "Dữ liệu chưa hợp lệ, vui lòng kiểm tra lại",
                //        data = validationResult
                //    }, JsonRequestBehavior.AllowGet);

                await _quanLyNhaCungCapService.Create_NhaCungCap(nhaCungCaps: nhaCungCap_NEWs);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_NhaCungCap() {
            try {
                var nhaCungCap_NEWs = JsonConvert.DeserializeObject<List<tbNhaCungCapExtend>>(Request.Form["nhaCungCaps"]);
                if (nhaCungCap_NEWs == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                //var isExisted = await _quanLyNhaCungCapService.IsExisted_NhaCungCap(
                //    nhaCungCap: nhaCungCap_NEW.NhaCungCap);
                //if (isExisted)
                //    return Json(new { status = "error", mess = "Mã đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNhaCungCapService.Update_NhaCungCap(nhaCungCaps: nhaCungCap_NEWs);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_NhaCungCap() {
            try {
                var idNhaCungCaps = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idNhaCungCaps"]);
                if (idNhaCungCaps == null || idNhaCungCaps.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyNhaCungCapService.Delete_NhaCungCaps(idNhaCungCaps: idNhaCungCaps);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Nhà cung cấp - Excel
        [HttpPost]
        public async Task<ActionResult> importPreview_NhaCungCap_Excel() {
            try {
                if (Request.Files == null || Request.Files.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn file Excel." });

                var files = new List<HttpPostedFileBase>();
                for (int i = 0; i < Request.Files.Count; i++) {
                    var f = Request.Files[i];
                    if (f != null && f.ContentLength > 0) files.Add(f);
                }
                if (files.Count == 0)
                    return Json(new { status = "error", mess = "File Excel không hợp lệ." });

                var data = _excelNhaCungCapExcelService.ReadImportData(files.ToArray());
                var validate = await _excelNhaCungCapExcelService.ValidateImportData(data);

                // ✅ luôn lưu validate vào memory
                var downloadToken = Guid.NewGuid().ToString("N");
                MemoryCache.Default.Set(
                    key: "NCC_IMPORT_VALIDATE_" + downloadToken,
                    value: validate,
                    absoluteExpiration: DateTimeOffset.Now.AddMinutes(10)
                );

                bool hasError = validate.NhaCungCap_KhongHopLe != null && validate.NhaCungCap_KhongHopLe.Count > 0;

                if (hasError) {
                    return Json(new {
                        status = "error",
                        mess = "Dữ liệu chưa hợp lệ. Hệ thống sẽ tự tải file lỗi về để bạn chỉnh sửa.",
                        downloadToken = downloadToken
                    });
                }

                return Json(new {
                    status = "success",
                    mess = "Dữ liệu hợp lệ, vui lòng kiểm tra và bấm Lưu.",
                    downloadToken = downloadToken,
                    data = validate.NhaCungCap_HopLe
                });
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> downloadImportError_NhaCungCap_Excel(string downloadToken) {
            if (string.IsNullOrWhiteSpace(downloadToken))
                return new HttpStatusCodeResult(400, "Token không hợp lệ.");

            var key = "NCC_IMPORT_VALIDATE_" + downloadToken;
            var validate = MemoryCache.Default.Get(key) as ValidateImportData_Output_Dto;

            if (validate == null)
                return new HttpStatusCodeResult(410, "Dữ liệu đã hết hạn hoặc không tồn tại.");

            // ✅ luôn export FULL dữ liệu (hợp lệ + không hợp lệ) để user sửa
            var fullRows = validate.NhaCungCap_TongHop
                           ?? validate.NhaCungCap_KhongHopLe
                           ?? new List<tbNhaCungCapExtend>();

            var wb = await _excelNhaCungCapExcelService.GenerateErrorFile(fullRows);

            using (var ms = new MemoryStream()) {
                wb.SaveAs(ms);
                ms.Position = 0;

                downloadDialog(ms,
                    fileName: Server.UrlEncode("NhaCungCap_Import_Errors.xlsx"),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

            return new EmptyResult();
        }

        [HttpGet]
        public async Task<ActionResult> exportTemplate_NhaCungCap_Excel() {
            try {
                // 1) Gọi service tạo template (rỗng)
                var wb = await _excelNhaCungCapExcelService.GenerateTemplateFile(new List<tbNhaCungCapExtend>());

                // 2) Trả file về client để tự chọn chỗ lưu
                using (var ms = new MemoryStream()) {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    // Server.UrlEncode để tránh lỗi tên file tiếng Việt/ký tự đặc biệt
                    downloadDialog(
                        data: ms,
                        fileName: Server.UrlEncode("NhaCungCap_Template.xlsx"),
                        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    );
                }

                // Response.End() đã kết thúc request
                return new EmptyResult();
            }
            catch (Exception ex) {
                // Nếu lỗi thì trả trang/JSON tuỳ bạn
                return Content("Đã xảy ra lỗi: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult> saveImport_NhaCungCap_Excel(string downloadToken) {
            try {
                if (string.IsNullOrWhiteSpace(downloadToken))
                    return Json(new { status = "error", mess = "Token không hợp lệ." });

                var key = "NCC_IMPORT_VALIDATE_" + downloadToken;
                var validate = MemoryCache.Default.Get(key) as ValidateImportData_Output_Dto;

                if (validate == null)
                    return Json(new { status = "error", mess = "Dữ liệu import đã hết hạn. Vui lòng import lại." });

                if (validate.NhaCungCap_KhongHopLe != null && validate.NhaCungCap_KhongHopLe.Count > 0)
                    return Json(new { status = "error", mess = "Dữ liệu còn lỗi. Vui lòng sửa và import lại." });

                if (validate.NhaCungCap_HopLe == null || validate.NhaCungCap_HopLe.Count == 0)
                    return Json(new { status = "error", mess = "Không có dữ liệu hợp lệ để lưu." });

                // nếu cần sắp xếp cha-con trước khi lưu
                var ordered = _excelNhaCungCapExcelService.SapXepChaCon_Tree(validate.NhaCungCap_HopLe);
                await _excelNhaCungCapExcelService.LuuTreeAsync(ordered);

                // save xong thì xoá token
                MemoryCache.Default.Remove(key);

                return Json(new { status = "success", mess = "Lưu thành công." });
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        #endregion

        #region Private Methods
        private void downloadDialog(Stream fileStream, string fileName, string contentType) {
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            fileStream.CopyTo(Response.OutputStream);
            Response.Flush();
            Response.End();
        }

        private void downloadDialog(MemoryStream data, string fileName, string contentType) {
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            Response.BinaryWrite(data.ToArray());
            Response.Flush();
            Response.End();
        }
        private static byte[] WorkbookToBytes(XLWorkbook wb) {
            using (var ms = new MemoryStream()) {
                wb.SaveAs(ms);
                return ms.ToArray();
            }
        }
        #endregion

        #region NOT USE - Phần không sử dụng
        //[HttpPost]
        //public async Task<ActionResult> displayModal_CRUD_NhaCungCap(DisplayModal_CRUD_NhaCungCap_Input_Dto input)
        //{
        //    var output = await _quanLyNhaCungCapService.DisplayModal_CRUD_NhaCungCap(input: input);

        //    return PartialView($"{VIEW_PATH}/nhacungcap/nhacungcap-crud.cshtml", output);
        //}
        #endregion
    }
}
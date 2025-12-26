using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Excel.Interfaces;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using ClosedXML.Excel;
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

        #endregion

        public QuanLyNhaCungCapController(
            IQuanLyNhaCungCapService quanLyNhaCungCapService,
            IExcelNhaCungCapExcelService excelNhaCungCapExcelService
            ) {
            _quanLyNhaCungCapService = quanLyNhaCungCapService;
            _excelNhaCungCapExcelService = excelNhaCungCapExcelService;
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

                //var validationResult = await _quanLyNhaCungCapService.Validate_NhaCungCap(
                //    nhaCungCaps: nhaCungCap_NEWs);
                //if (validationResult.NhaCungCap_KhongHopLe.Count != 0)
                //    return Json(new
                //    {
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
                var nhaCungCap_NEW = JsonConvert.DeserializeObject<tbNhaCungCapExtend>(Request.Form["nhaCungCap"]);
                if (nhaCungCap_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                //var isExisted = await _quanLyNhaCungCapService.IsExisted_NhaCungCap(
                //    nhaCungCap: nhaCungCap_NEW.NhaCungCap);
                //if (isExisted)
                //    return Json(new { status = "error", mess = "Mã đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyNhaCungCapService.Update_NhaCungCap(nhaCungCap: nhaCungCap_NEW);
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

        #region Excel
        [HttpPost]
        public async Task<ActionResult> importPreview_NhaCungCap_Excel() {
            try {
                if (Request.Files == null || Request.Files.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn file Excel." });

                var files = new System.Collections.Generic.List<HttpPostedFileBase>();
                for (int i = 0; i < Request.Files.Count; i++) {
                    var f = Request.Files[i];
                    if (f != null && f.ContentLength > 0) files.Add(f);
                }

                var data = _excelNhaCungCapExcelService.ReadImportData(files.ToArray());
                var validate = await _excelNhaCungCapExcelService.ValidateImportData(data);

                // Có lỗi -> tạo file lỗi -> lưu tạm vào MemoryCache -> trả token
                if (validate.NhaCungCap_KhongHopLe != null && validate.NhaCungCap_KhongHopLe.Count > 0) {
                    var wb = await _excelNhaCungCapExcelService.GenerateErrorFile(validate.NhaCungCap_KhongHopLe);

                    byte[] bytes;
                    using (var ms = new MemoryStream()) {
                        wb.SaveAs(ms);
                        bytes = ms.ToArray();
                    }

                    var token = Guid.NewGuid().ToString("N");

                    // lưu 2 phút (không ghi file ra disk)
                    MemoryCache.Default.Set(
                        key: "NCC_IMPORT_ERR_" + token,
                        value: bytes,
                        absoluteExpiration: DateTimeOffset.Now.AddMinutes(2)
                    );

                    return Json(new {
                        status = "error",
                        mess = "Dữ liệu chưa hợp lệ. Hệ thống sẽ tự tải file lỗi về để bạn chỉnh sửa.",
                        downloadToken = token
                    });
                }

                // Không lỗi -> trả data preview để user xem và chọn lưu
                return Json(new {
                    status = "success",
                    mess = "Dữ liệu hợp lệ, vui lòng kiểm tra và bấm Lưu.",
                    data = validate.NhaCungCap_HopLe
                });
            }
            catch (Exception ex) {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
        [HttpGet]
        public ActionResult downloadImportError_NhaCungCap_Excel(string token) {
            if (string.IsNullOrWhiteSpace(token))
                return new HttpStatusCodeResult(400, "Token không hợp lệ.");

            var key = "NCC_IMPORT_ERR_" + token;
            var bytes = MemoryCache.Default.Get(key) as byte[];

            if (bytes == null || bytes.Length == 0)
                return new HttpStatusCodeResult(410, "File đã hết hạn hoặc không tồn tại.");

            // có thể remove luôn sau khi tải để tránh tải lại
            MemoryCache.Default.Remove(key);

            using (var ms = new MemoryStream(bytes)) {
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
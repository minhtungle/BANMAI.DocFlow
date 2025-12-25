using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Enums;
using Applications.QuanLyNhaCungCap.Excel.Dtos;
using Applications.QuanLyNhaCungCap.Excel.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using Applications.QuanLyNhaCungCap.Validations;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EDM_DB;
using Public.Dtos;
using Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Applications.QuanLyNhaCungCap.Excel.Services {
    public class ExcelNhaCungCapExcelService : IExcelNhaCungCapExcelService {
        private readonly IsExistedNhaCungCapValidation _isExistedNhaCungCapValidation;
        private readonly IsValidNhaCungCapValidation _isValidNhaCungCapValidation;

        public ExcelNhaCungCapExcelService(
            IsExistedNhaCungCapValidation isExistedNhaCungCapValidation,
            IsValidNhaCungCapValidation isValidNhaCungCapValidation
            ) {
            _isExistedNhaCungCapValidation = isExistedNhaCungCapValidation;
            _isValidNhaCungCapValidation = isValidNhaCungCapValidation;
        }
        /// <summary>
        /// Reads supplier import data from one or more uploaded Excel files and returns a list of extended supplier
        /// entities.
        /// </summary>
        /// <remarks>Only worksheets with names containing "ThongTinChung" are processed. Each row in the
        /// relevant worksheet is mapped to a supplier entity, including parent supplier information if present. The
        /// method does not validate the file format beyond the expected worksheet and column names.</remarks>
        /// <param name="files">An array of uploaded Excel files containing supplier information to import. Each file is expected to include
        /// a worksheet named "ThongTinChung" with the relevant data.</param>
        /// <returns>A list of extended supplier entities parsed from the provided Excel files. The list will be empty if no
        /// valid data is found.</returns>
        public List<tbNhaCungCapExtend> ReadImportData(HttpPostedFileBase[] files) {
            var output = new List<tbNhaCungCapExtend>();

            foreach (HttpPostedFileBase f in files) {
                #region Đọc file
                var workBook = new XLWorkbook(f.InputStream);
                var workSheets = workBook.Worksheets;
                foreach (var sheet in workSheets) {
                    // Lấy dữ liệu từ sheet chính
                    if (sheet.Name.Contains("ThongTinChung")) {
                        /**
                         * Xóa bảng đang có vì nó chiếm vùng dữ liệu nhưng không đầy đủ
                         * Bảng này chỉ chưa dữ liệu được tạo mặc định trong hàm download
                         */
                        sheet.Tables.Remove(sheet.Name.Replace(" ", String.Empty));
                        var table = sheet.RangeUsed().AsTable(); // Tạo bảng mới trên vùng dữ liệu đầy đủ
                        foreach (var row in table.DataRange.Rows()) {
                            if (!row.IsEmpty()) {
                                var nhaCungCap = new tbNhaCungCapExtend();
                                nhaCungCap.NhaCungCap.TenNhaCungCap = row.Field("Tên nhà cung cấp").GetString();
                                nhaCungCap.NhaCungCap.TenMatHang = row.Field("Tên mặt hàng").GetString();
                                nhaCungCap.NhaCungCap.SoDienThoai = row.Field("Số điện thoại").GetString();
                                nhaCungCap.NhaCungCap.Email = row.Field("Email").GetString();
                                nhaCungCap.NhaCungCap.DiaChi = row.Field("Địa chỉ").GetString();
                                nhaCungCap.NhaCungCap.GhiChu = row.Field("Ghi chú").GetString();

                                string tenNhaCungCapCha = row.Field("Tên nhà cung cấp cha").GetString();
                                #region Tìm cha
                                nhaCungCap.NhaCungCapCha = new tbNhaCungCap {
                                    IdNhaCungCap = Guid.Empty,
                                    TenNhaCungCap = tenNhaCungCapCha
                                };
                                nhaCungCap.NhaCungCap.IdNhaCungCapCha = nhaCungCap.NhaCungCapCha.IdNhaCungCap;
                                #endregion
                                output.Add(nhaCungCap);
                            }
                        }
                    }
                }
                #endregion
            }
            return output;
        }
        /// <summary>
        /// Validates a list of supplier import data entries, checking required fields such as supplier code and name,
        /// and categorizes each entry as valid or invalid.
        /// </summary>
        /// <remarks>This method checks for the existence and validity of key supplier fields, such as the
        /// supplier code. Entries that fail validation are marked as invalid and include descriptive error messages.
        /// The method is asynchronous and should be awaited.</remarks>
        /// <param name="input">A list of supplier data entries to validate. Each entry should contain the necessary supplier information to
        /// be checked for validity.</param>
        /// <returns>A result object containing collections of valid and invalid supplier entries, along with validation messages
        /// for each entry.</returns>
        public async Task<ValidateImportData_Output_Dto> ValidateImportData(List<tbNhaCungCapExtend> input) {
            /**
             * Các thông tin cần kiểm tra
             * Mã nhà cung cấp
             * Tên nhà cung cấp
             */

            var output = new ValidateImportData_Output_Dto();

            foreach (var nhaCungCap in input) {
                var isExisted = await _isExistedNhaCungCapValidation.IsExisted(
                      nhaCungCap: nhaCungCap.NhaCungCap,
                      NhaCungCapFieldEnum.MaNhaCungCap);
                // Kiểm tra tính hợp lệ của dữ liệu
                var isValid = await _isValidNhaCungCapValidation.ValidateFieldsOnlyAsync(
                   nhaCungCap: nhaCungCap.NhaCungCap,
                   new FieldValidationOptionDto<NhaCungCapFieldEnum> {
                       Field = NhaCungCapFieldEnum.TenNhaCungCap,
                       DisplayName = "Tên nhà cung cấp",
                       Rules = ValidateRule.Required
                       //| ValidateRule.MinLength | ValidateRule.MaxLength | ValidateRule.Regex,
                       //MinLen = 3,
                       //MaxLen = 20,
                       //Pattern = @"^[A-Za-z0-9\-_]+$",
                       //PatternMessage = "Mã NCC chỉ gồm chữ/số và - _."
                   });
                // Kiểm tra tính hợp lệ của nhà cung cấp cha
                var isValidParent = await _isValidNhaCungCapValidation.ValidateFieldsOnlyAsync(
                   nhaCungCap: nhaCungCap.NhaCungCapCha,
                   new FieldValidationOptionDto<NhaCungCapFieldEnum> {
                       Field = NhaCungCapFieldEnum.TenNhaCungCap,
                       DisplayName = "Tên nhà cung cấp cha",
                       Rules = ValidateRule.Required
                   });
                // Kiểm tra trùng tên nhà cung cấp
                var isDuplicate = input.Where(x => x.NhaCungCap.TenNhaCungCap == nhaCungCap.NhaCungCap.TenNhaCungCap).ToList().Count > 1;
                // Kiểm tra nhà cung cấp có trùng với nhà cung cấp cha
                var isSelfParent = nhaCungCap.NhaCungCap.TenNhaCungCap.Trim().ToLower() == nhaCungCap.NhaCungCapCha.TenNhaCungCap.Trim().ToLower();

                // Xử lý kết quả kiểm tra
                if (isDuplicate) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Tên nhà cung cấp bị trùng.");
                }
                if (isSelfParent) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Tên nhà cung cấp không được trùng tên nhà cung cấp cha.");
                }
                if (isExisted.IsValid) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add(string.Join(", ", isExisted.InvalidFields.Select(x => x.Message)));

                }
                if (isValid.IsValid) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add(string.Join(", ", isValid.InvalidFields.Select(x => x.Message)));
                }


                // Tổng kết
                if ((int)nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu == (int)TrangThaiKiemTraDuLieuEnum.HopLe)
                    output.NhaCungCap_HopLe.Add(nhaCungCap);
                else output.NhaCungCap_KhongHopLe.Add(nhaCungCap);
            }

            return output;
        }
        /// <summary>
        /// Generates an Excel workbook containing a table of data validation errors for the supplied list of supplier
        /// records.
        /// </summary>
        /// <remarks>The generated workbook includes a worksheet named "LoiKiemTraDuLieu" with columns for
        /// supplier information and validation error details. The table is formatted for readability and does not
        /// include autofilter functionality.</remarks>
        /// <param name="input">A list of supplier extension records to include in the error report. Each item represents a supplier and its
        /// associated validation results. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an XLWorkbook with a worksheet
        /// listing validation errors for each supplier record.</returns>
        public async Task<XLWorkbook> GenerateErrorFile(List<tbNhaCungCapExtend> input) {
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("LoiKiemTraDuLieu");
            // Tạo bảng lỗi
            var headers = new List<string> {
                "Tên nhà cung cấp",
                "Tên nhà cung cấp cha",
                "Tên mặt hàng",
                "Số điện thoại",
                "Email",
                "Địa chỉ",
                "Ghi chú",
                "Trạng thái kiểm tra dữ liệu",
                "Thông báo lỗi"
            };
            var tableRange = sheet.Range(1, 1, input.Count + 1, headers.Count);
            for (int i = 0; i < headers.Count; i++) {
                tableRange.Cell(1, i + 1).Value = headers[i];
            }
            for (int r = 0; r < input.Count; r++) {
                var row = input[r];
                tableRange.Cell(r + 2, 1).Value = row.NhaCungCap.TenNhaCungCap;
                tableRange.Cell(r + 2, 2).Value = row.NhaCungCapCha.TenNhaCungCap;
                tableRange.Cell(r + 2, 3).Value = row.NhaCungCap.TenMatHang;
                tableRange.Cell(r + 2, 4).Value = row.NhaCungCap.SoDienThoai;
                tableRange.Cell(r + 2, 5).Value = row.NhaCungCap.Email;
                tableRange.Cell(r + 2, 6).Value = row.NhaCungCap.DiaChi;
                tableRange.Cell(r + 2, 7).Value = row.NhaCungCap.GhiChu;
                tableRange.Cell(r + 2, 8).Value = ((TrangThaiKiemTraDuLieuEnum)row.KiemTraDuLieu.TrangThaiKiemTraDuLieu).ToString();
                tableRange.Cell(r + 2, 9).Value = string.Join("; ", row.KiemTraDuLieu.Messages);
            }
            var table = tableRange.CreateTable("LoiKiemTraDuLieu");
            // Định dạng bảng
            table.ShowAutoFilter = false;
            table.Theme = XLTableTheme.TableStyleMedium9;
            // Tùy chọn định dạng cột
            sheet.Columns().AdjustToContents();
            return await Task.FromResult(workbook);
        }
        /// <summary>
        /// Generates an Excel workbook containing a template sheet populated with supplier data.
        /// </summary>
        /// <remarks>The generated worksheet includes columns for supplier name, parent supplier name,
        /// product name, phone number, email, address, and notes. The table is formatted with a predefined style and
        /// auto-adjusted column widths. The caller is responsible for disposing of the returned XLWorkbook when it is
        /// no longer needed.</remarks>
        /// <param name="input">A list of supplier extension objects to include in the template. Each item provides data for a row in the
        /// generated worksheet. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an XLWorkbook with a worksheet
        /// named "DuLieuNhap" populated with the supplied data.</returns>
        public async Task<XLWorkbook> GenerateTemplateFile(List<tbNhaCungCapExtend> input) {
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("DuLieuNhap");
            // Tạo bảng dữ liệu nhập
            var headers = new List<string> {
                "Tên nhà cung cấp",
                "Tên nhà cung cấp cha",
                "Tên mặt hàng",
                "Số điện thoại",
                "Email",
                "Địa chỉ",
                "Ghi chú",
            };
            var tableRange = sheet.Range(1, 1, input.Count + 1, headers.Count);
            for (int i = 0; i < headers.Count; i++) {
                tableRange.Cell(1, i + 1).Value = headers[i];
            }
            for (int r = 0; r < input.Count; r++) {
                var row = input[r];
                tableRange.Cell(r + 2, 1).Value = row.NhaCungCap.TenNhaCungCap;
                tableRange.Cell(r + 2, 2).Value = row.NhaCungCapCha.TenNhaCungCap;
                tableRange.Cell(r + 2, 3).Value = row.NhaCungCap.TenMatHang;
                tableRange.Cell(r + 2, 4).Value = row.NhaCungCap.SoDienThoai;
                tableRange.Cell(r + 2, 5).Value = row.NhaCungCap.Email;
                tableRange.Cell(r + 2, 6).Value = row.NhaCungCap.DiaChi;
                tableRange.Cell(r + 2, 7).Value = row.NhaCungCap.GhiChu;
            }
            var table = tableRange.CreateTable("ThongTinChung");
            // Định dạng bảng
            table.ShowAutoFilter = false;
            table.Theme = XLTableTheme.TableStyleMedium9;
            // Tùy chọn định dạng cột
            sheet.Columns().AdjustToContents();
            return await Task.FromResult(workbook);
        }
        public static List<tbNhaCungCapExtend> GroupDataAsTree(List<tbNhaCungCapExtend> input) {
            var daXuLy = new List<tbNhaCungCapExtend>();

            // set nhanh để check đã thêm
            bool DaThem(tbNhaCungCapExtend x)
                => daXuLy.Any(a => a.NhaCungCap.TenNhaCungCap == x.NhaCungCap.TenNhaCungCap);

            void timCon(tbNhaCungCapExtend cha) {
                var cons = input
                    .Where(x => x.NhaCungCapCha != null
                             && x.NhaCungCapCha.TenNhaCungCap == cha.NhaCungCap.TenNhaCungCap)
                    .ToList();

                foreach (var con in cons) {
                    if (DaThem(con)) continue;

                    // gán liên kết cha-con bằng ID giả trong RAM
                    con.NhaCungCap.IdNhaCungCap = con.NhaCungCap.IdNhaCungCap == Guid.Empty
                        ? Guid.NewGuid()
                        : con.NhaCungCap.IdNhaCungCap;

                    con.NhaCungCap.IdNhaCungCapCha = cha.NhaCungCap.IdNhaCungCap;
                    con.NhaCungCap.CapDo = (cha.NhaCungCap.CapDo ?? 1) + 1;

                    daXuLy.Add(con);

                    timCon(con);
                }
            }

            void timCha(tbNhaCungCapExtend con) {
                // tìm cha của "con" trong input
                var chaTrongDs = input.FirstOrDefault(x =>
                    x.NhaCungCap != null
                    && con.NhaCungCapCha != null
                    && x.NhaCungCap.TenNhaCungCap == con.NhaCungCapCha.TenNhaCungCap);

                if (chaTrongDs == null) {
                    // không có cha trong input => coi con là root (cha = null/Guid.Empty)
                    con.NhaCungCap.IdNhaCungCap = con.NhaCungCap.IdNhaCungCap == Guid.Empty
                        ? Guid.NewGuid()
                        : con.NhaCungCap.IdNhaCungCap;

                    con.NhaCungCap.IdNhaCungCapCha = null; // hoặc Guid.Empty tuỳ bạn
                    con.NhaCungCap.CapDo = 1;

                    if (!DaThem(con))
                        daXuLy.Add(con);

                    timCon(con);
                }
                else {
                    // có cha trong input => đệ quy tìm cha cao nhất
                    if (!DaThem(chaTrongDs))
                        timCha(chaTrongDs);
                    else {
                        // cha đã có rồi -> gán con theo cha đã gán
                        con.NhaCungCap.IdNhaCungCap = con.NhaCungCap.IdNhaCungCap == Guid.Empty
                            ? Guid.NewGuid()
                            : con.NhaCungCap.IdNhaCungCap;

                        con.NhaCungCap.IdNhaCungCapCha = chaTrongDs.NhaCungCap.IdNhaCungCap;
                        con.NhaCungCap.CapDo = (chaTrongDs.NhaCungCap.CapDo ?? 1) + 1;

                        if (!DaThem(con))
                            daXuLy.Add(con);

                        timCon(con);
                    }
                }
            }

            foreach (var ncc in input) {
                if (!DaThem(ncc))
                    timCha(ncc);
            }

            return daXuLy;
        }

    }
}

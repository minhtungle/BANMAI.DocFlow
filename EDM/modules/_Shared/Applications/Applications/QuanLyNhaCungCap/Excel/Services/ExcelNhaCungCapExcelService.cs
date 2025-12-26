using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Enums;
using Applications.QuanLyNhaCungCap.Excel.Dtos;
using Applications.QuanLyNhaCungCap.Excel.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using Applications.QuanLyNhaCungCap.Validations;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Dtos;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Applications.QuanLyNhaCungCap.Excel.Services {
    public class ExcelNhaCungCapExcelService : BaseService, IExcelNhaCungCapExcelService {
        private readonly IsExistedNhaCungCapValidation _isExistedNhaCungCapValidation;
        private readonly IsValidNhaCungCapValidation _isValidNhaCungCapValidation;

        public ExcelNhaCungCapExcelService(
                IUserContext userContext,
            IUnitOfWork unitOfWork,

            IsExistedNhaCungCapValidation isExistedNhaCungCapValidation,
            IsValidNhaCungCapValidation isValidNhaCungCapValidation
            ) : base(userContext, unitOfWork) {


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
        public async Task<ValidateImportData_Output_Dto> _ValidateImportData(List<tbNhaCungCapExtend> input) {
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
        public async Task<ValidateImportData_Output_Dto> ValidateImportData(List<tbNhaCungCapExtend> input) {
            var output = new ValidateImportData_Output_Dto();

            // ====== Chuẩn hoá so sánh tên ======
            string Norm(string s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLower();

            // ====== 1) Pre-calc duplicate tên NCC ======
            var dupTenNcc = input
                .GroupBy(x => Norm(x?.NhaCungCap?.TenNhaCungCap))
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            // ====== 2) Build map: childName -> parentName (để detect cycle) ======
            // Nếu tên NCC trống thì bỏ qua map (vì bản thân đã lỗi Required)
            var parentMap = new Dictionary<string, string>();
            foreach (var x in input) {
                var child = Norm(x?.NhaCungCap?.TenNhaCungCap);
                if (string.IsNullOrEmpty(child)) continue;

                // parent có thể rỗng (root) => vẫn cho vào map với parent="" để xử lý logic
                var parent = Norm(x?.NhaCungCapCha?.TenNhaCungCap);
                parentMap[child] = parent; // nếu child trùng, dupTenNcc đã bắt, map lấy cái cuối cũng OK vì vẫn lỗi
            }

            // ====== helper detect cycle ======
            bool HasCycleNear(string child, string parent) {
                // vòng lặp gần: A->B và B->A
                if (string.IsNullOrEmpty(child) || string.IsNullOrEmpty(parent)) return false;
                if (!parentMap.ContainsKey(parent)) return false; // nếu parent không có trong file thì bỏ qua theo yêu cầu
                return Norm(parentMap[parent]) == child;
            }

            bool HasCycleFar(string startChild) {
                // vòng lặp xa: A->...->A
                if (string.IsNullOrEmpty(startChild)) return false;

                var visited = new HashSet<string>();
                var cur = startChild;

                while (true) {
                    if (!parentMap.ContainsKey(cur)) return false; // parent không có trong file -> bỏ qua (không check "không tồn tại")
                    var p = Norm(parentMap[cur]);

                    if (string.IsNullOrEmpty(p)) return false;      // root
                    if (p == startChild) return true;               // quay về chính nó => cycle xa

                    // nếu gặp lại một node đã đi qua => cũng là cycle (không nhất thiết quay về start)
                    if (visited.Contains(p)) return true;

                    visited.Add(p);
                    cur = p;
                }
            }

            foreach (var nhaCungCap in input) {
                // đảm bảo có container message
                if (nhaCungCap.KiemTraDuLieu == null)
                    nhaCungCap.KiemTraDuLieu = new DataValidResultDto(); // bạn thay đúng type trong project
                if (nhaCungCap.KiemTraDuLieu.Messages == null)
                    nhaCungCap.KiemTraDuLieu.Messages = new List<string>();

                // mặc định hợp lệ
                nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = TrangThaiKiemTraDuLieuEnum.HopLe;

                var tenCon = Norm(nhaCungCap?.NhaCungCap?.TenNhaCungCap);
                var tenCha = Norm(nhaCungCap?.NhaCungCapCha?.TenNhaCungCap);

                // ====== 3) Required: Tên NCC ======
                var validTenCon = await _isValidNhaCungCapValidation.ValidateFieldsOnlyAsync(
                    nhaCungCap: nhaCungCap.NhaCungCap,
                    new FieldValidationOptionDto<NhaCungCapFieldEnum> {
                        Field = NhaCungCapFieldEnum.TenNhaCungCap,
                        DisplayName = "Tên nhà cung cấp",
                        Rules = ValidateRule.Required
                    });

                if (!validTenCon.IsValid) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.AddRange(validTenCon.InvalidFields.Select(x => x.Message));
                }

                // ====== 4) Required: Tên NCC cha (nếu bạn bắt buộc cha phải có) ======
                // Nếu bạn muốn: cha có thể trống (root) => bỏ khối này
                var validTenCha = await _isValidNhaCungCapValidation.ValidateFieldsOnlyAsync(
                    nhaCungCap: nhaCungCap.NhaCungCapCha,
                    new FieldValidationOptionDto<NhaCungCapFieldEnum> {
                        Field = NhaCungCapFieldEnum.TenNhaCungCap,
                        DisplayName = "Tên nhà cung cấp cha",
                        Rules = ValidateRule.Required
                    });

                if (!validTenCha.IsValid) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.AddRange(validTenCha.InvalidFields.Select(x => x.Message));
                }

                // ====== 5) Duplicate tên NCC ======
                if (!string.IsNullOrEmpty(tenCon) && dupTenNcc.Contains(tenCon)) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Tên nhà cung cấp bị trùng.");
                }

                // ====== 6) Self-parent ======
                if (!string.IsNullOrEmpty(tenCon) && !string.IsNullOrEmpty(tenCha) && tenCon == tenCha) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Tên nhà cung cấp không được trùng tên nhà cung cấp cha.");
                }

                // ====== 7) Cycle gần (A<->B) ======
                if (!string.IsNullOrEmpty(tenCon) && !string.IsNullOrEmpty(tenCha) && HasCycleNear(tenCon, tenCha)) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Vòng lặp cha con gần.");
                }

                // ====== 8) Cycle xa (A->...->A) ======
                if (!string.IsNullOrEmpty(tenCon) && HasCycleFar(tenCon)) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.Add("Vòng lặp cha con xa.");
                }

                // ====== 9) Existed (DB) theo field bạn chọn ======
                // (nếu bạn muốn giữ check này)
                var existed = await _isExistedNhaCungCapValidation.IsExisted(
                    nhaCungCap: nhaCungCap.NhaCungCap,
                    NhaCungCapFieldEnum.MaNhaCungCap);

                // nếu existed.IsValid == false mới là có lỗi
                if (!existed.IsValid) {
                    nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu = (int)TrangThaiKiemTraDuLieuEnum.KhongHopLe;
                    nhaCungCap.KiemTraDuLieu.Messages.AddRange(existed.InvalidFields.Select(x => x.Message));
                }

                // ====== Tổng kết ======
                if ((int)nhaCungCap.KiemTraDuLieu.TrangThaiKiemTraDuLieu == (int)TrangThaiKiemTraDuLieuEnum.HopLe)
                    output.NhaCungCap_HopLe.Add(nhaCungCap);
                else
                    output.NhaCungCap_KhongHopLe.Add(nhaCungCap);
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
        /// <summary>
        /// Organizes a flat list of supplier data into a hierarchical tree structure based on parent-child
        /// relationships.
        /// </summary>
        /// <remarks>The method assigns temporary unique identifiers to suppliers with missing IDs to
        /// establish parent-child links during grouping. The resulting list reflects the tree structure, with each
        /// supplier's hierarchy level indicated by its 'CapDo' property. The method does not modify the original input
        /// list's order.</remarks>
        /// <param name="input">The list of supplier data to be grouped as a tree. Each item should contain information about its parent
        /// supplier, if applicable. Cannot be null.</param>
        /// <returns>A list of supplier data arranged in hierarchical order, where parent suppliers precede their children.
        /// Returns an empty list if the input is null or empty.</returns>
        public List<Tree<tbNhaCungCapExtend>> SapXepChaCon_Tree(List<tbNhaCungCapExtend> input) {
            if (input == null || input.Count == 0)
                return new List<Tree<tbNhaCungCapExtend>>();

            var roots = new List<Tree<tbNhaCungCapExtend>>();
            var daXuLy = new List<tbNhaCungCapExtend>();
            int stt = 0;

            bool DaXuLy(tbNhaCungCapExtend x)
                => daXuLy.Any(a => a.NhaCungCap.TenNhaCungCap == x.NhaCungCap.TenNhaCungCap);

            Tree<tbNhaCungCapExtend> timCon(tbNhaCungCapExtend cha) {
                var node = new Tree<tbNhaCungCapExtend> {
                    SoThuTu = ++stt,
                    root = cha
                };

                var cons = input
                    .Where(x => x.NhaCungCapCha != null
                             && x.NhaCungCapCha.TenNhaCungCap == cha.NhaCungCap.TenNhaCungCap)
                    .ToList();

                foreach (var con in cons) {
                    if (DaXuLy(con)) continue;

                    con.NhaCungCap.CapDo = (cha.NhaCungCap.CapDo ?? 1) + 1;
                    daXuLy.Add(con);

                    var childNode = timCon(con);
                    node.nodes.Add(childNode);
                }

                return node;
            }

            void timCha(tbNhaCungCapExtend con) {
                var chaTrongDs = input.FirstOrDefault(x =>
                    x.NhaCungCap != null
                    && con.NhaCungCapCha != null
                    && x.NhaCungCap.TenNhaCungCap == con.NhaCungCapCha.TenNhaCungCap);

                if (chaTrongDs == null) {
                    // root
                    con.NhaCungCap.CapDo = 1;

                    if (!DaXuLy(con))
                        daXuLy.Add(con);

                    var rootNode = timCon(con);
                    roots.Add(rootNode);
                }
                else {
                    if (!DaXuLy(chaTrongDs))
                        timCha(chaTrongDs);
                }
            }

            foreach (var x in input) {
                if (!DaXuLy(x))
                    timCha(x);
            }

            return roots;
        }
        /// <summary>
        /// Asynchronously saves a list of supplier extensions and their parent-child relationships to the database.
        /// </summary>
        /// <remarks>This method creates new supplier records and establishes parent-child relationships
        /// based on the provided order. Each supplier's parent is determined by the name specified in the corresponding
        /// extension object. The operation is performed within a database transaction to ensure consistency.</remarks>
        /// <param name="ordered">The ordered list of supplier extension objects to be saved. Each item may specify a parent supplier by name.
        /// Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task LuuTreeAsync(List<Tree<tbNhaCungCapExtend>> trees) {
            if (trees == null || trees.Count == 0) return;

            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var tree in trees) {
                    await LuuNodeAsync(tree, parentId: null);
                }
            });
        }

        #region Private Methods
        /// <summary>
        /// Asynchronously saves a supplier node and its associated school links to the data store, including all
        /// descendant nodes in the tree structure.
        /// </summary>
        /// <remarks>This method recursively processes the entire tree, saving each supplier node and its
        /// related school links. The operation is performed asynchronously for each node and its descendants.</remarks>
        /// <param name="tree">The tree node representing the supplier and its children to be saved. The root of the tree contains supplier
        /// data and associated schools.</param>
        /// <param name="parentId">The unique identifier of the parent supplier node. Specify null if the current node is a root node.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        private async Task LuuNodeAsync(
            Tree<tbNhaCungCapExtend> tree,
            Guid? parentId) {
            var ex = tree.root;

            var entity = new tbNhaCungCap {
                IdNhaCungCap = Guid.NewGuid(),
                IdNhaCungCapCha = parentId,

                TenNhaCungCap = ex.NhaCungCap.TenNhaCungCap,
                TenMatHang = ex.NhaCungCap.TenMatHang,
                SoDienThoai = ex.NhaCungCap.SoDienThoai,
                Email = ex.NhaCungCap.Email,
                DiaChi = ex.NhaCungCap.DiaChi,
                GhiChu = ex.NhaCungCap.GhiChu,

                TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                NgayTao = DateTime.Now
            };

            await _unitOfWork.InsertAsync<tbNhaCungCap, Guid>(entity);

            // lưu trường học
            if (ex.TruongHocs != null) {
                foreach (var truongHoc in ex.TruongHocs) {
                    var link = new tbNhaCungCapTruongHoc {
                        IdNhaCungCapTruongHoc = Guid.NewGuid(),
                        IdTruongHoc = truongHoc.IdTruongHoc,
                        IdNhaCungCap = entity.IdNhaCungCap,

                        TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now
                    };

                    await _unitOfWork.InsertAsync<tbNhaCungCapTruongHoc, Guid>(link);
                }
            }

            // đệ quy lưu con
            foreach (var child in tree.nodes) {
                await LuuNodeAsync(child, entity.IdNhaCungCap);
            }
        }
        #endregion
    }
}

using Applications.QuanLyTaiLieu.Dtos;
using Applications.QuanLyTaiLieu.Filters;
using Applications.QuanLyTaiLieu.Interfaces;
using Applications.QuanLyTaiLieu.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Applications.QuanLyTaiLieu.Services {
    public class QuanLyTaiLieuService : BaseService, IQuanLyTaiLieuService {
        private readonly IRepository<tbTaiLieu, Guid> _taiLieuRepo;
        private readonly IRepository<tbNhaCungCap, Guid> _nhaCungCapRepo;

        public QuanLyTaiLieuService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbTaiLieu, Guid> taiLieuRepo,
            IRepository<tbNhaCungCap, Guid> nhaCungCapRepo
            ) : base(userContext, unitOfWork) {
            _taiLieuRepo = taiLieuRepo;
            _nhaCungCapRepo = nhaCungCapRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);

        public async Task<Index_Output_Dto> Index(Index_Input_Dto input) {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyTaiLieu");
            var nhaCungCaps = await _nhaCungCapRepo.Query()
                .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .OrderByDescending(x => x.Stt)
                .ToListAsync();

            return new Index_Output_Dto {
                ThaoTacs = thaoTacs,
                IdNhaCungCap = input.IdNhaCungCap,
                NhaCungCaps = nhaCungCaps
            };
        }

        public async Task<List<tbTaiLieuExtend>> Get_TaiLieus(GetList_TaiLieu_Input_Dto input) {
            var query = _taiLieuRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin != null && input.LocThongTin.IdTaiLieus != null && input.LocThongTin.IdTaiLieus.Any()) {
                query = query.Where(x => input.LocThongTin.IdTaiLieus.Contains(x.IdFile));
            }

            var data = await query
                .OrderByDescending(x => x.Stt)
                .Select(x => new tbTaiLieuExtend { TaiLieu = x })
                .ToListAsync();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FormAddTaiLieuDto> AddBanGhi_Modal_CRUD(AddBanGhi_Modal_CRUD_Input_Dto input) {
            var output = new AddBanGhi_Modal_CRUD_Output_Dto {
                Loai = input.Loai,
            };
            if (input.Loai == "create") {
                output.TaiLieus = await AddFromFile(input: new AddFromFile_Input_Dto {
                    IdNhaCungCap = input.IdNhaCungCap,
                    Files = input.Files,
                });
            }
            else if (input.Loai == "update" || input.Loai == "detail") {
                var taiLieu = await GetDetail_TaiLieus(idTaiLieus: input.IdTaiLieus);
                // Chỉ lấy những bài đăng có trạng thái (chờ đăng)
                output.TaiLieus = taiLieu.ToList();
            }

            return new FormAddTaiLieuDto {
                Data = output,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTaiLieus"></param>
        /// <returns></returns>
        public async Task<List<tbTaiLieuExtend>> GetDetail_TaiLieus(List<Guid> idTaiLieus = null) {
            var taiLieus = await _taiLieuRepo.Query()
                .Where(x =>
                    idTaiLieus.Contains(x.IdFile) &&
                    x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .Join(_nhaCungCapRepo.Query(),
                tl => tl.IdNhaCungCap,
                ncc => ncc.IdNhaCungCap,
                (tl, ncc) => new tbTaiLieuExtend {
                    TaiLieu = tl,
                    NhaCungCap = ncc,
                })
                .ToListAsync();

            return taiLieus;
        }
        public async Task<bool> IsExisted_TaiLieu(tbTaiLieu taiLieu) {
            var existed = await _taiLieuRepo.Query()
                .FirstOrDefaultAsync(x =>
                    (x.FileNameUpdate == taiLieu.FileNameUpdate)
                    && x.IdFile != taiLieu.IdFile
                    && x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                    && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return existed != null;
        }

        public async Task Create_TaiLieu(List<tbTaiLieuExtend> taiLieus, HttpPostedFileBase[] files) {

        }

        public async Task Update_TaiLieu(tbTaiLieuExtend taiLieu) {
            await _unitOfWork.ExecuteInTransaction(async () => {
                var _taiLieu = await _taiLieuRepo.GetByIdAsync(taiLieu.TaiLieu.IdFile);
                if (_taiLieu == null)
                    throw new Exception("Tài liệu không tồn tại.");

                _taiLieu.FileNameUpdate = taiLieu.TaiLieu.FileNameUpdate;
                _taiLieu.FileExtension = taiLieu.TaiLieu.FileExtension;
                _taiLieu.DuongDanTepVatLy = taiLieu.TaiLieu.DuongDanTepVatLy;
                _taiLieu.DuongDanTepOnline = taiLieu.TaiLieu.DuongDanTepOnline;
                _taiLieu.GhiChu = taiLieu.TaiLieu.GhiChu;

                _taiLieu.NgaySua = DateTime.Now;
                _taiLieu.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                await _unitOfWork.UpdateAsync<tbTaiLieu, Guid>(_taiLieu);
            });
        }

        public async Task Delete_TaiLieus(List<Guid> idTaiLieus) {
            await _unitOfWork.ExecuteInTransaction(async () => {
                var taiLieus_DELETE = await _taiLieuRepo.Query()
                    .Where(x => idTaiLieus.Contains(x.IdFile))
                    .ToListAsync();

                foreach (var _taiLieu in taiLieus_DELETE) {
                    _taiLieu.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _taiLieu.NgaySua = DateTime.Now;
                    _taiLieu.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbTaiLieu, Guid>(_taiLieu);
                }
            });
        }

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<List<tbTaiLieuExtend>> AddFromFile(AddFromFile_Input_Dto input) {
            /**
             * Xóa thư mục cache
             * Lưu tất cả vào thư mục cache "Assets/upload/TAILIEUNHACUNGCAP/{currentDonVi.MaDonViSuDung}/{currentNguoiDung.IdNguoiDung}"
             * Tạo new tbTaiLieuExtend với các thông tin cơ bản (FileName, FileExtension, Đường dẫn tệp vật lý, Đường dẫn tệp online)
             * Hiển thị ra view
             * Khi lưu thực hiện chuyển file về đúng thư mục "/upload/TAILIEU/{currentDonVi.MaDonViSuDung}/{taiLieu.IdFile}"
             */

            if (input.Files == null || input.Files.Length == 0)
                throw new ArgumentException("Chưa tải lên file nào");

            var nhaCungCap = await _nhaCungCapRepo.GetByIdAsync(id: input.IdNhaCungCap);
            if (nhaCungCap == null)
                throw new ArgumentException("Không tồn tại nhà cung cấp.");

            // ✅ SỬA: bỏ dấu } dư
            string baseOnlineUrl = string.Format("{0}/{1}/TAILIEUNHACUNGCAP_CACHE",
                "/Assets/upload",
                CurrentDonViSuDung.MaDonViSuDung);

            string rootPhysical = HostingEnvironment.MapPath(baseOnlineUrl);

            // ✅ đảm bảo thư mục tồn tại
            if (!Directory.Exists(rootPhysical))
                Directory.CreateDirectory(rootPhysical);

            var taiLieus = new List<tbTaiLieuExtend>();

            // 1️⃣ XỬ LÝ FILE + TẠO ENTITY (ngoài transaction)
            for (int i = 0; i < input.Files.Length; i++) {
                var file = input.Files[i];

                if (file == null || file.ContentLength <= 0)
                    continue;

                // ✅ TẠO IdFile TRƯỚC để dùng làm tên file
                var idFile = Guid.NewGuid();

                // Lấy extension & mime (có thể lấy bằng helper như bạn đang dùng)
                var fileInfo = Public.Helpers.FileSaveInfoHelper.BuildFileSaveInfo(
                    file,
                    rootPhysical,
                    baseOnlineUrl,
                    separator: '-',        // không quan trọng nữa
                    addUniqueSuffix: false // ✅ không cần suffix vì đã dùng GUID
                );

                // ✅ ÉP tên file vật lý = IdFile + extension
                string newFileName = idFile.ToString() + fileInfo.Extension;           // vd: {guid}.pdf
                string physicalPath = Path.Combine(rootPhysical, newFileName);
                string onlineUrl = $"{baseOnlineUrl}/{newFileName}";

                // ✅ Lưu file vật lý
                file.SaveAs(physicalPath);

                var taiLieu = new tbTaiLieuExtend {
                    TaiLieu = new tbTaiLieu {
                        IdFile = Guid.Empty,
                        IdNhaCungCap = nhaCungCap.IdNhaCungCap,
                        NgayDangKy = DateTime.Now,

                        // ✅ lưu theo GUID để đồng bộ
                        FileName = idFile.ToString(),
                        FileNameUpdate = newFileName,
                        FileExtension = fileInfo.Extension,
                        LoaiTep = fileInfo.MimeType,
                        DuongDanTepVatLy = physicalPath,
                        DuongDanTepOnline = onlineUrl,

                        TrangThai = (int?)TrangThaiDuLieuEnum.ChoPheDuyet,
                        NgayTao = DateTime.Now,
                        IdNguoiTao = CurrentUserId,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                    }
                };

                taiLieus.Add(taiLieu);
            }

            return taiLieus;
        }
        private async Task createByZip() { }
        private async Task createByFile(CreateByFile_Input_Dto input) {
            if (input.TaiLieus == null || !input.TaiLieus.Any())
                throw new ArgumentException("Danh sách tài liệu không được để trống.");

            if (input.Files == null || input.Files.Length != input.TaiLieus.Count)
                throw new ArgumentException("Danh sách file phải khớp thứ tự với tài liệu.");

            var nhaCungCap = await _nhaCungCapRepo.GetByIdAsync(id: input.IdNhaCungCap);
            if (nhaCungCap == null)
                throw new ArgumentException("Không tồn tại nhà cung cấp.");

            // ✅ SỬA: bỏ dấu } dư
            string baseOnlineUrl = string.Format("{0}/{1}/TAILIEUNHACUNGCAP",
                "/Assets/upload",
                CurrentDonViSuDung.MaDonViSuDung);

            string rootPhysical = HostingEnvironment.MapPath(baseOnlineUrl);

            // ✅ đảm bảo thư mục tồn tại
            if (!Directory.Exists(rootPhysical))
                Directory.CreateDirectory(rootPhysical);

            var taiLieuEntities = new List<tbTaiLieu>();

            // 1️⃣ XỬ LÝ FILE + TẠO ENTITY (ngoài transaction)
            for (int i = 0; i < input.TaiLieus.Count; i++) {
                var taiLieu_NEW = input.TaiLieus[i];
                var file = input.Files[i];

                if (file == null || file.ContentLength <= 0)
                    continue;

                // ✅ TẠO IdFile TRƯỚC để dùng làm tên file
                var idFile = Guid.NewGuid();

                // Lấy extension & mime (có thể lấy bằng helper như bạn đang dùng)
                var fileInfo = Public.Helpers.FileSaveInfoHelper.BuildFileSaveInfo(
                    file,
                    rootPhysical,
                    baseOnlineUrl,
                    separator: '-',        // không quan trọng nữa
                    addUniqueSuffix: false // ✅ không cần suffix vì đã dùng GUID
                );

                // ✅ ÉP tên file vật lý = IdFile + extension
                string newFileName = idFile.ToString() + fileInfo.Extension;           // vd: {guid}.pdf
                string physicalPath = Path.Combine(rootPhysical, newFileName);
                string onlineUrl = $"{baseOnlineUrl}/{newFileName}";

                // ✅ Lưu file vật lý
                file.SaveAs(physicalPath);

                var taiLieu = new tbTaiLieu {
                    IdFile = idFile,
                    IdNhaCungCap = nhaCungCap.IdNhaCungCap,
                    NgayDangKy = DateTime.Now,
                    NgayHetHan = taiLieu_NEW.TaiLieu.NgayHetHan,

                    // ✅ lưu theo GUID để đồng bộ
                    FileName = idFile.ToString(),
                    FileNameUpdate = newFileName,
                    FileExtension = fileInfo.Extension,
                    LoaiTep = fileInfo.MimeType,
                    DuongDanTepVatLy = physicalPath,
                    DuongDanTepOnline = onlineUrl,

                    TrangThai = (int?)TrangThaiDuLieuEnum.ChoPheDuyet,
                    NgayTao = DateTime.Now,
                    IdNguoiTao = CurrentUserId,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                };

                taiLieuEntities.Add(taiLieu);
            }

            // 2️⃣ TRANSACTION – lưu DB
            await _unitOfWork.ExecuteInTransaction(async () => {
                foreach (var tl in taiLieuEntities) {
                    await _unitOfWork.InsertAsync<tbTaiLieu, Guid>(tl);
                }
            });
        }

        #endregion
    }
}

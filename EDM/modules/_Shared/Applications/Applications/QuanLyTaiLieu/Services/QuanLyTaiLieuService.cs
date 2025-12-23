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

namespace Applications.QuanLyTaiLieu.Services
{
    public class QuanLyTaiLieuService : BaseService, IQuanLyTaiLieuService
    {
        private readonly IRepository<tbTaiLieu, Guid> _taiLieuRepo;
        private readonly IRepository<tbNhaCungCap, Guid> _nhaCungCapRepo;

        public QuanLyTaiLieuService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbTaiLieu, Guid> taiLieuRepo,
            IRepository<tbNhaCungCap, Guid> nhaCungCapRepo
            ) : base(userContext, unitOfWork)
        {
            _taiLieuRepo = taiLieuRepo;
            _nhaCungCapRepo = nhaCungCapRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);

        public async Task<Index_Output_Dto> Index(Index_Input_Dto input)
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyTaiLieu");
            var nhaCungCaps = await _nhaCungCapRepo.Query()
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .OrderByDescending(x => x.Stt)
                .ToListAsync();

            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
                NhaCungCaps = nhaCungCaps
            };
        }

        public async Task<List<tbTaiLieuExtend>> Get_TaiLieus(GetList_TaiLieu_Input_Dto input)
        {
            var query = _taiLieuRepo.Query()
                .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung)
                .AsNoTracking();

            if (input.Loai == "single" && input.LocThongTin != null && input.LocThongTin.IdTaiLieus != null && input.LocThongTin.IdTaiLieus.Any())
            {
                query = query.Where(x => input.LocThongTin.IdTaiLieus.Contains(x.IdFile));
            }

            var data = await query
                .OrderByDescending(x => x.Stt)
                .Select(x => new tbTaiLieuExtend { TaiLieu = x })
                .ToListAsync();

            return data;
        }

        public async Task<DisplayModal_CRUD_TaiLieu_Output_Dto> DisplayModal_CRUD_TaiLieu(DisplayModal_CRUD_TaiLieu_Input_Dto input)
        {
            var taiLieus = await Get_TaiLieus(new GetList_TaiLieu_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto { IdTaiLieus = new List<Guid> { input.IdTaiLieu } }
            });

            return new DisplayModal_CRUD_TaiLieu_Output_Dto
            {
                Loai = input.Loai,
                TaiLieu = taiLieus.FirstOrDefault() ?? new tbTaiLieuExtend { TaiLieu = new tbTaiLieu() }
            };
        }

        public async Task<bool> IsExisted_TaiLieu(tbTaiLieu taiLieu)
        {
            var existed = await _taiLieuRepo.Query()
                .FirstOrDefaultAsync(x =>
                    (x.FileNameUpdate == taiLieu.FileNameUpdate)
                    && x.IdFile != taiLieu.IdFile
                    && x.TrangThai != 0
                    && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return existed != null;
        }

        public async Task Create_TaiLieu(tbTaiLieuExtend taiLieu, HttpPostedFileBase files)
        {
            //if (baiDangs == null || !baiDangs.Any())
            //    throw new ArgumentException("Danh sách bài đăng không được để trống.");

            //var tepDinhKemMappings = new List<(tbBaiDang baiDang, List<tbTepDinhKem> teps)>();

            //// 1. Upload ảnh trước khi transaction
            //foreach (var baiDang_NEW in baiDangs)
            //{
            //    var tepList = new List<tbTepDinhKem>();

            //    if (files != null && (baiDang_NEW.BaiDang.TuTaoAnhAI == false))
            //    {
            //        for (int i = 0; i < files.Length; i++)
            //        {
            //            if (baiDang_NEW.RowNumber == rowNumbers[i])
            //            {
            //                var file = files[i];
            //                if (file == null || file.ContentLength <= 0) continue;

            //                var result = await UploadToFreeImageHost(file);
            //                if (result == null || result.StatusCode != 200)
            //                    throw new Exception("Upload ảnh thất bại hoặc không có phản hồi từ server.");

            //                var tep = new tbTepDinhKem
            //                {
            //                    IdTep = Guid.NewGuid(),
            //                    FileName = Path.GetFileNameWithoutExtension(file.FileName),
            //                    DuongDanTepOnline = result.Image.Url,
            //                    TrangThai = 1,
            //                    IdNguoiTao = CurrentUserId,
            //                    NgayTao = DateTime.Now,
            //                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
            //                };

            //                tepList.Add(tep);
            //            }
            //        }
            //    }
            //    var trangThaiDangBai = chuyenTrangThai_BaiDang(
            //            loai: loai,
            //            trangThaiBanDau: (int)TrangThaiDangBai_BaiDang.WaitToPost);
            //    var baiDang = new tbBaiDang
            //    {
            //        IdBaiDang = Guid.NewGuid(),
            //        IdChienDich = baiDang_NEW.BaiDang.IdChienDich,
            //        IdTaiKhoan = baiDang_NEW.BaiDang.IdTaiKhoan,
            //        NoiDung = baiDang_NEW.BaiDang.NoiDung,
            //        ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
            //        TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI,
            //        TrangThaiDangBai = trangThaiDangBai,
            //        TrangThai = 1,
            //        IdNguoiTao = CurrentUserId,
            //        NgayTao = DateTime.Now,
            //        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
            //    };
            //    tepDinhKemMappings.Add((baiDang, tepList));
            //}

            //// 2. Transaction: lưu bài đăng, file đính kèm và liên kết
            //await _unitOfWork.ExecuteInTransaction(async () =>
            //{
            //    foreach (var (baiDang, tepList) in tepDinhKemMappings)
            //    {
            //        await _unitOfWork.InsertAsync<tbBaiDang, Guid>(baiDang);

            //        foreach (var tep in tepList)
            //        {
            //            await _unitOfWork.InsertAsync<tbTepDinhKem, Guid>(tep);

            //            var baiDangTep = new tbBaiDangTepDinhKem
            //            {
            //                IdBaiDangTepDinhKem = Guid.NewGuid(),
            //                IdBaiDang = baiDang.IdBaiDang,
            //                IdTepDinhKem = tep.IdTep
            //            };

            //            await _unitOfWork.InsertAsync<tbBaiDangTepDinhKem, Guid>(baiDangTep);
            //        }
            //    }
            //});
        }

        public async Task Update_TaiLieu(tbTaiLieuExtend taiLieu)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
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

        public async Task Delete_TaiLieus(List<Guid> idTaiLieus)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var taiLieus_DELETE = await _taiLieuRepo.Query()
                    .Where(x => idTaiLieus.Contains(x.IdFile))
                    .ToListAsync();

                foreach (var _taiLieu in taiLieus_DELETE)
                {
                    _taiLieu.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _taiLieu.NgaySua = DateTime.Now;
                    _taiLieu.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbTaiLieu, Guid>(_taiLieu);
                }
            });
        }

        #region Private Methods
        private async Task createByZip() { }
        private async Task createByFile(CreateByFile_Input_Dto input)
        {
            if (input.TaiLieus == null || !input.TaiLieus.Any())
                throw new ArgumentException("Danh sách tài liệu không được để trống.");

            if (input.Files == null || input.Files.Length != input.TaiLieus.Count)
                throw new ArgumentException("Danh sách file phải khớp thứ tự với tài liệu.");

            var nhaCungCap = await _nhaCungCapRepo.GetByIdAsync(id: input.IdNhaCungCap);
            if (nhaCungCap == null)
                throw new ArgumentException("Không tồn tại nhà cung cấp.");

            string baseOnlineUrl = string.Format("{0}/{1}/TaiLieuNhaCungCap/{2}", "/Assets/upload", CurrentDonViSuDung.MaDonViSuDung, nhaCungCap.IdNhaCungCap);
            string rootPhysical = HostingEnvironment.MapPath(baseOnlineUrl);

            var taiLieuEntities = new List<tbTaiLieu>();

            // 1️⃣ XỬ LÝ FILE + TẠO ENTITY (ngoài transaction)
            for (int i = 0; i < input.TaiLieus.Count; i++)
            {
                var taiLieu_NEW = input.TaiLieus[i];
                var file = input.Files[i];

                if (file == null || file.ContentLength <= 0)
                    continue;

                // Chuẩn hoá tên file + đường dẫn
                var fileInfo = Public.Helpers.FileSaveInfoHelper.BuildFileSaveInfo(
                    file,
                    rootPhysical,
                    baseOnlineUrl,
                    separator: '-',        // có thể đổi thành '_'
                    addUniqueSuffix: true  // tránh trùng tên
                );

                // Lưu file vật lý
                file.SaveAs(fileInfo.PhysicalPath);

                var taiLieu = new tbTaiLieu
                {
                    IdFile = Guid.NewGuid(),
                    IdNhaCungCap = nhaCungCap.IdNhaCungCap,
                    NgayDangKy = taiLieu_NEW.TaiLieu.NgayDangKy,
                    NgayHetHan = taiLieu_NEW.TaiLieu.NgayHetHan,

                    FileName = fileInfo.FileNameWithoutExtension,
                    FileNameUpdate = fileInfo.SafeName,
                    FileExtension = fileInfo.Extension,
                    LoaiTep = fileInfo.MimeType,
                    DuongDanTepVatLy = fileInfo.PhysicalPath,
                    DuongDanTepOnline = fileInfo.OnlineUrl,

                    TrangThai = 1,
                    NgayTao = DateTime.Now,
                    IdNguoiTao = CurrentUserId,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung
                };

                taiLieuEntities.Add(taiLieu);
            }

            // 2️⃣ TRANSACTION – lưu DB
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var tl in taiLieuEntities)
                {
                    await _unitOfWork.InsertAsync<tbTaiLieu, Guid>(tl);
                }
            });
        }

        #endregion
    }
}

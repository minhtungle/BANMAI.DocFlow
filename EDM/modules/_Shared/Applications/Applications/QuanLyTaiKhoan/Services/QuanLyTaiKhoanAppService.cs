using Applications.QuanLyTaiKhoan.Dtos;
using Applications.QuanLyTaiKhoan.Enums;
using Applications.QuanLyTaiKhoan.Interfaces;
using Applications.QuanLyTaiKhoan.Models;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Applications.QuanLyTaiKhoan.Services
{
    public class QuanLyTaiKhoanAppService : BaseService, IQuanLyTaiKhoanAppService
    {
        private readonly IRepository<tbTaiKhoan, Guid> _taiKhoanRepo;
        private readonly IRepository<tbNenTang, Guid> _nenTangRepo;
        private readonly ISocialApiService _socialApiService;

        public QuanLyTaiKhoanAppService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbTaiKhoan, Guid> taiKhoanRepo,
            IRepository<tbNenTang, Guid> nenTangRepo,
            ISocialApiService socialApiService)
            : base(userContext, unitOfWork)
        {
            _taiKhoanRepo = taiKhoanRepo;
            _nenTangRepo = nenTangRepo;
            _socialApiService = socialApiService;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_OutPut_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyTaiKhoan");

            return new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }
        public async Task<List<tbTaiKhoanExtend>> GetTaiKhoans(
          string loai = "all",
          List<Guid> idTaiKhoans = null,
          LocThongTinDto locThongTin = null)
        {
            var query = _taiKhoanRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idTaiKhoans != null)
            {
                query = query.Where(x => idTaiKhoans.Contains(x.IdTaiKhoan));
            }
          ;

            var data = await (
               from tk in query

               join nt in _nenTangRepo.Query() on tk.IdNenTang equals nt.IdNenTang into ntGroup
               from nt in ntGroup.DefaultIfEmpty()

               select new tbTaiKhoanExtend
               {
                   TaiKhoan = tk,
                   NenTang = nt
               }
           )
           .OrderByDescending(x => x.TaiKhoan.NgayTao)
           .ToListAsync();

            return data;
        }
        public async Task<DisplayModel_CRUD_TaiKhoan_Output_Dto> DisplayModel_CRUD_TaiKhoan_Ouput(
            DisplayModel_CRUD_TaiKhoan_Input_Dto input)
        {
            var taiKhoan = await GetTaiKhoans(
                loai: "single",
                idTaiKhoans: new List<Guid> { input.IdTaiKhoan });
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyTaiKhoan");
            var nenTangs = await _nenTangRepo.Query()
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .OrderBy(x => x.Stt)
                .ToListAsync();

            return new DisplayModel_CRUD_TaiKhoan_Output_Dto
            {
                TaiKhoan = taiKhoan.FirstOrDefault() ?? new tbTaiKhoanExtend(),
                ThaoTacs = GetThaoTacs(maChucNang: "QuanLyTaiKhoan"),
                NenTangs = nenTangs,
                Loai = input.Loai,
            };
        }
        public async Task<tbNenTang> ChonNenTang(Guid input)
        {
            var nenTang = await _nenTangRepo.GetByIdAsync(input);
            if (nenTang == null)
                throw new Exception($"Không tìm thấy nền tảng với Id: {input}");
            return nenTang;
        }
        [HttpPost]
        public async Task<SocialUserWithPagesDto> GetUserAndPages(GetSocialInfo_Input_Dto input)
        {
            if (input == null || string.IsNullOrEmpty(input.AccessToken) || input.IdNenTang == null)
                throw new Exception("AccessToken và nền tảng là bắt buộc");

            var nenTang = await _nenTangRepo.GetByIdAsync(input.IdNenTang);
            if (nenTang == null)
                throw new Exception($"Không tìm thấy nền tảng với Id: {input.IdNenTang}");

            var result = await _socialApiService.GetUserAndPagesAsync(input: new SocialApi_Input_Dto
            {
                AccessToken = input.AccessToken,
                TenNenTang = nenTang.TenNenTang.ToLower()
            });
            return result;
        }
        [HttpPost]
        public async Task<SocialPageDto> GetPageInfo(GetSocialInfo_Input_Dto input)
        {
            if (input == null || string.IsNullOrEmpty(input.AccessToken) || input.IdNenTang == null)
                throw new Exception("AccessToken và nền tảng là bắt buộc");

            var nenTang = await _nenTangRepo.GetByIdAsync(input.IdNenTang);
            if (nenTang == null)
                throw new Exception($"Không tìm thấy nền tảng với Id: {input.IdNenTang}");

            var result = await _socialApiService.GetPageInfoAsync(input: new SocialApi_Input_Dto
            {
                AccessToken = input.AccessToken,
                TenNenTang = nenTang.TenNenTang.ToLower()
            });
            return result;
        }
        public async Task<(bool, string)> IsExisted_TaiKhoan(tbTaiKhoan taiKhoan)
        {
            var query = _taiKhoanRepo.Query()
                .Where(x => x.IdTaiKhoan != taiKhoan.IdTaiKhoan
                            && x.TrangThai != 0
                            && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);

            if (taiKhoan.LoaiTaiKhoan.ToLower() == "page")
            {
                query = query.Where(x => x.Page_Access_Token == taiKhoan.Page_Access_Token
                                      && x.Page_Id == taiKhoan.Page_Id);
                var _rs = await query.FirstOrDefaultAsync();
                return _rs != null
                    ? (false, "Tài khoản trang đã tồn tại")
                    : (true, string.Empty);
            }
            query = query.Where(x => x.User_Access_Token == taiKhoan.User_Access_Token
                                  && x.User_Id == taiKhoan.User_Id);
            return await query.FirstOrDefaultAsync() != null
                ? (false, "Tài khoản người dùng đã tồn tại")
                : (true, string.Empty);
        }
        public async Task<(bool, string, string)> Is_ValidToSave(tbTaiKhoan taiKhoan)
        {
            /**
             * Kiểm tra loại tài khoản
             *   1. Kiểm tra user
             *   2. Loại tài khoản là user
             *      2.1. Kiểm tra tồn tại trong db
             *   3. Loại tài khoản là page
             *      3.1. Kiểm tra page
             *      3.2. Kiểm tra page thuộc user
             *      3.3. Kiểm tra tồn tại trong db
             */

            // 1
            var user = await GetUserAndPages(new GetSocialInfo_Input_Dto
            {
                Id = taiKhoan.User_Id,
                AccessToken = taiKhoan.User_Access_Token,
                IdNenTang = taiKhoan.IdNenTang.Value,
                LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan
            });

            // 2
            if (user == null || user.User == null)
                return (false, "Thông tin người dùng không hợp lệ hoặc không tồn tại", "");
            // 2.1
            if (taiKhoan.LoaiTaiKhoan == "user")
            {
                var isExistedUser = await IsExisted_TaiKhoan(taiKhoan);
                return (isExistedUser.Item1, isExistedUser.Item2, user.User.Name);
            }

            // 3
            var page = await GetPageInfo(new GetSocialInfo_Input_Dto
            {
                Id = taiKhoan.Page_Id,
                AccessToken = taiKhoan.Page_Access_Token,
                IdNenTang = taiKhoan.IdNenTang.Value,
                LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan
            });
            // 3.1
            if (page == null || page.Id == null)
                return (false, "Thông tin trang không hợp lệ hoặc không tồn tại", "");
            // 3.2
            if (user.Pages == null || !user.Pages.Any(p => p.Id == page.Id))
                return (false, "Trang không thuộc về người dùng", "");
            // 3.3
            var isExistedPage = await IsExisted_TaiKhoan(taiKhoan);
            return (isExistedPage.Item1, isExistedPage.Item2, page.Name);
        }

        public async Task Create_TaiKhoan(tbTaiKhoan taiKhoan)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                // 4. Tạo entity
                var entity = new tbTaiKhoan
                {
                    IdTaiKhoan = Guid.NewGuid(),
                    Name = taiKhoan.Name,
                    IdNenTang = taiKhoan.IdNenTang,
                    LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan,
                    User_Id = taiKhoan.User_Id,
                    User_Access_Token = taiKhoan.User_Access_Token,
                    Page_Id = taiKhoan.Page_Id,
                    Page_Access_Token = taiKhoan.Page_Access_Token,
                    GhiChu = taiKhoan.GhiChu,

                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                };

                // 5. Lưu
                await _unitOfWork.InsertAsync<tbTaiKhoan, Guid>(entity);
            });
        }
        public async Task Update_TaiKhoan(tbTaiKhoan taiKhoan)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = await _taiKhoanRepo.GetByIdAsync(taiKhoan.IdTaiKhoan);
                if (entity == null)
                    throw new Exception($"Không tìm thấy AI Tool với Id: {taiKhoan.IdTaiKhoan}");

                entity.Name = taiKhoan.Name;
                entity.IdNenTang = taiKhoan.IdNenTang;
                entity.LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan;
                entity.User_Id = taiKhoan.User_Id;
                entity.User_Access_Token = taiKhoan.User_Access_Token;
                entity.Page_Id = taiKhoan.Page_Id;
                entity.Page_Access_Token = taiKhoan.Page_Access_Token;
                entity.GhiChu = taiKhoan.GhiChu;

                entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                entity.NgaySua = DateTime.Now;

                // 5. Lưu
                await _unitOfWork.UpdateAsync<tbTaiKhoan, Guid>(entity);
            });
        }
        public async Task Delete_TaiKhoan(List<Guid> idTaiKhoans)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var taiKhoans_DELETE = await _taiKhoanRepo.Query()
                    .Where(x => idTaiKhoans.Contains(x.IdTaiKhoan))
                    .ToListAsync();

                foreach (var entity in taiKhoans_DELETE)
                {
                    entity.TrangThai = 0;
                    entity.NgaySua = DateTime.Now;
                    entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbTaiKhoan, Guid>(entity);
                }
            });
        }
    }
}
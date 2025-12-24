using Applications.QuanLyTruongHoc.Dtos;
using Applications.QuanLyTruongHoc.Filters;
using Applications.QuanLyTruongHoc.Interfaces;
using Applications.QuanLyTruongHoc.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Applications.QuanLyTruongHoc.Services
{
    public class QuanLyTruongHocService : BaseService, IQuanLyTruongHocService
    {
        private readonly IRepository<tbTruongHoc, Guid> _truongHocRepo;

        public QuanLyTruongHocService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbTruongHoc, Guid> truongHocRepo
            ) : base(userContext, unitOfWork)
        {
            _truongHocRepo = truongHocRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);

        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyTruongHoc");
            // You can extend Index DTO with more data as needed
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs
            };
        }

        public async Task<List<tbTruongHocExtend>> Get_TruongHocs(GetList_TruongHoc_Input_Dto input)
        {
            var query = _truongHocRepo.Query()
              .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdTruongHocs != null && input.LocThongTin.IdTruongHocs.Any())
            {
                query = query.Where(x => input.LocThongTin.IdTruongHocs.Contains(x.IdTruongHoc));
            }

            // If input is used for filtering in the future, apply here
            var data = await query
                .Select(x => new tbTruongHocExtend
                {
                    TruongHoc = x
                })
                .OrderByDescending(x => x.TruongHoc.Stt)
                .ToListAsync();

            return data;
        }

        public async Task<DisplayModal_CRUD_TruongHoc_Output_Dto> DisplayModal_CRUD_TruongHoc(
            DisplayModal_CRUD_TruongHoc_Input_Dto input)
        {
            var truongHocs = await Get_TruongHocs(input: new GetList_TruongHoc_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdTruongHocs = new List<Guid> { input.IdTruongHoc }
                }
            });

            var output = new DisplayModal_CRUD_TruongHoc_Output_Dto
            {
                Loai = input.Loai,
                TruongHoc = truongHocs.FirstOrDefault() ?? new tbTruongHocExtend()
                {
                    TruongHoc = new tbTruongHoc()
                },
            };
            return output;
        }

        public async Task<bool> IsExisted_TruongHoc(tbTruongHoc truongHoc)
        {
            var existed = await _truongHocRepo.Query()
                .FirstOrDefaultAsync(x =>
                    x.TenTruongHoc == truongHoc.TenTruongHoc
                    && x.IdTruongHoc != truongHoc.IdTruongHoc
                    && x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                    && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return existed != null;
        }

        public async Task Create_TruongHoc(tbTruongHocExtend truongHoc)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                truongHoc.TruongHoc.TenVietTat = Public.Helpers.XuLyTenTruongHoc.ToCode(text: truongHoc.TruongHoc.TenTruongHoc);
                truongHoc.TruongHoc.Slug = Public.Helpers.XuLyTenTruongHoc.ToSlug(text: truongHoc.TruongHoc.TenTruongHoc);
                var entity = new tbTruongHoc
                {
                    IdTruongHoc = Guid.NewGuid(),
                    TenTruongHoc = truongHoc.TruongHoc.TenTruongHoc,
                    TenVietTat = truongHoc.TruongHoc.TenVietTat,
                    Slug = truongHoc.TruongHoc.Slug,
                    Email = truongHoc.TruongHoc.Email,
                    SoDienThoai = truongHoc.TruongHoc.SoDienThoai,
                    DiaChi = truongHoc.TruongHoc.DiaChi,
                    GhiChu = truongHoc.TruongHoc.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };

                await _unitOfWork.InsertAsync<tbTruongHoc, Guid>(entity);
            });
        }

        public async Task Update_TruongHoc(tbTruongHocExtend truongHoc)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _truongHoc = await _truongHocRepo.GetByIdAsync(truongHoc.TruongHoc.IdTruongHoc);
                if (_truongHoc == null)
                    throw new Exception("Trường học không tồn tại.");

                truongHoc.TruongHoc.TenVietTat = Public.Helpers.XuLyTenTruongHoc.ToCode(text: truongHoc.TruongHoc.TenTruongHoc);
                truongHoc.TruongHoc.Slug = Public.Helpers.XuLyTenTruongHoc.ToSlug(text: truongHoc.TruongHoc.TenTruongHoc);

                _truongHoc.TenTruongHoc = truongHoc.TruongHoc.TenTruongHoc;
                _truongHoc.TenVietTat = truongHoc.TruongHoc.TenVietTat;
                _truongHoc.Slug = truongHoc.TruongHoc.Slug;
                _truongHoc.Email = truongHoc.TruongHoc.Email;
                _truongHoc.SoDienThoai = truongHoc.TruongHoc.SoDienThoai;
                _truongHoc.DiaChi = truongHoc.TruongHoc.DiaChi;
                _truongHoc.GhiChu = truongHoc.TruongHoc.GhiChu;

                _truongHoc.NgaySua = DateTime.Now;
                _truongHoc.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                await _unitOfWork.UpdateAsync<tbTruongHoc, Guid>(_truongHoc);
            });
        }

        public async Task Delete_TruongHocs(List<Guid> idTruongHocs)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var truongHocs_DELETE = await _truongHocRepo.Query()
                    .Where(x => idTruongHocs.Contains(x.IdTruongHoc))
                    .ToListAsync();

                foreach (var _truongHoc in truongHocs_DELETE)
                {
                    _truongHoc.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _truongHoc.NgaySua = DateTime.Now;
                    _truongHoc.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbTruongHoc, Guid>(_truongHoc);
                }
            });
        }
    }
}
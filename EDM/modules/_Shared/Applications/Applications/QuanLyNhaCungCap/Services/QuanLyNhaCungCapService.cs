using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Filters;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Services
{
    public class QuanLyNhaCungCapService : BaseService, IQuanLyNhaCungCapService
    {
        private readonly IRepository<tbNhaCungCap, Guid> _nhaCungCapRepo;
        private readonly IRepository<tbNhaCungCapTruongHoc, Guid> _nhaCungCapTruongHocRepo;
        private readonly IRepository<tbTruongHoc, Guid> _truongHocRepo;
        private readonly IRepository<tbTaiLieuNhaCungCap, Guid> _taiLieuNhaCungCapRepo;

        public QuanLyNhaCungCapService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,

            IRepository<tbNhaCungCap, Guid> nhaCungCapRepo,
            IRepository<tbNhaCungCapTruongHoc, Guid> nhaCungCapTruongHocRepo,
            IRepository<tbTruongHoc, Guid> truongHocRepo,
            IRepository<tbTaiLieuNhaCungCap, Guid> taiLieuNhaCungCapRepo

            ) : base(userContext, unitOfWork)
        {
            _nhaCungCapRepo = nhaCungCapRepo;
            _nhaCungCapTruongHocRepo = nhaCungCapTruongHocRepo;
            _truongHocRepo = truongHocRepo;
            _taiLieuNhaCungCapRepo = taiLieuNhaCungCapRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
            var nhaCungCaps = await Get_NhaCungCaps(input: new GetList_NhaCungCap_Input_Dto());
            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region Nhà cung cấp
        public async Task<List<tbNhaCungCapExtend>> Get_NhaCungCaps(GetList_NhaCungCap_Input_Dto input)
        {
            var query = _nhaCungCapRepo.Query()
               .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdNhaCungCaps != null && input.LocThongTin.IdNhaCungCaps.Any())
            {
                query = query.Where(x => input.LocThongTin.IdNhaCungCaps.Contains(x.IdNhaCungCap));
            }

            // 1) Đếm tài liệu theo NCC
            var taiLieuCount =
                from tl in _taiLieuNhaCungCapRepo.Query()
                group tl by tl.IdNhaCungCap into g
                select new
                {
                    IdNhaCungCap = g.Key,
                    Cnt = g.Count()
                };

            // 2) Đếm trường học theo NCC
            var truongHocCount =
                from th in _nhaCungCapTruongHocRepo.Query()
                group th by th.IdNhaCungCap into g
                select new
                {
                    IdNhaCungCap = g.Key,
                    Cnt = g.Count()
                };

            // 3) Join NCC + 2 bảng đếm (LEFT JOIN)
            var data = await
            (
                from ncc in query

                join tlc in taiLieuCount
                    on ncc.IdNhaCungCap equals tlc.IdNhaCungCap into tlcLeft
                from tlc in tlcLeft.DefaultIfEmpty()

                join thc in truongHocCount
                    on ncc.IdNhaCungCap equals thc.IdNhaCungCap into thcLeft
                from thc in thcLeft.DefaultIfEmpty()

                select new tbNhaCungCapExtend
                {
                    NhaCungCap = ncc,
                    SoLuongTaiLieu = tlc == null ? 0 : tlc.Cnt,
                    SoLuongTruongHoc = thc == null ? 0 : thc.Cnt
                }
            )
            .OrderByDescending(x => x.NhaCungCap.Stt)
            .ToListAsync();

            return data;
        }
        public async Task<DisplayModal_CRUD_NhaCungCap_Output_Dto> DisplayModal_CRUD_NhaCungCap(
            DisplayModal_CRUD_NhaCungCap_Input_Dto input)
        {
            var nhaCungCaps = await Get_NhaCungCaps(input: new GetList_NhaCungCap_Input_Dto
            {
                Loai = "single",
                LocThongTin = new LocThongTinDto
                {
                    IdNhaCungCaps = new List<Guid> { input.IdNhaCungCap }
                }
            });

            var truongHocs = await _truongHocRepo.Query()
                .Where(x => x.TrangThai != (int)TrangThaiDuLieuEnum.XoaBo
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var output = new DisplayModal_CRUD_NhaCungCap_Output_Dto
            {
                Loai = input.Loai,
                NhaCungCap = nhaCungCaps.FirstOrDefault() ?? new tbNhaCungCapExtend()
                {
                    NhaCungCap = new tbNhaCungCap()
                },
                TruongHocs = truongHocs,
            };
            return output;
        }
        public async Task<bool> IsExisted_NhaCungCap(tbNhaCungCap nhaCungCap)
        {
            var nhaCungCap_OLD = await _nhaCungCapRepo.Query()
                .FirstOrDefaultAsync(x =>
                    x.MaNhaCungCap == nhaCungCap.MaNhaCungCap
                    && (x.SoDienThoai == nhaCungCap.SoDienThoai || x.Email == nhaCungCap.Email)
                    && x.IdNhaCungCap != nhaCungCap.IdNhaCungCap
                    && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return nhaCungCap_OLD != null;
        }
        public async Task Create_NhaCungCap(tbNhaCungCapExtend nhaCungCap)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _nhaCungCap = new tbNhaCungCap
                {
                    IdNhaCungCap = Guid.NewGuid(),
                    MaNhaCungCap = nhaCungCap.NhaCungCap.MaNhaCungCap,
                    TenNhaCungCap = nhaCungCap.NhaCungCap.TenNhaCungCap,
                    TenMatHang = nhaCungCap.NhaCungCap.TenMatHang,
                    SoDienThoai = nhaCungCap.NhaCungCap.SoDienThoai,
                    Email = nhaCungCap.NhaCungCap.Email,
                    DiaChi = nhaCungCap.NhaCungCap.DiaChi,
                    GhiChu = nhaCungCap.NhaCungCap.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now
                };
                await _unitOfWork.InsertAsync<tbNhaCungCap, Guid>(_nhaCungCap);

                foreach (var truongHoc in nhaCungCap.TruongHocs)
                {
                    var _nhaCungCapTruongHoc = new tbNhaCungCapTruongHoc
                    {
                        IdNhaCungCapTruongHoc = Guid.NewGuid(),
                        IdTruongHoc = truongHoc.IdTruongHoc,
                        IdNhaCungCap = _nhaCungCap.IdNhaCungCap,

                        TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now,
                    };
                    await _unitOfWork.InsertAsync<tbNhaCungCapTruongHoc, Guid>(_nhaCungCapTruongHoc);
                }
                //foreach (var truongHoc in nhaCungCap.TruongHocs)
                //{
                //    truongHoc.TenVietTat = Public.Handles.Handle.ToCode(text: truongHoc.TenTruongHoc);
                //    truongHoc.Slug = Public.Handles.Handle.ToSlug(text: truongHoc.Slug);
                //    var _truongHoc = new tbTruongHoc
                //    {
                //        IdTruongHoc = Guid.NewGuid(),
                //        TenTruongHoc = truongHoc.TenTruongHoc,
                //        TenVietTat = truongHoc.TenVietTat,
                //        Slug = truongHoc.Slug,
                //        SoDienThoai = truongHoc.SoDienThoai,
                //        Email = truongHoc.Email,
                //        DiaChi = truongHoc.DiaChi,
                //        GhiChu = truongHoc.GhiChu,

                //        TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                //        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                //        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                //        NgayTao = DateTime.Now,
                //    };
                //    await _unitOfWork.InsertAsync<tbTruongHoc, Guid>(_truongHoc);
                //}
                ;
            });
        }
        public async Task Update_NhaCungCap(tbNhaCungCapExtend nhaCungCap)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var _nhaCungCap = await _nhaCungCapRepo.GetByIdAsync(nhaCungCap.NhaCungCap.IdNhaCungCap);

                if (_nhaCungCap == null)
                    throw new Exception("Nhà cung cấp không tồn tại.");

                // Cập nhật thông tin chính
                {
                    _nhaCungCap.MaNhaCungCap = nhaCungCap.NhaCungCap.MaNhaCungCap;
                    _nhaCungCap.TenNhaCungCap = nhaCungCap.NhaCungCap.TenNhaCungCap;
                    _nhaCungCap.TenMatHang = nhaCungCap.NhaCungCap.TenMatHang;
                    _nhaCungCap.SoDienThoai = nhaCungCap.NhaCungCap.SoDienThoai;
                    _nhaCungCap.Email = nhaCungCap.NhaCungCap.Email;
                    _nhaCungCap.DiaChi = nhaCungCap.NhaCungCap.DiaChi;
                    _nhaCungCap.GhiChu = nhaCungCap.NhaCungCap.GhiChu;

                    _nhaCungCap.NgaySua = DateTime.Now;
                    _nhaCungCap.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbNhaCungCap, Guid>(_nhaCungCap);
                }
                ;

                // Cập nhật thông tin người thân
                {
                    var truongHocs_OLD = await _nhaCungCapTruongHocRepo.Query()
                        .Where(x => x.IdNhaCungCap == _nhaCungCap.IdNhaCungCap)
                        .ToListAsync(); // Danh sách hiện có

                    var truognHocs_DELETE = truongHocs_OLD
                        .Where(x => !nhaCungCap.TruongHocs.Any(y => y.IdTruongHoc == x.IdTruongHoc))
                        .ToList(); // Danh sách cần xóa

                    foreach (var truongHoc in truognHocs_DELETE)
                    {
                        await _unitOfWork.DeleteAsync<tbNhaCungCapTruongHoc, Guid>(truongHoc);
                    }
                    ;

                    foreach (var truongHoc in nhaCungCap.TruongHocs)
                    {
                        var isExisted = truongHocs_OLD
                            .Any(y => y.IdTruongHoc == truongHoc.IdTruongHoc);
                        if (!isExisted)
                        {
                            var _nhaCungCapTruongHoc = new tbNhaCungCapTruongHoc
                            {
                                IdNhaCungCapTruongHoc = Guid.NewGuid(),
                                IdTruongHoc = truongHoc.IdTruongHoc,
                                IdNhaCungCap = _nhaCungCap.IdNhaCungCap,

                                TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                                MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                                IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                            };
                            await _unitOfWork.InsertAsync<tbNhaCungCapTruongHoc, Guid>(_nhaCungCapTruongHoc);
                        }
                        ;
                    }
                    ;
                }
                ;
            });
        }
        public async Task Delete_NhaCungCaps(List<Guid> idNhaCungCaps)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var nhaCungCaps_DELETE = await _nhaCungCapRepo.Query()
                    .Where(x => idNhaCungCaps.Contains(x.IdNhaCungCap))
                    .ToListAsync();

                foreach (var _nhaCungCap in nhaCungCaps_DELETE)
                {
                    _nhaCungCap.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _nhaCungCap.NgaySua = DateTime.Now;
                    _nhaCungCap.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbNhaCungCap, Guid>(_nhaCungCap);

                    var truongHocs_OLD = await _nhaCungCapTruongHocRepo.Query()
                      .Where(x => x.IdNhaCungCap == _nhaCungCap.IdNhaCungCap)
                      .ToListAsync(); // Danh sách hiện có

                    foreach (var truongHoc in truongHocs_OLD)
                    {
                        truongHoc.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                        truongHoc.NgaySua = DateTime.Now;
                        truongHoc.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                        await _unitOfWork.UpdateAsync<tbNhaCungCapTruongHoc, Guid>(truongHoc);
                    }
                    ;

                    // Xóa lịch hẹn + bệnh án ... nếu có
                }
            });
        }
        #endregion

        #region Xem chi tiết
        //public async Task<XemChiTiet_NhaCungCap_Output_Dto> XemChiTiet_NhaCungCap(Guid idNhaCungCap)
        //{
        //    var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
        //    var nhaCungCaps = await Get_NhaCungCaps(input: new GetList_NhaCungCap_Input_Dto()) ?? new List<tbNhaCungCapExtend>();
        //    var nhaCungCap = nhaCungCaps.FirstOrDefault(x => x.NhaCungCap.IdNhaCungCap == idNhaCungCap) ?? new tbNhaCungCapExtend();
        //    var output = new XemChiTiet_NhaCungCap_Output_Dto
        //    {
        //        ThaoTacs = thaoTacs,
        //        NhaCungCaps = nhaCungCaps,
        //        NhaCungCap = nhaCungCap,
        //    };
        //    return output;
        //}
        //public async Task<ShowTab_ThongTinCoBan_Output_Dto> ShowTab_ThongTinCoBan_NhaCungCap(Guid idNhaCungCap)
        //{
        //    var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
        //    var nhaCungCaps = await Get_NhaCungCaps(input: new GetList_NhaCungCap_Input_Dto
        //    {
        //        Loai = "single",
        //        LocThongTin = new LocThongTinDto
        //        {
        //            IdNhaCungCaps = new List<Guid?> { idNhaCungCap }
        //        }
        //    });
        //    var output = new ShowTab_ThongTinCoBan_Output_Dto
        //    {
        //        ThaoTacs = thaoTacs,
        //        NhaCungCap = nhaCungCaps.FirstOrDefault() ?? new tbNhaCungCapExtend()
        //        {
        //            NhaCungCap = new tbNhaCungCap()
        //        },
        //    };
        //    return output;
        //}
        //public async Task<ShowTab_LichHen_Output_Dto> ShowTab_LichHen_NhaCungCap(Guid idNhaCungCap)
        //{
        //    var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");
        //    var lichHens = await _lichHenRepo.Query()
        //        .Where(x =>
        //        x.IdNhaCungCap == idNhaCungCap
        //        && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
        //        .OrderByDescending(x => x.NgayHen)
        //        .ToListAsync();

        //    var output = new ShowTab_LichHen_Output_Dto
        //    {
        //        ThaoTacs = thaoTacs,
        //        LichHens = lichHens,
        //    };
        //    return output;
        //}
        //public async Task<ShowTab_PhieuKham_Output_Dto> ShowTab_PhieuKham_NhaCungCap(Guid idNhaCungCap)
        //{
        //    var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");

        //    // 1) Lấy phiếu + lịch điều trị
        //    var baseData = await _phieuKhamRepo.Query().AsNoTracking()
        //        .Where(pk => pk.IdNhaCungCap == idNhaCungCap
        //            && pk.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
        //            && pk.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
        //        .Select(pk => new ShowTab_PhieuKham_Output_Dto
        //        {
        //            PhieuKham = pk,
        //        })
        //        .FirstOrDefaultAsync();

        //    if (baseData == null)
        //        return new ShowTab_PhieuKham_Output_Dto { ThaoTacs = thaoTacs };

        //    baseData.LichDieuTris = await _lichDieuTriRepo.Query().AsNoTracking()
        //                .Where(ldt => ldt.IdPhieuKham == baseData.PhieuKham.IdPhieuKham
        //                    && ldt.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung)
        //                .Select(x => new tbLichDieuTriExtend
        //                {
        //                    LichDieuTri = x,
        //                })
        //                .OrderByDescending(x => x.LichDieuTri.NgayDieuTri) // tuỳ field
        //                .ToListAsync();

        //    var lichIds = baseData.LichDieuTris.Select(x => x.LichDieuTri.IdLichDieuTri).ToList();

        //    var allTienTrinhs =
        //    await (from tt in _tienTrinhDieuTriRepo.Query().AsNoTracking()
        //           where tt.IdLichDieuTri.HasValue
        //                 && lichIds.Contains(tt.IdLichDieuTri.Value)
        //                 && tt.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung

        //           // LEFT JOIN bác sỹ điều trị
        //           join bsDieuTri in _bacSyRepo.Query().AsNoTracking()
        //               on tt.IdBacSy equals bsDieuTri.IdBacSy into gBsDieuTri
        //           from bsDieuTri in gBsDieuTri.DefaultIfEmpty()

        //               // LEFT JOIN phụ tá
        //           join phuTa in _bacSyRepo.Query().AsNoTracking()
        //               on tt.IdPhuTa equals phuTa.IdBacSy into gPhuTa
        //           from phuTa in gPhuTa.DefaultIfEmpty()

        //               // LEFT JOIN tình trạng răng
        //           join tinhTrangRang in _tinhTrangRangRepo.Query().AsNoTracking()
        //               on tt.IdTinhTrangRang equals tinhTrangRang.IdTinhTrangRang into gTinhTrangRang
        //           from tinhTrangRang in gTinhTrangRang.DefaultIfEmpty()

        //               // LEFT JOIN thủ thuật
        //           join thuThuat in _thuThuatRepo.Query().AsNoTracking()
        //               on tt.IdThuThuat equals thuThuat.IdThuThuat into gThuThuat
        //           from thuThuat in gThuThuat.DefaultIfEmpty()

        //           orderby tt.Stt descending
        //           select new tbTienTrinhDieuTriExtend
        //           {
        //               TienTrinhDieuTri = tt,
        //               BacSyDieuTri = bsDieuTri,  // có thể null
        //               PhuTa = phuTa,         // có thể null
        //               TinhTrangRang = tinhTrangRang,
        //               ThuThuat = thuThuat,
        //           })
        //    .ToListAsync();

        //    // 1) Lookup: IdLichDieuTri -> List<TienTrinh>
        //    var ttLookup = allTienTrinhs
        //        .Where(t => t.TienTrinhDieuTri?.IdLichDieuTri != null)                 // tránh null
        //        .GroupBy(t => t.TienTrinhDieuTri.IdLichDieuTri.Value)                 // Guid? -> Guid
        //        .ToDictionary(
        //            g => g.Key,
        //            g => g.Select(t => new tbTienTrinhDieuTriExtend                    // clone an toàn
        //            {
        //                TienTrinhDieuTri = t.TienTrinhDieuTri,
        //                BacSyDieuTri = t.BacSyDieuTri,
        //                PhuTa = t.PhuTa,
        //                TinhTrangRang = t.TinhTrangRang,
        //                ThuThuat = t.ThuThuat
        //            }).ToList()
        //        );

        //    // 2) Build result an toàn null (baseData hoặc LichDieuTris có thể null)
        //    var result = new ShowTab_PhieuKham_Output_Dto
        //    {
        //        PhieuKham = baseData?.PhieuKham,
        //        ThaoTacs = thaoTacs,
        //        LichDieuTris = (baseData?.LichDieuTris ?? new List<tbLichDieuTriExtend>())
        //            .Select(ldt =>
        //            {
        //                var idLdt = ldt?.LichDieuTri?.IdLichDieuTri ?? Guid.Empty;     // phòng null
        //                ttLookup.TryGetValue(idLdt, out var ttList);
        //                return new tbLichDieuTriExtend
        //                {
        //                    LichDieuTri = ldt.LichDieuTri,
        //                    TienTrinhDieuTris = ttList ?? new List<tbTienTrinhDieuTriExtend>(),
        //                    //AnhMoTas = (anhLookup.TryGetValue(idLdt, out var anh) ? anh : new List<tbLichDieuTri_AnhMoTa>())
        //                };
        //            })
        //            .ToList()
        //    };


        //    return result;
        //}

        #endregion
    }
}
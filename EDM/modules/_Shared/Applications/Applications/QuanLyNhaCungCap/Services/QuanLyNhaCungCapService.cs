using Applications.QuanLyNhaCungCap.Dtos;
using Applications.QuanLyNhaCungCap.Enums;
using Applications.QuanLyNhaCungCap.Filters;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using Applications.QuanLyNhaCungCap.Validations;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Dtos;
using Public.Enums;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Services {
    public class QuanLyNhaCungCapService : BaseService, IQuanLyNhaCungCapService {
        private readonly IRepository<tbNhaCungCap, Guid> _nhaCungCapRepo;
        private readonly IRepository<tbNhaCungCapTruongHoc, Guid> _nhaCungCapTruongHocRepo;
        private readonly IRepository<tbTruongHoc, Guid> _truongHocRepo;
        private readonly IRepository<tbTaiLieu, Guid> _taiLieuRepo;
        public QuanLyNhaCungCapService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,

            IRepository<tbNhaCungCap, Guid> nhaCungCapRepo,
            IRepository<tbNhaCungCapTruongHoc, Guid> nhaCungCapTruongHocRepo,
            IRepository<tbTruongHoc, Guid> truongHocRepo,
            IRepository<tbTaiLieu, Guid> taiLieuRepo
            ) : base(userContext, unitOfWork) {
            _nhaCungCapRepo = nhaCungCapRepo;
            _nhaCungCapTruongHocRepo = nhaCungCapTruongHocRepo;
            _truongHocRepo = truongHocRepo;
            _taiLieuRepo = taiLieuRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_Output_Dto> Index() {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNhaCungCap");

            var truongHocs = await _truongHocRepo.Query()
              .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
              && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
              .ToListAsync();

            var nhaCungCaps = await _nhaCungCapRepo.Query()
                .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            return new Index_Output_Dto {
                ThaoTacs = thaoTacs,
                NhaCungCaps = nhaCungCaps,
                TruongHocs = truongHocs,
            };
        }

        #region Nhà cung cấp
        /// <summary>
        /// Retrieves a list of suppliers with extended information based on the specified filter criteria.
        /// </summary>
        /// <remarks>The returned list includes, for each supplier, the total number of associated
        /// documents and schools. Results are ordered by the supplier's display order in descending order.</remarks>
        /// <param name="input">An object containing filter options and selection criteria for retrieving suppliers. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of extended supplier
        /// objects matching the filter criteria. The list is empty if no suppliers are found.</returns>
        public async Task<List<tbNhaCungCapExtend>> Get_NhaCungCaps(GetList_NhaCungCap_Input_Dto input) {
            var query = _nhaCungCapRepo.Query()
               .ApplyFilters(input.LocThongTin, CurrentDonViSuDung.MaDonViSuDung);

            if (input.Loai == "single" && input.LocThongTin.IdNhaCungCaps != null && input.LocThongTin.IdNhaCungCaps.Any()) {
                query = query.Where(x => input.LocThongTin.IdNhaCungCaps.Contains(x.IdNhaCungCap));
            }

            // 1) Đếm tài liệu theo NCC
            var taiLieuCount =
                from tl in _taiLieuRepo.Query()
                group tl by tl.IdNhaCungCap into g
                select new {
                    IdNhaCungCap = g.Key,
                    Cnt = g.Count()
                };

            // 2) Đếm trường học theo NCC
            var truongHocCount =
                from th in _nhaCungCapTruongHocRepo.Query()
                group th by th.IdNhaCungCap into g
                select new {
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

                join nccc in _nhaCungCapRepo.Query()
                    on ncc.IdNhaCungCapCha equals nccc.IdNhaCungCap into ncccGroup
                from nccc in ncccGroup.DefaultIfEmpty()

                select new tbNhaCungCapExtend {
                    NhaCungCap = ncc,
                    NhaCungCapCha = nccc,
                    SoLuongTaiLieu = tlc == null ? 0 : tlc.Cnt,
                    SoLuongTruongHoc = thc == null ? 0 : thc.Cnt
                }
            )
            .OrderByDescending(x => x.NhaCungCap.Stt)
            .ToListAsync();

            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FormAddNhaCungCapDto> AddBanGhi_Modal_CRUD_Output(AddBanGhi_Modal_CRUD_Input_Dto input) {
            var truongHocs = await _truongHocRepo.Query()
                .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var nhaCungCaps = await _nhaCungCapRepo.Query()
                .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var output = new AddBanGhi_Modal_CRUD_Output_Dto {
                Loai = input.Loai,
            };
            if (input.Loai == "create") {
                output.NhaCungCaps = new List<tbNhaCungCapExtend>
                {
                    new tbNhaCungCapExtend()
                    {
                        NhaCungCap = new tbNhaCungCap(),
                    }
                };
            }
            else if (input.Loai == "update" || input.Loai == "detail") {
                var nhaCungCap = await GetDetail_NhaCungCaps(idNhaCungCaps: input.IdNhaCungCaps);
                // Chỉ lấy những bài đăng có trạng thái (chờ đăng)
                output.NhaCungCaps = nhaCungCap.ToList();
            }
            //else if (input.Loai == "draftToSave")
            //{
            //    var nhaCungCap = await GetDetail_NhaCungCaps(idNhaCungCaps: input.IdNhaCungCaps);
            //    // Chỉ lấy những bài đăng có trạng thái (nháp)
            //    output.NhaCungCaps = nhaCungCap
            //        .Where(x => x.NhaCungCap.TrangThaiDangBai == (int?)TrangThaiDangBai_NhaCungCap.Draft)
            //        .ToList();
            //}

            return new FormAddNhaCungCapDto {
                Data = output,
                NhaCungCaps = nhaCungCaps,
                TruongHocs = truongHocs,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idNhaCungCaps"></param>
        /// <returns></returns>
        public async Task<List<tbNhaCungCapExtend>> GetDetail_NhaCungCaps(List<Guid> idNhaCungCaps = null) {
            var nhaCungCaps = await _nhaCungCapRepo.Query()
                .Where(x =>
                    idNhaCungCaps.Contains(x.IdNhaCungCap) &&
                    x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung &&
                    x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .GroupJoin(_taiLieuRepo.Query(),
                ncc => ncc.IdNhaCungCap,
                tl => tl.IdNhaCungCap,
                (ncc, tl) => new tbNhaCungCapExtend {
                    NhaCungCap = ncc,
                    TaiLieus = tl.ToList(),
                })
                .ToListAsync();

            return nhaCungCaps;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DisplayModal_CRUD_NhaCungCap_Output_Dto> DisplayModal_CRUD_NhaCungCap(
            DisplayModal_CRUD_NhaCungCap_Input_Dto input) {
            var nhaCungCaps = await Get_NhaCungCaps(input: new GetList_NhaCungCap_Input_Dto {
                Loai = "single",
                LocThongTin = new LocThongTinDto {
                    IdNhaCungCaps = new List<Guid> { input.IdNhaCungCap }
                }
            });

            var truongHocs = await _truongHocRepo.Query()
                .Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung
                && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung)
                .ToListAsync();

            var output = new DisplayModal_CRUD_NhaCungCap_Output_Dto {
                Loai = input.Loai,
                NhaCungCap = nhaCungCaps.FirstOrDefault() ?? new tbNhaCungCapExtend() {
                    NhaCungCap = new tbNhaCungCap()
                },
                NhaCungCaps = nhaCungCaps.Where(x => x.NhaCungCap.IdNhaCungCap != input.IdNhaCungCap).ToList(), // Loại trừ chính nó
                TruongHocs = truongHocs,
            };
            return output;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="nhaCungCaps"></param>
        /// <returns></returns>
        public async Task Create_NhaCungCap(List<tbNhaCungCapExtend> nhaCungCaps) {
            await _unitOfWork.ExecuteInTransaction(async () => {
                foreach (var nhaCungCap in nhaCungCaps) {
                    var _nhaCungCap = new tbNhaCungCap {
                        IdNhaCungCap = Guid.NewGuid(),
                        IdNhaCungCapCha = nhaCungCap.NhaCungCap.IdNhaCungCapCha,
                        //MaNhaCungCap = nhaCungCap.NhaCungCap.MaNhaCungCap,
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

                    foreach (var truongHoc in nhaCungCap.TruongHocs) {
                        var _nhaCungCapTruongHoc = new tbNhaCungCapTruongHoc {
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
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nhaCungCap"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Update_NhaCungCap(List<tbNhaCungCapExtend> nhaCungCaps) {
            await _unitOfWork.ExecuteInTransaction(async () => {
                foreach (var nhaCungCap in nhaCungCaps) {

                    var _nhaCungCap = await _nhaCungCapRepo.Query()
                    .FirstOrDefaultAsync(x => 
                    (x.IdNhaCungCap == nhaCungCap.NhaCungCap.IdNhaCungCap
                    || x.TenNhaCungCap == nhaCungCap.NhaCungCap.TenNhaCungCap)); // Sai vì có thể đang cần cập nhật tên

                    if (_nhaCungCap == null)
                        throw new Exception("Nhà cung cấp không tồn tại.");

                    // Cập nhật thông tin chính
                    {
                        _nhaCungCap.IdNhaCungCapCha = nhaCungCap.NhaCungCap.IdNhaCungCapCha;
                        //_nhaCungCap.MaNhaCungCap = nhaCungCap.NhaCungCap.MaNhaCungCap;
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

                        foreach (var truongHoc in truognHocs_DELETE) {
                            await _unitOfWork.DeleteAsync<tbNhaCungCapTruongHoc, Guid>(truongHoc);
                        }
                        ;

                        foreach (var truongHoc in nhaCungCap.TruongHocs) {
                            var isExisted = truongHocs_OLD
                                .Any(y => y.IdTruongHoc == truongHoc.IdTruongHoc);
                            if (!isExisted) {
                                var _nhaCungCapTruongHoc = new tbNhaCungCapTruongHoc {
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
                }
                ;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idNhaCungCaps"></param>
        /// <returns></returns>
        public async Task Delete_NhaCungCaps(List<Guid> idNhaCungCaps) {
            await _unitOfWork.ExecuteInTransaction(async () => {
                var nhaCungCaps_DELETE = await _nhaCungCapRepo.Query()
                    .Where(x => idNhaCungCaps.Contains(x.IdNhaCungCap))
                    .ToListAsync();

                foreach (var _nhaCungCap in nhaCungCaps_DELETE) {
                    _nhaCungCap.TrangThai = (int?)TrangThaiDuLieuEnum.XoaBo;
                    _nhaCungCap.NgaySua = DateTime.Now;
                    _nhaCungCap.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbNhaCungCap, Guid>(_nhaCungCap);

                    var truongHocs_OLD = await _nhaCungCapTruongHocRepo.Query()
                      .Where(x => x.IdNhaCungCap == _nhaCungCap.IdNhaCungCap)
                      .ToListAsync(); // Danh sách hiện có

                    foreach (var truongHoc in truongHocs_OLD) {
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
    }
}
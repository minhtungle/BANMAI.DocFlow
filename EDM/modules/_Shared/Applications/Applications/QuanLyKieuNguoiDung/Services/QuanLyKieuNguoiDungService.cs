using Applications.QuanLyKieuNguoiDung.Dtos;
using Applications.QuanLyKieuNguoiDung.Interfaces;
using Applications.QuanLyKieuNguoiDung.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Applications.QuanLyKieuNguoiDung.Services
{
    public class QuanLyKieuNguoiDungService : BaseService, IQuanLyKieuNguoiDungService
    {
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/QuanLyKieuNguoiDung";
        private readonly IRepository<tbKieuNguoiDung, Guid> _kieuNguoiDungRepo;
        private readonly IRepository<tbNguoiDungKieuNguoiDung, Guid> _nguoiDungKieuNguoiDungRepo;
        private readonly IRepository<default_tbChucNang, Guid> _chucNangRepo;
        private readonly IRepository<default_tbChucNang_ThaoTac, Guid> _thaoTacRepo;

        public QuanLyKieuNguoiDungService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbKieuNguoiDung, Guid> kieuNguoiDungRepo,
            IRepository<tbNguoiDungKieuNguoiDung, Guid> nguoiDungKieuNguoiDungRepo,
            IRepository<default_tbChucNang, Guid> chucNangRepo,
            IRepository<default_tbChucNang_ThaoTac, Guid> thaoTacRepo
            )
            : base(userContext, unitOfWork)
        {
            _kieuNguoiDungRepo = kieuNguoiDungRepo;
            _nguoiDungKieuNguoiDungRepo = nguoiDungKieuNguoiDungRepo;
            _chucNangRepo = chucNangRepo;
            _thaoTacRepo = thaoTacRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<DisplayModal_CRUD_KieuNguoiDung_Output_Dto> DisplayModal_CRUD_NguoiDung(DisplayModal_CRUD_KieuNguoiDung_Input_Dto input)
        {
            var kieuNguoiDungs = new List<tbKieuNguoiDung>();
            if (input.Loai != "create" && input.IdKieuNguoiDung != Guid.Empty)
                kieuNguoiDungs = await Get_KieuNguoiDungs(
                    loai: "single",
                    idKieuNguoiDungs: new List<Guid> { input.IdKieuNguoiDung });

            var output = new DisplayModal_CRUD_KieuNguoiDung_Output_Dto
            {
                Loai = input.Loai,
                KieuNguoiDung = kieuNguoiDungs.FirstOrDefault() ?? new tbKieuNguoiDung(),
            };
            return output;
        }
        public async Task<Index_Output_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNguoiDung");
            var kieuNguoiDungs = await _kieuNguoiDungRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == Guid.Empty).ToListAsync(); // Lấy trong kho chung

            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
                KieuNguoiDungs = kieuNguoiDungs,
            };
        }
        public async Task<List<tbKieuNguoiDung>> Get_KieuNguoiDungs(
            string loai = "all",
            List<Guid> idKieuNguoiDungs = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _kieuNguoiDungRepo.Query()
               .Where(x =>
                   x.TrangThai != 0); // Lấy tất cả người dùng

            // Áp dụng lọc trước khi join để tối ưu
            if (locThongTin != null)
            {
                //if (!string.IsNullOrWhiteSpace(locThongTin.NoiDung))
                //    query = query.Where(x => x.NoiDung.Contains(locThongTin.NoiDung));

                //if (locThongTin.IdChienDich.HasValue)
                //    query = query.Where(x => x.IdChienDich == locThongTin.IdChienDich.Value);

                //if (locThongTin.TrangThaiDangBai.HasValue)
                //    query = query.Where(x => x.TrangThaiDangBai == locThongTin.TrangThaiDangBai.Value);

                //if (locThongTin.IdNguoiTao.HasValue)
                //    query = query.Where(x => x.IdNguoiTao == locThongTin.IdNguoiTao.Value);

                ////if (locThongTin.IdNenTang.HasValue)
                ////    query = query.Where(x => x.IdNenTang == locThongTin.IdNenTang.Value);

                //var ngayTaoRange = DateHelper.ParseThangNam(locThongTin.NgayTao);
                //if (ngayTaoRange.Start.HasValue && ngayTaoRange.End.HasValue)
                //{
                //    query = query.Where(x =>
                //        x.NgayTao >= ngayTaoRange.Start.Value &&
                //        x.NgayTao <= ngayTaoRange.End.Value);
                //}

                //var ngayDangRange = DateHelper.ParseThangNam(locThongTin.NgayDangBai);
                //if (ngayDangRange.Start.HasValue && ngayDangRange.End.HasValue)
                //{
                //    query = query.Where(x =>
                //        x.ThoiGian.HasValue &&
                //        x.ThoiGian.Value >= ngayDangRange.Start.Value &&
                //        x.ThoiGian.Value <= ngayDangRange.End.Value);
                //}

            }

            if (loai == "single" && idKieuNguoiDungs != null && idKieuNguoiDungs.Any())
            {
                query = query.Where(x => idKieuNguoiDungs.Contains(x.IdKieuNguoiDung));
            }

            // var data = await (
            //    from nd in query

            //    join knd in _kieuNguoiDungRepo.Query() on nd.IdKieuNguoiDung equals knd.IdKieuNguoiDung into kndGroup
            //    from knd in kndGroup.DefaultIfEmpty()

            //    select new tbKieuNguoiDung
            //    {
            //        NguoiDung = nd,
            //        KieuNguoiDung = knd,
            //        CoCauToChuc = cc,
            //        ChucVu = cv,
            //    }
            //)
            //.OrderByDescending(x => x.NguoiDung.NgayTao)
            //.ToListAsync();
            var data = await query.ToListAsync();
            return data;
        }
        public async Task<DisplayModal_CRUD_KieuNguoiDung_Output_Dto> DisplayModal_CRUD_KieuNguoiDung_Ouput(DisplayModal_CRUD_KieuNguoiDung_Input_Dto input)
        {
            var kieuNguoiDung = new tbKieuNguoiDung();
            // Tìm cấp độ thấp nhất của cây
            int gioiHan = 0;
            var sql_max = await (from cn in _chucNangRepo.Query()
                                 where cn.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung
                                 select cn.CapDo).ToListAsync();

            if (sql_max.Any()) gioiHan = sql_max.Max().Value + 1;

            if (input.Loai != "create" && input.IdKieuNguoiDung != Guid.Empty)
            {
                var kieuNguoiDungs = await Get_KieuNguoiDungs(loai: "single", idKieuNguoiDungs: new List<Guid> { input.IdKieuNguoiDung });
                kieuNguoiDung = kieuNguoiDungs.FirstOrDefault() ?? new tbKieuNguoiDung();
            }
            ;

            async Task<string> taoTree(Guid idCha, int capDo)
            {
                string html = default(string);
                string khoangCach = String.Join("", Enumerable.Repeat("<td></td>", (int)capDo).ToArray());
                var _chucNangs = await _chucNangRepo.Query()
                    .Where(x => x.IdCha == idCha && x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung).ToListAsync();

                foreach (default_tbChucNang _chucNang in _chucNangs)
                {
                    // Tìm các thao tác của chức năng
                    List<default_tbChucNang_ThaoTac> thaoTacs = await _thaoTacRepo.Query()
                        .Where(x => x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung && x.IdChucNang == _chucNang.IdChucNang)
                        .ToListAsync();

                    html += "<tr>" +
                        khoangCach +
                    $"<td class=\"text-center w-5\">" +
                    $"  <input type=\"checkbox\" class=\"form-check-input checkbox-chucnang\" data-machucnang=\"{_chucNang.MaChucNang}\" data-id=\"{_chucNang.IdChucNang}\" data-idcha=\"{_chucNang.IdCha}\" data-capdo=\"{_chucNang.CapDo}\"/></td>" +
                    $"<td colspan=\"{gioiHan - capDo}\">{_chucNang.TenChucNang}</td>" + // colspan theo cấp độ
                    "<td>";
                    foreach (default_tbChucNang_ThaoTac thaoTac in thaoTacs)
                    {
                        html += $"<div class=\"form-group\">" +
                            $"<input type=\"checkbox\" class=\"form-check-input checkbox-thaotac\" data-mathaotac=\"{thaoTac.MaThaoTac}\" data-idthaotac=\"{thaoTac.IdThaoTac}\"/>" +
                            $"<span>&ensp;{thaoTac.TenThaoTac}</span>" +
                            $"</div>";
                    }
                    ;
                    html += "</td>" +
                        "</tr>" + taoTree((Guid)_chucNang.IdChucNang, capDo + 1);
                }
                ;
                return html;
            }
            ;
            var htmlChucNangs = await taoTree(idCha: Guid.Empty, 0);

            var output = new DisplayModal_CRUD_KieuNguoiDung_Output_Dto
            {
                Loai = input.Loai,
                KieuNguoiDung = kieuNguoiDung,
                HtmlChucNangs = htmlChucNangs,
            };

            return output;
        }
        public async Task<bool> IsExisted_KieuNguoiDung(tbKieuNguoiDung kieuNguoiDung)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            var kieuNguoiDung_OLD = await _kieuNguoiDungRepo.Query()
                .FirstOrDefaultAsync(x => x.TenKieuNguoiDung == kieuNguoiDung.TenKieuNguoiDung
                && x.IdKieuNguoiDung != kieuNguoiDung.IdKieuNguoiDung
                && x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return kieuNguoiDung_OLD != null;
        }
        public async Task Create_KieuNguoiDung(
        tbKieuNguoiDung kieuNguoiDung)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                // Tạo hồ sơ
                var entity = new tbKieuNguoiDung
                {
                    IdKieuNguoiDung = Guid.NewGuid(),
                    TenKieuNguoiDung = kieuNguoiDung.TenKieuNguoiDung,
                    IdChucNang = kieuNguoiDung.IdChucNang,
                    GhiChu = kieuNguoiDung.GhiChu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                };
                await _unitOfWork.InsertAsync<tbKieuNguoiDung, Guid>(entity);
            });
        }
        public async Task Update_KieuNguoiDung(
            tbKieuNguoiDung kieuNguoiDung)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var kieuNguoiDung_OLD = await _kieuNguoiDungRepo.GetByIdAsync(kieuNguoiDung.IdKieuNguoiDung);
                if (kieuNguoiDung_OLD == null)
                    throw new Exception("Kiểu người dùng không tồn tại");

                kieuNguoiDung_OLD.TenKieuNguoiDung = kieuNguoiDung.TenKieuNguoiDung;
                kieuNguoiDung_OLD.IdChucNang = kieuNguoiDung.IdChucNang;
                kieuNguoiDung_OLD.GhiChu = kieuNguoiDung.GhiChu;

                kieuNguoiDung_OLD.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                kieuNguoiDung_OLD.NgaySua = DateTime.Now;

                await _unitOfWork.UpdateAsync<tbKieuNguoiDung, Guid>(kieuNguoiDung_OLD);
            });
        }
        public async Task Delete_KieuNguoiDungs(
            List<Guid> idKieuNguoiDungs,
            Guid idKieuNguoiDung_THAYTHE)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var idKieuNguoiDung in idKieuNguoiDungs)
                {
                    var kieuNguoiDung = await _kieuNguoiDungRepo.GetByIdAsync(idKieuNguoiDung);
                    if (kieuNguoiDung == null)
                        throw new Exception("Người dùng không tồn tại");

                    // Xóa kiểu người dùng
                    kieuNguoiDung.TrangThai = 0; // Đánh dấu đã xóa
                    kieuNguoiDung.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                    kieuNguoiDung.NgaySua = DateTime.Now;
                    await _unitOfWork.UpdateAsync<tbKieuNguoiDung, Guid>(kieuNguoiDung);

                    // Cập nhật lại người dùng về kiểu người dùng thay thế
                    var nguoiDungKieuNguoiDungs = await _nguoiDungKieuNguoiDungRepo.Query()
                        .Where(x => x.IdKieuNguoiDung == idKieuNguoiDung
                        && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung).ToListAsync();
                    foreach (var nguoiDungKieuNguoiDung in nguoiDungKieuNguoiDungs)
                    {
                        // Nếu chưa tồn tại kiểu người dùng thay thế thì mới cập nhật
                        var nguoiDungKieuNguoiDung_THAYTHE = await _nguoiDungKieuNguoiDungRepo.Query()
                            .FirstOrDefaultAsync(x => x.IdNguoiDung == nguoiDungKieuNguoiDung.IdNguoiDung
                            && x.IdKieuNguoiDung == idKieuNguoiDung_THAYTHE
                            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);

                        if (nguoiDungKieuNguoiDung_THAYTHE == null)
                        {
                            nguoiDungKieuNguoiDung.IdKieuNguoiDung = idKieuNguoiDung_THAYTHE;
                            nguoiDungKieuNguoiDung.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                            nguoiDungKieuNguoiDung.NgaySua = DateTime.Now;

                            await _unitOfWork.UpdateAsync<tbNguoiDungKieuNguoiDung, Guid>(nguoiDungKieuNguoiDung);
                        }
                    }
                }
            });
        }

        #region Private Methods

        #endregion
    }
}
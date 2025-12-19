using Applications.QuanLyCoCauToChuc.Interfaces;
using Applications.QuanLyCoCauToChuc.Services;
using Applications.QuanLyNguoiDung.Dtos;
using Applications.QuanLyNguoiDung.Interfaces;
using Applications.QuanLyNguoiDung.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Enums;
using Public.Helpers;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNguoiDung.Services
{
    public class QuanLyNguoiDungService : BaseService, IQuanLyNguoiDungService
    {
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/QuanLyNguoiDung";
        private readonly IRepository<tbNguoiDung, Guid> _nguoiDungRepo;
        private readonly IRepository<tbKieuNguoiDung, Guid> _kieuNguoiDungRepo;
        private readonly IRepository<tbCoCauToChuc, Guid> _coCauToChucRepo;
        private readonly IRepository<default_tbChucVu, Guid> _chucVuRepo;
        private readonly IViewRenderer _viewRenderer;

        private readonly IQuanLyCoCauToChucService _quanLyCoCauToChucService;

        public QuanLyNguoiDungService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbNguoiDung, Guid> nguoiDungRepo,
            IRepository<tbKieuNguoiDung, Guid> kieuNguoiDungRepo,
            IRepository<tbCoCauToChuc, Guid> coCauToChucRepo,
            IRepository<default_tbChucVu, Guid> chucVuRepo,

            IQuanLyCoCauToChucService quanLyCoCauToChucService,

            IViewRenderer viewRenderer)
            : base(userContext, unitOfWork)
        {
            _nguoiDungRepo = nguoiDungRepo;
            _kieuNguoiDungRepo = kieuNguoiDungRepo;
            _coCauToChucRepo = coCauToChucRepo;
            _chucVuRepo = chucVuRepo;
            _quanLyCoCauToChucService = quanLyCoCauToChucService;

            _viewRenderer = viewRenderer;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<DisplayModal_CRUD_NguoiDung_Output_Dto> DisplayModal_CRUD_NguoiDung(DisplayModal_CRUD_NguoiDung_Input_Dto input)
        {
            var nguoiDungs = new List<tbNguoiDungExtend>();
            if (input.Loai != "create" && input.IdNguoiDung != Guid.Empty)
                nguoiDungs = await Get_NguoiDungs(
                    loai: "single",
                    idNguoiDungs: new List<Guid> { input.IdNguoiDung });

            #region Kiểu người dùng
            var kieuNguoiDungs = await _kieuNguoiDungRepo.Query()
                .Where(x => x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung && x.TrangThai != 0)
                .ToListAsync() ?? new List<tbKieuNguoiDung>();
            #endregion

            #region Cơ cấu tổ chức
            var coCauToChucs = new List<tbCoCauToChuc>();
            var _coCauToChucs_Tree = await _quanLyCoCauToChucService.Get_CoCauToChucs_Tree(idCoCau: Guid.Empty);
            var coCauToChucs_Tree = _coCauToChucs_Tree.nodes;
            await _quanLyCoCauToChucService.XuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs);
            #endregion

            var output = new DisplayModal_CRUD_NguoiDung_Output_Dto
            {
                Loai = input.Loai,
                NguoiDung = nguoiDungs.FirstOrDefault() ?? new tbNguoiDungExtend(),
                KieuNguoiDungs = kieuNguoiDungs,
                CoCauToChucs = coCauToChucs,
                CoCauToChucs_TREE = coCauToChucs_Tree,
            };
            return output;
        }
        public async Task<Index_Output_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyNguoiDung");
            var kieuNguoiDungs = await _kieuNguoiDungRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == Guid.Empty).ToListAsync(); // Lấy trong kho chung
            var coCauToChucs = await _coCauToChucRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == Guid.Empty).ToListAsync(); // Lấy trong kho chung
            var chucVus = await _chucVuRepo.Query().Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == Guid.Empty).ToListAsync(); // Lấy trong kho chung

            return new Index_Output_Dto
            {
                ThaoTacs = thaoTacs,
                KieuNguoiDungs = kieuNguoiDungs,
                CoCauToChucs = coCauToChucs,
                ChucVus = chucVus
            };
        }
        public async Task<List<tbNguoiDungExtend>> Get_NguoiDungs(
            string loai = "all",
            List<Guid> idNguoiDungs = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _nguoiDungRepo.Query()
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

            if (loai == "single" && idNguoiDungs != null && idNguoiDungs.Any())
            {
                query = query.Where(x => idNguoiDungs.Contains(x.IdNguoiDung));
            }

            var data = await (
               from nd in query

               join knd in _kieuNguoiDungRepo.Query() on nd.IdKieuNguoiDung equals knd.IdKieuNguoiDung into kndGroup
               from knd in kndGroup.DefaultIfEmpty()

               join cc in _coCauToChucRepo.Query() on nd.IdCoCauToChuc equals cc.IdCoCauToChuc into ccGroup
               from cc in ccGroup.DefaultIfEmpty()

               join cv in _chucVuRepo.Query() on nd.IdChucVu equals cv.IdChucVu into cvGroup
               from cv in cvGroup.DefaultIfEmpty()

               select new tbNguoiDungExtend
               {
                   NguoiDung = nd,
                   KieuNguoiDung = knd,
                   CoCauToChuc = cc,
                   ChucVu = cv,
               }
           )
           .OrderByDescending(x => x.NguoiDung.NgayTao)
           .ToListAsync();

            return data;
        }
        public async Task<bool> CapNhatNguoiDungHoatDong(Guid idNguoiDung)
        {
            bool rs = false;
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var nguoiDung = await _nguoiDungRepo.GetByIdAsync(idNguoiDung);

                if (nguoiDung == null)
                    throw new Exception("Người dùng không tồn tại");

                nguoiDung.Online = true;
                _nguoiDungRepo.Update(nguoiDung);
                rs = true;
            });
            return rs;
        }
        //public async Task<DisplayModal_CRUD_NguoiDung_Output_Dto> DisplayModal_CRUD_NguoiDung(DisplayModal_CRUD_NguoiDung_Input_Dto input)
        //{
        //    var nguoiDung = await Get_NguoiDungs(loai: "single", idNguoiDungs: new List<Guid> { input.IdNguoiDung });
        //    var output = new DisplayModal_CRUD_NguoiDung_Output_Dto
        //    {
        //        Loai = input.Loai,
        //        NguoiDung = nguoiDung.FirstOrDefault() ?? new tbNguoiDungExtend(),
        //    };

        //    return output;
        //}
        public async Task<bool> IsExisted_NguoiDung(tbNguoiDung nguoiDung)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            var nguoiDung_OLD = await _nguoiDungRepo.Query()
                .FirstOrDefaultAsync(x => x.TenDangNhap == nguoiDung.TenDangNhap
                && x.IdNguoiDung != nguoiDung.IdNguoiDung
                && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return nguoiDung_OLD != null;
        }
        public async Task Create_NguoiDung(
        tbNguoiDungExtend nguoiDung)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                nguoiDung.NguoiDung.TenDangNhap = TaoTenDangNhap(tenDangNhap: nguoiDung.NguoiDung.TenDangNhap);
                string matKhau = Public.Handles.Handle.HashToMD5(nguoiDung.NguoiDung.MatKhau);

                // Tạo hồ sơ
                tbNguoiDung entity = new tbNguoiDung
                {
                    IdNguoiDung = Guid.NewGuid(),
                    TenDangNhap = nguoiDung.NguoiDung.TenDangNhap,
                    MatKhau = matKhau,
                    TenNguoiDung = nguoiDung.NguoiDung.TenNguoiDung,
                    GioiTinh = nguoiDung.NguoiDung.GioiTinh,
                    KichHoat = nguoiDung.NguoiDung.KichHoat,
                    SoLanDangNhap = 0,
                    YeuCauDoiMatKhau = true,
                    Online = false,
                    Email = nguoiDung.NguoiDung.Email,
                    SoDienThoai = nguoiDung.NguoiDung.SoDienThoai,
                    SoTaiKhoanNganHang = nguoiDung.NguoiDung.SoTaiKhoanNganHang,
                    NgaySinh = nguoiDung.NguoiDung.NgaySinh,
                    GhiChu = nguoiDung.NguoiDung.GhiChu,
                    LinkLienHe = nguoiDung.NguoiDung.LinkLienHe,

                    IdKieuNguoiDung = nguoiDung.NguoiDung.IdKieuNguoiDung,
                    IdCoCauToChuc = nguoiDung.NguoiDung.IdCoCauToChuc,
                    IdChucVu = nguoiDung.NguoiDung.IdChucVu,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                };
                await _unitOfWork.InsertAsync<tbNguoiDung, Guid>(entity);

                await GuiMai(
                   nguoiDung_NEW: nguoiDung,
                   loaiView: "create");
            });
        }
        public async Task Update_NguoiDung(
            tbNguoiDungExtend nguoiDung)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                nguoiDung.NguoiDung.TenDangNhap = TaoTenDangNhap(tenDangNhap: nguoiDung.NguoiDung.TenDangNhap);
                var nguoiDung_OLD = await _nguoiDungRepo.GetByIdAsync(nguoiDung.NguoiDung.IdNguoiDung);
                if (nguoiDung_OLD == null)
                    throw new Exception("Người dùng không tồn tại");

                var _nguoiDung_OLD = new tbNguoiDungExtend();

                if (nguoiDung.NguoiDung.IdKieuNguoiDung != null || nguoiDung.NguoiDung.IdKieuNguoiDung != Guid.Empty)
                {
                    nguoiDung.KieuNguoiDung = await _kieuNguoiDungRepo.GetByIdAsync(nguoiDung.NguoiDung.IdKieuNguoiDung.Value) ?? new tbKieuNguoiDung();
                    _nguoiDung_OLD.KieuNguoiDung = await _kieuNguoiDungRepo.GetByIdAsync(nguoiDung_OLD.IdKieuNguoiDung.Value) ?? new tbKieuNguoiDung();
                }
                            ;
                if (nguoiDung.NguoiDung.IdCoCauToChuc != null || nguoiDung.NguoiDung.IdCoCauToChuc != Guid.Empty)
                {
                    nguoiDung.CoCauToChuc = await _coCauToChucRepo.GetByIdAsync(nguoiDung.NguoiDung.IdCoCauToChuc.Value) ?? new tbCoCauToChuc();
                    _nguoiDung_OLD.CoCauToChuc = await _coCauToChucRepo.GetByIdAsync(nguoiDung_OLD.IdCoCauToChuc.Value) ?? new tbCoCauToChuc();
                }
                ;

                nguoiDung_OLD.TenDangNhap = nguoiDung.NguoiDung.TenDangNhap;
                nguoiDung_OLD.TenNguoiDung = nguoiDung.NguoiDung.TenNguoiDung;
                nguoiDung_OLD.GioiTinh = nguoiDung.NguoiDung.GioiTinh;
                nguoiDung_OLD.KichHoat = nguoiDung.NguoiDung.KichHoat;
                nguoiDung_OLD.Email = nguoiDung.NguoiDung.Email;
                nguoiDung_OLD.SoDienThoai = nguoiDung.NguoiDung.SoDienThoai;
                nguoiDung_OLD.SoTaiKhoanNganHang = nguoiDung.NguoiDung.SoTaiKhoanNganHang;
                nguoiDung_OLD.NgaySinh = nguoiDung.NguoiDung.NgaySinh;
                nguoiDung_OLD.GhiChu = nguoiDung.NguoiDung.GhiChu;
                nguoiDung_OLD.LinkLienHe = nguoiDung.NguoiDung.LinkLienHe;
                nguoiDung_OLD.IdKieuNguoiDung = nguoiDung.NguoiDung.IdKieuNguoiDung;
                nguoiDung_OLD.IdCoCauToChuc = nguoiDung.NguoiDung.IdCoCauToChuc;
                nguoiDung_OLD.IdChucVu = nguoiDung.NguoiDung.IdChucVu;

                nguoiDung_OLD.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                nguoiDung_OLD.NgaySua = DateTime.Now;

                await _unitOfWork.UpdateAsync<tbNguoiDung, Guid>(nguoiDung_OLD);

                await GuiMai(
                    nguoiDung_OLD: _nguoiDung_OLD,
                    nguoiDung_NEW: nguoiDung,
                    loaiView: "update");
            });
        }
        public async Task Update_MatKhau(
            tbNguoiDungExtend nguoiDung)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                nguoiDung.NguoiDung.TenDangNhap = TaoTenDangNhap(tenDangNhap: nguoiDung.NguoiDung.TenDangNhap);
                var nguoiDung_OLD = await _nguoiDungRepo.GetByIdAsync(nguoiDung.NguoiDung.IdNguoiDung);
                if (nguoiDung_OLD == null)
                    throw new Exception("Người dùng không tồn tại");

                string matKhau_MD5 = Public.Handles.Handle.HashToMD5(nguoiDung.MatKhauMoi);
                if (nguoiDung_OLD.MatKhau != matKhau_MD5) throw new Exception("Mật khẩu cũ chưa chính xác");

                // Kiểm tra độ bảo mật
                var conditions = Public.Handles.Handle.CheckPassPattern(nguoiDung.MatKhauMoi);
                // Kiểm tra từng điều kiện
                foreach (var condition in conditions)
                {
                    if (!condition.Value.status) throw new Exception("Chưa thoả mãn điều kiện");
                }
                ;

                nguoiDung_OLD.MatKhau = matKhau_MD5;

                nguoiDung_OLD.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                nguoiDung_OLD.NgaySua = DateTime.Now;

                await _unitOfWork.UpdateAsync<tbNguoiDung, Guid>(nguoiDung_OLD);

                await GuiMai(
                    nguoiDung_NEW: nguoiDung,
                    loaiView: "update");
            });
        }
        public async Task Delete_NguoiDungs(
            List<Guid> idNguoiDungs)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                foreach (var idNguoiDung in idNguoiDungs)
                {
                    var nguoiDung = await _nguoiDungRepo.GetByIdAsync(idNguoiDung);
                    if (nguoiDung == null)
                        throw new Exception("Người dùng không tồn tại");

                    nguoiDung.TrangThai = 0; // Đánh dấu đã xóa
                    nguoiDung.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                    nguoiDung.NgaySua = DateTime.Now;
                    await _unitOfWork.UpdateAsync<tbNguoiDung, Guid>(nguoiDung);
                }
            });
        }

        #region Private Methods
        private async Task GuiMai(
            tbNguoiDungExtend nguoiDung_OLD = null,
            tbNguoiDungExtend nguoiDung_NEW = null,
            string loaiView = "create")
        {
            Task<string> mail()
            {
                string viewName = loaiView == "create"
                   ? $"{VIEW_PATH}/nguoidung-mail.taotaikhoan.cshtml"
                   : $"{VIEW_PATH}/nguoidung-mail.capnhattaikhoan.cshtml";

                var model = new CapNhatTaiKhoanMail<tbNguoiDungExtend>
                {
                    NguoiDung_OLD = nguoiDung_OLD,
                    NguoiDung_NEW = nguoiDung_NEW,
                    DonViSuDung = CurrentDonViSuDung,
                    HinhThucCapNhat = loaiView == "update_matkhau" ? "doimatkhau" : null
                };

                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                string viewAsString = _viewRenderer.RenderViewToString(
                    controllerName: "QuanLyNguoiDung",
                    viewName: viewName,
                    model: model);
                // Trả về chuỗi đã được tạo ra từ view
                return Task.FromResult(viewAsString);
            }
            string tieuDeMail = "[📣 banmai] - THÔNG TIN TÀI KHOẢN CRM❗";
            string mailBody = await mail();
            // Gửi mail
            Public.Handles.Handle.SendEmail(
                sendTo: nguoiDung_OLD.NguoiDung.Email,
                subject: tieuDeMail,
                body: mailBody,
                isHTML: true,
                donViSuDung: CurrentDonViSuDung);
        }
        private string TaoTenDangNhap(string tenDangNhap)
        {
            return string.Format("{0}@banmai.com", tenDangNhap.Replace("@banmai.com", ""));
        }
        private void downloadDialog(MemoryStream data, string fileName, string contentType)
        {
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = contentType;
            HttpContext.Current.Response.AddHeader("content-disposition", "inline; filename=\"" + fileName + "\"");
            HttpContext.Current.Response.BinaryWrite(data.ToArray());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        #endregion
    }
}
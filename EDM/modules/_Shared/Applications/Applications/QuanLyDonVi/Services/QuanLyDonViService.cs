using Applications.QuanLyDonVi.Dtos;
using Applications.QuanLyDonVi.Interfaces;
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

namespace Applications.QuanLyDonVi.Services
{

    public class QuanLyDonViService : BaseService, IQuanLyDonViService
    {
        private readonly IRepository<tbDonViSuDung, Guid> _donViRepo;

        public QuanLyDonViService(
         IUserContext userContext,
         IUnitOfWork unitOfWork,
         IRepository<tbDonViSuDung, Guid> donViRepo)
         : base(userContext, unitOfWork)
        {
            _donViRepo = donViRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<Index_OutPut_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyDonVi");

            return new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }
        public async Task<List<tbDonViSuDung>> GetDonVis(
            string loai = "all",
            List<Guid> idDonVis = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _donViRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung != CurrentDonViId); // Lấy các đơn vị khác đang dùng

            if (loai == "single" && idDonVis != null)
            {
                query = query.Where(x => idDonVis.Contains(x.MaDonViSuDung));
            }
            ;

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return data;
        }
        public async Task<bool> IsExisted_DonVi(tbDonViSuDung donVi)
        {
            var donVi_OLD = await _donViRepo.Query()
                .FirstOrDefaultAsync(x => x.TenMien == donVi.TenMien
            && x.MaDonViSuDung != donVi.MaDonViSuDung
            && x.TrangThai == (int?)TrangThaiDuLieuEnum.DangSuDung);
            return donVi_OLD != null;
        }
        public async Task Create_DonVi(tbDonViSuDung donVi, HttpPostedFileBase logo)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                #region Lưu file trong server
                if (logo != null)
                {
                    byte[] logoData = null;
                    using (var binaryReader = new BinaryReader(logo.InputStream))
                    {
                        logoData = binaryReader.ReadBytes(logo.ContentLength);
                    }
                    ;

                    string folderPath = string.Format("{0}",
                        $"/Assets/uploads/{CurrentDonViSuDung.MaDonViSuDung}/THIETLAPCHUNG/");
                    string folderPath_SERVER = HttpContext.Current.Server.MapPath(folderPath); // Replace 'Request.MapPath' with 'HttpContext.Current.Server.MapPath'

                    if (!System.IO.Directory.Exists(folderPath_SERVER)) System.IO.Directory.CreateDirectory(folderPath_SERVER); // Tạo folder

                    string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(logo.FileName), khoangCach: "-");
                    string inputFilePath = string.Format("{0}/[{1}]{2}", folderPath, "LOGO", inputFileName);
                    string inputFilePath_SERVER = HttpContext.Current.Server.MapPath(inputFilePath); // Replace 'Request.MapPath' with 'HttpContext.Current.Server.MapPath'
                    donVi.Logo = inputFilePath;
                    if (System.IO.File.Exists(inputFilePath_SERVER)) System.IO.File.Delete(inputFilePath_SERVER);
                    ;
                    //logo.SaveAs(inputFilePath_SERVER);
                    System.IO.File.WriteAllBytes(inputFilePath_SERVER, logoData);
                }
                ;
                #endregion

                var entity = new tbDonViSuDung
                {
                    MaDonViSuDung = Guid.NewGuid(),
                    TenMien = donVi.TenMien,
                    TenDonViSuDung = donVi.TenDonViSuDung,
                    TieuDeTrangChu = donVi.TieuDeTrangChu,
                    SuDungTrangNguoiDung = false,

                    Logo = donVi.Logo,

                    DiaChi = donVi.DiaChi,
                    Email = donVi.Email,
                    SoDienThoai = donVi.SoDienThoai,

                    TrangThai = (int?)TrangThaiDuLieuEnum.DangSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                };

                // 5. Lưu
                await _unitOfWork.InsertAsync<tbDonViSuDung, Guid>(entity);
            });
        }
        public async Task Update_DonVi(tbDonViSuDung donVi, HttpPostedFileBase logo)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                #region Lưu file trong server
                if (logo != null)
                {
                    byte[] logoData = null;
                    using (var binaryReader = new BinaryReader(logo.InputStream))
                    {
                        logoData = binaryReader.ReadBytes(logo.ContentLength);
                    }
                    ;

                    string folderPath = string.Format("{0}",
                        $"/Assets/uploads/{CurrentDonViSuDung.MaDonViSuDung}/THIETLAPCHUNG/");
                    string folderPath_SERVER = HttpContext.Current.Server.MapPath(folderPath); // Replace 'Request.MapPath' with 'HttpContext.Current.Server.MapPath'

                    if (!System.IO.Directory.Exists(folderPath_SERVER)) System.IO.Directory.CreateDirectory(folderPath_SERVER); // Tạo folder

                    string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(logo.FileName), khoangCach: "-");
                    string inputFilePath = string.Format("{0}/[{1}]{2}", folderPath, "LOGO", inputFileName);
                    string inputFilePath_SERVER = HttpContext.Current.Server.MapPath(inputFilePath); // Replace 'Request.MapPath' with 'HttpContext.Current.Server.MapPath'
                    donVi.Logo = inputFilePath;
                    if (System.IO.File.Exists(inputFilePath_SERVER)) System.IO.File.Delete(inputFilePath_SERVER);
                    ;
                    //logo.SaveAs(inputFilePath_SERVER);
                    System.IO.File.WriteAllBytes(inputFilePath_SERVER, logoData);
                }
                ;
                #endregion

                var entity = await _donViRepo.GetByIdAsync(donVi.MaDonViSuDung);
                if (entity == null)
                    throw new Exception($"Không tìm thấy AI Tool với Id: {donVi.MaDonViSuDung}");

                entity.MaDonViSuDung = Guid.NewGuid();
                entity.TenMien = donVi.TenMien;
                entity.TenDonViSuDung = donVi.TenDonViSuDung;
                entity.TieuDeTrangChu = donVi.TieuDeTrangChu;
                entity.SuDungTrangNguoiDung = false;

                entity.Logo = donVi.Logo;

                entity.DiaChi = donVi.DiaChi;
                entity.Email = donVi.Email;
                entity.SoDienThoai = donVi.SoDienThoai;

                entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;
                entity.NgaySua = DateTime.Now;

                // 5. Lưu
                await _unitOfWork.UpdateAsync<tbDonViSuDung, Guid>(entity);
            });
        }
        public async Task Delete_DonVi(List<Guid> idDonVis)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var donVis_DELETE = await _donViRepo.Query()
                    .Where(x => idDonVis.Contains(x.MaDonViSuDung)
                        && x.MaDonViSuDung != CurrentDonViSuDung.MaDonViSuDung)
                    // Không xóa đơn vị đang dùng
                    .ToListAsync();

                foreach (var entity in donVis_DELETE)
                {
                    entity.TrangThai = 0;
                    entity.NgaySua = DateTime.Now;
                    entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbDonViSuDung, Guid>(entity);
                }
            });
        }
    }
}
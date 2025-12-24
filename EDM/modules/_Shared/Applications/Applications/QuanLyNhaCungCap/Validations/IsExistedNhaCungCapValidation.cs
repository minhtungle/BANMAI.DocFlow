using Applications.QuanLyNhaCungCap.Enums;
using Applications.QuanLyNhaCungCap.Interfaces;
using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Dtos;
using Public.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Validations
{
   
    public class IsExistedNhaCungCapValidation : BaseService
    {
        private readonly IRepository<tbNhaCungCap, Guid> _nhaCungCapRepo;

        public IsExistedNhaCungCapValidation(
            IUserContext userContext,
            IUnitOfWork unitOfWork,

            IRepository<tbNhaCungCap, Guid> nhaCungCapRepo

            ) : base(userContext, unitOfWork)
        {
            _nhaCungCapRepo = nhaCungCapRepo;
        }
        public async Task<ValidationResultEx> IsExisted(
            tbNhaCungCap nhaCungCap,
            params NhaCungCapFieldEnum[] fieldsToCheck)
        {
            if (nhaCungCap == null) throw new ArgumentNullException(nameof(nhaCungCap));
            if (fieldsToCheck == null || fieldsToCheck.Length == 0)
                fieldsToCheck = new[] {
                    NhaCungCapFieldEnum.MaNhaCungCap
                };

            // Chuẩn hoá input (giảm false-negative)
            string ma = (nhaCungCap.MaNhaCungCap ?? "").Trim();

            var result = new ValidationResultEx();

            // Base query: cùng đơn vị, đang sử dụng, khác bản ghi hiện tại
            var baseQuery = _nhaCungCapRepo.Query().Where(x =>
                x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung &&
                x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung &&
                x.IdNhaCungCap != nhaCungCap.IdNhaCungCap
            );

            // Check từng trường (mỗi check 1 query exists, rõ ràng & dễ debug)
            if (fieldsToCheck.Contains(NhaCungCapFieldEnum.MaNhaCungCap) && !string.IsNullOrWhiteSpace(ma))
            {
                bool existed = await baseQuery.AnyAsync(x => x.MaNhaCungCap == ma);
                if (existed)
                {
                    result.InvalidFields.Add(new FieldInvalidDto<NhaCungCapFieldEnum>
                    {
                        Field = NhaCungCapFieldEnum.MaNhaCungCap,
                        Message = "Mã nhà cung cấp đã tồn tại."
                    });
                }
            }

            return result;
        }
    }
}
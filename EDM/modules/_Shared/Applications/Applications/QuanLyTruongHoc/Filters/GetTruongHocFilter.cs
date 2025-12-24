using Applications.QuanLyTruongHoc.Dtos;
using EDM_DB;
using Public.Enums;
using System;
using System.Linq;

namespace Applications.QuanLyTruongHoc.Filters
{
    public static class GetTruongHocFilter
    {
        public static IQueryable<tbTruongHoc> ApplyFilters(
            this IQueryable<tbTruongHoc> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
        {
            // Điều kiện chung
            q = q.Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung);
            if (maDonVi.HasValue)
                q = q.Where(x => x.MaDonViSuDung == maDonVi.Value);

            if (locThongTin == null) return q;
            {
                // IdTruongHocs
                if (locThongTin.IdTruongHocs != null && locThongTin.IdTruongHocs.Any())
                    q = q.Where(x => locThongTin.IdTruongHocs.Contains(x.IdTruongHoc));

                // TenTruongHoc
                if (!string.IsNullOrWhiteSpace(locThongTin.TenTruongHoc))
                {
                    q = q.Where(x => (x.TenTruongHoc ?? "").ToLower().Contains(locThongTin.TenTruongHoc.Trim().ToLower()));
                }

                // Email
                if (!string.IsNullOrWhiteSpace(locThongTin.Email))
                {
                    q = q.Where(x => (x.Email ?? "").ToLower().Contains(locThongTin.Email.Trim().ToLower()));
                }

                // SoDienThoai
                if (!string.IsNullOrWhiteSpace(locThongTin.SoDienThoai))
                {
                    q = q.Where(x => (x.SoDienThoai ?? "").ToLower().Contains(locThongTin.SoDienThoai.Trim().ToLower()));
                }

                // DiaChi
                if (!string.IsNullOrWhiteSpace(locThongTin.DiaChi))
                {
                    q = q.Where(x => (x.DiaChi ?? "").ToLower().Contains(locThongTin.DiaChi.Trim().ToLower()));
                }

                // GhiChu
                if (!string.IsNullOrWhiteSpace(locThongTin.GhiChu))
                {
                    q = q.Where(x => (x.GhiChu ?? "").ToLower().Contains(locThongTin.GhiChu.Trim().ToLower()));
                }
            }

            return q;
        }
    }
}
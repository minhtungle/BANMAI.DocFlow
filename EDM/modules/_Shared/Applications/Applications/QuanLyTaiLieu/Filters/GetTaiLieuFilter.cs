using Applications.QuanLyTaiLieu.Dtos;
using EDM_DB;
using Public.Enums;
using System;
using System.Linq;

namespace Applications.QuanLyTaiLieu.Filters
{
    public static class GetTaiLieuFilter
    {
        public static IQueryable<tbTaiLieu> ApplyFilters(
            this IQueryable<tbTaiLieu> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
        {
            // Điều kiện chung
            q = q.Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung);
            if (maDonVi.HasValue)
                q = q.Where(x => x.MaDonViSuDung == maDonVi.Value);

            if (locThongTin == null) return q;

            // IdTaiLieus (maps to IdFile)
            if (locThongTin.IdTaiLieus != null && locThongTin.IdTaiLieus.Any())
                q = q.Where(x => locThongTin.IdTaiLieus.Contains(x.IdFile));

            // TenTaiLieu -> FileName or FileExtension
            if (!string.IsNullOrWhiteSpace(locThongTin.TenTaiLieu))
            {
                var term = locThongTin.TenTaiLieu.Trim().ToLower();
                q = q.Where(x => ((x.FileName ?? "").ToLower().Contains(term)
                                 || (x.FileExtension ?? "").ToLower().Contains(term)));
            }

            // LoaiTep
            if (!string.IsNullOrWhiteSpace(locThongTin.LoaiTep))
            {
                var term = locThongTin.LoaiTep.Trim().ToLower();
                q = q.Where(x => (x.LoaiTep ?? "").ToLower().Contains(term));
            }

            // GhiChu
            if (!string.IsNullOrWhiteSpace(locThongTin.GhiChu))
            {
                var term = locThongTin.GhiChu.Trim().ToLower();
                q = q.Where(x => (x.GhiChu ?? "").ToLower().Contains(term));
            }

            return q;
        }
    }
}
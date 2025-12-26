using Applications.QuanLyNhaCungCap.Dtos;
using EDM_DB;
using Public.Enums;
using System;
using System.Linq;

namespace Applications.QuanLyNhaCungCap.Filters
{
    public static class GetNhaCungCapFilter
    {
        public static IQueryable<tbNhaCungCap> ApplyFilters(
           this IQueryable<tbNhaCungCap> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
        {
            // Điều kiện chung
            q = q.Where(x => x.TrangThai == (int)TrangThaiDuLieuEnum.DangSuDung);
            if (maDonVi.HasValue)
                q = q.Where(x => x.MaDonViSuDung == maDonVi.Value);

            // IdNhaCungCaps
            if (locThongTin.IdNhaCungCaps != null && locThongTin.IdNhaCungCaps.Any())
                q = q.Where(x => locThongTin.IdNhaCungCaps.Contains(x.IdNhaCungCap));

            // IdTenNhaCungCap
            if (locThongTin.IdNhaCungCaps.Count > 0)
            {
                q = q.Where(x => locThongTin.IdNhaCungCaps.Contains(x.IdNhaCungCap));
            }

            // IdTenNhaCungCapCha
            if (locThongTin.IdNhaCungCapChas.Count > 0)
            {
                q = q.Where(x => locThongTin.IdNhaCungCapChas.Contains(x.IdNhaCungCap));
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

            return q;
        }
    }
}
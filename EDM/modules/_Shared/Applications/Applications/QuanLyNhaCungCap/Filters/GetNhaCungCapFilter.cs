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

            // Stt - sửa lại
            if (!string.IsNullOrWhiteSpace(locThongTin.Stt.ToString()) && locThongTin.Stt != 0)
            {
                q = q.Where(x => (x.Stt.ToString() ?? "").ToLower().Contains(locThongTin.Stt.ToString().Trim().ToLower()));
            }

            // TenNhaCungCap
            if (!string.IsNullOrWhiteSpace(locThongTin.TenNhaCungCap))
            {
                q = q.Where(x => (x.TenNhaCungCap ?? "").ToLower().Contains(locThongTin.TenNhaCungCap.Trim().ToLower()));
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
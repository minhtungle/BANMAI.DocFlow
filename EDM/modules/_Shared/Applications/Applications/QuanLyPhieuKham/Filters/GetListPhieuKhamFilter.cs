using Applications.QuanLyPhieuKham.Dtos;
using EDM_DB;
using System;
using System.Linq;

namespace Applications.QuanLyPhieuKham.Filters
{
    public static class GetListPhieuKhamFilter
    {
        public static IQueryable<tbPhieuKham> ApplyFilters(
  this IQueryable<tbPhieuKham> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
        {
            // Điều kiện chung
            q = q.Where(x => x.TrangThai != 0);
            if (maDonVi.HasValue)
                q = q.Where(x => x.MaDonViSuDung == maDonVi.Value);

            if (locThongTin == null) return q;
            {
                //ThoiGianBatDau
                if (locThongTin.ThoiGianBatDau.HasValue)
                    q = q.Where(x => x.NgayTao >= locThongTin.ThoiGianBatDau.Value);
                //ThoiGianKetThuc
                if (locThongTin.ThoiGianKetThuc.HasValue)
                    q = q.Where(x => x.NgayTao <= locThongTin.ThoiGianKetThuc.Value);
                //IdPhieuKhams
                if (locThongTin.IdPhieuKhams != null && locThongTin.IdPhieuKhams.Any())
                    q = q.Where(x => locThongTin.IdPhieuKhams.Contains(x.IdPhieuKham));
                ////IdBacSyKham
                //if (locThongTin.IdBacSyKham != null)
                //    q = q.Where(x => locThongTin.IdBacSyKham == x.IdBacSyKham);
                ////IdBacSyDieuTri
                //if (locThongTin.IdBacSyDieuTri != null)
                //    q = q.Where(x => locThongTin.IdBacSyDieuTri == x.IdBacSyDieuTri);
                ////LoaiKham
                //if (locThongTin.LoaiKham != null)
                //    q = q.Where(x => locThongTin.LoaiKham == x.LoaiKham);

                ////NoiDungKham
                //if (!string.IsNullOrWhiteSpace(locThongTin.NoiDungKham))
                //{
                //    q = q.Where(x => (x.NoiDungKham ?? "")
                //    .ToLower()
                //    .Contains(locThongTin.NoiDungKham.Trim().ToLower()));
                //}
                ////NoiDungDieuTri
                //if (!string.IsNullOrWhiteSpace(locThongTin.NoiDungDieuTri))
                //{
                //    q = q.Where(x => (x.NoiDungDieuTri ?? "")
                //    .ToLower()
                //    .Contains(locThongTin.NoiDungDieuTri.Trim().ToLower()));
                //}
                //GhiChu
                if (!string.IsNullOrWhiteSpace(locThongTin.GhiChu))
                {
                    q = q.Where(x => (x.GhiChu ?? "")
                    .ToLower()
                    .Contains(locThongTin.GhiChu.Trim().ToLower()));
                }
            }
  ;

            return q;
        }
    }
}
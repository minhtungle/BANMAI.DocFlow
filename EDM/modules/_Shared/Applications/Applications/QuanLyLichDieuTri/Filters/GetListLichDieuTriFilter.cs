using Applications.QuanLyLichDieuTri.Dtos;
using EDM_DB;
using System;
using System.Linq;

namespace Applications.QuanLyLichDieuTri.Filters
{
    public static class GetListLichDieuTriFilter
    {
        public static IQueryable<tbLichDieuTri> ApplyFilters(
  this IQueryable<tbLichDieuTri> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
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
                //IdLichDieuTris
                if (locThongTin.IdLichDieuTris != null && locThongTin.IdLichDieuTris.Any())
                    q = q.Where(x => locThongTin.IdLichDieuTris.Contains(x.IdLichDieuTri));
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
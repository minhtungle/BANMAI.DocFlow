using Applications.QuanLyLichHen.Dtos;
using EDM_DB;
using System;
using System.Linq;

namespace Applications.QuanLyLichHen.Filters
{
    public static class GetLichHenFilter
    {
        public static IQueryable<tbLichHen> ApplyFilters(
          this IQueryable<tbLichHen> q, LocThongTinDto locThongTin, Guid? maDonVi = null)
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
                //IdLichHens
                if (locThongTin.IdLichHens != null && locThongTin.IdLichHens.Any())
                    q = q.Where(x => locThongTin.IdLichHens.Contains(x.IdLichHen));
                //IdBenhNhan
                if (locThongTin.IdBenhNhan != null)
                    q = q.Where(x => locThongTin.IdBenhNhan == x.IdBenhNhan);
                //IdBacSy
                if (locThongTin.IdBacSy != null)
                    q = q.Where(x => locThongTin.IdBacSy == x.IdBacSy);
                //TrangThaiKham
                if (locThongTin.TrangThaiKham != null)
                    q = q.Where(x => locThongTin.TrangThaiKham == x.TrangThaiKham);

                //LyDoHuyHen
                if (!string.IsNullOrWhiteSpace(locThongTin.LyDoHuyHen))
                {
                    q = q.Where(x => (x.LyDoHuyHen ?? "")
                    .ToLower()
                    .Contains(locThongTin.LyDoHuyHen.Trim().ToLower()));
                }
                //NoiDungKham
                if (!string.IsNullOrWhiteSpace(locThongTin.NoiDungKham))
                {
                    q = q.Where(x => (x.NoiDungKham ?? "")
                    .ToLower()
                    .Contains(locThongTin.NoiDungKham.Trim().ToLower()));
                }
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
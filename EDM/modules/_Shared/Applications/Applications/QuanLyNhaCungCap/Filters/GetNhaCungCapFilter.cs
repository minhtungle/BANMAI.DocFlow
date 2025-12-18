using Applications.QuanLyNhaCungCap.Dtos;
using EDM_DB;
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
            q = q.Where(x => x.TrangThai != 0);
            if (maDonVi.HasValue)
                q = q.Where(x => x.MaDonViSuDung == maDonVi.Value);

            //if (locThongTin == null) return q;
            //{
            //    //ThoiGianBatDau
            //    if (locThongTin.ThoiGianBatDau.HasValue)
            //        q = q.Where(x => x.NgayTao >= locThongTin.ThoiGianBatDau.Value);
            //    //ThoiGianKetThuc
            //    if (locThongTin.ThoiGianKetThuc.HasValue)
            //        q = q.Where(x => x.NgayTao <= locThongTin.ThoiGianKetThuc.Value);
            //    //IdBenhNhans
            //    if (locThongTin.IdBenhNhans != null && locThongTin.IdBenhNhans.Any())
            //        q = q.Where(x => locThongTin.IdBenhNhans.Contains(x.IdBenhNhan));
            //    //HoTen
            //    if (!string.IsNullOrWhiteSpace(locThongTin.HoTen))
            //    {
            //        q = q.Where(x => (x.HoTen ?? "")
            //       .ToLower()
            //       .Contains(locThongTin.HoTen.Trim().ToLower()));
            //    }
            //    //GioiTinh
            //    //if (locThongTin.GioiTinh.HasValue)
            //    //    q = q.Where(x => x.GioiTinh == locThongTin.GioiTinh.Value);
            //    //NgaySinh
            //    if (locThongTin.NgaySinh.HasValue)
            //        q = q.Where(x => x.NgaySinh == locThongTin.NgaySinh.Value);
            //    //SoDienThoai
            //    if (!string.IsNullOrWhiteSpace(locThongTin.SoDienThoai))
            //    {
            //        q = q.Where(x => (x.SoDienThoai ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.SoDienThoai.Trim().ToLower()));
            //    }
            //    //Email
            //    if (!string.IsNullOrWhiteSpace(locThongTin.Email))
            //    {
            //        q = q.Where(x => (x.Email ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.Email.Trim().ToLower()));
            //    }
            //    //CCCD
            //    if (!string.IsNullOrWhiteSpace(locThongTin.CCCD))
            //    {
            //        q = q.Where(x => (x.CCCD ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.CCCD.Trim().ToLower()));
            //    }
            //    //NgheNghiep
            //    if (!string.IsNullOrWhiteSpace(locThongTin.NgheNghiep))
            //    {
            //        q = q.Where(x => (x.NgheNghiep ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.NgheNghiep.Trim().ToLower()));
            //    }
            //    //DiaChi
            //    if (!string.IsNullOrWhiteSpace(locThongTin.DiaChi))
            //    {
            //        q = q.Where(x => (x.DiaChi ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.DiaChi.Trim().ToLower()));
            //    }
            //    //TienSuBenh
            //    if (!string.IsNullOrWhiteSpace(locThongTin.TienSuBenh))
            //    {
            //        q = q.Where(x => (x.TienSuBenh ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.TienSuBenh.Trim().ToLower()));
            //    }
            //    //GhiChu
            //    if (!string.IsNullOrWhiteSpace(locThongTin.GhiChu))
            //    {
            //        q = q.Where(x => (x.GhiChu ?? "")
            //        .ToLower()
            //        .Contains(locThongTin.GhiChu.Trim().ToLower()));
            //    }
            //}
           ;

            // 3) Khoảng thời gian theo ngày(DateTime ?) – bỏ qua nếu null


            // 4) So sánh >, < cho trường số (ví dụ: TongTien, SoLuot, v.v.)
            //if (locThongTin.MinTongTien.HasValue)
            //    q = q.Where(x => x.TongTien >= locThongTin.MinTongTien.Value);

            //if (locThongTin.MaxTongTien.HasValue)
            //    q = q.Where(x => x.TongTien <= locThongTin.MaxTongTien.Value);


            //var ngayTaoRange = DateHelper.ParseThangNam(locThongTin.NgayTao);
            //if (ngayTaoRange.Start.HasValue && ngayTaoRange.End.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.NgayTao >= ngayTaoRange.Start.Value &&
            //        x.NgayTao <= ngayTaoRange.End.Value);
            //}
            return q;
        }
    }
}
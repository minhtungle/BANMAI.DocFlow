using Public.Helpers;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyLichDieuTri.Dtos
{
    public class LocThongTinDto
    {
        public DateTimeOffset? ThoiGianBatDau { get; set; }
        public DateTimeOffset? ThoiGianKetThuc { get; set; }
        public List<Guid?> IdLichDieuTris { get; set; }
        public Guid? IdPhieuKham { get; set; }
        public Guid? IdBacSyKham { get; set; }
        public Guid? IdBacSyDieuTri { get; set; }
        public string NoiDungKham { get; set; }
        public string NoiDungDieuTri { get; set; }
        public int? LoaiKham { get; set; }
        public string GhiChu { get; set; }
        public LocThongTinDto()
        {
            var now = ClockHelper.UtcNow; // hoặc ClockHelper.UtcNow nếu bạn muốn UTC

            // 🔹 Đầu tháng hiện tại
            var firstDay = new DateTime(now.Year, now.Month, 1);

            // 🔹 Cuối tháng hiện tại (ngày cuối cùng trong tháng)
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // 🔹 Format theo yêu cầu MM/yyyy
            ThoiGianBatDau = firstDay;
            //ThoiGianKetThuc = lastDay;
        }
    }
}
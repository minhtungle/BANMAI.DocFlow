using Public.Helpers;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyBenhNhan.Dtos
{
    public class LocThongTinDto
    {
        public DateTimeOffset? ThoiGianBatDau { get; set; }
        public DateTimeOffset? ThoiGianKetThuc { get; set; }
        public List<Guid?> IdBenhNhans { get; set; }
        public string HoTen { get; set; }

        public Nullable<bool> GioiTinh { get; set; }

        public Nullable<System.DateTime> NgaySinh { get; set; }

        public string SoDienThoai { get; set; }

        public string Email { get; set; }

        public string CCCD { get; set; }

        public string NgheNghiep { get; set; }

        public string DiaChi { get; set; }

        public string TienSuBenh { get; set; }

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
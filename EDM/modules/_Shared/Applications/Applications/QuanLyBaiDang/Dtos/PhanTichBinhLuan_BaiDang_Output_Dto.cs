using System.Collections.Generic;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class PhanTichBinhLuan_BaiDang_Output_Dto
    {
        public string DanhGiaTongQuanBaiDang { get; set; } // Đánh giá tổng quan của bài đăng
        public List<BinhLuanDaPhanTich> BinhLuanDaPhanTichs { get; set; } // Danh sách bình luận đã được phân tích
    }
    public class BinhLuanDaPhanTich
    {
        public string BinhLuanGoc { get; set; } // Bản gốc của bình luận
        public string DanhGiaBinhLuan { get; set; } // Bình luận đã được phân tích
        public string GoiYTraLoi { get; set; } // Gợi ý trả lời cho bình luận
    }
}
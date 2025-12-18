using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class PhanTichBinhLuan_BaiDang_Input_Dto
    {
        public Guid IdBaiDang { get; set; } // Id của bài đăng cần phân tích
        public Guid IdAITool { get; set; } // Id của công cụ AI được sử dụng để phân tích bài đăng
        public string Content { get; set; } // Nội dung của bài đăng cần phân tích
    }

}
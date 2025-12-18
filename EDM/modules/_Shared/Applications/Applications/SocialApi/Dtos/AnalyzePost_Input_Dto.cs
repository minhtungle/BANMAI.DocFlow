using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class AnalyzePost_Input_Dto
    {
        public Guid IdAITool { get; set; } // Id của công cụ AI được sử dụng để phân tích bài viết
        public string Prompt { get; set; } // Prompt được sử dụng để phân tích bài viết
        = "";
    }
}
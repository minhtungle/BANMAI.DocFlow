using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAIBot.Dtos
{
    public class TestNoiDungAI_Input_Dto
    {
        public Guid IdAITool { get; set; }
        public Guid IdAIBot { get; set; }
        public string Keywords { get; set; } = string.Empty;
    }
}
using Applications.QuanLyBaiDang.Models;
using Applications.SocialApi.Dtos;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class XemChiTiet_Output_Dto
    {
        public tbBaiDangExtend BaiDang { get; set; }
        public List<SocialCommentDto> BinhLuans { get; set; }

        public List<tbChienDich> ChienDichs { get; set; } = new List<tbChienDich>();
        public List<tbTaiKhoan> TaiKhoans { get; set; } = new List<tbTaiKhoan>();
        public List<tbNenTang> NenTangs { get; set; } = new List<tbNenTang>();
        public List<tbAIBot> AIBots { get; set; } = new List<tbAIBot>();
        public List<tbAITool> AITools { get; set; } = new List<tbAITool>();
    }
}
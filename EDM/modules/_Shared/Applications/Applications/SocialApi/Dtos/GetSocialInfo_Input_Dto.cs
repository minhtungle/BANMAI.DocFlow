using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class GetSocialInfo_Input_Dto
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public Guid IdNenTang { get; set; }
        public string LoaiTaiKhoan { get; set; }
    }
}
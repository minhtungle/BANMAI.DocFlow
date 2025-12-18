using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialApi_Input_Dto
    {
        private string _tenNenTang;

        public string UserId { get; set; } = "me";
        public string PageId { get; set; } = "me";
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string AccessToken { get; set; }
        public string TenNenTang
        {
            get => _tenNenTang?.ToLower();
            set => _tenNenTang = value;
        }
    }
}
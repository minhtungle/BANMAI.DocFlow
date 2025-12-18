using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialCommentAnalysis
    {
        public string Message { get; set; }
        public string CamXuc { get; set; }
        public string TyLeMuaHang { get; set; }
        public string GiaiDoanMuaHang { get; set; }
        public string HanhDongKhuyenNghi { get; set; }
        public string SuggestedReply { get; set; }
        public List<SocialCommentAnalysis> SubComments { get; set; } = new List<SocialCommentAnalysis>();
    }
}
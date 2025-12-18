using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Applications.SocialApi.Helper.TopicHelper;

namespace Applications.SocialApi.Helper
{
    public static class SocialAnalyzer
    {
        public static SocialTopic DetectTopic(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return SocialTopic.Khac;

            content = content.ToLower();
            if (content.Contains("giảm giá") || content.Contains("mua ngay") || content.Contains("sản phẩm"))
                return SocialTopic.BanHang;
            if (content.Contains("tuyển dụng") || content.Contains("việc làm") || content.Contains("ứng tuyển"))
                return SocialTopic.TuyenDung;
            if (content.Contains("sự kiện") || content.Contains("mời tham gia"))
                return SocialTopic.SuKien;
            if (content.Contains("giải trí") || content.Contains("xem ngay") || content.Contains("video"))
                return SocialTopic.GiaiTri;
            return SocialTopic.Khac;
        }

        public static List<SocialCommentAnalysis> AnalyzeComments(SocialTopic topic, List<string> comments)
        {
            var result = new List<SocialCommentAnalysis>();
            var criteria = TopicCriteriaDictionary.Criteria.ContainsKey(topic) ? TopicCriteriaDictionary.Criteria[topic] : TopicCriteriaDictionary.Criteria[SocialTopic.Khac];

            foreach (var comment in comments)
            {
                var camXuc = DetectSentiment(comment);

                result.Add(new SocialCommentAnalysis
                {
                    Message = comment,
                    CamXuc = camXuc,
                    TyLeMuaHang = criteria.TyleMuaTheoCamXuc[camXuc],
                    //GiaiDoanMuaHang = criteria.GiaiDoanTheoCamXuc[camXuc],
                    HanhDongKhuyenNghi = criteria.HanhDongTheoCamXuc[camXuc],
                    SuggestedReply = SuggestReply(camXuc)
                });
            }

            return result;
        }

        private static string DetectSentiment(string comment)
        {
            comment = comment.ToLower();
            if (comment.Contains("ghét") || comment.Contains("xấu") || comment.Contains("chán") || comment.Contains("không thích"))
                return "Tiêu cực";
            if (comment.Contains("thích") || comment.Contains("đẹp") || comment.Contains("tuyệt") || comment.Contains("ưng ý"))
                return "Tích cực";
            return "Trung lập";
        }

        private static string SuggestReply(string camXuc)
        {
            if (camXuc == "Tiêu cực")
                return "Cảm ơn bạn đã góp ý. Chúng tôi sẽ cải thiện để phục vụ bạn tốt hơn!";
            if (camXuc == "Tích cực")
                return "Cảm ơn bạn đã ủng hộ sản phẩm/dịch vụ của chúng tôi!";
            return "Cảm ơn bạn đã bình luận. Nếu có thắc mắc thêm hãy liên hệ với chúng tôi nhé!";
        }
    }
}
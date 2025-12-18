using System.Collections.Generic;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;

namespace Applications.SocialApi.Helper
{
    public class TopicHelper
    {
        public static class TopicCriteriaDictionary
        {
            public static readonly Dictionary<SocialTopic, TopicAnalysisCriteria> Criteria = new Dictionary<SocialTopic, TopicAnalysisCriteria>()
            {
                [SocialTopic.BanHang] = new TopicAnalysisCriteria
                {
                    TopicName = "Bán hàng",
                    TyleMuaTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "90%",
                        ["Tiêu cực"] = "10%",
                        ["Trung lập"] = "50%"
                    },
                    HanhDongTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "Đẩy mạnh remarketing hoặc ưu đãi.",
                        ["Tiêu cực"] = "CSKH ngay lập tức, cải thiện sản phẩm.",
                        ["Trung lập"] = "Gửi ưu đãi riêng để tạo động lực mua."
                    }
                },
                [SocialTopic.TuyenDung] = new TopicAnalysisCriteria
                {
                    TopicName = "Tuyển dụng",
                    TyleMuaTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "80%",
                        ["Tiêu cực"] = "15%",
                        ["Trung lập"] = "45%"
                    },
                    HanhDongTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "Mời tham gia phỏng vấn, gửi JD chi tiết.",
                        ["Tiêu cực"] = "Tìm hiểu nguyên nhân phản ứng, cải thiện JD.",
                        ["Trung lập"] = "Hỏi thêm nhu cầu ứng viên."
                    }
                },
                [SocialTopic.SuKien] = new TopicAnalysisCriteria
                {
                    TopicName = "Sự kiện",
                    TyleMuaTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "85%",
                        ["Tiêu cực"] = "20%",
                        ["Trung lập"] = "60%"
                    },
                    HanhDongTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "Gửi link đăng ký tham gia.",
                        ["Tiêu cực"] = "Cải thiện tổ chức, phản hồi nhanh.",
                        ["Trung lập"] = "Giới thiệu thêm lợi ích khi tham gia."
                    }
                },
                [SocialTopic.GiaiTri] = new TopicAnalysisCriteria
                {
                    TopicName = "Giải trí",
                    TyleMuaTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "95%",
                        ["Tiêu cực"] = "5%",
                        ["Trung lập"] = "40%"
                    },
                    HanhDongTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "Kêu gọi chia sẻ hoặc follow.",
                        ["Tiêu cực"] = "Lắng nghe góp ý nội dung.",
                        ["Trung lập"] = "Đăng nội dung tiếp theo liên quan."
                    }
                },
                [SocialTopic.Khac] = new TopicAnalysisCriteria
                {
                    TopicName = "Khác",
                    TyleMuaTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "60%",
                        ["Tiêu cực"] = "25%",
                        ["Trung lập"] = "35%"
                    },
                    HanhDongTheoCamXuc = new Dictionary<string, string>()
                    {
                        ["Tích cực"] = "Tương tác duy trì.",
                        ["Tiêu cực"] = "Ghi nhận và xem xét lại nội dung.",
                        ["Trung lập"] = "Quan sát thêm."
                    }
                }
                // TODO: Add remaining topics following the same structure
            };
        }
    }
}
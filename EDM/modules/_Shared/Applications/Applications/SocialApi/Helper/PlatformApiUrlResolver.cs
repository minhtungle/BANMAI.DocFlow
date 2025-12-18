using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;
using Applications.SocialApi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Applications.SocialApi.Helpers
{
    public class PlatformApiUrlResolver : IPlatformApiUrlResolver
    {
        private readonly Dictionary<(string NenTang, LoaiApiEnum LoaiApi), Func<SocialApi_Input_Dto, string>> _urlGenerators;
        private readonly string _faceBookApi_Ver = "v22.0";
        private readonly int limit = 5;

        public PlatformApiUrlResolver()
        {
            _urlGenerators = new Dictionary<(string, LoaiApiEnum), Func<SocialApi_Input_Dto, string>>(new PlatformKeyComparer())
        {
            // Facebook
            { ("facebook", LoaiApiEnum.UserInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}?fields=id,name,link,email&access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.PageList), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}/accounts?access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.PageInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}?fields=id,name,about,category,fan_count,link&access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.PostInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PostId}?fields=message,likes.summary(true),comments.summary(true)&access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.PostComments), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PostId}/comments?summary=true&limit=100&access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.SubComments), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PostId}/comments?limit={limit}&access_token={input.AccessToken}" },
            { ("facebook", LoaiApiEnum.ReplyComment), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.CommentId}/comments" },
            // Zalo
            { ("zalo", LoaiApiEnum.UserInfo), input => $"https://openapi.zalo.me/v2.0/{input.PageId}?access_token={input.AccessToken}" },
            { ("zalo", LoaiApiEnum.PageInfo), input => $"https://openapi.zalo.me/v2.0/page/info?access_token={input.AccessToken}" },

            // Instagram - special handling marker
            // Cách 1: Nhận input của Fb User => danh sách Page => lấy Instagram Business Account có liên kết với page
            //{ ("instagram", LoaiApiEnum.PageInfo), input => "SPECIAL:InstagramFlow" },
            // Cách 2: Id là của instagram Business Account, Access Token là của Page có liên kết
            { ("instagram", LoaiApiEnum.PageInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}?fields=id,username,profile_picture_url,biography&access_token={input.AccessToken}" },

            { ("instagram", LoaiApiEnum.PostInfo), input => $"https://graph.facebook.com/ {_faceBookApi_Ver} / {input.PostId} ?fields=caption,like_count,comments_count&access_token= {input.AccessToken}" },
            { ("instagram", LoaiApiEnum.PostComments), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PostId}/comments?summary=true&limit=100&access_token={input.AccessToken}" },
            { ("instagram", LoaiApiEnum.SubComments), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PostId}/comments?limit={limit}&access_token={input.AccessToken}" },
            { ("instagram", LoaiApiEnum.ReplyComment), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.CommentId}/comments" },
            // Threads
            { ("threads", LoaiApiEnum.UserInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}?fields=id,username&access_token={input.AccessToken}" },
            { ("threads", LoaiApiEnum.PageInfo), input => $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}/threads?access_token={input.AccessToken}" },

            // TikTok (ví dụ)
            { ("tiktok", LoaiApiEnum.UserInfo), input => $"https://open.tiktokapis.com/v2/user/info/?access_token={input.AccessToken}" },

            // LinkedIn (ví dụ)
            { ("linkedin", LoaiApiEnum.UserInfo), input => $"https://api.linkedin.com/v2/{input.PageId}?oauth2_access_token={input.AccessToken}" }
        };
        }

        public bool CanResolve(LoaiApiEnum loaiApi, SocialApi_Input_Dto input)
            => _urlGenerators.ContainsKey((input.TenNenTang, loaiApi));

        public string ResolveUrl(LoaiApiEnum loaiApi, SocialApi_Input_Dto input)
        {
            if (!_urlGenerators.TryGetValue((input.TenNenTang, loaiApi), out var generator))
                throw new Exception($"Không tìm thấy cấu hình API cho {input.TenNenTang} - {loaiApi}");

            return generator(input);
        }
    }
}
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using Applications.SocialApi._Facebook.Dtos;
using Applications.SocialApi._Facebook.Interfaces;
using Applications.SocialApi._Instagram.Dtos;
using Applications.SocialApi._Instagram.Interfaces;
using Applications.SocialApi._Thread.Dtos;
using Applications.SocialApi._Thread.Interfaces;
using Applications.SocialApi._Zalo.Dtos;
using Applications.SocialApi._Zalo.Interfaces;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.SocialApi.Services
{
    public class SocialApiService : ISocialApiService
    {
        private readonly IFacebookApiService _facebook;
        private readonly IInstagramApiService _instagram;
        private readonly IZaloApiService _zalo;
        private readonly IThreadsApiService _threads;
        private readonly IQuanLyAIToolService _quanLyAIToolService;

        public SocialApiService(
            IQuanLyAIToolService quanLyAIToolService,

            IFacebookApiService facebook,
            IInstagramApiService instagram,
            IThreadsApiService threads,
            IZaloApiService zalo)
        {
            _quanLyAIToolService = quanLyAIToolService;

            _facebook = facebook;
            _instagram = instagram;
            _threads = threads;
            _zalo = zalo;
        }
        public async Task<SocialUserWithPagesDto> GetUserAndPagesAsync(SocialApi_Input_Dto input)
        {
            var nenTang = input.TenNenTang.ToLower();
            if (nenTang == "facebook")
            {
                var user = JsonConvert.DeserializeObject<FacebookUserDto>(await _facebook.GetUserInfoAsync(input: input));
                var pages = JsonConvert.DeserializeObject<FacebookPageListDto>(await _facebook.GetPageListAsync(input: input));

                var result = new SocialUserWithPagesDto
                {
                    User = new SocialUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Link = user.Link,
                        Platform = input.TenNenTang,
                    },
                    Pages = pages.Data.Select(p => new SocialPageDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Category = p.Category,
                        AccessToken = p.AccessToken,
                        Platform = "facebook",
                        About = p.About,
                        FanCount = p.FanCount,
                        Link = p.Link
                    }).ToList()
                };

                return result;
            }
            //if (nenTang == "instagram")
            //{
            //    var userIg = JsonConvert.DeserializeObject<InstagramUserDto>(await _instagram.GetUserInfoAsync(input.AccessToken));
            //    result.User.Id = userIg.Id;
            //    result.User.Name = userIg.Name;
            //}
            if (nenTang == "threads")
            {
                var user = JsonConvert.DeserializeObject<ThreadsUserDto>(await _threads.GetUserInfoAsync(input: input));
                var result = new SocialUserWithPagesDto
                {
                    User = new SocialUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Platform = input.TenNenTang
                    },
                    Pages = new List<SocialPageDto>()
                };

                return result;
            }
            if (nenTang == "zalo")
            {
                var user = JsonConvert.DeserializeObject<ZaloUserDto>(await _zalo.GetUserInfoAsync(input: input));
                var result = new SocialUserWithPagesDto
                {
                    User = new SocialUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Platform = input.TenNenTang
                    },
                    Pages = new List<SocialPageDto>()
                };

                return result;
            }
            return null;
        }
        public async Task<SocialPageDto> GetPageInfoAsync(SocialApi_Input_Dto input)
        {
            var nenTang = input.TenNenTang.ToLower();
            string json = null;

            if (nenTang == "facebook")
            {
                json = await _facebook.GetPageInfoAsync(input: input);
                var page = JsonConvert.DeserializeObject<FacebookPageDto>(json); // tạm dùng chung DTO Facebook cho tất cả

                return new SocialPageDto
                {
                    Id = page.Id,
                    Name = page.Name,
                    Category = page.Category,
                    AccessToken = page.AccessToken,
                    Platform = input.TenNenTang,
                    About = page.About,
                    FanCount = page.FanCount,
                    Link = page.Link,
                };
            }
            if (nenTang == "zalo")
            {
                json = await _zalo.GetPageInfoAsync(input: input);
                var page = JsonConvert.DeserializeObject<ZaloUserDto>(json); // tạm dùng chung DTO Facebook cho tất cả

                return new SocialPageDto
                {
                    Id = page.Id,
                    Name = page.Name,
                };
            }
            if (nenTang == "instagram")
            {
                json = await _instagram.GetPageInfoAsync(input: input);
                var page = JsonConvert.DeserializeObject<InstaPageDto>(json); // tạm dùng chung DTO Facebook cho tất cả
                return new SocialPageDto
                {
                    Id = page.Id,
                    Name = page.Name,
                    Platform = input.TenNenTang,
                    Link = $"https://www.instagram.com/{page.Name}"
                };
            }
            if (nenTang == "threads")
            {
                json = await _threads.GetPageInfoAsync(input: input);
            }
            throw new NotSupportedException($"Chưa hỗ trợ nền tảng {input.TenNenTang}");
        }
        public async Task<SocialPostInfo> GetPostInfoAsync(SocialApi_Input_Dto input)
        {
            var nenTang = input.TenNenTang.ToLower();
            string json = null;

            if (nenTang == "facebook")
            {
                json = await _facebook.GetPostInfoAsync(input: input);
                var obj = JsonConvert.DeserializeObject<FacebookPostInfoDto>(json);
                var rs = new SocialPostInfo()
                {
                    PostId = input.PostId,
                    Platform = input.TenNenTang,
                    //Content = (string)obj.Ca,
                    //LikeCount = (int)obj.like_count,
                    //CommentCount = (int)obj.comments_count,
                    RawJson = json,
                    RetrievedAt = DateTime.Now
                };

                return rs;
            }
            if (nenTang == "zalo")
            {
                json = await _zalo.GetPageInfoAsync(input: input);
                var page = JsonConvert.DeserializeObject<ZaloUserDto>(json); // tạm dùng chung DTO Facebook cho tất cả
            }
            if (nenTang == "instagram")
            {
                json = await _facebook.GetPostInfoAsync(input: input);
                var obj = JsonConvert.DeserializeObject<InstagramPostInfoDto>(json);
                var rs = new SocialPageDto() { };

                return new SocialPostInfo
                {
                    PostId = input.PostId,
                    Platform = input.TenNenTang,
                    //Content = input.TenNenTang == "instagram" ? (string)obj.caption : (string)obj.message,
                    //LikeCount = platform.ToLower() == "instagram" ? (int)obj.like_count : (int)obj.likes.summary.total_count,
                    //CommentCount = platform.ToLower() == "instagram" ? (int)obj.comments_count : (int)obj.comments.summary.total_count,
                    RawJson = json,
                    RetrievedAt = DateTime.Now
                };
            }
            if (nenTang == "threads")
            {
                json = await _threads.GetPageInfoAsync(input: input);
                var page = JsonConvert.DeserializeObject<ThreadsUserDto>(json); // tạm dùng chung DTO Facebook cho tất cả
            }
            throw new NotSupportedException($"Chưa hỗ trợ nền tảng {input.TenNenTang}");
        }
        public async Task<List<SocialCommentDto>> GetPostCommentsAsync(SocialApi_Input_Dto input)
        {
            var nenTang = input.TenNenTang.ToLower();
            string json = null;

            if (nenTang == "facebook")
            {
                json = await _facebook.GetSubCommentsAsync(input: input);
                var dataOnly = JObject.Parse(json)["data"].ToString();
                var cmts = JsonConvert.DeserializeObject<List<FacebookCommentDto>>(dataOnly);

                var rs = cmts.Select(c => new SocialCommentDto
                {
                    Id = c.Id,
                    Message = c.Message,
                    CreatedTime = c.CreatedTime,
                    From = c.From == null
                    ? new SocialCommentFrom()
                    : new SocialCommentFrom
                    {
                        Id = c.From.Id,
                        Name = c.From.Name
                    }
                }).ToList();
                return rs;
            }
            if (nenTang == "instagram")
            {
                json = await _instagram.GetPostCommentsAsync(input: input);
                var dataOnly = JObject.Parse(json)["data"].ToString();
                var cmts = JsonConvert.DeserializeObject<List<InstagramCommentDto>>(dataOnly);

                var rs = cmts.Select(c => new SocialCommentDto
                {
                    Id = c.Id,
                    Message = c.Message,
                    CreatedTime = c.CreatedTime,
                    From = c.From == null
                    ? new SocialCommentFrom()
                    : new SocialCommentFrom
                    {
                        Id = c.From.Id,
                        Name = c.From.Name
                    }
                }).ToList();
                return rs;
            }
            //if (nenTang == "zalo")
            //{
            //    json = await _zalo.GetPostCommentsAsync(input.AccessToken);
            //    var page = JsonConvert.DeserializeObject<ZaloUserDto>(json); // tạm dùng chung DTO Facebook cho tất cả

            //    return new SocialPageDto
            //    {
            //        Id = page.Id,
            //        Name = page.Name,
            //        Category = page.Category,
            //        AccessToken = page.AccessToken,
            //        Platform = input.TenNenTang,
            //        About = page.About,
            //        FanCount = page.FanCount,
            //        Link = page.Link,
            //    };
            //}

            if (nenTang == "threads")
            {
                //json = await _threads.GetPostCommentsAsync(input: input);
                //var cmts = JsonConvert.DeserializeObject<List<InstagramCommentDto>>(json); // tạm dùng chung DTO Facebook cho tất cả

                //var rs = cmts.Select(c => new SocialCommentDto
                //{
                //    Id = c.Id,
                //    Message = c.Message,
                //    CreatedTime = c.CreatedTime,
                //    From = new SocialCommentFrom
                //    {
                //        Id = c.From.Id,
                //        Name = c.From.Name
                //    }
                //}).ToList();
            }
            throw new NotSupportedException($"Chưa hỗ trợ nền tảng {input.TenNenTang}");
        }
        public async Task<List<SocialCommentDto>> GetSubCommentsAsync(SocialApi_Input_Dto input)
        {
            var nenTang = input.TenNenTang.ToLower();
            string json = null;

            var rs = new List<SocialCommentDto>();
            if (nenTang == "facebook")
            {
                json = await _facebook.GetSubCommentsAsync(input: input);
                var dataOnly = JObject.Parse(json)["data"].ToString();
                var cmts = JsonConvert.DeserializeObject<List<FacebookCommentDto>>(dataOnly);

                foreach (var c in cmts)
                {
                    rs.Add(new SocialCommentDto
                    {
                        Message = c.Message,
                        CreatedTime = c.CreatedTime,
                        From = c.From == null
                        ? new SocialCommentFrom()
                        : new SocialCommentFrom
                        {
                            Id = c.From.Id,
                            Name = c.From.Name
                        }
                    });
                }
                return rs;
            }
            if (nenTang == "instagram")
            {
                json = await _instagram.GetSubCommentsAsync(input: input);
                var dataOnly = JObject.Parse(json)["data"].ToString();
                var cmts = JsonConvert.DeserializeObject<List<InstagramCommentDto>>(dataOnly);

                foreach (var c in cmts)
                {
                    rs.Add(new SocialCommentDto
                    {
                        Message = c.Message,
                        CreatedTime = c.CreatedTime,
                        From = c.From == null
                        ? new SocialCommentFrom()
                        : new SocialCommentFrom
                        {
                            Id = c.From.Id,
                            Name = c.From.Name
                        }
                    });
                }
                return rs;
            }
            throw new NotSupportedException($"Chưa hỗ trợ nền tảng {input.TenNenTang}");
        }
        public async Task ReplyToCommentAsync(string commentId, string message, string accessToken)
        {
            //var url = $"https://graph.facebook.com/v19.0/{commentId}/comments";
            //var content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("message", message),
            //    new KeyValuePair<string, string>("access_token", accessToken)
            //});
            //var response = await _httpClient.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();
        }
        public async Task<string> AnalyzePostAsync(AnalyzePost_Input_Dto input)
        {
            // Phân tích bình luận bằng AI Tool
            string rs = await _quanLyAIToolService.WorkWithAITool(input: new WorkWithAITool_Input_Dto
            {
                IdAITool = input.IdAITool,
                Prompt = input.Prompt,
            });
            return rs;
        }
    }
}
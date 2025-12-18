using Applications.SocialApi._Instagram.Interfaces;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;
using Applications.SocialApi.Helper;
using Applications.SocialApi.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applications.SocialApi._Instagram.Services
{
    public class InstagramApiService : IInstagramApiService
    {
        private readonly HttpClient _http;
        private readonly IPlatformApiUrlResolver _resolver;

        public InstagramApiService(HttpClient httpClient, IPlatformApiUrlResolver resolver)
        {
            _resolver = resolver;
            _http = httpClient;
        }

        public async Task<string> GetPageInfoAsync(SocialApi_Input_Dto input)
        {
            // Cách 1: Nhận input của Fb User => danh sách Page => lấy Instagram Business Account có liên kết với page
            //var url = await InstagramHelper.BuildInstagramInfoUrlAsync(input: input, http: _http);
            // Cách 2: Id là của instagram Business Account, Access Token là của Page có liên kết
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PageInfo, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> GetPostInfoAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PostInfo, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> GetPostCommentsAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PostComments, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> GetSubCommentsAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.SubComments, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> ReplyCommentsAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.SubComments, input: input);
            return await _http.GetStringAsync(url);
        }
    }
}
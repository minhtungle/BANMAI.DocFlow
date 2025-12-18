using Applications.SocialApi._Thread.Interfaces;
using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;
using Applications.SocialApi.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Applications.SocialApi._Thread.Services
{
    public class ThreadsApiService : IThreadsApiService
    {
        private readonly HttpClient _http;
        private readonly IPlatformApiUrlResolver _resolver;

        public ThreadsApiService(HttpClient httpClient, IPlatformApiUrlResolver resolver)
        {
            _resolver = resolver;
            _http = httpClient;
        }

        public async Task<string> GetUserInfoAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.UserInfo, input: input);
            return await _http.GetStringAsync(url);
        }

        public async Task<string> GetPageInfoAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PageInfo, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> GetPostInfoAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PostInfo, input: input);
            return await _http.GetStringAsync(url);
        }
        public async Task<string> GetPostCommentsInfoAsync(SocialApi_Input_Dto input)
        {
            var url = _resolver.ResolveUrl(loaiApi: LoaiApiEnum.PostComments, input: input);
            return await _http.GetStringAsync(url);
        }
    }
}
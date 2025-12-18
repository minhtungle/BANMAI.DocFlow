using Applications.SocialApi.Dtos;
using System.Threading.Tasks;

namespace Applications.SocialApi._Thread.Interfaces
{
    public interface IThreadsApiService
    {
        Task<string> GetUserInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPageInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostCommentsInfoAsync(SocialApi_Input_Dto input);
    }
}

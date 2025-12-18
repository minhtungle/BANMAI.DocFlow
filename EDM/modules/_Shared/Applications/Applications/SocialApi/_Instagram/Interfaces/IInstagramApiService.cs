using Applications.SocialApi.Dtos;
using System.Threading.Tasks;

namespace Applications.SocialApi._Instagram.Interfaces
{
    public interface IInstagramApiService
    {
        Task<string> GetPageInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostCommentsAsync(SocialApi_Input_Dto input);
        Task<string> GetSubCommentsAsync(SocialApi_Input_Dto input);
        Task<string> ReplyCommentsAsync(SocialApi_Input_Dto input);
    }
}

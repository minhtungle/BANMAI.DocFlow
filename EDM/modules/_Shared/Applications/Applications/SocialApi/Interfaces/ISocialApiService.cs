using Applications.SocialApi.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.SocialApi.Interfaces
{
    public interface ISocialApiService
    {
        Task<SocialUserWithPagesDto> GetUserAndPagesAsync(SocialApi_Input_Dto input);
        Task<SocialPageDto> GetPageInfoAsync(SocialApi_Input_Dto input);
        Task<SocialPostInfo> GetPostInfoAsync(SocialApi_Input_Dto input);
        Task<List<SocialCommentDto>> GetPostCommentsAsync(SocialApi_Input_Dto input);
        Task<string> AnalyzePostAsync(AnalyzePost_Input_Dto input);
    }
}

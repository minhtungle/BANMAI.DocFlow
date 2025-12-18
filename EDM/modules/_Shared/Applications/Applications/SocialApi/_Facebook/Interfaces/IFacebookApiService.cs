using Applications.SocialApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.SocialApi._Facebook.Interfaces
{
    public interface IFacebookApiService
    {
        Task<string> GetUserInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPageListAsync(SocialApi_Input_Dto input);
        Task<string> GetPageInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetSubCommentsAsync(SocialApi_Input_Dto input);
        Task<string> ReplyCommentsAsync(SocialApi_Input_Dto input);
    }
}

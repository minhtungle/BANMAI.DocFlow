using Applications.SocialApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.SocialApi._Zalo.Interfaces
{
    public interface IZaloApiService
    {
        Task<string> GetUserInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPageInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostInfoAsync(SocialApi_Input_Dto input);
        Task<string> GetPostCommentsInfoAsync(SocialApi_Input_Dto input);
    }
}

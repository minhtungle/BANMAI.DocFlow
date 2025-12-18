using Applications.SocialApi.Dtos;
using Applications.SocialApi.Enums;

namespace Applications.SocialApi.Interfaces
{
    public interface IPlatformApiUrlResolver
    {
        bool CanResolve(LoaiApiEnum loaiApi, SocialApi_Input_Dto input);
        string ResolveUrl(LoaiApiEnum loaiApi, SocialApi_Input_Dto input);
    }
}
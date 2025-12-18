using Applications.SocialApi.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Applications.SocialApi.Helper
{
    public class InstagramHelper
    {
        private static string _faceBookApi_Ver = "v22.0";
        public static async Task<string> BuildInstagramInfoUrlAsync(SocialApi_Input_Dto input, HttpClient http)
        {
            string urlPages = $"https://graph.facebook.com/{_faceBookApi_Ver}/{input.PageId}/accounts?access_token={input.AccessToken}";
            var pagesJson = await http.GetStringAsync(urlPages);
            dynamic pages = JsonConvert.DeserializeObject(pagesJson);

            if (pages?.data == null || pages.data.Count == 0)
                throw new Exception("Không tìm thấy Facebook Page nào.");

            foreach (var page in pages.data)
            {
                string pageId = page.id;
                string pageAccessToken = page.access_token;

                //string _urlGetIg = $"https://graph.facebook.com/{_faceBookApi_Ver}/{pageId}?fields=instagram_business_account&access_token={input.AccessToken}";
                string urlGetIg = $"https://graph.facebook.com/{_faceBookApi_Ver}/{pageId}?fields=instagram_business_account,name,link&access_token={pageAccessToken}";

                var pageDetailJson = await http.GetStringAsync(urlGetIg);
                //var _pageDetailJson = await http.GetStringAsync(_urlGetIg);
                dynamic pageDetail = JsonConvert.DeserializeObject(pageDetailJson);

                string igId = pageDetail?.instagram_business_account?.id;
                if (!string.IsNullOrEmpty(igId))
                {
                    string a = $"https://graph.facebook.com/{_faceBookApi_Ver}/{igId}?fields=id,username,profile_picture_url,biography&access_token={pageAccessToken}";
                    //string a = $"https://graph.facebook.com/{_faceBookApi_Ver}/{igId}?fields=id,username,profile_picture_url,biography&access_token={input.AccessToken}";
                    //var b = await http.GetStringAsync(a);
                    return a;
                }
            }

            throw new Exception("Không tìm thấy Instagram liên kết với Page nào.");
        }
    }
}
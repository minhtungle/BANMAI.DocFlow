using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi._Instagram.Dtos
{
    public class InstagramCommentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("text")]
        public string Message { get; set; }
        public InstagramUser From { get; set; }
        [JsonProperty("time_stamp")]
        public DateTime CreatedTime { get; set; }
    }

    public class InstagramUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Name { get; set; }
    }

}
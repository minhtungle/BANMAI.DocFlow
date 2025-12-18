using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi._Facebook.Dtos
{
    public class FacebookCommentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        public FacebookCommentFrom From { get; set; }
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
    }

    public class FacebookCommentFrom
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
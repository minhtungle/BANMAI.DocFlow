using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi._Facebook.Dtos
{
    public class FacebookPageDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("fan_count")]
        public int FanCount { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("tasks")]
        public List<string> Tasks { get; set; }
    }
}
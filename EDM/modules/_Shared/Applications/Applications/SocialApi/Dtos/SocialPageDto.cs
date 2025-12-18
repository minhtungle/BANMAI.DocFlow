using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialPageDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public string Category { get; set; }
        public string AccessToken { get; set; }
        public string About { get; set; }
        public int FanCount { get; set; }
        public string Link { get; set; }
    }
}
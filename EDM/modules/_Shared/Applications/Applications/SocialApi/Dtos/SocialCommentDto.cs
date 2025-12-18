using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialCommentDto
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public SocialCommentFrom From { get; set; }
        public DateTime CreatedTime { get; set; }

    }
    public class SocialCommentFrom
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
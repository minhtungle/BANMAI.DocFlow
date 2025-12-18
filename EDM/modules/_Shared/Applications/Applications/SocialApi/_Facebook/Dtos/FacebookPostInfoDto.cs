using Applications.SocialApi.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi._Facebook.Dtos
{
    public class FacebookPostInfoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string PostId { get; set; }
        public string Platform { get; set; }
        public string AccountType { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public string RawJson { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.Now;
        public string DetectedTopic { get; set; }
        public List<SocialCommentAnalysis> AnalyzedComments { get; set; } = new List<SocialCommentAnalysis>();
    }
}
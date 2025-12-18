using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class TopicAnalysisCriteria
    {
        public string TopicName { get; set; }
        public Dictionary<string, string> TyleMuaTheoCamXuc { get; set; }
        public Dictionary<string, string> HanhDongTheoCamXuc { get; set; }
    }
}
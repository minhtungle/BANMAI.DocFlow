using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; } // facebook | instagram | threads | zalo
        public string Link { get; set; }
    }
}
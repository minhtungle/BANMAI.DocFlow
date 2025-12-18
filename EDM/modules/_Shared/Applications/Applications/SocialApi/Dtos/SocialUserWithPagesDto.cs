using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.SocialApi.Dtos
{
    public class SocialUserWithPagesDto
    {
        public SocialUserDto User { get; set; }            // Thông tin user
        public List<SocialPageDto> Pages { get; set; }
    }
}
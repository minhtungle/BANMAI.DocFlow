using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDonVi.Dtos
{
    public class GetList_DonVi_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbDonViSuDung> DonVis { get; set; }
    }
}
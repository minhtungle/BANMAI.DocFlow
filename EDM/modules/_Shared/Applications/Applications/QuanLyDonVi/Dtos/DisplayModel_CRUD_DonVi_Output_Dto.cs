using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDonVi.Dtos
{
    public class DisplayModel_CRUD_DonVi_Output_Dto
    {
        public tbDonViSuDung DonVi { get; set; } = new tbDonViSuDung();
        public string Loai { get; set; }
    }
}
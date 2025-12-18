using Applications.QuanLyBenhNhan.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBenhNhan.Models
{
    public class XemChiTiet_BenhNhan_Partial_Model<T>
    {
        public XemChiTiet_BenhNhan_Output_Dto SharedData { get; set; }
        public T Data { get; set; }
    }
}
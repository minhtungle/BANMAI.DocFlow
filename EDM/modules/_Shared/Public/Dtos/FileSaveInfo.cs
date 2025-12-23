using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Dtos
{
    public class FileSaveInfo
    {
        public string OriginalFileName { get; set; }
        public string FileNameWithoutExtension { get; set; }          // tên gốc
        public string SafeName { get; set; }          // tên không dấu, đã chuẩn hoá (không ext)
        public string Extension { get; set; }         // ".pdf"
        public string StoredFileName { get; set; }    // "ten-khong-dau-abc123.pdf"
        public string RelativePath { get; set; }      // "2025/12/23/ten-khong-dau-abc123.pdf"
        public string PhysicalPath { get; set; }      // "D:\...\Uploads\TaiLieu\2025\12\23\ten-....pdf"
        public string OnlineUrl { get; set; }         // "/Uploads/TaiLieu/2025/12/23/ten-....pdf"
        public string MimeType { get; set; }          // "application/pdf"
        public long SizeBytes { get; set; }
    }
}
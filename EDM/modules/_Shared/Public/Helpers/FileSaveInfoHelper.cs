using Public.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Public.Helpers
{
    public class FileSaveInfoHelper
    {
        /// <summary>
        /// Chuẩn hoá tên (không ext): bỏ dấu, khoảng trắng -> separator, bỏ ký tự đặc biệt.
        /// </summary>
        public static string ToSlugNoDiacritics(string input, char separator = '-')
        {
            if (string.IsNullOrWhiteSpace(input)) return "file";

            // 1) Normalize + remove diacritics
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            // 2) Lowercase + replace Đ/đ
            noDiacritics = noDiacritics.Replace('Đ', 'D').Replace('đ', 'd').ToLowerInvariant();

            // 3) Replace whitespace with separator
            noDiacritics = Regex.Replace(noDiacritics, @"\s+", separator.ToString());

            // 4) Remove invalid chars: keep a-z 0-9, separator, underscore, dot (tuỳ bạn)
            //    Ở đây giữ: chữ/số, separator, '_' 
            var sepEsc = Regex.Escape(separator.ToString());
            noDiacritics = Regex.Replace(noDiacritics, $"[^a-z0-9{sepEsc}_]+", "");

            // 5) Collapse multiple separators
            noDiacritics = Regex.Replace(noDiacritics, $"{sepEsc}{{2,}}", separator.ToString());

            // 6) Trim separators
            noDiacritics = noDiacritics.Trim(separator, '_');

            return string.IsNullOrWhiteSpace(noDiacritics) ? "file" : noDiacritics;
        }

        /// <summary>
        /// Tạo thông tin lưu file: tên an toàn, tên lưu thực tế, relative/physical/online.
        /// </summary>
        /// <param name="file">HttpPostedFileBase</param>
        /// <param name="rootPhysical">VD: Server.MapPath("~/Uploads/TaiLieu")</param>
        /// <param name="baseOnlineUrl">VD: "/Uploads/TaiLieu" hoặc "https://domain.com/Uploads/TaiLieu"</param>
        /// <param name="separator">ký tự thay khoảng trắng, mặc định '-'</param>
        /// <param name="addUniqueSuffix">thêm hậu tố unique để tránh trùng</param>
        /// <param name="subFolder">null => tự chia theo yyyy/MM/dd</param>
        public static FileSaveInfo BuildFileSaveInfo(
            HttpPostedFileBase file,
            string rootPhysical,
            string baseOnlineUrl,
            char separator = '-',
            bool addUniqueSuffix = true,
            string subFolder = null
        )
        {
            if (file == null || file.ContentLength <= 0)
                throw new ArgumentException("File không hợp lệ.");

            if (string.IsNullOrWhiteSpace(rootPhysical))
                throw new ArgumentException("rootPhysical không được rỗng.");

            if (string.IsNullOrWhiteSpace(baseOnlineUrl))
                baseOnlineUrl = ""; // cho phép để trống nếu bạn không cần online url

            var original = Path.GetFileName(file.FileName) ?? "file";
            var ext = (Path.GetExtension(original) ?? "").ToLowerInvariant();

            // tên gốc không ext
            var nameNoExt = Path.GetFileNameWithoutExtension(original);
            var safeName = ToSlugNoDiacritics(nameNoExt, separator);

            // folder tương đối
            var relFolder = string.IsNullOrWhiteSpace(subFolder)
                ? DateTime.Now.ToString("yyyy/MM/dd")
                : subFolder.Trim().TrimStart('/', '\\').Replace("\\", "/");

            // suffix unique
            var suffix = addUniqueSuffix ? "-" + Guid.NewGuid().ToString("N").Substring(0, 8) : "";

            var storedFileName = $"{safeName}{suffix}{ext}";
            var relativePath = $"{relFolder}/{storedFileName}"; // dùng "/" để consistent

            // physical path
            var physicalFolder = Path.Combine(rootPhysical, relFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
            Directory.CreateDirectory(physicalFolder);

            var physicalPath = Path.Combine(physicalFolder, storedFileName);

            // online url
            var baseOnline = baseOnlineUrl.TrimEnd('/');
            var onlineUrl = string.IsNullOrWhiteSpace(baseOnline) ? relativePath : $"{baseOnline}/{relativePath}";

            return new FileSaveInfo
            {
                OriginalFileName = original,
                FileNameWithoutExtension = Path.GetFileNameWithoutExtension(original),
                SafeName = safeName,
                Extension = ext,
                StoredFileName = storedFileName,
                RelativePath = relativePath,
                PhysicalPath = physicalPath,
                OnlineUrl = onlineUrl,
                MimeType = file.ContentType,
                SizeBytes = file.ContentLength
            };
        }
    }
}
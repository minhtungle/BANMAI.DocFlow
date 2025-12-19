using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Public.Helpers
{
    public static class XuLyTenTruongHoc
    {
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var formD = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            // xử lý riêng đ/Đ
            return sb.ToString()
                     .Normalize(NormalizationForm.FormC)
                     .Replace('đ', 'd')
                     .Replace('Đ', 'D');
        }
        public static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // 1) Bỏ dấu tiếng Việt
            var normalized = RemoveDiacritics(text);

            // 2) Lowercase
            normalized = normalized.ToLowerInvariant();

            // 3) Chuẩn hoá các ký tự nối thành dấu -
            // thay mọi thứ KHÔNG phải chữ/số bằng -
            normalized = Regex.Replace(normalized, @"[^a-z0-9]+", "-");

            // 4) Bỏ - dư ở đầu/cuối
            normalized = normalized.Trim('-');

            // 5) Gom nhiều - liên tiếp thành 1 -
            normalized = Regex.Replace(normalized, @"-+", "-");

            return normalized;
        }
        // Map suffix (tùy bạn đổi)
        // 0: Mầm non; 1: Tiểu học; 2: THCS; 3: THPT; 9: Liên cấp/khác
        public static string ToCode(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Bỏ dấu, chuẩn hoá khoảng trắng
            var normalized = RemoveDiacritics(text).Trim();
            normalized = Regex.Replace(normalized, @"\s+", " ");

            // 1) Xác định cấp học
            var level = DetectLevel(normalized); // 0/1/2/3/9/null

            // 2) Loại bỏ các từ chung + token cấp học để lấy phần tên riêng
            var words = Regex.Split(normalized, @"[\s\.\,\-\(\)\/\\]+")
                             .Where(w => !string.IsNullOrWhiteSpace(w))
                             .ToArray();

            var stop = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // từ chung
            "truong", "so", "tt", "thi", "xa", "huyen", "quan", "tinh", "tp", "thanhpho",
            // cấp học / dạng viết tắt hay gặp
            "mam", "non", "mau", "giao", "nha", "tre", "mn",
            "tieu", "hoc", "th", "primary",
            "trung", "co", "so", "thcs", "lower", "secondary",
            "pho", "thong", "thpt", "upper",
            "ptcs", "pt", "giao-duc", "gdt", "gdtx", "ttgdtx",
            "lien", "cap", "liencap",
            // cấp 1/2/3
            "cap", "1", "2", "3", "i", "ii", "iii"
        };

            // Lọc từ: giữ từ có chữ cái/số và không thuộc stop
            var filtered = words.Where(w =>
            {
                var token = w.Trim();
                if (token.Length == 0) return false;

                // bỏ các token dạng "&"
                if (token == "&") return false;

                return !stop.Contains(token);
            }).ToArray();

            // fallback: nếu lọc hết thì dùng toàn bộ words
            if (filtered.Length == 0) filtered = words;

            // 3) Tạo acronym (LMT)
            var sb = new StringBuilder();
            foreach (var w in filtered)
            {
                var ch = w.FirstOrDefault(char.IsLetterOrDigit);
                if (ch != default) sb.Append(char.ToUpperInvariant(ch));
            }

            // 4) Append suffix
            if (level.HasValue)
                sb.Append(level.Value);

            return sb.ToString();
        }

        /// <summary>
        /// Trả về: 0 (MN), 1 (TH), 2 (THCS), 3 (THPT), 9 (Liên cấp/khác), null (không rõ)
        /// </summary>
        private static int? DetectLevel(string normalizedNoDiacritics)
        {
            var s = normalizedNoDiacritics.ToLowerInvariant();

            // Chuẩn hoá các ký tự nối
            var compact = Regex.Replace(s, @"\s+", " ");
            // Nhận diện liên cấp trước (vì có thể chứa nhiều cấp)
            // VD: "tieu hoc & thcs", "th-thcs", "thcs-thpt", "lien cap"
            if (Regex.IsMatch(compact, @"\b(lien\s*cap|liencap)\b")) return 9;
            if (Regex.IsMatch(compact, @"\b(th\s*&\s*thcs|tieu\s*hoc\s*&\s*thcs|th[-\/]thcs|th&thcs)\b")) return 9;
            if (Regex.IsMatch(compact, @"\b(thcs\s*&\s*thpt|thcs[-\/]thpt|thcs&thpt)\b")) return 9;

            // Mầm non / mẫu giáo / nhà trẻ
            if (Regex.IsMatch(compact, @"\b(mam\s*non|mau\s*giao|nha\s*tre|mn)\b")) return 0;

            // Tiểu học / cấp 1 / primary
            if (Regex.IsMatch(compact, @"\b(tieu\s*hoc|tieu-hoc|cap\s*1|cap\s*i|primary|th\b)\b"))
            {
                // Lưu ý: "TH" có thể là "Tiểu học" hoặc chỉ viết tắt chung "Trường học".
                // Ở VN, "TH" thường là tiểu học, nên mình map = 1.
                return 1;
            }

            // THCS / trung học cơ sở / cấp 2 / lower secondary / PTCS
            if (Regex.IsMatch(compact, @"\b(thcs|trung\s*hoc\s*co\s*so|cap\s*2|cap\s*ii|lower\s*secondary|ptcs)\b"))
                return 2;

            // THPT / trung học phổ thông / cấp 3 / upper secondary / phổ thông
            if (Regex.IsMatch(compact, @"\b(thpt|trung\s*hoc\s*pho\s*thong|cap\s*3|cap\s*iii|upper\s*secondary)\b"))
                return 3;

            // “Trung học” chung chung (không nói CS/PT) -> bạn muốn LMT2
            if (Regex.IsMatch(compact, @"\b(trung\s*hoc)\b"))
                return 2;

            // GDTX / TTGDTX -> nếu bạn muốn phân loại riêng, đổi số khác
            if (Regex.IsMatch(compact, @"\b(gdtx|ttgdtx)\b"))
                return 9;

            return null;
        }
    }
}
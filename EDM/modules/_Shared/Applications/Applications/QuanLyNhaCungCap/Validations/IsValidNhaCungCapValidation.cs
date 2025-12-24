using Applications.QuanLyNhaCungCap.Enums;
using Applications.QuanLyNhaCungCap.Models;
using EDM_DB;
using Public.Dtos;
using Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyNhaCungCap.Validations
{
     public class IsValidNhaCungCapValidation
    {
        public Task<ValidationResultEx> ValidateFieldsOnlyAsync(
        tbNhaCungCap nhaCungCap,
        params FieldValidationOptionDto<NhaCungCapFieldEnum>[] options)
        {
            if (nhaCungCap == null) throw new ArgumentNullException(nameof(nhaCungCap));

            if (options == null || options.Length == 0)
                options = new[] {
                    new FieldValidationOptionDto<NhaCungCapFieldEnum> {
                        Field = NhaCungCapFieldEnum.MaNhaCungCap,
                        Rules = ValidateRule.Required,
                        DisplayName = "Mã nhà cung cấp"
                    }
            };

            var result = new ValidationResultEx();

            void AddError(NhaCungCapFieldEnum field, string msg)
            {
                result.InvalidFields.Add(new FieldInvalidDto<NhaCungCapFieldEnum> { Field = field, Message = msg });
            }

            string GetValue(NhaCungCapFieldEnum field)
            {
                switch (field)
                {
                    case NhaCungCapFieldEnum.MaNhaCungCap: return (nhaCungCap.MaNhaCungCap ?? "").Trim();
                    //case NhaCungCapFieldEnum.SoDienThoai: return (nhaCungCap.SoDienThoai ?? "").Trim();
                    //case NhaCungCapFieldEnum.Email: return (nhaCungCap.Email ?? "").Trim();
                    default: return "";
                }
            }

            foreach (var opt in options)
            {
                var display = string.IsNullOrWhiteSpace(opt.DisplayName) ? opt.Field.ToString() : opt.DisplayName;
                var value = GetValue(opt.Field);

                // Required
                if (opt.Rules.HasFlag(ValidateRule.Required) && string.IsNullOrWhiteSpace(value))
                {
                    AddError(opt.Field, $"{display} không được để trống.");
                    // nếu đã required fail, thường không cần check tiếp
                    continue;
                }

                // MinLength
                if (opt.Rules.HasFlag(ValidateRule.MinLength) && opt.MinLen.HasValue && value.Length < opt.MinLen.Value)
                    AddError(opt.Field, $"{display} tối thiểu {opt.MinLen.Value} ký tự.");

                // MaxLength
                if (opt.Rules.HasFlag(ValidateRule.MaxLength) && opt.MaxLen.HasValue && value.Length > opt.MaxLen.Value)
                    AddError(opt.Field, $"{display} tối đa {opt.MaxLen.Value} ký tự.");

                // Regex
                if (opt.Rules.HasFlag(ValidateRule.Regex) && !string.IsNullOrWhiteSpace(opt.Pattern))
                {
                    if (!Regex.IsMatch(value, opt.Pattern))
                        AddError(opt.Field, opt.PatternMessage ?? $"{display} không đúng định dạng.");
                }

                // Format (tùy field)
                if (opt.Rules.HasFlag(ValidateRule.Format))
                {
                    //if (opt.Field == NhaCungCapFieldEnum.Email && !string.IsNullOrWhiteSpace(value))
                    //{
                    //    try { _ = new MailAddress(value); }
                    //    catch { AddError(opt.Field, $"{display} không đúng định dạng email."); }
                    //}

                    //if (opt.Field == NhaCungCapFieldEnum.SoDienThoai && !string.IsNullOrWhiteSpace(value))
                    //{
                    //    // rule phone đơn giản: chỉ số + cho phép + ở đầu, 9-15 số
                    //    var phone = value.Replace(" ", "").Replace("-", "");
                    //    if (!Regex.IsMatch(phone, @"^\+?\d{9,15}$"))
                    //        AddError(opt.Field, $"{display} không đúng định dạng số điện thoại.");
                    //}
                }
            }

            return Task.FromResult(result);
        }
    }
}
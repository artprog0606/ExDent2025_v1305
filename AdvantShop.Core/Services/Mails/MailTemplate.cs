using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Letters;

namespace AdvantShop.Mails
{
    public abstract class MailTemplate
    {
        public abstract MailType Type { get; }

        public string Subject { get; set; }
        public string Body { get; set; }
        protected bool IsBuilt { get; set; }

        public MailTemplate BuildMail()
        {
            if (!IsBuilt)
            {
                BuildMail(Type.ToString());
                IsBuilt = true;
            }
            return this;
        }

        public void BuildMail(string mailType)
        {
            var mailFormat = MailFormatService.GetByType(mailType);
            if (mailFormat != null)
                BuildMail(mailFormat.FormatSubject, mailFormat.FormatText);
            else
            {
                BuildMail("", "");
                Debug.Log.Warn("Mail type '" + mailType + "' not found");
            }
        }

        public void BuildMail(string subject, string mailBody)
        {
            Subject = !string.IsNullOrEmpty(subject) ? FormatString(subject) : string.Empty;
            Body = !string.IsNullOrEmpty(mailBody) ? FormatString(mailBody) : string.Empty;

            if (Body.Contains("#LOGO#"))
            {
                var logo = SettingsMain.LogoImageName.IsNotEmpty()
                    ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                        SettingsMain.SiteUrl.Trim('/') + '/' +
                        FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.LogoImageName, false),
                        SettingsMain.ShopName)
                    : string.Empty;

                Body = Body.Replace("#LOGO#", logo);
            }

            if (Body.Contains("#MAIN_PHONE#"))
            {
                Body = Body.Replace("#MAIN_PHONE#", SettingsMain.Phone);
            }
        }

        public string FormatValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var texting = string.Copy(text);

            texting = FormatString(texting);

            if (texting.Contains("#LOGO#"))
            {
                var logo = SettingsMain.LogoImageName.IsNotEmpty()
                    ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                        SettingsMain.SiteUrl.Trim('/') + '/' +
                        FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.LogoImageName, false),
                        SettingsMain.ShopName)
                    : string.Empty;

                texting = texting.Replace("#LOGO#", logo);
            }

            if (texting.Contains("#MAIN_PHONE#"))
            {
                texting = texting.Replace("#MAIN_PHONE#", SettingsMain.Phone);
            }

            return texting;
        }

        public string FormatValue(string text, Coupon couponTemplate, string triggerCouponCode)
        {
            var value = FormatValue(text);

            var triggerOptions = new TriggerLetterBuilderOptions(couponTemplate, triggerCouponCode);
            if (!triggerOptions.IsEmpty)
                value = new TriggerLetterBuilder(triggerOptions).FormatText(value);

            return value;
        }
        
        public Dictionary<string, string> GetFormattedParams(string text, Coupon couponTemplate, string triggerCouponCode)
        {
            var dictionary = GetFormattedParams(text);
            
            var triggerOptions = new TriggerLetterBuilderOptions(couponTemplate, triggerCouponCode);
            if (!triggerOptions.IsEmpty)
            {
                var dict = new TriggerLetterBuilder(triggerOptions).GetFormattedParams(text);
                if (dict != null)
                {
                    foreach (var pair in dict)
                        if (!dictionary.ContainsKey(pair.Key))
                            dictionary.Add(pair.Key, pair.Value);
                }
            }

            return dictionary;
        }
        
        public virtual List<LetterFormatKey> GetKeyDescriptions() => new List<LetterFormatKey>();

        protected virtual string FormatString(string text)
        {
            return string.Empty;
        }
        
        protected virtual Dictionary<string, string> GetFormattedParams(string text)
        {
            return null;
        }
    }
}
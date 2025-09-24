using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Helpers;
using AdvantShop.Web.Admin.Models.Settings;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AdvantShop.Web.Admin.Handlers.Settings.Socials
{
    public class SaveSocialSettingsHandler
    {
        private SocialSettingsModel _model;
        public SaveSocialSettingsHandler(SocialSettingsModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            #region SocialCommon

            SettingsSocial.SocialShareCustomCode = _model.SocialShareCustomCode;
            SettingsSocial.SocialShareEnabled = _model.SocialShareEnabled;

            SettingsDesign.IsVkTemplateActive = _model.VkTemplateActive;
            SettingsDesign.IsFbTemplateActive = _model.FbTemplateActive;

            SettingsSocial.LinkVk = StringHelper.CompileUrl(_model.LinkVk);
            SettingsSocial.LinkFacebook = StringHelper.CompileUrl(_model.LinkFacebook);
            SettingsSocial.LinkInstagramm = StringHelper.CompileUrl(_model.LinkInstagramm);
            SettingsSocial.LinkOk = StringHelper.CompileUrl(_model.LinkOk);
            SettingsSocial.LinkTwitter = StringHelper.CompileUrl(_model.LinkTwitter);
            SettingsSocial.LinkTelegram = StringHelper.CompileUrl(_model.LinkTelegram);
            SettingsSocial.LinkYoutube = StringHelper.CompileUrl(_model.LinkYoutube);
            SettingsSocial.LinkRutube = StringHelper.CompileUrl(_model.LinkRutube);
            SettingsSocial.LinkYandexZen = StringHelper.CompileUrl(_model.LinkYandexZen);

            SettingsSocial.LinkVkActive = _model.LinkVkActive;
            SettingsSocial.LinkFacebookActive = _model.LinkFacebookActive;
            SettingsSocial.LinkInstagrammActive = _model.LinkInstagrammActive;
            SettingsSocial.LinkOkActive = _model.LinkOkActive;
            SettingsSocial.LinkTwitterActive = _model.LinkTwitterActive;
            SettingsSocial.LinkTelegramActive = _model.LinkTelegramActive;
            SettingsSocial.LinkYoutubeActive = _model.LinkYoutubeActive;
            SettingsSocial.LinkYandexZenActive = _model.LinkYandexZenActive;
            SettingsSocial.LinkRutubeActive = _model.LinkRutubeActive;

            #endregion SocialCommon

            #region Widgets

            SettingsSocial.WidgetVk = _model.WidgetVk;
            SettingsSocial.WidgetFacebook = _model.WidgetFacebook;
            SettingsSocial.WidgetInstagramm = _model.WidgetInstagramm;
            SettingsSocial.WidgetOk = _model.WidgetOk;
            SettingsSocial.WidgetTwitter = _model.WidgetTwitter;
            SettingsSocial.WidgetTelegram = _model.WidgetTelegram;
            SettingsSocial.WidgetYoutube = _model.WidgetYoutube;

            SettingsSocial.WidgetVkActive = _model.WidgetVkActive;
            SettingsSocial.WidgetFacebookActive = _model.WidgetFacebookActive;
            SettingsSocial.WidgetInstagrammActive = _model.WidgetInstagrammActive;
            SettingsSocial.WidgetOkActive = _model.WidgetOkActive;
            SettingsSocial.WidgetTwitterActive = _model.WidgetTwitterActive;
            SettingsSocial.WidgetTelegramActive = _model.WidgetTelegramActive;
            SettingsSocial.WidgetYoutubeActive = _model.WidgetYoutubeActive;

            if (_model.WidgetVkActive && string.IsNullOrEmpty(SettingsSocial.WidgetVk) && !string.IsNullOrEmpty(_model.LinkVk))
            {
                var groupId = GetWebSocialId(_model.LinkVk);
                if (!string.IsNullOrEmpty(groupId))
                {
                    SettingsSocial.WidgetVk =
                        "<script type=\"text/javascript\" src=\"https://vk.com/js/api/openapi.js?160\"></script><!--VK Widget--><div id=\"vk_groups\"></div><script type=\"text/javascript\">VK.Widgets.Group(\"vk_groups\", { mode: 3}, " + groupId + ");</script>";
                }
            }

            #endregion Widgets

            #region SocialWidget

            SettingsSocialWidget.IsActive = _model.SocialWidgetIsActive;

            SettingsSocialWidget.IsShowVkInDesktop = _model.SocialWidgetVkShowInDesktop;
            SettingsSocialWidget.IsShowVkInMobile = _model.SocialWidgetVkShowInMobile;

            SettingsSocialWidget.IsShowFbInDesktop = _model.SocialWidgetFbShowInDesktop;
            SettingsSocialWidget.IsShowFbInMobile = _model.SocialWidgetFbShowInMobile;

            SettingsSocialWidget.IsShowJivositeInDesktop = _model.SocialWidgetJivositeShowInDesktop;
            SettingsSocialWidget.IsShowJivositeInMobile = _model.SocialWidgetJivositeShowInMobile;

            SettingsSocialWidget.IsShowCallbackInDesktop = _model.SocialWidgetCallbackShowInDesktop;
            SettingsSocialWidget.IsShowCallbackInMobile = _model.SocialWidgetCallbackShowInMobile;

            SettingsSocialWidget.IsShowWhatsAppInMobile = _model.SocialWidgetWhatsAppShowInMobile;
            SettingsSocialWidget.IsShowWhatsAppInDesktop = _model.SocialWidgetWhatsAppShowInDesktop;
            SettingsSocialWidget.WhatsAppPhone = _model.SocialWidgetWhatsAppPhone;

            SettingsSocialWidget.IsShowViberInMobile = _model.SocialWidgetViberShowInMobile;
            SettingsSocialWidget.IsShowViberInDesktop = _model.SocialWidgetViberShowInDesktop;
            SettingsSocialWidget.ViberPhone = _model.SocialWidgetViberPhone;

            SettingsSocialWidget.IsShowOdnoklassnikiInDesktop = _model.SocialWidgetOdnoklassnikiShowInDesktop;
            SettingsSocialWidget.IsShowOdnoklassnikiInMobile = _model.SocialWidgetOdnoklassnikiShowInMobile;
            SettingsSocialWidget.LinkOdnoklassniki = StringHelper.CompileUrl(_model.SocialWidgetLinkOdnoklassniki);

            SettingsSocialWidget.IsShowTelegramInDesktop = _model.SocialWidgetTelegramShowInDesktop;
            SettingsSocialWidget.IsShowTelegramInMobile = _model.SocialWidgetTelegramShowInMobile;
            SettingsSocialWidget.LinkTelegram = StringHelper.CompileUrl(_model.SocialWidgetLinkTelegram);

            SettingsSocialWidget.CustomLink1 = _model.SocialWidgetCustomLink1;
            SettingsSocialWidget.CustomLinkText1 = _model.SocialWidgetCustomLinkText1;

            SettingsSocialWidget.CustomLink2 = _model.SocialWidgetCustomLink2;
            SettingsSocialWidget.CustomLinkText2 = _model.SocialWidgetCustomLinkText2;

            SettingsSocialWidget.CustomLink3 = _model.SocialWidgetCustomLink3;
            SettingsSocialWidget.CustomLinkText3 = _model.SocialWidgetCustomLinkText3;

            SettingsSocialWidget.ShowPopover = _model.SocialWidgetShowPopover;
            SettingsSocialWidget.PopoverText = _model.SocialWidgetPopoverText;
            
            #endregion SocialWidget
        }

        private string GetWebSocialId(string url)
        {
            try
            {
                var result = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    string data = readStream.ReadToEnd();

                    Regex regex = new Regex(@"/wall-\d+");
                    MatchCollection matches = regex.Matches(data);

                    if (matches.Count > 0)
                    {
                        result = matches[0].Value.Substring(matches[0].Value.LastIndexOf("-") + 1);
                    }

                    response.Close();
                    readStream.Close();
                }

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
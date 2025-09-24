using System;
using System.Collections.Generic;
using AdvantShop.Diagnostics;
using AdvantShop.App.Landing.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;
using System.Linq;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Blocks;
using AdvantShop.App.Landing.Domain.SubBlocks;
using AdvantShop.App.Landing.Models;
using AdvantShop.App.Landing.Domain.Settings;

namespace AdvantShop.App.Landing.Extensions
{
    public static class LpBlockExtensions
    {
        public static dynamic TryGetSetting(this LpBlock block, string key)
        {
            if (block.Settings == null)
                return null;

            if (block.MappedSettings == null)
            {
                //try
                //{
                block.MappedSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(block.Settings);
                //}
                //catch (Exception ex)
                //{
                //    Debug.Log.Error(ex);
                //}
            }

            return block.MappedSettings != null && block.MappedSettings.ContainsKey(key)
                ? block.MappedSettings[key]
                : null;
        }

        public static T TryGetSetting<T>(this LpBlock block, string key) where T : IConvertibleBlockModel
        {
            var value = block.TryGetSetting(key);
            return value == null ? default(T) : (T)value.ToObject(typeof(T));
        }


        public static T TryGetSetting<T, OldT>(this LpBlock block, string key) where T : IConvertibleBlockModel
                                                                               where OldT : IConvertibleBlockModel
        {
            var value = block.TryGetSetting(key);
            if (value == null) return default(T);

            var t = (T)value.ToObject(typeof(T));

            if (t != null)
                return t;

            var oldt = (T)((OldT)value.ToObject(typeof(OldT))).ConvertToType(typeof(OldT));
            return oldt;
        }


        public static T TryGetValuableSetting<T>(this LpBlock block, string key) where T : IConvertible
        {
            var value = block.TryGetSetting(key);
            return value == null ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }


        public static List<T> TryGetSettingAsList<T>(this LpBlock block, string key) where T : IConvertibleBlockModel
        {
            var value = block.TryGetSetting(key);
            if (value == null)
                return null;

            var arr = value as JArray;
            if (arr != null)
                return arr.Select(x => (T)x.ToObject(typeof(T))).ToList();

            return null;
        }

        public static List<T> TryGetValuebleSettingAsList<T>(this LpBlock block, string key) where T : IConvertible
        {
            var value = block.TryGetSetting(key);
            if (value == null)
                return null;

            var arr = value as JArray;
            if (arr != null)
                return arr.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();

            return null;
        }


        public static List<T> TryGetSettingAsList<T, OldT>(this LpBlock block, string key) where T : IConvertibleBlockModel
                                                                                           where OldT : IConvertibleBlockModel
        {
            var value = block.TryGetSetting(key);
            if (value == null)
                return null;

            List<T> res = null;

            var arr = value as JArray;
            if (arr != null) {
                res = new List<T>();
                foreach (var val in arr)
                {
                    var t = (T)val.ToObject(typeof(T));

                    if (!t.IsNull())
                        res.Add(t);
                    else
                    {
                        t = (T)((OldT)val.ToObject(typeof(OldT))).ConvertToType(typeof(T));
                        if (!t.IsNull()) 
                            res.Add(t);
                    }
                       
                }
            }

            return res;
        }


        public static List<T> TryGetSettingAsListValueble<T, OldT>(this LpBlock block, string key) where T : IConvertibleBlockModel, new()
                                                                                                    where OldT : IConvertible
        {
            var value = block.TryGetSetting(key);
            if (value == null)
                return null;

            List<T> res = null;

            var arr = value as JArray;
            if (arr != null)
            {
                res = new List<T>();
                foreach (var val in arr)
                {
                    T t = default(T);
                    try
                    {
                        t = (T)val.ToObject(typeof(T));

                    } catch (Exception)
                    {      
                    }

                    if (t != null && !t.IsNull())
                        res.Add(t);
                    else
                    {
                        t = (T)new T().ConvertFromType((OldT)Convert.ChangeType(val, typeof(OldT)), typeof(OldT));
                        res.Add(t);
                    }
                }
            }

            return res;
        }


        public static List<T> TryGetSettingAsList<T>(LpSubBlock subBlock, string key) where T : IConvertible
        {

            var value = subBlock.TryGetSetting(key);
            if (value == null)
                return null;

            var arr = value as JArray;
            if (arr != null)
                return arr.Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();

            return null;
        }

        public static List<T> TryGetSettingsAsList<T, OldT>(LpSubBlock subBlock, string key) where T : IConvertibleBlockModel
                                                                                                  where OldT : IConvertibleBlockModel
        {
            var value = subBlock.TryGetSetting(key);
            if (value == null)
                return null;

            List<T> res = null;

            var arr = value as JArray;
            if (arr != null)
            {
                res = new List<T>();
                foreach (var val in arr)
                {
                    var t = (T)value.ToObject(typeof(T));

                    if (t != null)
                        res.Add(t);
                    else
                        res.Add((T)((OldT)value.ToObject(typeof(OldT))).ConvertToType(typeof(OldT)));
                }
            }

            return res;
        }


        public static bool TrySetSetting(this LpBlock block, string key, object value)
        {
            if (block.Settings == null)
                block.Settings = "";

            //try
            //{
            if (block.MappedSettings == null)
            {
                block.MappedSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(block.Settings); //JObject.Parse(block.Settings);
            }

            if (block.MappedSettings.ContainsKey(key))
                block.MappedSettings[key] = value;
            else
                block.MappedSettings.Add(key, value);

            block.Settings = JsonConvert.SerializeObject(block.MappedSettings);

            return true;
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log.Error(ex);
            //}

            //return false;
        }

        public static HtmlString TryGetStyleString(this LpBlock block, string keyStyleObject = "style")
        {
            var styleObj = TryGetSetting(block, keyStyleObject);
            var result = new StringBuilder();

            if (styleObj != null)
            {
                var stylesArray = styleObj.ToObject<Dictionary<string, string>>();

                foreach (KeyValuePair<string, string> entry in stylesArray)
                {
                    if (!string.IsNullOrWhiteSpace(entry.Key) && !string.IsNullOrWhiteSpace(entry.Value))
                    {
                        result.AppendFormat("{0}:{1};", entry.Key, entry.Value);
                    }  
                }
            }

            return new HtmlString(result.ToString());
        }

        public static HtmlString TryGetStyleString(this LpBlock block, List<string> excludeStyles, string keyStyleObject = "style")
        {
            var styleObj = TryGetSetting(block, keyStyleObject);
            var result = new StringBuilder();

            if (styleObj != null)
            {
                Dictionary<string, string> stylesArray = styleObj.ToObject<Dictionary<string, string>>();

                Dictionary<string, string> stylesArrayFiltered;

                if (excludeStyles != null)
                {
                    stylesArrayFiltered = stylesArray.Where(x => excludeStyles.Contains(x.Key) == false).ToDictionary(k => k.Key, k => k.Value);
                }
                else
                {
                    stylesArrayFiltered = stylesArray;
                }

                foreach (KeyValuePair<string, string> entry in stylesArrayFiltered)
                {
                    result.AppendFormat("{0}:{1};", entry.Key, entry.Value);
                }
            }

            return new HtmlString(result.ToString());
        }

        public static HtmlString TryGetStyleFromStringStyles(this LpBlock block, string style, string keyStyleObject = "style")
        {
            var styleObj = TryGetSetting(block, keyStyleObject);
            var result = new StringBuilder();

            if (styleObj != null)
            {
                Dictionary<string, string> stylesArray = styleObj.ToObject<Dictionary<string, string>>();

                Dictionary<string, string> stylesArrayFiltered;

                stylesArrayFiltered = stylesArray.Where(x => style == x.Key).ToDictionary(k => k.Key, k => k.Value);

                foreach (KeyValuePair<string, string> entry in stylesArrayFiltered)
                {
                    result.AppendFormat("{0}:{1};", entry.Key, entry.Value);
                }
            }

            return new HtmlString(result.ToString());
        }


        public static HtmlString TryGetClassesString(this LpBlock block)
        {
            var styleObj = TryGetSetting(block, "classes");
            var result = string.Empty;

            if (styleObj != null)
                result = String.Join(" ", styleObj.ToObject<List<string>>());

            return new HtmlString(result);
        }

        public static HtmlString GetWidthInColumnsAsClasses(this LpBlock block)
        {
            var value = Convert.ToInt32(block.TryGetSetting("width_in_columns"));

            return new HtmlString(value != 0 ? String.Format("col-xs-12 col-md-{0}", value) : "col-xs-12");
        }

        public static dynamic TryGetSetting(this LpSubBlock subBlock, string key)
        {
            if (subBlock.Settings == null)
                return null;

            if (subBlock.MappedSettings == null)
            {
                //try
                //{
                subBlock.MappedSettings = JObject.Parse(subBlock.Settings);
                //}
                //catch (Exception ex)
                //{
                //    Debug.Log.Error(ex);
                //}
            };

            return subBlock.MappedSettings != null ? subBlock.MappedSettings[key] : null;
        }

        public static HtmlString TryGetStyleString(this LpSubBlock subBlock)
        {
            var styleObj = TryGetSetting(subBlock, "style");
            var result = new StringBuilder();

            if (styleObj != null)
            {
                var stylesArray = styleObj.ToObject<Dictionary<string, string>>();

                foreach (KeyValuePair<string, string> entry in stylesArray)
                {
                    result.AppendFormat("{0}:{1};", entry.Key, entry.Value);
                }
            }

            return new HtmlString(result.ToString());
        }

        public static HtmlString RenderInplaceNgStyle(this LpSubBlock subBlock, string name)
        {
            return new HtmlString(LpService.Inplace ? String.Format("data-ng-style=\"{0}.settings.style\"", name) : string.Empty);
        }

        public static bool ExistSocials(this LpBlock block)
        {
            if (block.TryGetSetting("vk_enabled") == true ||
                block.TryGetSetting("fb_enabled") == true ||
                block.TryGetSetting("youtube_enabled") == true ||
                block.TryGetSetting("twitter_enabled") == true ||
                block.TryGetSetting("instagram_enabled") == true ||
                block.TryGetSetting("telegram_enabled") == true ||
                block.TryGetSetting("odnoklassniki_enabled") == true ||
                block.TryGetSetting("rutube_enabled") == true ||
                block.TryGetSetting("yandexzen_enabled") == true)
                return true;

            return false;
        }

        public static bool NeedRenderMenu(this LpBlock block)
        {
            var menuItems = block.TryGetSettingAsList<LpMenuItem>("menu_items");

            return menuItems != null && menuItems.Any();
        }

        public static HtmlString GetQuickViewClass(this LpBlock block)
        {
            var scheme = TryGetSetting(block, "color_scheme");
            var blockId = block.Id;
            scheme = scheme == "color-scheme--custom" ? "block_" + blockId + "-" + scheme : scheme;
            return new HtmlString(scheme + " color-scheme__divider--border");
        }

        public static bool IsActiveModule(this LpBlock block)
        {
            var stringId = TryGetSetting(block, "string_id");
            if(stringId != null)
            {
                return Core.Modules.ModulesRepository.IsActiveModule(stringId);
            }
            return false;
        }

        public static bool IsInstallModule(this LpBlock block)
        {
            var stringId = TryGetSetting(block, "string_id");
            if (stringId != null)
            {
                return Core.Modules.ModulesRepository.IsInstallModule(stringId);
            }
            return false;
        }
    }
}

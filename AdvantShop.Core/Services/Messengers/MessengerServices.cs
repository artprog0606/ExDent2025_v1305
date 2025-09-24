using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;
using System.IO;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.Messengers
{
	public class MessengerServices
	{
        public static void SendMessage(Message message)
		{
			Task.Run(() => SendMessageNow(message));
		}

		public static  List<IMessengerService> GetActiveModules()
		{

            var list = new List<IMessengerService>();

            foreach (var moduleType in AttachedModules.GetModules<IMessengerService>())
            {
                var module = (IMessengerService)Activator.CreateInstance(moduleType, null);

                list.Add(module);
            }

            return list.Count > 0 ? list : null;
		}

		private static string SendMessageNow(Message message, bool? throwException = false)
		{
			string result = null;
            var throwError = throwException != null && throwException.Value;

            if (string.IsNullOrWhiteSpace(message.Text))
            {
                if (throwError)
                    throw new BlException("Укажите валидный текст");
                return result;
            }

            try
            {
                var modules = GetActiveModules();
                if (modules == null)
                {
                    if (throwError)
                        throw new BlException("Не подключен модуль");
                    return result;
                }

                foreach(var module in modules)
                    result = module.SendMessage(message);
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Error(error);

                                    if (throwError) throw;
                                }
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                if (throwError) throw;
            }
			return result;
		}
    }
}

using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Customers.Subscription
{
    public class ImportSubscriptionHandlers
    {
        private readonly HttpPostedFileBase _file;
        private readonly string _outputFilePath;

        public class Results
        {
            public bool Result { get; set; }
            public string Error { get; set; }
        } 

        public ImportSubscriptionHandlers(HttpPostedFileBase file, string outputFilePath)
        {
            _file = file;
            _outputFilePath = outputFilePath;

            FileHelpers.DeleteFile(outputFilePath);
        }

        public Results Execute()
        {
            if (_file == null || string.IsNullOrEmpty(_file.FileName))
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };
            _file.SaveAs(_outputFilePath);
            if (!File.Exists(_outputFilePath))
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Error.FileNotFound") };

            var result = Import(_outputFilePath);

            return result;
        }

        public Results Import(string filePath)
        {
            try
            {
                using (var csvReader = new CsvHelper.CsvReader(new StreamReader(filePath, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        if (csvReader.Parser.Count != 4)
                            return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Subscribe.Import.WrongFile") };

                        var email = csvReader[0];
                        if (!ValidationHelper.IsValidEmail(email))
                            continue;

                        var subscription = SubscriptionService.GetSubscription(email);
                        if (subscription != null)
                        {
                            subscription.Email = email;
                            subscription.Subscribe = csvReader[1] == "1";
                            SubscriptionService.UpdateSubscription(subscription);
                        }
                        else
                        {
                            SubscriptionService.AddSubscription(new AdvantShop.Customers.Subscription
                            {
                                Email = email,
                                Subscribe = csvReader[1] == "1"
                            });
                        }
                    }
                }
                return new Results { Result = true };
            }
            catch
            {
                return new Results { Result = false, Error = LocalizationService.GetResource("Admin.Subscribe.Import.ImportError") };
            }
        }
    }
}

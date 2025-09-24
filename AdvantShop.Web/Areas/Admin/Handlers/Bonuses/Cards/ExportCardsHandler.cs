using System.Globalization;
using System.IO;
using System.Text;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.ExportImport;
using AdvantShop.Web.Infrastructure.Handlers;
using CsvHelper.Configuration;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class ExportCardsHandler : AbstractCommandHandler<byte[]>
    {
        protected override byte[] Handle()
        {
            using (var ms = new MemoryStream())
            {
                using (var csvWriter = new CsvHelper.CsvWriter(new StreamWriter(ms, Encoding.UTF8), CsvConstants.DefaultCsvConfiguration))
                {
                    var headers = new[] {
                        "Mobile",
                        "Email",
                        "Card",
                        "LastName",
                        "FirstName",
                        "SecondName",
                        "DateOfBirth",
                        "Bonus",
                        "Grade",
                        "Basis"
                    };

                    foreach (var item in headers)
                    {
                        csvWriter.WriteField(item);
                    }
                    csvWriter.NextRecord();

                    foreach (var card in CardService.GetExportCards())
                    {
                        csvWriter.WriteField(card.Phone ?? "");
                        csvWriter.WriteField(card.Email ?? "");
                        csvWriter.WriteField(card.CardNumber);
                        csvWriter.WriteField(card.LastName ?? "");
                        csvWriter.WriteField(card.FirstName ?? "");
                        csvWriter.WriteField(card.Patronymic ?? "");
                        csvWriter.WriteField(card.BirthDay.ToString());
                        csvWriter.WriteField(card.BonusAmount.ToString());
                        csvWriter.WriteField(card.GradeName);
                        csvWriter.WriteField(string.Empty);

                        csvWriter.NextRecord();
                    }
                }
                return ms.ToArray();
            }
        }
    }
}

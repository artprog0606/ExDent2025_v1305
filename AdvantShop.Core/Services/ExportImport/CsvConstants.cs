using System.Globalization;
using System.Text;
using CsvHelper.Configuration;

namespace AdvantShop.ExportImport
{
    public static class CsvConstants
    {
        public static CsvConfiguration DefaultCsvConfiguration => new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            Encoding = Encoding.UTF8,
            Delimiter = ";"
        };
    }
}

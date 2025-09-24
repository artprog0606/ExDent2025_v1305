using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using AdvantShop.Web.Infrastructure.Handlers;
using CsvHelper;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public abstract class BaseGetFieldsFromCsvFileHandler<TResult> : AbstractCommandHandler<TResult>
    {
        protected readonly string OutputFilePath;
        protected readonly bool HaveHeader;
        protected readonly string ColumnSeparator;
        protected readonly string Encoding;

        protected List<string[]> CsvRows;

        protected Dictionary<string, string> AllFields;
        protected List<string> Headers;
        protected List<string> FirstItem;
        protected List<string> SelectedFields;

        /// <summary>
        /// file already validated, can load CsvRows
        /// </summary>
        protected virtual void LoadData() { }
        /// <summary>
        /// Csv file data already validated
        /// </summary>
        protected virtual void ValidateData() { }
        /// <summary>
        /// CsvRows, Headers and FirstItem already validated and handled
        /// </summary>
        protected virtual TResult HandleData()
        {
            throw new System.NotImplementedException();
        }

        public BaseGetFieldsFromCsvFileHandler(BaseImportModel model, string outputFilePath)
        {
            OutputFilePath = outputFilePath;
            HaveHeader = model.HaveHeader;
            ColumnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            Encoding = model.Encoding;

            AllFields = new Dictionary<string, string>();
            Headers = new List<string>();
            FirstItem = new List<string>();
        }

        /// <summary>
        /// Validate file and settings before calling overridable LoadData method
        /// </summary>
        protected sealed override void Load()
        {
            if (!File.Exists(OutputFilePath))
                throw new BlException(T("Admin.Import.Errors.FileNotFound"));
            if (ColumnSeparator.IsNullOrEmpty())
                throw new BlException(T("Admin.Import.Errors.NotColumnSeparator"));

            try
            {
                LoadData();
            }
            catch (BadDataException)
            {
                throw new BlException(T("Admin.Import.Errors.BadDataFound"));
            }
        }

        /// <summary>
        /// Validate csv data and call overridable ValidateData method
        /// </summary>
        protected sealed override void Validate()
        {
            if (CsvRows == null || CsvRows.Count == 0)
                throw new BlException(T("Admin.Import.Errors.WrongFile"));
            if (HaveHeader && CsvRows.Count == 1)
                throw new BlException(T("Admin.Import.Errors.NoHeaders"));
            if (HaveHeader && CsvRows[0].HasDuplicates())
                throw new BlException(T("Admin.Import.Errors.DuplicateHeaders"));

            ValidateData();
        }

        /// <summary>
        /// Get Headers and FirstItem and call overridable method HandleData 
        /// or override this method in child classes
        /// </summary>
        protected override TResult Handle()
        {
            // get headers
            for (int i = 0; i < CsvRows[0].Length; i++)
                Headers.Add(HaveHeader ? CsvRows[0][i].Reduce(50).Trim().ToLower() : T("Admin.ImportProducts.Empty"));

            // get first item
            var dataRow = HaveHeader && CsvRows.Count > 1 ? CsvRows[1] : CsvRows[0];
            if (dataRow != null)
            {
                foreach (var data in dataRow)
                    FirstItem.Add(data.DefaultOrEmpty().Reduce(50).HtmlEncode());
            }

            return HandleData();
        }
    }
}

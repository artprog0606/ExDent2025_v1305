using System.Collections.Generic;
using System.IO;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.ExportImport;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.ViewModels.Catalog.Import;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Import
{
    public abstract class BaseStartImportHandler : AbstractCommandHandler
    {
        protected readonly string InputFilePath;
        protected readonly string ColumnSeparator;
        protected readonly string Encoding;
        protected readonly bool HaveHeader;

        protected readonly Dictionary<string, int> FieldMapping;

        public BaseStartImportHandler(BaseImportModel model, string inputFilePath)
        {
            InputFilePath = inputFilePath;

            ColumnSeparator = model.ColumnSeparator.IsNotEmpty() && model.ColumnSeparator.ToLower() == SeparatorsEnum.Custom.ToString().ToLower()
                ? model.CustomColumnSeparator
                : model.ColumnSeparator;
            Encoding = model.Encoding;
            HaveHeader = model.HaveHeader;

            FieldMapping = new Dictionary<string, int>();
            for (int i = 0; i < model.SelectedFields.Count; i++)
            {
                if (!FieldMapping.ContainsKey(model.SelectedFields[i]))
                    FieldMapping.Add(model.SelectedFields[i], i);
            }
        }

        protected sealed override void Validate()
        {
            if (CommonStatistic.IsRun)
                throw new BlException(T("Admin.CommonStatistic.AlreadyRunning", CommonStatistic.CurrentProcess, CommonStatistic.CurrentProcessName.Default(CommonStatistic.CurrentProcess)));
            if (!File.Exists(InputFilePath))
                throw new BlException(T("Admin.Import.Errors.FileNotFound"));
            if (ColumnSeparator.IsNullOrEmpty())
                throw new BlException(T("Admin.Import.Errors.NotColumnSeparator"));

            ValidateData();
        }

        protected virtual void ValidateData() { }
    }
}

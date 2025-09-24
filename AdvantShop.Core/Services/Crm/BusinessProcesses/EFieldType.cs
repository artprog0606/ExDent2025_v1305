using System;

namespace AdvantShop.Core.Services.Crm.BusinessProcesses
{
    public enum EFieldType
    {
        None = 0,
        Text = 1,
        Number = 2,
        Select = 3,
        Checkbox = 4,
        Date = 5,
        TextArea = 6,
        ProductChooser = 7,
        CategoryChooser = 8,
        Datetime = 9,
        Tel = 10,
        Time = 11,
        Picture = 12,
        FileArchive = 13,
        Email = 14
    }

    [Flags]
    public enum EFieldUsage
    {
        None = 0,
        Filter = 1,
        Edit = 2,
    }

}

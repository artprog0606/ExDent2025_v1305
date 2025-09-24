
-- SQL_4.1.5_Part1

Insert Into [Settings].[Settings] (Name, Value) VALUES ('ShowColorFilter', 'True')
Insert Into [Settings].[Settings] (Name, Value) VALUES ('ShowSizeFilter', 'True')

GO--

if not exists(select * from sys.columns 
            where Name = N'LetterComment' and Object_ID = Object_ID(N'[Order].[OrderByRequest]'))
begin
    alter table [Order].[OrderByRequest] add LetterComment nvarchar(max) null
end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.5' WHERE [settingKey] = 'db_version'



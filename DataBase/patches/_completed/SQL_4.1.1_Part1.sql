
-- SQL_4.1.1_Part1


Alter Table [Order].[ShippingMethod]
Add [ShowInDetails] bit Null

GO--

Update [Order].[ShippingMethod] Set [ShowInDetails] = 1

GO--

Alter Table [Order].[ShippingMethod]
Alter Column [ShowInDetails] bit NOT NULL

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.1' WHERE [settingKey] = 'db_version'

ALTER TABLE [Order].[OrderPickPoint] ALTER COLUMN
	[PickPointId] nvarchar(255) NOT NULL

GO--

UPDATE [Catalog].[Category] SET Enabled = 1 WHERE CategoryID = 0;

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Warehouses.Title', 'Магазины'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Warehouses.Title', 'Shops'

GO--
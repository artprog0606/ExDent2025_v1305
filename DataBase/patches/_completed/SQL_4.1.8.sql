
-- SQL_4.1.8_Part1


ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID] @PropertyID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValueID]
		,[Property].[PropertyID]
		,[Value]
		,[PropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,Property.NAME AS PropertyName
		,Property.SortOrder AS PropertySortOrder
		,Property.Expanded
		,Property.Type
		,GroupId
		,GroupName
		,GroupSortorder
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	LEFT JOIN [Catalog].PropertyGroup ON PropertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [Property].[PropertyID] = @PropertyID
	order by [PropertyValue].[SortOrder]
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID] @ProductID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		 [PropertyValue].[PropertyValueID]
		,[PropertyValue].[PropertyID]
		,[PropertyValue].[Value]
		,[ProductPropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].[Name] as PropertyName
		,[Property].[SortOrder] as PropertySortOrder
		,[Property].[Expanded] as Expanded
		,[Property].[Type] as [Type]
		,[Property].GroupId as GroupId
		,GroupName
		,GroupSortorder
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[ProductPropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]
	inner join [Catalog].[Property] on [Property].[PropertyID] = [PropertyValue].[PropertyID]
	left join Catalog.PropertyGroup on propertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [ProductID] = @ProductID
	ORDER BY case when PropertyGroup.GroupSortOrder is null then 1 else 0 end, PropertyGroup.GroupSortOrder, [Property].[SortOrder], [PropertyValue].[SortOrder]
END

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]
	@OrderItemID int,
	@OrderID int,
	@Name nvarchar(100),
	@Price float,
	@Amount float,
	@ProductID int,
	@ArtNo nvarchar(50),
	@SupplyPrice float,
	@Weight float,
	@IsCouponApplied bit,
	@Color nvarchar(50),
	@Size nvarchar(50),
	@DecrementedAmount float,
	@PhotoID int
		
AS
BEGIN
	Update [Order].[OrderItems]
           set
		    [Name] = @Name
           ,[Price] = @Price
           ,[Amount] = @Amount
           ,[ArtNo] = @ArtNo
           ,[SupplyPrice] = @SupplyPrice
           ,[Weight] = @Weight
           ,[IsCouponApplied] = @IsCouponApplied
           ,[Color] = Color
		   ,[Size] = Size
		   ,[DecrementedAmount] = DecrementedAmount
           ,[PhotoID] = @PhotoID
	 WHERE OrderItemID = @OrderItemID
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
	@ProductId int,
	@CategoryId int,
	@SortOrder int
AS
BEGIN

DECLARE @Main bit
	SET NOCOUNT ON;
if (select count(*) from [Catalog].[ProductCategories] where ProductID=@ProductID and main=1) = 0
	set @Main = 1
else
	set @Main = 0
if (select count(*) from [Catalog].[ProductCategories] where CategoryID=@CategoryID and ProductID=@ProductID) = 0 
begin
	INSERT INTO [Catalog].[ProductCategories] (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryID, @ProductID, @SortOrder, @Main);
end
END

GO--



UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.8' WHERE [settingKey] = 'db_version'





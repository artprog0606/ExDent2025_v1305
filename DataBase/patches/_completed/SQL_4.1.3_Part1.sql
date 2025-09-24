
-- SQL_4.1.3_Part1

update [Catalog].[Property] set [Type]=1 where type is null

GO--

alter table [Catalog].[Property]
alter column [Type] int not null

GO--

ALTER PROCEDURE [Catalog].[sp_ParseProductProperty]
		@nameProperty nvarchar(100),
		@propertyValue nvarchar(255),
		@productId int,
		@sort int
AS
BEGIN
	-- select or create property
	Declare @propertyId int
	if ((select count(PropertyID) from Catalog.[Property] where Name = @nameProperty)= 0)
		begin
			insert into Catalog.[Property] (Name,UseInFilter,Useindetails,SortOrder,[type]) values (@nameProperty,1,1,0,1)
			set @propertyId = (Select SCOPE_IDENTITY())
		end
	else
		set @propertyId = (select top(1) PropertyID from Catalog.[Property] where Name = @nameProperty)

		-- select or create value
	 Declare @propertyValueId int

	 Declare @useinfilter bit
	 set @useinfilter = (Select Top 1 UseInFilter from Catalog.[Property] Where PropertyID=@propertyId)
	 Declare @useindetails bit
	 set @useindetails = (Select Top 1 UseInDetails from Catalog.[Property] Where PropertyID=@propertyId)

	 if ((select count(PropertyValueID) from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)= 0)
	  begin
	   insert into Catalog.[PropertyValue] 
		  (PropertyId, Value, UseInFilter, UseInDetails, SortOrder) 
		values (@propertyId, @propertyValue, @useinfilter, @useindetails, 0)
	   set @propertyValueId = (Select SCOPE_IDENTITY())
	  end
	 else
	  set @propertyValueId = (select top(1) PropertyValueID from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)
	
	--create link between product and property value
	if ((select Count(*) from Catalog.ProductPropertyValue where ProductID=@productId and PropertyValueID=@propertyValueId)=0)
		insert into Catalog.ProductPropertyValue (ProductID,PropertyValueID,SortOrder) values (@productId,@propertyValueId,@sort)	
END

GO--

delete from cms.staticblock where [key]='CompareProductsTop'
delete from cms.staticblock where [key]='LeftBlockText'
delete from cms.staticblock where [key]='mainPageBlock'
delete from cms.staticblock where [key]='socialShareDetails'
delete from cms.staticblock where [key]='socialShareNews'
delete from cms.staticblock where [key]='widgets'
delete from cms.staticblock where [key]='Плагин социальной сети'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.3' WHERE [settingKey] = 'db_version'



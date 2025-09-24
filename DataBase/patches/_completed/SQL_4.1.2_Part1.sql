
-- SQL_4.1.2_Part1


ALTER PROCEDURE [Customers].[sp_AddCustomerContact]  
    @CustomerID uniqueidentifier,  
    @Name nvarchar(150),  
    @Country nvarchar(70),  
    @City nvarchar(70),  
    @Zone nvarchar(70),  
    @Address nvarchar(255),  
    @Zip nvarchar(25),  
    @CountryID int = NULL,  
    @RegionID int = NULL  
AS  
BEGIN  
 
 INSERT INTO [Customers].[Contact]  
           ([CustomerID]  
           ,[Name]  
           ,[Country]  
           ,[City]  
           ,[Zone]  
           ,[Address]  
           ,[Zip]  
           ,[CountryID]  
           ,[RegionID]) 
     OUTPUT Inserted.ContactID
     VALUES  
           (@CustomerID  
           ,@Name  
           ,@Country  
           ,@City  
           ,@Zone  
           ,@Address  
           ,@Zip  
           ,@CountryID  
           ,@RegionID)
END

GO--

Insert Into [Settings].[Settings] (Name, Value) VALUES ('BonusSystem.MaxOrderPercent','100')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.2' WHERE [settingKey] = 'db_version'



ALTER PROCEDURE [Catalog].[sp_AddPhoto] 
	@ObjId INT, @Description NVARCHAR(255),  
	@OriginName NVARCHAR(255),  
	@Type NVARCHAR(50),  
	@Extension NVARCHAR(10),  
	@ColorID int,  
	@PhotoSortOrder int,
	@PhotoName NVARCHAR(255) = 'none',
	@PhotoNameSize1 NVARCHAR(255),	
	@PhotoNameSize2 NVARCHAR(255)
AS  
BEGIN  
	DECLARE @PhotoId int  
	DECLARE @ismain bit  
	SET @ismain = 1  
	
	IF EXISTS(SELECT * FROM [Catalog].[Photo] WHERE ObjId = @ObjId and [Type]=@Type AND main = 1)  
		SET @ismain = 0  

	if @PhotoName is null 
	begin
	set @PhotoName = 'none'
	end

	INSERT INTO [Catalog].[Photo] ([ObjId],[PhotoName],[Description],[ModifiedDate],[PhotoSortOrder],[Main],[OriginName],[Type],[ColorID], PhotoNameSize1, PhotoNameSize2)  
		VALUES (@ObjId, @PhotoName,@Description,Getdate(),@PhotoSortOrder,@ismain,@OriginName,@Type,@ColorID, @PhotoNameSize1, @PhotoNameSize2)  

	SET @PhotoId = Scope_identity()  
	if @PhotoName = 'none'
	begin
		
		DECLARE @newphoto NVARCHAR(255)  
		Set @newphoto=Convert(NVARCHAR(255),@PhotoId)+@Extension  
	
		UPDATE [Catalog].[Photo] SET [PhotoName] = @newphoto WHERE [PhotoId] = @PhotoId
	end

	SELECT * FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId
	--select @newphoto  
END  

GO--

If not Exists(Select 1 From [Settings].[Settings] Where [Name] = 'IsLimitedPhotoNameLength')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('IsLimitedPhotoNameLength', 'True')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.0.6' WHERE [settingKey] = 'db_version'
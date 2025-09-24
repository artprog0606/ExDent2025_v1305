IF EXISTS(SELECT *
          FROM [Settings].[Settings]
          WHERE [Name] = 'Features.EnableExperimentalFeatures')
    BEGIN
        UPDATE Settings.Settings SET [Value] = 'True' WHERE [Name] = 'Features.EnableExperimentalFeatures'
    END
ELSE
    BEGIN
        INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('Features.EnableExperimentalFeatures', 'True')
    END

GO--

IF EXISTS(SELECT *
          FROM [Settings].[Settings]
          WHERE [Name] = 'Features.EnableMobileAdmin')
    BEGIN
        UPDATE Settings.Settings SET [Value] = 'True' WHERE [Name] = 'Features.EnableMobileAdmin'
    END
ELSE
    BEGIN
        INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('Features.EnableMobileAdmin', 'True')
    END

GO--
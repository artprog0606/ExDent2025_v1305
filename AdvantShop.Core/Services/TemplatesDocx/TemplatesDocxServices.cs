//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Attributes.TemplateDocx;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using TemplateEngine.Docx;

namespace AdvantShop.Core.Services.TemplatesDocx
{
    public class TemplatesDocxServices
    {

        #region FillContent

        public static void TemplateFillContent<T>(string templateFilePath, T obj, bool isNeedToRemoveContentControls = true, bool isNeedToNoticeAboutErrors = false)
            where T : class
        {
            var templateItems = TypeToTemplateItems<T>(obj);
            var valuesToFill = new Content(FillContent(templateItems));

            using (var outputDocument = new TemplateProcessor(templateFilePath))
            {
                outputDocument.SetRemoveContentControls(isNeedToRemoveContentControls);
                outputDocument.SetNoticeAboutErrors(isNeedToNoticeAboutErrors);
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }

        }

        private static IContentItem[] FillContent(IEnumerable<TemplateDocxItem> templateItems)
        {
            if (templateItems != null)
            {
                var items = new List<IContentItem>();

                foreach (var templateItem in templateItems)
                {
                    IContentItem contentItem = null;

                    switch (templateItem.Type)
                    {
                        case TypeItem.Field:
                        case TypeItem.InheritedFields:
                            contentItem = new FieldContent(templateItem.Key,
                                templateItem.Value);
                            break;

                        case TypeItem.Image:
                            if (templateItem.Value != null)
                                contentItem = new ImageContent(templateItem.Key, templateItem.Value.ToString());
                            break;

                        case TypeItem.Table:
                            contentItem = new TableContent(templateItem.Key, new List<TableRowContent>());

                            if (templateItem.ChildItems != null &&
                                templateItem.ChildItems.Count > 0)
                            {
                                foreach (var childItem in templateItem.ChildItems)
                                    if (childItem.Any(x => x.Value != null))
                                    {
                                        var content = FillContent(childItem);
                                        if (content != null)
                                            ((TableContent)contentItem).AddRow(content);
                                    }
                            }

                            break;

                        case TypeItem.List:
                            contentItem = new ListContent(templateItem.Key, new List<ListItemContent>());

                            if (templateItem.ChildItems != null &&
                                templateItem.ChildItems.Count > 0)
                            {
                                foreach (var childItem in templateItem.ChildItems)
                                    if (childItem.Any(x => x.Value != null))
                                    {
                                        var content = FillContent(childItem);
                                        if (content != null)
                                            ((ListContent)contentItem).AddItem(content);
                                    }
                            }
                            break;

                        case TypeItem.Repeat:
                            contentItem = new RepeatContent(templateItem.Key, new List<Content>());

                            if (templateItem.ChildItems != null &&
                                templateItem.ChildItems.Count > 0)
                            {
                                foreach (var childItem in templateItem.ChildItems)
                                    if (childItem.Any(x => x.Value != null))
                                    {
                                        var content = FillContent(childItem);
                                        if (content != null)
                                            ((RepeatContent)contentItem).AddItem(content);
                                    }
                            }
                            break;
                    }

                    if (contentItem != null)
                        items.Add(contentItem);
                }

                return items.ToArray();
            }

            return null;
        }

        #region TypeToTemplateItems

        private static readonly Type TemplateDocxPropertyAttributeType = typeof(TemplateDocxPropertyAttribute);
        private static readonly Type IImplementsDynamicPropertyType = typeof(IImplementsDynamicProperty);
        private static readonly Type LocalizeAttributeType = typeof(LocalizeAttribute);
        private static readonly Type IEnumerableType = typeof(IEnumerable);
        private static readonly MethodInfo TypeToTemplateItemsMethod = typeof(TemplatesDocxServices).GetMethod("TypeToTemplateItems");
        private const string ArrayItemKey = "Item";

        public static List<TemplateDocxItem> TypeToTemplateItems<T>(T objType = null, TypeItem? typeParentItem = null, string keyParentItem = null, bool loadDescription = true)
            where T : class
        {
            var items = new List<TemplateDocxItem>();
            var type = typeof(T);

            if (type.GetInterfaces().Contains(IImplementsDynamicPropertyType))
                type = objType != null
                    ? objType.GetType()
                    : ((IImplementsDynamicProperty) Activator.CreateInstance(type)).GetTypeWithDynamicProperties();

            var properties = type.GetProperties().Where(x => x.IsDefined(TemplateDocxPropertyAttributeType, true));

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(TemplateDocxPropertyAttributeType, true);

                if (attributes.Length > 0)
                {
                    foreach (TemplateDocxPropertyAttribute attribute in attributes)
                    {
                        var item = new TemplateDocxItem(
                            string.IsNullOrEmpty(keyParentItem)
                                ? attribute.Key
                                : string.Format("{0}.{1}", keyParentItem, attribute.Key))
                        {
                            Type = attribute.Type,
                            Hidden = attribute.Hide,
                            TypeValue = property.PropertyType
                        };

                        /*if (items.Any(x => x.Key == item.Key))
                            throw new ArgumentException("Item with Same Key has already been added.");*/

                        if (loadDescription && !string.IsNullOrEmpty(attribute.LocalizeDescription))
                            item.Description = LocalizationService.GetResource(attribute.LocalizeDescription);

                        var propertyType = property.PropertyType;

                        if (propertyType.IsEnum)
                        {
                            if (item.Type != TypeItem.Field)
                                throw new ArgumentException(
                                    string.Format("Для свойства \"{0}\" объекта \"{1}\" указан атрибут с недопустимым TypeItem.",
                                    property.Name,
                                    type.FullName));

                            if (objType != null)
                            {
                                item.Value = GetEnumValue(objType, property);
                            }
                        }
                        else if (propertyType.IsValueType || propertyType == typeof(string))
                        {
                            if (item.Type != TypeItem.Field &&
                                item.Type != TypeItem.Image)
                                throw new ArgumentException(
                                    string.Format("Для свойства \"{0}\" объекта \"{1}\" указан атрибут с недопустимым TypeItem.",
                                    property.Name,
                                    type.FullName));

                            if (objType != null)
                            {
                                item.Value = GetValue(objType, property);

                                if (item.Type == TypeItem.Image)
                                {
                                    item.Value =
                                        item.Value != null && File.Exists(item.Value.ToString())
                                            ? item.Value
                                            : null;
                                }
                            }
                        }
                        else if (propertyType.GetInterfaces().Contains(IEnumerableType))
                        {
                            if (item.Type != TypeItem.List &&
                                item.Type != TypeItem.Table &&
                                item.Type != TypeItem.Repeat)
                                throw new ArgumentException(
                                    string.Format("Для свойства \"{0}\" объекта \"{1}\" указан атрибут с недопустимым TypeItem.",
                                    property.Name,
                                    type.FullName));

                            item.ChildItems = GetItemsByIEnumerable(objType, property, item.Type, item.Key, loadDescription);
                        }
                        else if (propertyType.IsClass)
                        {
                            if (typeParentItem == null &&
                                item.Type != TypeItem.InheritedFields)
                                throw new ArgumentException(
                                    string.Format("Для свойства \"{0}\" объекта \"{1}\" указан атрибут с недопустимым TypeItem.",
                                    property.Name,
                                    type.FullName));

                            var classItems = GetItemsByClass(objType, property, typeParentItem, item.Key, loadDescription);

                            if (item.Type == TypeItem.InheritedFields)
                            {
                                /*if (classItems.Any(classItem => items.Any(x => x.Key == classItem.Key)))
                                    throw new ArgumentException("Item with Same Key has already been added.");*/

                                if (item.Hidden)
                                    classItems.ForEach(x => x.Hidden = item.Hidden);

                                items.AddRange(classItems);
                            }
                            else
                                item.ChildItems = new List<TemplateDocxItem[]>() { classItems.ToArray() };
                        }
                        else
                        {
                            throw new NotImplementedException(string.Format("Не реализована поддержка типа \"{0}\".", propertyType.FullName));
                        }

                        if (item.Type != TypeItem.InheritedFields)
                            items.Add(item);
                    }
                }
            }

            return items;
        }

        private static TemplateDocxItem[] GetItemsByClass(object objType, PropertyInfo property, TypeItem? typeParentItem, string keyParentItem, bool loadDescription)
        {
            MethodInfo genericMethod = TypeToTemplateItemsMethod.MakeGenericMethod(property.PropertyType);

            return ((List<TemplateDocxItem>)genericMethod.Invoke(null, new object[] { objType != null ? property.GetValue(objType) : null, typeParentItem, keyParentItem, loadDescription })).ToArray();
        }

        private static List<TemplateDocxItem[]> GetItemsByIEnumerable(object objType, PropertyInfo property, TypeItem typeItem, string keyItem, bool loadDescription)
        {
            Type typeEnumerableItem = null;

            if (property.PropertyType.IsConstructedGenericType &&
                property.PropertyType.GenericTypeArguments.Length == 1)
            {
                typeEnumerableItem = property.PropertyType.GenericTypeArguments[0];
            }
            else if (property.PropertyType.IsArray)
            {
                typeEnumerableItem = property.PropertyType.GetElementType();
            }

            if (typeEnumerableItem != null)
            {
                var values = objType == null ? null : (IEnumerable)property.GetValue(objType);

                if (typeEnumerableItem.IsEnum || typeEnumerableItem.IsValueType || typeEnumerableItem == typeof(string))
                {
                    if (objType == null || values == null)
                        return new List<TemplateDocxItem[]>()
                        {
                            new[] {new TemplateDocxItem(ArrayItemKey) {Description = "Element", Type = TypeItem.Field}}
                        };
                    else
                    {
                        List<TemplateDocxItem[]> list = new List<TemplateDocxItem[]>();
                        foreach (var enumerableItem in values)
                            list.Add(new[]
                            {
                                new TemplateDocxItem(ArrayItemKey)
                                {
                                    Type = TypeItem.Field, Description = "Element", Value = typeEnumerableItem.IsEnum ? GetEnumValue(enumerableItem) : GetValue(enumerableItem, typeEnumerableItem)
                                }
                            });
                        return list;
                    }
                }
                else
                {
                    MethodInfo genericMethod = TypeToTemplateItemsMethod.MakeGenericMethod(typeEnumerableItem);
                    if (objType == null || values == null)
                        return new List<TemplateDocxItem[]>()
                        {
                            ((List<TemplateDocxItem>) genericMethod.Invoke(null, new object[] {null, typeItem, keyItem, loadDescription})).ToArray()
                        };
                    else
                    {
                        var valueItems = new List<TemplateDocxItem[]>();
                        foreach (var enumerableItem in values)
                            valueItems.Add(
                                ((List<TemplateDocxItem>)genericMethod.Invoke(null, new object[] { enumerableItem, typeItem, keyItem, loadDescription })).ToArray());
                        return valueItems;
                    }
                }
            }

            throw new NotImplementedException(string.Format("Не реализована поддержка типа \"{0}\".", property.PropertyType.FullName));
        }

        private static object GetEnumValue(object objType, PropertyInfo property)
        {
            return GetEnumValue(property.GetValue(objType));
        }

        private static object GetEnumValue(object value)
        {
            if (value != null)
            {
                var field = value.GetType().GetField(value.ToString());

                if (field != null)
                {
                    var localizeAttributes = field.GetCustomAttributes(LocalizeAttributeType, false);
                    if (localizeAttributes.Length > 0)
                    {
                        return ((IAttribute<string>)localizeAttributes[0]).Value;
                    }
                }

                return value;
            }

            return null;
        }

        private static object GetValue(object objType, PropertyInfo property)
        {
            return GetValue(property.GetValue(objType), property.PropertyType);
        }

        private static object GetValue(object value, Type typeValue = null)
        {
            typeValue = typeValue ?? value.GetType();
            if (value != null)
            {
                if (typeValue == typeof(bool))
                {
                    return (bool)value ? "Да" : "Нет";
                }
                else if (typeValue.IsConstructedGenericType &&
                         typeValue.GenericTypeArguments[0].IsEnum)
                {
                    return GetEnumValue(value);
                }
                else
                {
                    return value;
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Files

        public static string GetPathAbsolut(TemplateDocx template)
        {
            return GetPathAbsolut(template.FileName);
        }

        public static string GetPathAbsolut(string fileName)
        {
            if (fileName.IsNullOrEmpty())
                throw new ArgumentException("is null", "fileName");

            var directoryPath = string.Format("{0}/", FoldersHelper.GetPathAbsolut(FolderType.TemplateDocx));
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            if (string.IsNullOrWhiteSpace(fileName))
                return directoryPath;
            return string.Format("{0}{1}", directoryPath, fileName);
        }

        public static string GetPath(string fileName, bool isForAdministration)
        {
            return string.Format("{0}{1}", FoldersHelper.GetPath(FolderType.TemplateDocx, null, isForAdministration), fileName);
        }

        public static bool CheckFileExtension(string fileName)
        {
            return FileHelpers.CheckFileExtensionByType(fileName, EFileType.Document);
        }

        public static bool DeleteTemplate<T>(int id)
            where T : TemplateDocx, new()
        {
            var template = Get<T>(id);
            if (template == null)
                return false;

            FileHelpers.DeleteFile(GetPathAbsolut(template));
            DeleteFromDB(id);

            return true;
        }


        #endregion

        #region DataBase

        public static T Get<T>(int id)
            where T : TemplateDocx, new()
        {
            return SQLDataAccess.ExecuteReadOne<T>(
                "SELECT * FROM [CMS].[TemplatesDocx] WHERE Id = @Id", CommandType.Text,
                GetTemplateDocxFromReader<T>, new SqlParameter("@Id", id));
        }

        public static List<T> GetList<T>()
            where T : TemplateDocx, new()
        {
            return
                SQLDataAccess.ExecuteReadList<T>(
                    "SELECT * FROM [CMS].[TemplatesDocx] WHERE Type = @Type ORDER BY SortOrder",
                    CommandType.Text, GetTemplateDocxFromReader<T>,
                    new SqlParameter("@Type", (int) new T().Type));
        }

        public static List<int> GetIds()
        {
            return SQLDataAccess.ExecuteReadColumn<int>("SELECT [Id] FROM [CMS].[TemplatesDocx]", CommandType.Text, "Id");
        }

        private static T GetTemplateDocxFromReader<T>(SqlDataReader reader)
            where T : TemplateDocx, new()
        {
            return new T
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Type = (TemplateDocxType)SQLDataHelper.GetInt(reader, "Type"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                FileName = SQLDataHelper.GetString(reader, "FileName"),
                FileSize = SQLDataHelper.GetInt(reader, "FileSize"),
                DebugMode = SQLDataHelper.GetBoolean(reader, "DebugMode"),
                DateCreated = SQLDataHelper.GetDateTime(reader, "DateCreated"),
                DateModified = SQLDataHelper.GetDateTime(reader, "DateModified")
            };
        }

        public static int Add(TemplateDocx templatesdocx)
        {
            if (templatesdocx.Type == TemplateDocxType.None)
                throw new ArgumentException("Invalid template type", "templatesdocx.Type");

            return SQLDataAccess.ExecuteScalar<int>(" INSERT INTO [CMS].[TemplatesDocx] " +
                                                    " ([Type], [Name], [SortOrder], [FileName], [FileSize], [DebugMode], [DateCreated], [DateModified]) " +
                                                    " VALUES (@Type, @Name, @SortOrder, @FileName, @FileSize, @DebugMode, GETDATE(), GETDATE()); SELECT SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Type", (int) templatesdocx.Type),
                new SqlParameter("@Name", templatesdocx.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", templatesdocx.SortOrder),
                new SqlParameter("@FileName", templatesdocx.FileName ?? (object) DBNull.Value),
                new SqlParameter("@FileSize", templatesdocx.FileSize),
                new SqlParameter("@DebugMode", templatesdocx.DebugMode)
                );
        }

        public static void Update(TemplateDocx templatesdocx)
        {
            SQLDataAccess.ExecuteNonQuery(
                " UPDATE [CMS].[TemplatesDocx] SET [Name] = @Name, [SortOrder] = @SortOrder, [FileName] = @FileName, [FileSize] = @FileSize, [DebugMode] = @DebugMode, [DateModified] = GETDATE() " +
                " WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", templatesdocx.Id),
                new SqlParameter("@Name", templatesdocx.Name ?? (object) DBNull.Value),
                new SqlParameter("@SortOrder", templatesdocx.SortOrder),
                new SqlParameter("@FileName", templatesdocx.FileName ?? (object) DBNull.Value),
                new SqlParameter("@FileSize", templatesdocx.FileSize),
                new SqlParameter("@DebugMode", templatesdocx.DebugMode)
                );
        }

        private static void DeleteFromDB(int id)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [CMS].[TemplatesDocx] WHERE Id = @Id", CommandType.Text,
                new SqlParameter("@Id", id));
        }

        #endregion
    }
}

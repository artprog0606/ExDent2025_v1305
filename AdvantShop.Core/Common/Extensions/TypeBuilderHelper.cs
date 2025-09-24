using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace AdvantShop.Core.Common.Extensions
{
    public class TypeBuilderHelper
    {
        private readonly Type _inherit;
        private readonly string _typeName;
        private readonly Dictionary<string, TypeBuilderPropertyData> _properties;
        private List<Expression<Func<Attribute>>> _attributeExpressions { get; set; }

        private TypeBuilderHelper()
        {
            _properties = new Dictionary<string, TypeBuilderPropertyData>();
        }

        public TypeBuilderHelper(string typeName) : this()
        {
            _typeName = typeName;
        }

        public TypeBuilderHelper(string typeName, Type inherit) : this(typeName)
        {
            _inherit = inherit;
        }

        public void AddAttribute(Expression<Func<Attribute>> attributeExpression)
        {
            if (_attributeExpressions is null) _attributeExpressions = new List<Expression<Func<Attribute>>>();
            _attributeExpressions.Add(attributeExpression);
        }

        public void AddProperty<T>(string name)
        {
            _properties.Add(name, new TypeBuilderPropertyData(name, typeof(T)));
        }

        public void AddProperty<T>(string name, T defaultValue)
        {
            _properties.Add(name, new TypeBuilderPropertyData(name, typeof(T)) {Value = defaultValue});
        }

        public void AddProperty<T>(string name, params Expression<Func<Attribute>>[] attributeExpressions)
        {
            _properties.Add(name, new TypeBuilderPropertyData(name, typeof(T)) { AttributeExpressions = attributeExpressions });
        }

        public void AddProperty<T>(string name, T defaultValue, params Expression<Func<Attribute>>[] attributeExpressions)
        {
            _properties.Add(name, new TypeBuilderPropertyData(name, typeof(T)) { Value = defaultValue, AttributeExpressions = attributeExpressions });
        }

        private TypeBuilder GetTypeBuilder()
        {
            var asemblyName = new AssemblyName(_typeName);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("AdvantShop");

            var typeBuilder = moduleBuilder.DefineType(asemblyName.FullName
                , TypeAttributes.Public |
                  TypeAttributes.Class |
                  TypeAttributes.AutoClass |
                  TypeAttributes.AnsiClass |
                  TypeAttributes.BeforeFieldInit |
                  TypeAttributes.AutoLayout
                , _inherit);
            
            if (_attributeExpressions != null)
                foreach (var attributeExpression in _attributeExpressions)
                    typeBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attributeExpression));
            
            return typeBuilder;
        }

        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }

        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, params Expression<Func<Attribute>>[] attributeExpressions)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            if (attributeExpressions != null)
                foreach (var attributeExpression in attributeExpressions)
                    propertyBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attributeExpression));

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private CustomAttributeBuilder GetCustomAttributeBuilder(Expression<Func<Attribute>> attributeExpression)
        {
            ConstructorInfo constructor = null;
            List<object> constructorArgs = new List<object>();
            List<PropertyInfo> namedProperties = new List<PropertyInfo>();
            List<object> propertyValues = new List<object>();
            List<FieldInfo> namedFields = new List<FieldInfo>();
            List<object> fieldValues = new List<object>();

            switch (attributeExpression.Body.NodeType)
            {
                case ExpressionType.New:
                    constructor = GetConstructor((NewExpression)attributeExpression.Body, constructorArgs);
                    break;
                case ExpressionType.MemberInit:
                    MemberInitExpression initExpression = (MemberInitExpression)attributeExpression.Body;
                    constructor = GetConstructor(initExpression.NewExpression, constructorArgs);

                    IEnumerable<MemberAssignment> bindings = from b in initExpression.Bindings
                                                             where b.BindingType == MemberBindingType.Assignment
                                                             select b as MemberAssignment;

                    foreach (MemberAssignment assignment in bindings)
                    {
                        LambdaExpression lambda = Expression.Lambda(assignment.Expression);
                        object value = lambda.Compile().DynamicInvoke();
                        switch (assignment.Member.MemberType)
                        {
                            case MemberTypes.Field:
                                namedFields.Add((FieldInfo)assignment.Member);
                                fieldValues.Add(value);
                                break;
                            case MemberTypes.Property:
                                namedProperties.Add((PropertyInfo)assignment.Member);
                                propertyValues.Add(value);
                                break;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("UnSupportedExpression", "attributeExpression");
            }

            return new CustomAttributeBuilder(
                constructor,
                constructorArgs.ToArray(),
                namedProperties.ToArray(),
                propertyValues.ToArray(),
                namedFields.ToArray(),
                fieldValues.ToArray());
        }

        private ConstructorInfo GetConstructor(NewExpression expression, List<object> constructorArgs)
        {
            foreach (Expression arg in expression.Arguments)
            {
                LambdaExpression lambda = Expression.Lambda(arg);
                object value = lambda.Compile().DynamicInvoke();
                constructorArgs.Add(value);
            }
            return expression.Constructor;
        }

        public Type CreateType()
        {
            var typeBuilder = GetTypeBuilder();
            CreateConstructor(typeBuilder);

            foreach (var propertyData in _properties.Values)
                CreateProperty(typeBuilder, propertyData.Name, propertyData.Type, propertyData.AttributeExpressions);

            return typeBuilder.CreateType();
        }

        public object CreateInstance()
        {
            var type = CreateType();
            var obj = Activator.CreateInstance(type);

            if (_properties.Count > 0)
                foreach (var propertyData in _properties.Values)
                    if (propertyData.Value != null)
                        type.GetProperty(propertyData.Name).SetValue(obj, propertyData.Value);

            return obj;
        }
    }

    public class TypeBuilderPropertyData
    {
        public TypeBuilderPropertyData(string name, Type type)
        {
            Name = name;
            Type = type;
        }
        public Type Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public Expression<Func<Attribute>>[] AttributeExpressions { get; set; }
    }
}

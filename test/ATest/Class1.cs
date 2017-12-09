using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MowaInfo.ProtoSocket.Annotations;
using ProtoBuf;

namespace ATest
{
    public static class Class1
    {
        public static Type AutoGeneratePackage<T>(Assembly assembly, string assemblyName, string moduleName, string className)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => typeof(T).IsAssignableFrom(t))
                .ToArray();

            var tb = GetTypeBuilder(assemblyName, moduleName, className);

            tb.SetCustomAttribute(typeof(ProtoContractAttribute).GetConstructor(new Type[] { }) ?? throw new InvalidOperationException(), new byte[] { });

            foreach (var type in types)
            {
                CreateProperty(tb, type.Name, type);
            }

            var objectType = tb.CreateTypeInfo().AsType();
            return objectType;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
#if NETSTANDARD1_3
            var tag = propertyType.GetTypeInfo().GetCustomAttributes<MessageTypeAttribute>().First().MessageType;

#else
            var tag = propertyType.GetCustomAttributes<MessageTypeAttribute>().First().MessageType;

#endif
            propertyBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(typeof(ProtoMemberAttribute).GetConstructor(new[] { typeof(int) }) ?? throw new InvalidOperationException(), new object[] { tag }));

            var getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static TypeBuilder GetTypeBuilder(string assemblyName, string moduleName, string className)
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName + ".dll");
            return moduleBuilder.DefineType(moduleName + "." + className,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(Package));
        }
    }
}

using System;
using ATest;
using MowaInfo.ProtoSocket.Abstract;
using Xunit;

#if NETCOREAPP1_1
using System.Reflection;
#endif

namespace AutoGenerateTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var assemblyName = "Assembly";
            var moduleName = "Module";
            var className = "Class";
#if NETCOREAPP1_1
            var type = Class1.AutoGeneratePackage<IUpMessage>(typeof(HiMessage).GetTypeInfo().Assembly, assemblyName, moduleName, className);
#else
            var type = Class1.AutoGeneratePackage<IUpMessage>(typeof(HiMessage).Assembly, assemblyName, moduleName, className);

#endif
            var type1 = typeof(ComparePackage);
            var properties = type.GetProperties();
            var properties1 = type1.GetProperties();
            var x = Activator.CreateInstance(type);
            type.GetProperty("Id")?.SetValue(x, (ulong)1);
            Assert.NotNull(type.GetProperty("HiMessage"));
            var message = type.GetProperty("HiMessage")?.GetValue(x);
            Assert.Null(message);
            var id = type.GetProperty("Id")?.GetValue(x);
            Assert.Equal((ulong)1, id);

            var y = Activator.CreateInstance(type1);
            type1.GetProperty("Id")?.SetValue(y, (ulong)1);
            Assert.NotNull(type1.GetProperty("WelcomeMessage"));
            var message1 = type1.GetProperty("WelcomeMessage")?.GetValue(y);
            Assert.Null(message1);
            var id1 = type1.GetProperty("Id")?.GetValue(y);
            Assert.Equal((ulong)1, id1);
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.DotNet.Cli.Utils;
using MowaInfo.ProtoSocket.Abstract;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;

namespace dotnet_proto
{
    public class ProtoCommand
    {
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }

        public int Proto()
        {
            try
            {
                var directory = Directory.GetCurrentDirectory();
                var assemblyPath = FindFile(directory, AssemblyName);
                var myAssembly = Assembly.LoadFrom(assemblyPath);
                var myType = myAssembly.GetTypes().SingleOrDefault(t => t.Name == ClassName);
                if (myType == null)
                {
                    try
                    {
                        Reporter.Output.WriteLine($"Type '{ClassName}' not found in assembly '{myAssembly}'. The following types are available:");
                        var types = myAssembly.GetTypes();
                        foreach (var definedType in types)
                            Reporter.Output.WriteLine($" - {definedType.Name}");
                    }
                    catch (Exception e)
                    {
                        Reporter.Output.WriteLine(JsonConvert.SerializeObject(e));
                        throw;
                    }
                }


                using (var sr = new StreamWriter(new FileStream(directory + "\\proto.txt", FileMode.Create, FileAccess.Write)))
                {
                    var protoString = RuntimeTypeModel.Default.GetSchema(myType, ProtoSyntax.Proto2);
                    //var protoString = Serializer.GetProto<MessageContainer>();
                    sr.WriteLine("syntax = \"proto2\";");
                    sr.WriteLine("package MowaLamp.OriginCommand;");
                    sr.WriteLine("option optimize_for = LITE_RUNTIME;");
                    sr.WriteLine(protoString);
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
        }

        static string FindFile(string directory, string assembly, bool throwIfNotFound = true)
        {
            foreach (string d in Directory.GetDirectories(directory))
            {
                var files = Directory.GetFiles(d, $"{assembly}.dll");
                
                if (files.Length == 1)
                    return files[0];
                else if (files.Length > 1)
                    throw new Exception("Found more than one matching assembly");

                var subdirResult = FindFile(d, assembly, throwIfNotFound: false);
                if (subdirResult != null)
                    return subdirResult;
            }

            if (throwIfNotFound)
                throw new Exception("Could not find the assembly");

            return null;
        }
    }
}

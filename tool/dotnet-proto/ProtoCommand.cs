using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.DotNet.Cli.Utils;
using MowaInfo.ProtoSocket.Abstract;
using Newtonsoft.Json;
using ProtoBuf.Meta;

namespace dotnet_proto
{
    public class ProtoCommand
    {
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string Syntax { get; set; }
        public string Output { get; set; }

        public int Proto()
        {
            try
            {
                var directory = Directory.GetCurrentDirectory();
                var assemblyPath = FindFile(directory, AssemblyName);
                var myAssembly = Assembly.LoadFrom(assemblyPath);
                Type myType;
                if (string.IsNullOrEmpty(ClassName))
                {
                    var containerTypes = myAssembly.GetTypes().Where(t => typeof(IMessageContainer).IsAssignableFrom(t)).ToList();
                    if (containerTypes.Count == 0)
                    {
                        Reporter.Output.WriteLine("No type inherited IMessageContainer");
                        return 1;
                    }
                    if (containerTypes.Count > 1)
                    {
                        Reporter.Output.WriteLine("Too Many types inherited IMessageContainer");
                        return 1;
                    }
                    myType = containerTypes[0];
                }
                else
                {
                    myType = myAssembly.GetTypes().SingleOrDefault(t => t.Name == ClassName);
                }
                if (myType == null)
                {
                    try
                    {
                        Reporter.Output.WriteLine($"Type '{ClassName}' not found in assembly '{myAssembly}'. The following types are available:");
                        var types = myAssembly.GetTypes();
                        foreach (var definedType in types)
                        {
                            Reporter.Output.WriteLine($" - {definedType.Name}");
                        }
                    }
                    catch (Exception e)
                    {
                        Reporter.Output.WriteLine(JsonConvert.SerializeObject(e));
                        throw;
                    }
                }

                string path;
                if (string.IsNullOrEmpty(Output))
                {
                    Output = $"{AssemblyName}.proto";
                    path = Path.Combine(directory, Output);
                }
                else
                {
                    if (Path.IsPathRooted(Output))
                    {
                        path = !string.IsNullOrEmpty(Path.GetFileName(Output)) ? Output : Path.Combine(Output, $"{AssemblyName}.proto");
                    }
                    else
                    {
                        path = !string.IsNullOrEmpty(Path.GetFileName(Output)) ? Path.Combine(directory, Output) : Path.Combine(directory, Output, $"{AssemblyName}.proto");
                    }
                }
                Reporter.Output.WriteLine(path);
                using (var sr = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                {
                    var protoString = RuntimeTypeModel.Default.GetSchema(myType, Syntax == "Proto2" ? ProtoSyntax.Proto2 : ProtoSyntax.Proto3);
                    sr.WriteLine(protoString);
                }
                return 0;
            }
            catch (Exception e)
            {
                Reporter.Error.WriteLine(e.Message);
                return 1;
            }
        }

        private static string FindFile(string directory, string assembly, bool throwIfNotFound = true)
        {
            foreach (var d in Directory.GetDirectories(directory))
            {
                var files = Directory.GetFiles(d, $"{assembly}.dll");

                if (files.Length == 1)
                {
                    return files[0];
                }
                if (files.Length > 1)
                {
                    throw new Exception("Found more than one matching assembly");
                }

                var subdirResult = FindFile(d, assembly, false);
                if (subdirResult != null)
                {
                    return subdirResult;
                }
            }

            if (throwIfNotFound)
            {
                throw new Exception("Could not find the assembly");
            }

            return null;
        }
    }
}

using System;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace dotnet_proto
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "proto",
                FullName = ".NET API CLI create proto tool",
                Description = "Create proto file"
            };
            app.HelpOption("-h|--help");

            var assemblyOption = app.Option("-a |--assembly <assembly>", "The assembly name", CommandOptionType.SingleValue);
            var classOption = app.Option("-c |--class <class>", "The fully qualified class name", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var proto = new ProtoCommand
                {
                    AssemblyName = assemblyOption.Value(),
                    ClassName = classOption.Value()
                };

                if (string.IsNullOrEmpty(proto.AssemblyName))
                {
                    proto.AssemblyName = MyMsBuild.GetAssemblyName();
                    if (string.IsNullOrEmpty(proto.AssemblyName))
                    {
                        Reporter.Error.WriteLine("No Assembly!");
                        return 1;
                    }
                }

                return proto.Proto();
            });

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Reporter.Error.WriteLine(ex.Message.Red());
                return 1;
            }
        }
    }
}

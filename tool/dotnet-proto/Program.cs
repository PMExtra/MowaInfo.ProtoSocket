using System;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;
using ProtoBuf.Meta;

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
            var syntaxOption = app.Option("-s |--syntax <syntax>", "ProtoSyntax", CommandOptionType.SingleValue);
            var outputOption = app.Option("-o |--output <output>", "Output filename", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var proto = new ProtoCommand
                {
                    AssemblyName = assemblyOption.Value(),
                    ClassName = classOption.Value(),
                    Syntax = string.IsNullOrEmpty(syntaxOption.Value()) ? "Proto2" : syntaxOption.Value(),
                    Output = outputOption.Value()
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

                if (!Enum.IsDefined(typeof(ProtoSyntax), proto.Syntax))
                {
                    Reporter.Error.WriteLine("-s is not vaild. Use Proto2 or Proto3!");
                    return 1;
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

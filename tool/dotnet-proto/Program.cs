using System;
using System.IO;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace dotnet_proto
{
    class Program
    {
        static int Main(string[] args)
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


            // var writer = new ConsoleWriter();
            // 
            // if (args.Length == 0)
            // {
            //     ShowHelp();
            //     return 0;
            // }

            // We are assuming just a single string on invocation (naive, I know)
            app.OnExecute(() =>
            {
                var proto = new ProtoCommand
                {
                    //AssemblyName = "dotnet-proto",
                    //ClassName = "Program"
                    AssemblyName = assemblyOption.Value(),
                    ClassName = classOption.Value()
                };

                if (String.IsNullOrEmpty(proto.AssemblyName))
                {
                    Reporter.Output.WriteLine("-a |--assembly <assembly> argument is required. Use -h|--help to see help");
                    return 0;
                }
                if (String.IsNullOrEmpty(proto.ClassName))
                {
                    Reporter.Output.WriteLine("-c |--class <class> argument is required. Use -h|--help to see help");
                    return 0;
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

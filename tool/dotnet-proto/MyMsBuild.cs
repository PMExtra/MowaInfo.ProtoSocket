using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;

namespace dotnet_proto
{
    public static class MyMsBuild
    {
        public static string GetAssemblyName()
        {
            var projectFile = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*proj") // filter xproj files, which are MSBuild meta-projects
                .FirstOrDefault(f => !f.EndsWith(".xproj"));

            var targetFileName = Path.GetFileName(projectFile) + ".dotnet-names.targets";
            var projectExtPath = Path.Combine(Path.GetDirectoryName(projectFile), "obj");
            var targetFile = Path.Combine(projectExtPath, targetFileName);
            File.WriteAllText(targetFile,
                @"<Project>
                <Target Name=""_GetDotNetNames"">
                    <ItemGroup>
                    <_DotNetNamesOutput Include=""AssemblyName: $(AssemblyName)"" />
                    <_DotNetNamesOutput Include=""RootNamespace: $(RootNamespace)"" />
                    <_DotNetNamesOutput Include=""TargetFramework: $(TargetFramework)"" />
                    <_DotNetNamesOutput Include=""TargetFrameworks: $(TargetFrameworks)"" />
                    </ItemGroup>
                    <WriteLinesToFile File=""$(_DotNetNamesFile)"" Lines=""@(_DotNetNamesOutput)"" Overwrite=""true"" />
                </Target>
            </Project>");

            var tmpFile = Path.GetTempFileName();
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"msbuild \"{projectFile}\" /t:_GetDotNetNames /nologo \"/p:_DotNetNamesFile={tmpFile}\""
            };
            var process = Process.Start(psi);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Reporter.Error.WriteLine("Invoking MSBuild target failed");
                return null;
            }

            var lines = File.ReadAllLines(tmpFile);
            File.Delete(tmpFile); // cleanup

            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in lines)
            {
                var idx = line.IndexOf(':');
                if (idx <= 0) continue;
                var name = line.Substring(0, idx)?.Trim();
                var value = line.Substring(idx + 1)?.Trim();
                properties.Add(name, value);
            }
            return properties["AssemblyName"];
        }
    }
}

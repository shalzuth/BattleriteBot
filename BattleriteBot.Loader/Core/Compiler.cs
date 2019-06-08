using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace BattleriteBot.Loader.Core
{
    public class Compiler
    {
        public static String ProjName { get; } = typeof(Compiler).Namespace.Split('.').First();
        public static String UnityDllPath = "";
        public static Boolean UpdateSources()
        {
            if (Directory.Exists($@"..\..\..\{ProjName}"))
                return false;
            Console.WriteLine("Getting latest version info from Github");
            System.Net.WebClient wc = new System.Net.WebClient();
            String hash = wc.DownloadString($"https://github.com/shalzuth/{ProjName}/tree/master/{ProjName}");
            String hashSearch = $"\"commit-tease-sha\" href=\"/shalzuth/{ProjName}/commit/";
            hash = hash.Substring(hash.IndexOf(hashSearch) + hashSearch.Length, 7);
            String hashFile = $@".\{ProjName}-master\hash.txt";
            if (File.Exists(hashFile))
            {
                if (hash != File.ReadAllText(hashFile))
                {
                    Console.WriteLine("Later version exists, removing existing version");
                    Directory.Delete($@".\{ProjName}-master", true);
                }
            }
            if (!File.Exists(hashFile))
            {
                Console.WriteLine("Downloading latest version");
                wc.DownloadFile($"https://github.com/shalzuth/{ProjName}/archive/master.zip", $"{ProjName}.zip");
                using (var archive = ZipFile.OpenRead($"{ProjName}.zip"))
                {
                    archive.ExtractToDirectory(@".\");
                }
                File.WriteAllText(hashFile, hash);
                return true;
            }
            return false;
        }
        public static Byte[] CompileDll(String randString)
        {
            var options = new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } };// { { "CompilerVersion", "v4.0" } };
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(options);
            //CodeDomProvider codeProvider2 = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
#if DEBUG
                IncludeDebugInformation = true,
#endif
                GenerateInMemory = false,
                OutputAssembly = randString + ".dll",
            };
            compilerParameters.ReferencedAssemblies.Clear();
            //compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
            var dlls = Directory.GetFiles(UnityDllPath, "*.dll", SearchOption.AllDirectories);
            foreach (var dll in dlls)
                if (!dll.Contains("mscorlib") && !dll.Contains("Threading"))
                    compilerParameters.ReferencedAssemblies.Add(dll);

            string[] sourceFiles;
            if (Directory.Exists($@"..\..\..\{ProjName}"))
                sourceFiles = Directory.GetFiles($@"..\..\..\{ProjName}", "*.cs", SearchOption.AllDirectories);
            else
                sourceFiles = Directory.GetFiles($@"{ProjName}-master\{ProjName}\", "*.cs", SearchOption.AllDirectories);
            var sources = new List<String>();
            foreach (var sourceFile in sourceFiles)
            {
                if (sourceFile.Contains(@"obj\\"))
                    continue;
                var source = File.ReadAllText(sourceFile);
                source = source.Replace(ProjName, randString);
                sources.Add(source);
            }
            var result = codeProvider.CompileAssemblyFromSource(compilerParameters, sources.ToArray());
            if (result.Errors.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (CompilerError error in result.Errors)
                    sb.AppendLine(error.ToString());
                throw new Exception(sb.ToString());
            }
            return File.ReadAllBytes(randString + ".dll");
        }
    }
}

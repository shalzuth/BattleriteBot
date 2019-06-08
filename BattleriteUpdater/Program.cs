using Mono.Cecil;
using System;
using System.Linq;

namespace BattleriteUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Program Files (x86)\Steam\steamapps\common\Battlerite\Battlerite_Data\Managed\MergedUnity.dll";
            var assembly = AssemblyDefinition.ReadAssembly(path);
            var baseGameType = assembly.MainModule.GetTypes().First(t => t.Properties.Count(f => f.Name == "Pathfinding") > 0);
            Console.WriteLine($"        static String baseGameNamespace = \"{baseGameType.Namespace}\";");
            Console.WriteLine($"        static String baseGameTypeName = \"{baseGameType.Name}\";");
            var gameDataContainer = assembly.MainModule.GetTypes().First(t => t.Namespace == baseGameType.Namespace && t.Fields.Count(f => f.FieldType.Name == "GameDataInner" && f.IsStatic) > 0);
            Console.WriteLine($"        static String gameDataTypeName = \"{gameDataContainer.Name}\";");
            var stringHashSystem = assembly.MainModule.GetTypes().First(t => t.Namespace == baseGameType.Namespace && t.Fields.Count(f => f.FieldType.Name == "StringHashSystem" && f.IsStatic) > 0);
            Console.WriteLine($"        static String stringHashTypeName = \"{stringHashSystem.Name}\";");
            var getListMethod = baseGameType.Methods.ToList().First(m => m.Parameters.Count == 2 && m.Parameters[1].ParameterType.ToString().Contains("String") && m.Name.Contains("#") && m.ReturnType.Name.Contains("#") && m.ReturnType.Resolve().Properties.Count(p => p.Name == "Count") > 0);
            Console.WriteLine($"        static String getStateListName = \"{getListMethod.Name}\";");
            var getStateListMethod = baseGameType.Methods.ToList().First(m => m.Parameters.Count == 2 && m.Parameters[1].ParameterType.ToString().Contains("String") && m.Name.Contains("#") && m.ReturnType.Name.Contains("#") && m.ReturnType.Resolve().Properties.Count(p => p.Name == "Count") > 0);
            Console.WriteLine($"        static String getStateListName = \"{getListMethod.Name}\";");
            var modelstate = assembly.MainModule.GetTypes().First(t => t.Methods.Count(m => m.HasParameters && m.Parameters[0].ParameterType.Name == "ModelState") > 0);
            //Console.WriteLine($"        public static String ModelStateNameSpace = \"{modelstate.Namespace}\";");
            //Console.WriteLine($"        public static String ModelStateListTypeName = \"{modelstate.Name}\";");
            Console.ReadLine();
        }
    }
}

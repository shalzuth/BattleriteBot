using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Gameplay;
using Gameplay.GameObjects;

namespace BattleriteBot
{
    public static class DeObfuscator
    {
        public static String baseGameNamespace = typeof(GameClient).BaseType.Namespace;
        public static String baseGameTypeName = typeof(GameClient).BaseType.Name;
        public static String gameDataTypeName = typeof(GameClient).Assembly.GetTypes().First(t => t.Namespace == baseGameNamespace && t.GetFields(Reflection.flags).Count(f => f.FieldType.Name == "GameDataInner" && f.IsStatic) > 0).Name;
        public static String stringHashTypeName = typeof(GameClient).Assembly.GetTypes().First(t => t.Namespace == baseGameNamespace && t.GetFields(Reflection.flags).Count(f => f.FieldType.Name == "StringHashSystem" && f.IsStatic) > 0).Name;
        public static MethodInfo GetStateListMethod = typeof(GameClient).GetMethods(Reflection.flags).First(m => m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Contains("String") && m.Name.Contains("#") && m.ReturnType.Name.Contains("#") && m.ReturnType.GetProperties(Reflection.flags).Count(p => p.Name == "Count") > 0);
        public static MethodInfo GetListMethod = typeof(GameClient).BaseType.GetMethods().FirstOrDefault(m => m.Name == "GetList" && m.GetParameters().Last().ParameterType.ToString().Contains("String"));
        public static MethodInfo GetStateMethod = typeof(GameClient).BaseType.GetMethods().FirstOrDefault(m => m.Name == "TryGetState" && m.GetParameters()[1].ParameterType.ToString().Contains("String") && m.GetParameters()[2].ParameterType.ToString().Contains("GameValue"));
        public static MethodInfo SetStateMethod = typeof(GameClient).BaseType.GetMethods().FirstOrDefault(m => m.Name == "SetState" && m.GetParameters()[1].ParameterType.ToString().Contains("String"));

        public static GameDataInner GameData = Type.GetType(baseGameNamespace + "." + gameDataTypeName + ",MergedUnity").GetField<GameDataInner>("#a");

        public static Type GameClientModelAssetsType = typeof(GameClient).Assembly.GetTypes().First(t => t.GetMethods().Count(m => m.Name == "Add" && m.ReturnType == typeof(Gameplay.View.ModelAssetIndex)) > 0);
        public static Type GameClientModelStatesType = typeof(GameClient).Assembly.GetTypes().First(t => t.GetMethods().Count(m => m.Name == "Add" && m.GetParameters().FirstOrDefault().ParameterType == typeof(Gameplay.View.ModelState)) > 0);
        static DeObfuscator()
        {
        }
    }
}

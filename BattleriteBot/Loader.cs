using System.Reflection;
using System.Linq;
namespace BattleriteBot
{
    public class Loader
    {
        public static UnityEngine.GameObject BaseObject;

        public static void Load()
        {
            while (BaseObject = UnityEngine.GameObject.Find("Battlerite Bot"))
                UnityEngine.GameObject.Destroy(BaseObject);
            BaseObject = new UnityEngine.GameObject("Battlerite Bot");
            UnityEngine.Object.DontDestroyOnLoad(BaseObject);
            BaseObject.SetActive(false);
            var types = Assembly.GetExecutingAssembly().GetTypes().ToList().Where(t => t.BaseType == typeof(UnityEngine.MonoBehaviour) && !t.IsNested);
            foreach(var type in types)
            {
                var component = (UnityEngine.MonoBehaviour)BaseObject.AddComponent(type);
                component.enabled = false;
            }
            BaseObject.GetComponent<API>().enabled = true;
            BaseObject.GetComponent<Menu>().enabled = true;
            BaseObject.SetActive(true);
            //var component = (UnityEngine.MonoBehaviour)BaseObject.AddComponent<Menu>();
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(BaseObject);
        }
    }
}

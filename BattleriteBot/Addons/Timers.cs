using System;
using System.Linq;
using System.Reflection;
using Gameplay;
using Gameplay.View;
using UnityEngine;

namespace BattleriteBot.Addons
{
    public class Timers : MonoBehaviour
    {
        public void Start()
        {
        }

        public void OnGUI()
        {
            if (API.Instance.ViewState == null || Camera.main == null || !API.Instance.InGame)
                return;
            foreach (var current in API.Instance.ViewState.ActiveObjects.Values)
            {
                if (current.TypeId.ToString(API.GameData) == "ShardManager")
                {
                    var currentTime = (Single)current.ObjectId.Get("Age");
                    var lastPickupTimes = current.ObjectId.GetStateList("ShardTimestamp").Select(i => (Single)i).ToList();
                    var shardPositions = current.ObjectId.GetList("ShardPosition").Select(i => (MathCore.Vector2)i).ToList();
                    for (int i = 0; i < lastPickupTimes.Count(); i++)
                    {
                        var lastPickupTime = lastPickupTimes[i];
                        var timeToSpawn = (25 - currentTime + lastPickupTime);
                        if (timeToSpawn > 0)
                        {
                            var shardPosition = shardPositions[i];
                            var screenPos = Camera.main.WorldToScreenPoint(shardPosition.ToUnityVector3(1));
                            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 30, 30), (timeToSpawn).ToString("0.0"));
                        }
                    }
                }
                if (current.TypeId.ToString(API.GameData).StartsWith("RiteOf") && current.TypeId.ToString(API.GameData).EndsWith("Spawner"))
                {
                    var age = current.ObjectId.Get("Age");
                    if (age == default(GameValue))
                        continue;
                    var screenPos = Camera.main.WorldToScreenPoint(current.Position.ToUnityVector3(1));
                    GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 30, 30), (20.0f - age).ToString("0.0"));
                }
            }
        }
    }
}
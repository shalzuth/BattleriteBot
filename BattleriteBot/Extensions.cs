using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodGUI;
using Gameplay;
using Gameplay.GameObjects;
using Gameplay.View;
using UnityEngine;
using Vector2 = MathCore.Vector2;

namespace BattleriteBot
{
    public static class Extensions
    {
        public static Vector3 Position(this GameObjectId gameObjectId)
        {
            foreach (ModelState current in API.Instance.ViewState.Models.Values)
            {
                if (current.Id == gameObjectId)
                {
                    return current.Position.ToUnityVector3(current.Height);
                }
            }
            return Vector3.zero;// Loader.Controller.UnityMain.GetModelPosition(playerInfo.ID.ToGame());
        }
        public static Vector2 PredictedPosition2d(this Data_PlayerInfo playerInfo, float time)
        {
            if (playerInfo.ID.Index == 0 && playerInfo.ID.Generation == 0)
            {
                return Vector2.Zero;
            }
            Vector2 position = playerInfo.ID.ToGame().Get("Position");
            Vector2 velocity = playerInfo.ID.ToGame().Get("Velocity");
            return position + velocity.Normalized * time;
        }
        public static Vector3 PredictedPosition(this Data_PlayerInfo playerInfo, float time)
        {
            if (playerInfo.ID.Index == 0 && playerInfo.ID.Generation == 0)
            {
                return new Vector3(0, 0, 0);
            }
            Vector2 position = playerInfo.ID.ToGame().Get("Position");
            Vector2 velocity = playerInfo.ID.ToGame().Get("Velocity");
            Vector2 predictedPosition = position + velocity.Normalized * time;
            return new Vector3(predictedPosition.X, 0, predictedPosition.Y);
            //return Loader.Controller.UnityMain.GetObjectPosition(playerInfo.ID.ToGame());
        }
        public static GameObjectId ToGame(this UIGameObjectId id)
        {
            return new GameObjectId(id.Index, id.Generation);
        }
        public static GameValue Get(this GameObjectId obj, string name)
        {
            return API.Instance.GetGameState(obj, name);
        }
        public static void Set<T>(this GameObjectId obj, string name, T value)
        {
            API.Instance.SetGameState(obj, name, value);
        }
        private static PropertyInfo GetListValueProperty = null;
        private static PropertyInfo GetListCountProperty = null;
        public static List<GameValue> GetList(this GameObjectId obj, String name)
        {
            var stateList = DeObfuscator.GetListMethod.Invoke(API.Instance.GameClientObject, new object[] { obj, name });
            if (GetListValueProperty == null)
            {
                var structMethods = stateList.GetType().GetMethods(Reflection.flags);
                GetListValueProperty = stateList.GetType().GetProperty("Item");
                GetListCountProperty = stateList.GetType().GetProperty("Count");
            }
            var count = (int)GetListCountProperty.GetValue(stateList, new object[0]);
            List<GameValue> elements = new List<GameValue>();
            for (int i = 0; i < count; i++)
                elements.Add((GameValue)GetListValueProperty.GetValue(stateList, new object[] { i }));
            return elements;
        }
        private static MethodInfo GetStateListValueMethod = null;
        private static PropertyInfo GetStateListCountProperty = null;
        public static List<GameValue> GetStateList(this GameObjectId obj, string name)
        {
            var stateList = DeObfuscator.GetStateListMethod.Invoke(API.Instance.GameClientObject, new object[] { obj, name });
            if (GetStateListValueMethod == null)
            {
                var structMethods = stateList.GetType().GetMethods(Reflection.flags);
                GetStateListValueMethod = structMethods.First(m => m.Name == "get_Item");
                GetStateListCountProperty = stateList.GetType().GetProperty("Count");
            }
            var count = (int)GetStateListCountProperty.GetValue(stateList, new object[0]);
            List<GameValue> elements = new List<GameValue>();
            for (int i = 0; i < count; i++)
                elements.Add((GameValue)GetStateListValueMethod.Invoke(stateList, new object[] { i }));
            return elements;
        }
        /*public static GameValue Get(this StateTableId state, String name)
        {
            return (GameValue)API.GetStateTableValueMethod.Invoke(API.Instance.StateSystem, new object[2] { state, API.StringHashSystem.GetHash(name) });
        }*/
        public static Double GetHealableHealth(this Data_PlayerInfo playerInfo)
        {
            return playerInfo.RecoveryHealth.Value.Current - playerInfo.Health.Value.Current;
        }
    }
}

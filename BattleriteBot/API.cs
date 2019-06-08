using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BloodGUI;
using BloodGUI_Binding.Base;
using BloodGUI_Binding.HUD;
using EffectSystem;
using Gameplay;
using Gameplay.GameObjects;
using Gameplay.View;
using RemoteClient;
using UnityEngine;
using Vector2 = MathCore.Vector2;

namespace BattleriteBot
{
    public class API : MonoBehaviour
    {
        public static API Instance;
        public static GameDataInner GameData = DeObfuscator.GameData;
        public delegate void GameStart();

        public IDictionary GlueInstances = (IDictionary)MergedUnity.Glue.GUI.GUIGlobals.Glue.GetField("_Instances");
        public T GetFromGlue<T>(String typeName)
        {
            var GlueType = Type.GetType("MergedUnity.Glue." + typeName + ",MergedUnity");
            var GlueInstance = GlueInstances[GlueType].GetField("Instance");
            return (T)GlueInstance.Invoke("GetInstance", null);
        }

        public static List<String> CounterAbilities = new List<String>() {
            "WujuReformAbility"
        };
        public static List<String> CounterObjects = new List<String>() {
            "BulwarkAlternateBuff", "ShadowGateAlternateBuff", "KunjuBuff",
            "SolarChargeTrance", "BerserkTrance", "WujuBuff", "TimeBenderBuff",
            "ArcaneBarrierBuff", "TidalWaveTrance", "ThunderShield", "Parry",
            "GustBuff"
        };
        public static List<String> InvincibleObjects = new List<String>() {
            "SearingDisplacement",
            "HeroicLeap", "HeroicStomp",
            "Displace", "DisplaceRecast",
            "Spring", "ShockVaultAlternate", "ThunderClapLeap", "ThunderSlam", "ThunderStompLeap",
            "JetPackAlternate",
            "BlastVault",
            "ViperSting",
            "Roll",
            "TimeShift", "TimeTravelFly",
            "BerserkShield", "ExProwlBuff",
            "Dive", "HungryFishSwallowedDebuff",
            "TheOtherSide", "SoulTransferRecastMoveBuff", "SoulTransferAlternateRecastMoveBuff", "GuardianSpiritAlternateTrigger",
            "HeavenlyStrike", "WrathOfTheTigerAreaSlash",
            "CrushingBlowLeap", "TremorLeap1", "TremorLeap2", "TremorLeap3",
            "NetherVoidTravelBuff",
            "Fleetfoot", "TempestRushBuff", "JavelinBuff",
            "SolarChargeFly", "MoonEllipse",
            "Zephyr", "TornadoBuff",
            "Infest", "InfestingBuff", "InfestSecondDash",
            "ReformTravelBuff", "PowersCombinedFly1", "PowersCombinedFly2",
        };

        //public static MethodInfo GetCameraInputs = typeof(Main).GetMethod("GetCameraInputs", Extensions.flags);
        public BloodgateModelPool bloodgateModelPool;
        //public UI_BloodgateChatBindings chat;
        public List<object> currentModels;
        public GameObject effects;
        public bool InGame;
        public GameStart OnMatchStart;
        public PrefabInstanceSystem prefabInstance;
        //public List<PrefabInstanceSystem.PrefabInstanceState> prefabStates;
        private int previousRound;

        public static StunShared.StringHashSystem StringHashSystem = Type.GetType(DeObfuscator.baseGameNamespace + "." + DeObfuscator.stringHashTypeName + ",MergedUnity").GetField<StunShared.StringHashSystem>("#a");

        public UI_HUDBase HudBase { get { return GetFromGlue<UI_HUDBase>("HUDBaseGlue"); } }
        public object ViewSystems { get { return GetFromGlue<object>("GameplayViewSystems"); } }
        public IGameClient GameClientInterfaceObject { get { return GetFromGlue<IGameClient>("GameClientGlue"); } }
        public GameClient GameClientObject { get { return GameClientInterfaceObject.GetGame(); } }
        public object StateSystem { get { return GameClientObject.GetField("StateSystem"); } }

        public UI_PlayersInfoBinding PlayersInfoBinding { get { return (UI_PlayersInfoBinding)HudBase.GetField("_PlayersInfoBinding"); } }

        public List<Data_PlayerInfo> EnemyTeamData
        {
            get
            {
                var enemyData = (List<Data_PlayerInfo>)PlayersInfoBinding.GetField("_EnemyTeamData");
                if (enemyData == null)
                {
                    return new List<Data_PlayerInfo>();
                }
                return enemyData;
            }
        }

        public List<Data_PlayerInfo> LocalTeamData
        {
            get
            {
                var localTeamData = (List<Data_PlayerInfo>)PlayersInfoBinding.GetField("_LocalTeamData");
                if (localTeamData == null)
                {
                    return new List<Data_PlayerInfo>();
                }
                return localTeamData;
            }
        }

        public Data_PlayerInfo LocalPlayer
        {
            get
            {
                var teamdata = LocalTeamData;
                if (teamdata.Count == 0)
                {
                    return default(Data_PlayerInfo);
                }
                return teamdata.Find(p => p.LocalPlayer);
            }
        }

        public ViewState ViewState { get { return GameClientInterfaceObject.GetViewState(); } }
        //public String ChampionName { get {  return ViewState.GetControlledObjectType()} }
        public ActiveObject ActiveCamera { get { return ViewState.ActiveObjects.Values.First(c => c.TypeId.ToString(GameData) == "CameraPreset_TopDownFollow"); } }

        public bool InRound
        {
            get
            {
                if (ViewState == null)
                    return false;
                var localPlayer = LocalPlayer;
                return (!LocalPlayer.IsDead || ViewState.ActiveObjects.Values.Count(o => o.TypeId.ToString().Contains("RoundPhase")) > 0);
            }
        }

        public Single Time { get { return ViewState.ActiveObjects.Values.First(ao => ao.TypeId.ToString(GameData) == "ShardManager").ObjectId.Get("Age"); } }

        public void GameStartInit()
        {
            //var bottomBar = (UI_BloodgateBottomBar)uiBloodgateBase.GetType().GetField("_BottomBar", flags).GetValue(uiBloodgateBase);
            //chat = (BloodGUI.Bloodgate.UI_BloodgateChatBindings)bottomBar.GetType().GetField("_ChatBindings", flags).GetValue(bottomBar);
            prefabInstance = (PrefabInstanceSystem)ViewSystems.GetField("PrefabInstance");
            //prefabStates = (List<PrefabInstanceSystem.PrefabInstanceState>)Loader.Controller.prefabInstance.GetField("_PrefabStates");
        }
        public GameValue GetGameState(GameObjectId id, string key)
        {
            if (GameClientObject != null)
            {
                GameValue gameValue = default(GameValue);
                object[] array = new object[] { id, key, gameValue };
                object success = DeObfuscator.GetStateMethod.Invoke(GameClientObject, array);
                return (GameValue)array[2];
            }
            return default(GameValue);
        }

        public void SetGameState<T>(GameObjectId id, string key, T value)
        {
            if (GameClientObject != null)
                DeObfuscator.SetStateMethod.MakeGenericMethod(typeof(T)).Invoke(GameClientObject, new object[] { id, key, value, false });
        }
        public void Awake()
        {
            Instance = this;
        }
        public void Start()
        {
            //prefabInstance = (PrefabInstanceSystem)ViewSystems.GetField("PrefabInstance");
            //prefabStates = (List<PrefabInstanceSystem.PrefabInstanceState>)Loader.Controller.prefabInstance.GetField("_PrefabStates");
            OnMatchStart = GameStartInit;
        }

        public Vector3 GetClosestTargetPos(Boolean ally, Boolean enemy, Boolean orb, Single time)
        {
            Single distance;
            Data_PlayerInfo player;
            return GetClosestTargetPos(ally, enemy, orb, time, out distance, out player);
        }
        public Vector3 GetClosestTargetPos(Boolean ally, Boolean enemy, Boolean orb, Single time, out Single distance, out Data_PlayerInfo player)
        {
            //orb = false;
            player = default(Data_PlayerInfo);
            Vector3 closest = Vector3.zero;
            //lane plane = new Plane(Vector3.up, Vector3.zero);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (plane.Raycast(ray, out distance))
            {
                Vector3 mousePosition = LocalPlayer.ID.ToGame().Position();// ray.GetPoint(distance);
                distance = Single.MaxValue;
                List<Data_PlayerInfo> search = new List<Data_PlayerInfo>();
                if (ally)
                    search.AddRange(LocalTeamData);
                if (enemy)
                    search.AddRange(EnemyTeamData);
                //object ListModelStateList = ListModelStateListField.GetValue(GameClientObject);
                //ModelState[] modelStates = (ModelState[])ModelStateListField.GetValue(ListModelStateList);
                var p = (CollisionLibrary.Pathfinder)GameClientObject.GetField("Pathfinding");
                foreach (Data_PlayerInfo tar in search)
                {
                    if (tar.IsDead
                        || tar.Health.Value.Current == 0
                        || (tar.Shield > 4 && distance != Single.MaxValue)
                        // || !modelStates.FirstOrDefault(m => m.Id == tar.ID.ToGame()).IsModelVisible
                        || (!tar.AllyTeam && AvoidTargetting(tar))
                        || tar.LocalPlayer
                        || tar.ID.ToGame().Get("IsUnHitable"))
                        continue;

                    var v = LocalPlayer.PredictedPosition2d(0);
                    var cansee = p.CanSee(mousePosition.ToGameplayVector2(), tar.PredictedPosition2d(0), 0.5f);
                    if (!cansee)
                        continue;
                    Single newDistance = Vector3.Distance(mousePosition, tar.ID.ToGame().Position());
                    if (newDistance < distance)
                    {
                        player = tar;
                        distance = newDistance;
                        closest = tar.PredictedPosition(time);
                    }
                }
                if (orb)
                {
                    foreach (ActiveObject current in ViewState.ActiveObjects.Values)
                    {
                        if (current.TypeId.ToString(GameData).Contains("RiteOf") && current.TypeId.ToString(GameData).EndsWith("Object"))
                        {
                            Vector2 orbPos = current.ObjectId.Get("Position");
                            Vector3 orbPos3d = new Vector3(orbPos.X, 0, orbPos.Y);
                            Single newDistance = Vector3.Distance(mousePosition, orbPos3d);
                            if (newDistance < distance)
                            {
                                player = new Data_PlayerInfo() { ID = new UIGameObjectId(current.ObjectId.Index, current.ObjectId.Generation) };
                                distance = newDistance;
                                closest = orbPos3d;
                            }
                            continue;
                        }
                    }
                }
                if (ViewState.ActiveObjects.Values.Count(o => o.TypeId.ToString(GameData).Contains("Training")) > 0)
                {
                    foreach (ActiveObject current in ViewState.ActiveObjects.Values)
                    {
                        if (current.TypeId.ToString(GameData).Contains("Arena") && current.TypeId.ToString(GameData).Contains("Dummy"))
                        {
                            if (current.ObjectId.Get("Health") <= 0.0f)
                                continue;
                            Vector2 orbPos = current.ObjectId.Get("Position");
                            Vector3 orbPos3d = new Vector3(orbPos.X, 0, orbPos.Y);
                            Single newDistance = Vector3.Distance(mousePosition, orbPos3d);
                            if (newDistance < distance)
                            {
                                player = new Data_PlayerInfo() { ID = new UIGameObjectId(current.ObjectId.Index, current.ObjectId.Generation) };
                                if (AvoidTargetting(player))
                                    continue;
                                distance = newDistance;
                                closest = orbPos3d;
                            }
                        }
                    }

                }
            }
            return closest;
        }
        public Boolean GetInjuredAlly(Single range, Single healing, Boolean self, out Single distance, out Data_PlayerInfo ally)
        {
            Boolean found = false;
            ally = default(Data_PlayerInfo);
            List<Data_PlayerInfo> search = new List<Data_PlayerInfo>();
            //object ListModelStateList = ListModelStateListField.GetValue(Loader.Controller.GameClientObject);
            //ModelState[] modelStates = (ModelState[])ModelStateListField.GetValue(ListModelStateList);
            Single maxMissingHealth = 0;
            distance = Single.MaxValue;
            foreach (Data_PlayerInfo tar in LocalTeamData)
            {
                Single missingHealth = tar.RecoveryHealth.Value.Current - tar.Health.Value.Current;
                if (tar.IsDead
                    || missingHealth < healing
                    //|| !modelStates.FirstOrDefault(m => m.Id == tar.ID.ToGame()).IsModelVisible
                    || (tar.LocalPlayer && !self))
                    continue;
                Single newDistance = Vector3.Distance(LocalPlayer.ID.ToGame().Position(), tar.ID.ToGame().Position());
                if (newDistance < range && missingHealth > maxMissingHealth)
                {
                    distance = newDistance;
                    ally = tar;
                    //location = tar.PredictedPosition(time);
                    found = true;
                }
            }
            return found;
        }
        public Boolean GetInDangerAlly(Double range, Boolean self, out Single distance, out Data_PlayerInfo ally)
        {
            Boolean found = false;
            ally = default(Data_PlayerInfo);
            List<Data_PlayerInfo> search = new List<Data_PlayerInfo>();
            //object ListModelStateList = ListModelStateListField.GetValue(Loader.Controller.GameClientObject);
            //ModelState[] modelStates = (ModelState[])ModelStateListField.GetValue(ListModelStateList);
            distance = Single.MaxValue;
            foreach (Data_PlayerInfo tar in LocalTeamData)
            {
                if (tar.IsDead
                    //|| !modelStates.FirstOrDefault(m => m.Id == tar.ID.ToGame()).IsModelVisible
                    || (tar.LocalPlayer && !self))
                    continue;
                if (InDanger(tar))
                {
                    ally = tar;
                    return true;
                }
            }
            return found;
        }

        public Data_PlayerInfo GetClosestEnemy()
        {
            Single distance = Single.MaxValue;
            Data_PlayerInfo closest = EnemyTeamData.FirstOrDefault();
            foreach (Data_PlayerInfo enemy in EnemyTeamData)
            {
                if (enemy.IsDead || (enemy.Shield > 4 && distance != Single.MaxValue))
                    continue;
                Single newDistance = Vector3.Distance(LocalPlayer.ID.ToGame().Position(), enemy.ID.ToGame().Position());
                if (newDistance < distance)
                {
                    distance = newDistance;
                    closest = enemy;
                }
            }
            return closest;
        }
        public Int32 EnemiesInRange(Single range)
        {
            Int32 count = 0;
            foreach (Data_PlayerInfo enemy in EnemyTeamData)
            {
                if (enemy.IsDead)
                    continue;
                Single distance = Vector3.Distance(LocalPlayer.ID.ToGame().Position(), enemy.ID.ToGame().Position());
                if (distance < range)
                    count++;
            }
            return count;
        }
        public Boolean InDanger(Data_PlayerInfo player)
        {
            foreach (ActiveObject current in ViewState.ActiveObjects.Values)
            {
                if (LocalPlayer.Team == current.Team)
                    continue;
                Vector2 StartPosition = current.ObjectId.Get("StartPosition");
                if (StartPosition == default(GameValue))
                {
                    Vector2 TargetsHit = current.ObjectId.Get("TargetsHit");
                    if (TargetsHit == default(GameValue))
                        continue;
                    var owner = ((GameObjectId)current.ObjectId.Get("Owner"));
                    var attackingEnemy = EnemyTeamData.FirstOrDefault(t => t.ID.ToGame() == owner);
                    if (Vector3.Distance(LocalPlayer.ID.ToGame().Position(), attackingEnemy.ID.ToGame().Position()) < 2.5f)
                        return true;
                }
                Vector2 Direction = current.ObjectId.Get("Direction");
                Single Range = current.ObjectId.Get("Range");
                Single Thickness = current.ObjectId.Get("SpellCollisionRadius");
                Vector2 EndPosition = StartPosition + Direction.Normalized * Range;
                Boolean intersect =
                    MathCore.GeometryMath.CircleVsThickLine(
                        new Vector2(player.ID.ToGame().Position().x, player.ID.ToGame().Position().z), 0.6f,
                        StartPosition, EndPosition, Thickness, true);
                if (intersect)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean HasBuff(Data_PlayerInfo player, String buffName)
        {
            return ViewState.ActiveObjects.Values.Count(
                ao =>
                    (buffName == ao.TypeId.ToString(GameData))
                    && ao.ObjectId.Get("Target") == player.ID.ToGame())
                > 0;
        }
        public Boolean AvoidTargetting(Data_PlayerInfo player)
        {
            return ViewState.ActiveObjects.Values.Count(
                ao =>
                    (CounterObjects.Contains(ao.TypeId.ToString(GameData))
                    || InvincibleObjects.Contains(ao.TypeId.ToString(GameData))
                    || "Incapacitate" == ao.TypeId.ToString(GameData))
                    && ao.ObjectId.Get("Target") == player.ID.ToGame())
                > 0;
        }

        public void Update()
        {
            if (ViewState != null && !ViewState.IsInLobby && !ViewState.IsLoading &&
                !ViewState.IsInCinematic)
            {
                Boolean inPractice = true;// ViewState.Huds.Count(a => a.Name == "Arena") == 0;
                Int32 currentRound = 0;
                //if (inPractice)
                //    currentRound = ViewState.HudStates.Find(a => a.Name == "Arena").Data.Get("CurrentRound");
                if (InGame == false || currentRound != previousRound)
                {
                    InGame = true;
                    previousRound = currentRound;
                    OnMatchStart();
                }
            }
            else
            {
                if (InGame)
                {
                    InGame = false;
                    //if (OnMatchEnd != null)
                    //    OnMatchEnd();
                }
            }
        }

        public void OnGUI()
        {
        }
    }
}
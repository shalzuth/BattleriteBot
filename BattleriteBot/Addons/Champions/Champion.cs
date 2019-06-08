using System;
using System.Collections.Generic;
using System.Linq;
using BloodGUI;
using Gameplay;
using Gameplay.GameObjects;
using UnityEngine;

namespace BattleriteBot.Addons.Champions
{
    public class Champion
    {
        public GameObjectId AbilityStateSystem;
        public List<GameValue> AbilityStateSystemAbilities;
        public List<GameValue> AbilityStateSystemAbilityGroups;
        public Ability AbilityLeft { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability1).Value; } }
        public Ability AbilityRight { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability2).Value; } }
        public Ability AbilitySpace { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability3).Value; } }
        public Ability AbilityQ { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability4).Value; } }
        public Ability AbilityE { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability5).Value; } }
        public Ability AbilityExLeft { get { return Abilities.First(a => a.Value.Data.InputFlags == (InputFlags.Ability1 | InputFlags.EX)).Value; } }
        public Ability AbilityExRight { get { return Abilities.First(a => a.Value.Data.InputFlags == (InputFlags.Ability2 | InputFlags.EX)).Value; } }
        public Ability AbilityExSpace { get { return Abilities.First(a => a.Value.Data.InputFlags == (InputFlags.Ability3 | InputFlags.EX)).Value; } }
        public Ability AbilityExQ { get { return Abilities.First(a => a.Value.Data.InputFlags == (InputFlags.Ability4 | InputFlags.EX)).Value; } }
        public Ability AbilityExE { get { return Abilities.First(a => a.Value.Data.InputFlags == (InputFlags.Ability5 | InputFlags.EX)).Value; } }
        public Ability AbilityR { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability6).Value; } }
        public Ability AbilityF { get { return Abilities.First(a => a.Value.Data.InputFlags == InputFlags.Ability7).Value; } }
        public class Ability
        {
            public String Name;
            public StateTableId SingleAbilityTable;
            public StateTableId GroupAbilityTable;
            public StateTableId AbilityTable { get { return Data.SharesCooldown ? GroupAbilityTable : SingleAbilityTable; } }
            public UInt16 Priority;
            public Boolean InRange(Double distance)
            {
                distance -= 0.5f;
                if (distance < Data.Range + Data.Radius && distance > Data.MinRange - Data.Radius)
                    return true;
                return false;
            }
            public Boolean IsReady
            {
                get
                {
                    if (API.Instance.LocalPlayer.Energy.Value.Current < Data.EnergyCost)
                        return false;
                    return /*Data.HasCharges ? /*AbilityTable.Get("Charges") > 0 :*/ Cooldown < 0;
                }
            }
            public Single EndCooldown { get { return 1.0f; } }// AbilityTable.Get("CooldownEndTime"); } }
            public Single Cooldown { get { return EndCooldown - API.Instance.Time; } }
            public AbilityData Data;
            ///public Boolean Cooldown { get { return AbilityTable.Get("CooldownEndTime") - Controller.GetTime(); } }
        }
        public void GetAbilities()
        {
            AbilityStateSystem = (GameObjectId)API.Instance.LocalPlayer.ID.ToGame().Get("AbilityStateSystem");
            AbilityStateSystemAbilities = AbilityStateSystem.GetStateList("Abilities");
            AbilityStateSystemAbilityGroups = AbilityStateSystem.GetStateList("AbilityGroups");
            foreach (var abilityStateTable in AbilityStateSystemAbilities)
            {
                var name = ((StateTableId)abilityStateTable).GetField<GameObjectTypeId>("#a").ToString(API.GameData).Replace("Ability", "");
                var ability = new Ability { Name = name, SingleAbilityTable = abilityStateTable };
                var group = 5;// ((StateTableId)abilityStateTable).Get("AbilityGroupIndex");
                if (group > -1)
                    ability.GroupAbilityTable = AbilityStateSystemAbilityGroups[group];
                ability.Data = AbilityData.ChampionAbilityData[GetType().Name].FirstOrDefault(a => a.Name == ability.Name);
                if (ability.Data != null)
                    Abilities.Add(name, ability);
            }
        }
        public Dictionary<String, Ability> Abilities = new Dictionary<String, Ability>();
        public Champion()
        {
            GetAbilities();
        }
        public static Champion GetChampion(String internalChampName)
        {
            if (internalChampName == "Vanguard") return new Bakko();
            /*if (internalChampName == "Alchemist") return new Lucie();
            else if (internalChampName == "Astronomer") return new Sirius();
            else if (internalChampName == "Glutton") return new Rook();
            else if (internalChampName == "Gunner") return new Jade();
            else if (internalChampName == "Harbinger") return new RuhKaan();
            else if (internalChampName == "Igniter") return new Ashka();
            else if (internalChampName == "Inhibitor") return new Varesh();
            else if (internalChampName == "Psychopomp") return new Poloma();
            else if (internalChampName == "Spearmaster") return new Shifu();
            else if (internalChampName == "Stormcaller") return new Ezmo();
            else if (internalChampName == "Vanguard") return new Bakko();
            else if (internalChampName == "Druid") return new Blossom();
            else if (internalChampName == "Swordmaster") return new Raigon();
            else if (internalChampName == "Seeker") return new Jumong();
            else if (internalChampName == "Ravener") return new Freya();
            else if (internalChampName == "BloodPriest") return new Pestilus();
            else if (internalChampName == "Herald") return new Oldur();
            else if (internalChampName == "Nomad") return new Taya();
            else if (internalChampName == "Ranid") return new Croak();
            else if (internalChampName == "MetalWarden") return new Destiny();
            else if (internalChampName == "MirrorMage") return new Zander();
            else if (internalChampName == "Paladin") return new Ulric();
            else if (internalChampName == "DragonLord") return new ShenRao();
            else if (internalChampName == "Stalker") return new Jamila();
            else if (internalChampName == "FrostMage") return new Alysia();
            else if (internalChampName == "Thorn") return new Thorn();
            else if (internalChampName == "Engineer") return new Iva();
            else if (internalChampName == "Inquisitor") return new Pearl();*/
            return new Champion();
        }
        public virtual InputFlags Combo(out Data_PlayerInfo target, out Vector3 targetPos) { target = new Data_PlayerInfo(); targetPos = new Vector3(); return new InputFlags(); }
    }
}
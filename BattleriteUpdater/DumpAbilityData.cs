using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleriteBot;

namespace BattleriteUpdater
{
    class DumpAbilityData
    {
        static BattleriteBot.Addons.Champions.AbilityData DumpAbility(Dictionary<String, Object> abilitySpell, List<Dictionary<String, Object>> go)
        {
            var abilityDataId = ((List<Object>)abilitySpell["Constants"]).Cast<Dictionary<String, Object>>().ToList().First(c => c["Name"].ToString() == "CastObject")["Value"].ToString();
            var abilityDataName = abilitySpell["Name"].ToString().Replace("Ability", "");
            var localization = ((List<Object>)abilitySpell["Constants"]).Cast<Dictionary<String, Object>>().ToList().FirstOrDefault(o => o["Name"].ToString() == "Name");
            if (localization == null)
            {
                var baseAbilityType = ((List<Object>)abilitySpell["BaseTypes"]).Last().ToString();
                var baseAbilityDataObject = go.First(o => o["Id"].ToString() == baseAbilityType);
                localization = ((List<Object>)baseAbilityDataObject["Constants"]).Cast<Dictionary<String, Object>>().ToList().FirstOrDefault(o => o["Name"].ToString() == "Name");
            }
            var localizationValue = localization["Value"].ToString();
            var abilityName = Localization.Get(localizationValue, true);

            var energy = ((List<Object>)abilitySpell["Constants"]).Cast<Dictionary<String, Object>>().ToList().FirstOrDefault(o => o["Name"].ToString() == "EnergyCost");
            var energyCost = (Double)0.0f;
            if (energy != null)
                energyCost = Double.Parse(energy["Value"].ToString());

            var charges = ((List<Object>)abilitySpell["Constants"]).Cast<Dictionary<String, Object>>().ToList().FirstOrDefault(o => o["Name"].ToString() == "MaxCharges");
            var maxCharges = false;
            if (charges != null)
                maxCharges = Int32.Parse(charges["Value"].ToString()) > 0;

            var abilityDataIdOverride = ((List<Object>)abilitySpell["Constants"]).Cast<Dictionary<String, Object>>().ToList().FirstOrDefault(c => c["Name"].ToString() == "AimPreviewCastObjectOverride");
            if (abilityDataIdOverride != null)
                abilityDataId = abilityDataIdOverride["Value"].ToString();
            var abilityDataObject = go.First(o => o["Id"].ToString() == abilityDataId);
            //var abilityDataObject = go.First(o => o["Name"].ToString() == abilityDataName);
            var abilityDataConstants = ((List<Object>)abilityDataObject["Constants"]).Cast<Dictionary<String, Object>>().ToList();
            var range = (Double)0.0f;
            var minRange = (Double)0.0f;
            var rangeElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "Range");
            if (rangeElement != null)
                range = Double.Parse(rangeElement["Value"].ToString());
            else
            {
                rangeElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "MaxRange");
                if (rangeElement != null)
                    range = Double.Parse(rangeElement["Value"].ToString());
                rangeElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "MinRange");
                if (rangeElement != null)
                    minRange = Double.Parse(rangeElement["Value"].ToString());
                if (range == 0.0f)
                {
                    rangeElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "Length");
                    if (rangeElement != null)
                        range = Double.Parse(rangeElement["Value"].ToString());
                }
            }

            var damage = (Double)0.0f;
            var damageElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "Damage");
            if (damageElement != null)
                damage = Double.Parse(damageElement["Value"].ToString());

            var radius = (Double)0.0f;
            var radiusElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "Radius");
            if (radiusElement != null)
                radius = Double.Parse(radiusElement["Value"].ToString());

            var heal = "None";
            var healElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "HealAmount");
            if (healElement != null)
                heal = healElement["Value"].ToString();
            else
            {
                healElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "InitHealing");
                if (healElement != null)
                    heal = healElement["Value"].ToString();
                healElement = abilityDataConstants.FirstOrDefault(c => c["Name"].ToString() == "Healing");
                if (healElement != null)
                    heal += " + " + healElement["Value"].ToString();

            }
            var data = new BattleriteBot.Addons.Champions.AbilityData
            {
                Name = abilityDataName,
                Range = range,
                MinRange = minRange,
                Radius = radius,
                HasCharges = maxCharges,
                EnergyCost = energyCost
            };
            return data;
        }
    }
}

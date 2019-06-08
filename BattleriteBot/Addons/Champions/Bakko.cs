using System;
using UnityEngine;
using BloodGUI;
using Gameplay;

namespace BattleriteBot.Addons.Champions
{
    public class Bakko : Champion
    {
        public override InputFlags Combo(out Data_PlayerInfo target, out Vector3 targetPos)
        {
            Single distance = 0.0f;
            targetPos = API.Instance.GetClosestTargetPos(false, true, true, 1.0f, out distance, out target);
            InputFlags inputFlags = 0;

            if (AbilityQ.IsReady && API.Instance.InDanger(API.Instance.LocalPlayer))
                inputFlags = AbilityQ.Data.InputFlags;
            else if (AbilityF.IsReady && AbilityF.InRange(distance) && distance > 4.5f)
                inputFlags = AbilityF.Data.InputFlags;
            else if (AbilitySpace.IsReady && AbilitySpace.InRange(distance) && distance > 4.5f)
                inputFlags = AbilitySpace.Data.InputFlags;
            else if (AbilityRight.IsReady && AbilityRight.InRange(distance))
                inputFlags = AbilityRight.Data.InputFlags;
            else if (AbilityE.IsReady && AbilityE.InRange(distance))
                inputFlags = AbilityE.Data.InputFlags;
            // else if (Abilities[""].IsReady && distance > 2.5f && distance < 4.0f)
            //     inputFlags = InputFlags.Ability5;
            else if (AbilityLeft.InRange(distance))
                inputFlags = AbilityLeft.Data.InputFlags;

            return inputFlags;
        }
    }
}

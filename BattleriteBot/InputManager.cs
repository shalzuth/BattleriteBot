using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityMain;
using InControl;

namespace BattleriteBot
{
    public static class InputManager
    {
        /*
        static InputControl MoveX = InputReader.ActiveDevice.GetControl(GameInputs.MoveXAxis.ControlType);
        static InputControl MoveY = InputReader.ActiveDevice.GetControl(GameInputs.MoveYAxis.ControlType);
        public static InputControl AimX = InputReader.ActiveDevice.GetControl(GameInputs.AimXAxis.ControlType);
        public static InputControl AimY = InputReader.ActiveDevice.GetControl(GameInputs.AimYAxis.ControlType);
        public static InputControl Move;

        public static InputControl LeftMouseButton = InputReader.ActiveDevice.GetControl(GameInputs.Ability1.ControlType);
        public static InputControl RightMouseButton = InputReader.ActiveDevice.GetControl(GameInputs.Ability2.ControlType);
        public static InputControl Space = InputReader.ActiveDevice.GetControl(GameInputs.Ability3.ControlType);
        public static InputControl Q = InputReader.ActiveDevice.GetControl(GameInputs.Ability4.ControlType);
        public static InputControl E = InputReader.ActiveDevice.GetControl(GameInputs.Ability5.ControlType);
        public static InputControl R = InputReader.ActiveDevice.GetControl(GameInputs.Ability6.ControlType);
        public static InputControl F = InputReader.ActiveDevice.GetControl(GameInputs.Ability7.ControlType);
        public static InputControl Mount = InputReader.ActiveDevice.GetControl(GameInputs.Mount.ControlType);
        public static InputControl Interrupt = InputReader.ActiveDevice.GetControl(GameInputs.Interrupt.ControlType);
        public static void Press(InputControl input)
        {
            var pendingTick = (ulong)input.GetField("pendingTick");
            input.Invoke("UpdateWithState", new object[] { true, pendingTick + 1, (float)0 });
        }
        public static void Walk(Single x, Single y)
        {
            //Debug.Log(MoveX);
            //var pendingTick = MoveX.GetField<UInt64>("pendingTick");
            //Move.Invoke("UpdateWithValue", new object[] { true, pendingTick + 1, (float)0 });
            //Debug.Log(AimX);
            //Debug.Log(AimY);
            //AimX.Invoke("UpdateWithValue", new object[] {x, pendingTick + 1, (float)0 });
            //AimY.Invoke("UpdateWithValue", new object[] {y, pendingTick + 1, (float)0 });
        }
        static InputManager()
        {
            /*
            var buttonMappings = InputReader.ActiveDevice.Profile.ButtonMappings.ToList();
            if (buttonMappings.Count(b => b.Handle == "Move") == 0)
            {
                var icm = new InputControlMapping
                {
                    Handle = "Move",
                    Target = InputControlType.Button16,
                    Source = JSONReaderProfile.GetControlSourceByString("k.B", false),
                };
                buttonMappings.Add(icm);
                InputReader.ActiveDevice.AddControl(icm.Target, icm.Handle);
                InputReader.ActiveDevice.Profile.SetField("<ButtonMappings>k__BackingField", 2, buttonMappings.ToArray());
            }
            if (buttonMappings.Count(b => b.Handle == "Stop") == 0)
            {
                var icm = new InputControlMapping
                {
                    Handle = "Stop",
                    Target = InputControlType.Button17,
                    Source = JSONReaderProfile.GetControlSourceByString("k.N", false)
                };
                buttonMappings.Add(icm);

                InputReader.ActiveDevice.AddControl(icm.Target, icm.Handle);
                InputReader.ActiveDevice.Profile.SetField("<ButtonMappings>k__BackingField", 2, buttonMappings.ToArray());
            }
            Move = InputReader.ActiveDevice.GetControl(GameInputs.Move.ControlType);

            int analogCount = InputReader.ActiveDevice.Profile.AnalogCount;
            for (int i = 0; i < analogCount; i++)
            {
                InputControlMapping inputControlMapping = InputReader.ActiveDevice.Profile.AnalogMappings[i];
                if (inputControlMapping.Handle.Contains("Aim"))
                    inputControlMapping.Raw = true;
            }

            analogCount = InputReader.ActiveDevice.Controls.Length;
            for (int i = 0; i < analogCount; i++)
            {
                var ic = InputReader.ActiveDevice.Controls[i];
                if (ic != null)
                {
                    if (ic.Handle.Contains("Aim"))
                        ic.Raw = true;
                }
            }
        }*/
    }
}
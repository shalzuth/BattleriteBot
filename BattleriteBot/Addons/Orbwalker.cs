using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityMain;
using BloodGUI;
using BloodGUI_Binding.Base;
using Gameplay;
using Gameplay.GameObjects;

namespace BattleriteBot.Addons
{
    public class Orbwalker : MonoBehaviour
    {
        object GlueInstance;
        public void SetGameInput(Gameplay.View.ViewState viewState, ref ClientInputData result)
        {
            Vector3 TargetPos = Vector3.zero;
            Data_PlayerInfo TargetHero;
            InputFlags inputFlags = 0;// InputReader.ReadInput();
            //var champion = Champions.Champion.GetChampion(API.Instance.ViewState.GetControlledObjectType().ToString(API.GameData));
            //inputFlags |= champion.Combo(out TargetHero, out TargetPos);
            inputFlags = InputReader.ReadInput();
            //CameraSettings cameraSettings = Camera.main.GetComponent<CameraSettings>();
            Vector2 targetPosition = new Vector2(TargetPos.x, TargetPos.z);
            Vector2 localPosition = API.Instance.LocalPlayer.PredictedPosition2d(0).ToUnityVector2();
            Vector2 targetDirection = targetPosition - localPosition;
            Single targetLength = targetDirection.magnitude;
            if (targetLength == 0)
                targetLength = 1.0f;
            targetDirection.Normalize();
            result = default(ClientInputData);
            //var p = API.Instance.GameClientObject.Get<CollisionLibrary.Pathfinder>("Pathfinding");
            var mousePos = UIHelper.GetMousePosition();
            var mousePos2d = new Vector2(mousePos.x, mousePos.y);
            result.InputDirection = ClampToMovable(mousePos2d);
            if (API.Instance.LocalPlayer.Team == 2)
                result.InputDirection *= -1;
            result.LocalSpaceInputDirection = MathCore.Vector2.Zero;
            result.AimDirection = targetDirection.ToGameplayVector2();
            result.AimLength = targetLength;
            ((JSONReaderProfile)InputReader.ActiveDevice.Profile).AimLengthFactor = targetLength;
            result.AimDirectionOffset = targetDirection.ToGameplayVector2();
            //InputManager.Walk(targetDirection.x, targetDirection.y);
            result.AimLengthOffset = targetLength;
            result.InputFlags = inputFlags;
            result.HoveredGameObject = GameObjectId.Empty;
            result.InterruptOnMove = false;
            //result.UseSmartCast = false;
            result.MousePosition = mousePos2d.ToGameplayVector2(); //targetPosition.ToGameplayVector2();
            result.MouseDelta = new Vector2(Input.GetAxis("mouse x"), Input.GetAxis("mouse y")).ToGameplayVector2();
            
            var GameToolCamera = MergedUnity.Glue.GUI.GUIGlobals.Glue.Get<GameToolCamera>(MergedUnity.Glue.LoadingState.Ready);
            result.CameraInput = (CameraInput)GlueInstance.Invoke("GetCameraInputs",new object[] { API.Instance.ViewState, GameToolCamera });

            Byte _NumOfInputs = (Byte)GlueInstance.GetField("_NumOfInputs");
            GlueInstance.SetField("_NumOfInputs", ++_NumOfInputs);
            result.NumOfInputs = _NumOfInputs++;
        }

        public MathCore.Vector2 ClampToMovable(Vector2 mousePos)
        {
            if (mousePos.magnitude < 1.0f)
                return new MathCore.Vector2(0, 0);
            var angle = mousePos.AngleBetween(Vector2.right);
            var tick = (int)(angle / 22.5f);
            if (Math.Abs(tick) >= 7)
                return new MathCore.Vector2(-1, 0).Normalized;
            else if (tick <= -5)
                return new MathCore.Vector2(-1, 1).Normalized;
            else if (tick <= -3)
                return new MathCore.Vector2(0, 1).Normalized;
            else if (tick <= -1)
                return new MathCore.Vector2(1, 1).Normalized;
            else if (tick <= 0)
                return new MathCore.Vector2(1, 0).Normalized;
            else if (tick <= 2)
                return new MathCore.Vector2(1, -1).Normalized;
            else if (tick <= 4)
                return new MathCore.Vector2(0, -1).Normalized;
            else if (tick <= 6)
                return new MathCore.Vector2(-1, -1).Normalized;
            return new MathCore.Vector2(0, 0);
        }
        Delegate InputDelegate;
        public void Start()
        {
            GlueInstance = API.Instance.GlueInstances[Type.GetType("MergedUnity.Glue.GameClientGlue,MergedUnity")].GetField("Instance");
            var GetGameClientInputField = GlueInstance.GetType().GetField("GetGameClientInput", BindingFlags.Public | BindingFlags.Instance);
            InputDelegate = Delegate.CreateDelegate(GetGameClientInputField.FieldType, this, GetType().GetMethod("SetGameInput", BindingFlags.Public | BindingFlags.Instance));
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                GlueInstance.SetField("GetGameClientInput", InputDelegate);
            if (Input.GetKeyUp(KeyCode.X))
                GlueInstance.SetField("GetGameClientInput", (Delegate)null);
        }
    }
}
using UnityEngine;

namespace BattleriteBot.Addons
{
    public class EnemyMouse : MonoBehaviour
    {
        public void OnGUI()
        {
            if (Camera.main == null)
                return;
            foreach (var current in API.Instance.EnemyTeamData)
            {
                var startPosition = (MathCore.Vector2)current.ID.ToGame().Get("Position");
                var targetDirection = (MathCore.Vector2)current.ID.ToGame().Get("AimDirection");
                var range = (float)current.ID.ToGame().Get("AimLength");
                var endPosition = startPosition + (targetDirection.Normalized * range);
                var screenPos = Camera.main.WorldToScreenPoint(new Vector3(endPosition.X, 0, endPosition.Y));
                GUI.Button(new Rect(screenPos.x, Screen.height - screenPos.y, 30, 30), "X");
            }
            /*foreach (var current in Loader.Controller._LocalTeamData)
            {
                var startPosition = (MathCore.Vector2)current.ID.ToGame().Get("Position");
                var targetDirection = (MathCore.Vector2)current.ID.ToGame().Get("AimDirection");
                var range = (float)current.ID.ToGame().Get("AimLength");
                var endPosition = startPosition + (targetDirection.Normalized * range);
                var screenPos = Camera.main.WorldToScreenPoint(new Vector3(endPosition.X, 0, endPosition.Y));
                GUI.Button(new Rect(screenPos.x, Screen.height - screenPos.y, 30, 30), "X");
            }*/
        }
    }
}
using UnityEngine;

namespace BattleriteBot.Addons
{
    public class DisplayEnergy : MonoBehaviour
    {
        private API.GameStart GameStart;

        public void Start()
        {
            GameStart = DisplayEnemyEnergy;
        }

        public void Update()
        {
            DisplayEnemyEnergy();
        }

        public void OnEnable()
        {
            API.Instance.OnMatchStart += GameStart;
            DisplayEnemyEnergy();
        }

        public void OnDisable()
        {
            API.Instance.OnMatchStart -= GameStart;
            DisplayEnemyEnergy(false);
        }

        public void DisplayEnemyEnergy()
        {
            DisplayEnemyEnergy(true);
        }

        public void DisplayEnemyEnergy(bool status)
        {
            if (API.Instance.HudBase != null)
            {
                for (var i = 0; i < API.Instance.HudBase.PlayersInfo.Settings.PlayerStyles.Count; i++)
                {
                    var style = API.Instance.HudBase.PlayersInfo.Settings.PlayerStyles[i];
                    style.ShowEnergy = status;
                    style.ShowRage = status;
                    style.ShowBlessings = status;
                    style.DontShowBarWhenFullHP = !status;
                    style.DontShowNameWhenFullHP = !status;
                    style.ShowNameAsHealth = status;
                    API.Instance.HudBase.PlayersInfo.Settings.PlayerStyles[i] = style;
                }
            }
            else
            {
                //Invoke("DisplayEnemyEnergy", 1);
            }
        }
    }
}
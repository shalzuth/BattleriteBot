using UnityEngine;

namespace BattleriteBot.Addons
{
    public class Zoom : MonoBehaviour
    {
        private readonly float defaultMaxZoom = 17.5f;
        private API.GameStart GameStart;
        private readonly float maxZoom = 95;

        public void Start()
        {
            GameStart = MaximizeZoom;
        }

        public void Update()
        {
            //MaximizeZoom();
        }

        public void OnEnable()
        {
            API.Instance.OnMatchStart += MaximizeZoom;
            MaximizeZoom();
        }

        public void OnDisable()
        {
            API.Instance.OnMatchStart -= MaximizeZoom;
            ChangeMaxZoom(defaultMaxZoom);
        }

        public void MaximizeZoom()
        {
            ChangeMaxZoom(maxZoom);
        }

        public void ChangeMaxZoom(float zoomVal)
        {
            //if (Loader.Controller.InGame)
            {
                API.Instance.ActiveCamera.ObjectId.Set("MaxZoom", zoomVal);
            }
        }

#if DEBUG
        public void OnGUI()
        {
        }
#endif
    }
}
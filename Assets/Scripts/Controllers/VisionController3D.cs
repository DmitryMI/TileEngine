using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    class VisionController3D : VisionController
    {
        private Hashtable _permanentLightSources = new Hashtable();
        private List<ILightInfo> _tempLightSources = new List<ILightInfo>();
        private Camera _camera;

        public override bool IsCellVisible(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override bool VisionProcessingEnabled { get; }

        [Obsolete("Try to avoid calling this function. Better use 3D-lighting system via LightBlocker spawning")]
        public override void SetBlock(int x, int y)
        {
            return;
        }

        [Obsolete("VisionMask is not present in the game.")]
        public override VisionMask GetMask(int x, int y)
        {
            return null;
        }

        public override void SetLightForOneFrame(ILightInfo info)
        {
            _tempLightSources.Add(info);
        }

        public override int SetLightContinuous(ILightInfo info)
        {
            return AddLightSource(info);
        }

        public override void RemoveLightById(int id)
        {
            RemoveLight(id);
        }

        public override void OnGameLoaded(IServerDataProvider controller)
        {
            _current = this;

            WasLoaded = true;
        }

        protected void LateUpdate()
        {
            if(IsReady)
                RefreshLighting();

            SetCameraView();
        }

        protected void SetCameraView()
        {
            if (ViewerPositionProvider == null)
            {
                Debug.LogWarning("No position provider found!");
                return;
            }

            float x = ViewerPositionProvider.X + ViewerPositionProvider.OffsetX;
            float y = ViewerPositionProvider.Y + ViewerPositionProvider.OffsetY;
            Vector3 position = Camera.main.transform.position;
            position.x = x;
            position.y = y;
            Camera.main.transform.position = position;
        }

        private void RefreshLighting()
        {
            _tempLightSources.Clear();
        }

        private int AddLightSource(ILightInfo info)
        {
            int id;
            do
            {
                id = Random.Range(1, Int32.MaxValue);
            } while (_permanentLightSources.ContainsKey(id));

            _permanentLightSources.Add(id, info);

            return id;
        }

        private void RemoveLight(int id)
        {
            _permanentLightSources.Remove(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public interface IVisionController
    {
        bool IsCellVisible(int x, int y);

        bool VisionProcessingEnabled { get; }

        void SetCameraPosition(Vector2 planeWorldPosition);

        void SetBlock(int x, int y);

        [Obsolete("VisionMask is not present in the game.")]
        VisionMask GetMask(int x, int y);

        void SetLightSource(LightSourceInfo info);
    }
}

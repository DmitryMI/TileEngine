using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using Assets.Scripts._Legacy;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public abstract class VisionController : Controller
    {
        [SerializeField]
        protected bool _DEBUG_VisionProcessingOn;

        protected Grid Grid;
        protected IPositionProvider ViewerPositionProvider;

        public abstract bool IsCellVisible(int x, int y);
        public abstract float GetCellBrightness(int x, int y);

        public abstract bool VisionProcessingEnabled { get; }

        public virtual void SetViewerPosition(IPositionProvider viewPositionProvider)
        {
            ViewerPositionProvider = viewPositionProvider;
        }

        [Obsolete("Try to avoid calling this function. Better use 3D-lighting system via LightBlocker spawning")]
        public abstract void SetBlock(int x, int y);

        [Obsolete("VisionMask is not present in the game.")]
        public abstract VisionMask GetMask(int x, int y);

        [Obsolete("This method will not work anymore. Use SetLightContinuous")]
        public abstract void SetLightForOneFrame(ILightInfo info);

        public abstract int SetLightContinuous(ILightInfo info);
        public abstract void RemoveLightById(int id);

        protected static VisionController _current;
        public static VisionController Current
        {
            get { return _current; }
        }
    }
}

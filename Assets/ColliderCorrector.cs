using Assets.Scripts.GameMechanics;
using Assets.Scripts.Objects.Mob.Humanoids;
using UnityEngine;

namespace Assets
{
    public class ColliderCorrector : MonoBehaviour
    {
        private AdvancedPolygonCollider.Script.MaskedPolygonCollider _advancedPolygonCollider;
        private Humanoid _humanoid;
        private bool _recalculated = false;

        // Start is called before the first frame update
        void Start()
        {
            _advancedPolygonCollider = GetComponent<AdvancedPolygonCollider.Script.MaskedPolygonCollider>();
            _humanoid = GetComponent<Humanoid>();
        }

        // Update is called once per frame
        void Update()
        {
            _advancedPolygonCollider.SetDirection(_humanoid.SpriteOrientation);
        }
    }
}

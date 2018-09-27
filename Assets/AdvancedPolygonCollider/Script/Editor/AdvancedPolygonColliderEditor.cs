using UnityEngine;
using UnityEditor;

using System.Collections;

namespace DigitalRuby.AdvancedPolygonCollider
{
    [CustomEditor(typeof(Assets.AdvancedPolygonCollider.Script.AdvancedPolygonCollider))]
    public class AdvancedPolygonColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Assets.AdvancedPolygonCollider.Script.AdvancedPolygonCollider c = target as Assets.AdvancedPolygonCollider.Script.AdvancedPolygonCollider;
            if (c != null)
            {
                EditorGUILayout.LabelField("Vertices: " + c.VerticesCount);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameMechanics;
using UnityEngine;

namespace Assets.AdvancedPolygonCollider.Script
{

    [RequireComponent(typeof(PolygonCollider2D))]
    [ExecuteInEditMode]
    class MaskedPolygonCollider : MonoBehaviour
    {
        [SerializeField] private Sprite _frontSprite;
        [SerializeField] private Sprite _rightSprite;
        [SerializeField] private Sprite _backSprite;
        [SerializeField] private Sprite _leftSprite;

        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Sprite _currentSprite;
        private Direction _lastDirection;

        public void SetDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Forward:
                    _currentSprite = _frontSprite;
                    break;
                case Direction.Backward:
                    _currentSprite = _backSprite;
                    break;
                case Direction.Left:
                    _currentSprite = _leftSprite;
                    break;
                case Direction.Right:
                    _currentSprite = _rightSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (_lastDirection != direction)
                dirty = true;

            _lastDirection = direction;
        }


        [Tooltip("Pixels with alpha greater than this count as solid.")]
        [Range(0, 254)]
        public byte AlphaTolerance = 20;

        [Tooltip("Points further away than this number of pixels will be consolidated.")]
        [Range(0, 64)]
        public int DistanceThreshold = 8;

        [Tooltip("Scale of the polygon.")]
        [Range(0.5f, 10.0f)]
        public float Scale = 1.0f;

        [Tooltip("Whether to decompse vertices into convex only polygons.")]
        public bool Decompose = false;

        [Tooltip("Whether to live update everything when in play mode. Typically for performance this can be false, " +
            "but if you plan on making changes to the sprite or parameters at runtime, you will want to set this to true.")]
        public bool RunInPlayMode = false;

        [Tooltip("True to use the cache, false otherwise. The cache is populated in editor and play mode and uses the most recent geometry " +
            "for a texture and rect regardless of other parameters. When ignoring the cache, values will not be added to the cache either. Cache is " +
            "only useful if you will be changing your sprite at run-time (i.e. animation)")]
        public bool UseCache;
        
        private PolygonCollider2D polygonCollider;
        private bool dirty;

        [SerializeField]
        [HideInInspector]
        private byte lastAlphaTolerance;

        [SerializeField]
        [HideInInspector]
        private float lastScale;

        [SerializeField]
        [HideInInspector]
        private int lastDistanceThreshold;

        [SerializeField]
        [HideInInspector]
        private bool lastDecompose;

        [SerializeField]
        [HideInInspector]
        private Sprite lastSprite;

        [SerializeField]
        [HideInInspector]
        private Rect lastRect = new Rect();

        [SerializeField]
        [HideInInspector]
        private Vector2 lastOffset = new Vector2(-99999.0f, -99999.0f);

        [SerializeField]
        [HideInInspector]
        private float lastPixelsPerUnit;

        [SerializeField]
        [HideInInspector]
        private bool lastFlipX;

        [SerializeField]
        [HideInInspector]
        private bool lastFlipY;

        private static readonly Dictionary<AdvancedPolygonCollider.CacheKey, List<Vector2[]>> cache = new Dictionary<AdvancedPolygonCollider.CacheKey, List<Vector2[]>>();

        [Tooltip("All the cached objects from the editor. Do not modify this data.")]
        [SerializeField]
        private List<AdvancedPolygonCollider.CacheEntry> editorCache = new List<AdvancedPolygonCollider.CacheEntry>();

        // private readonly AdvancedPolygonColliderAutoGeometry geometryDetector = new AdvancedPolygonColliderAutoGeometry();
        private readonly TextureConverter geometryDetector = new TextureConverter();

#if UNITY_EDITOR

        private Texture2D blackBackground;

#endif

        private void Awake()
        {
            if (Application.isPlaying)
            {
                // move editor cache to regular cache
                foreach (var v in editorCache)
                {
                    List<Vector2[]> list = new List<Vector2[]>();
                    cache[v.Key] = list;
                    foreach (AdvancedPolygonCollider.ArrayWrapper w in v.Value.List)
                    {
                        list.Add(w.Array);
                    }
                }
            }
        }

        private void Start()
        {

#if UNITY_EDITOR

            blackBackground = new Texture2D(1, 1);
            blackBackground.SetPixel(0, 0, new Color(0.0f, 0.0f, 0.0f, 0.8f));
            blackBackground.Apply();

#endif

            polygonCollider = GetComponent<PolygonCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void UpdateDirtyState()
        {
            
            if (_currentSprite != null)
            {
                if (lastOffset != _currentSprite.pivot)
                {
                    lastOffset = _currentSprite.pivot;
                    dirty = true;
                }
                if (lastRect != spriteRenderer.sprite.rect)
                {
                    lastRect = spriteRenderer.sprite.rect;
                    dirty = true;
                }
                if (lastPixelsPerUnit != _currentSprite.pixelsPerUnit)
                {
                    lastPixelsPerUnit = _currentSprite.pixelsPerUnit;
                    dirty = true;
                }
                if (lastFlipX != spriteRenderer.flipX)
                {
                    lastFlipX = spriteRenderer.flipX;
                    dirty = true;
                }
                if (lastFlipY != spriteRenderer.flipY)
                {
                    lastFlipY = spriteRenderer.flipY;
                    dirty = true;
                }
            }
            if (AlphaTolerance != lastAlphaTolerance)
            {
                lastAlphaTolerance = AlphaTolerance;
                dirty = true;
            }
            if (Scale != lastScale)
            {
                lastScale = Scale;
                dirty = true;
            }
            if (DistanceThreshold != lastDistanceThreshold)
            {
                lastDistanceThreshold = DistanceThreshold;
                dirty = true;
            }
            if (Decompose != lastDecompose)
            {
                lastDecompose = Decompose;
                dirty = true;
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (!RunInPlayMode)
                {
                    return;
                }
            }
            else if (!UseCache)
            {
                editorCache.Clear();
            }

            UpdateDirtyState();
            if (dirty)
            {
                RecalculatePolygon();
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.BeginGUI();
            GUI.color = Color.white;
            string text = " Vertices: " + VerticesCount + " ";
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(gameObject.transform.position);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.skin.box.normal.background = blackBackground;
            Rect rect = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(rect, text);
            UnityEditor.Handles.EndGUI();
        }

        private void AddEditorCache(ref PolygonParameters p, List<Vector2[]> list)
        {
            AdvancedPolygonCollider.CacheKey key = new AdvancedPolygonCollider.CacheKey();
            key.Texture = p.Texture;
            key.Rect = p.Rect;

            AdvancedPolygonCollider.CacheEntry e = new AdvancedPolygonCollider.CacheEntry();
            e.Key = key;
            e.Value = new AdvancedPolygonCollider.ListWrapper();
            e.Value.List = new List<AdvancedPolygonCollider.ArrayWrapper>();
            foreach (Vector2[] v in list)
            {
                AdvancedPolygonCollider.ArrayWrapper w = new AdvancedPolygonCollider.ArrayWrapper();
                w.Array = v;
                e.Value.List.Add(w);
            }

            for (int i = 0; i < editorCache.Count; i++)
            {
                if (editorCache[i].Key.Equals(key))
                {
                    editorCache.RemoveAt(i);
                    editorCache.Insert(i, e);
                    return;
                }
            }

            editorCache.Add(e);
        }

#endif

        public void RecalculatePolygon()
        {
            if (_currentSprite != null)
            {
                PolygonParameters p = new PolygonParameters();
                p.AlphaTolerance = AlphaTolerance;
                p.Decompose = Decompose;
                p.DistanceThreshold = DistanceThreshold;
                p.Rect = spriteRenderer.sprite.rect;
                p.Offset = _currentSprite.pivot;
                p.Texture = _currentSprite.texture;
                p.XMultiplier = (spriteRenderer.sprite.rect.width * 0.5f) / _currentSprite.pixelsPerUnit;
                p.YMultiplier = (spriteRenderer.sprite.rect.height * 0.5f) / _currentSprite.pixelsPerUnit;
                p.UseCache = UseCache;
                UpdatePolygonCollider(ref p);
            }
        }

        public void UpdatePolygonCollider(ref PolygonParameters p)
        {
            if (_currentSprite == null || _currentSprite.texture == null)
            {
                return;
            }

            dirty = false;
            List<Vector2[]> cached;

            if (Application.isPlaying && p.UseCache)
            {
                AdvancedPolygonCollider.CacheKey key = new AdvancedPolygonCollider.CacheKey();
                key.Texture = p.Texture;
                key.Rect = p.Rect;

                if (cache.TryGetValue(key, out cached))
                {
                    polygonCollider.pathCount = cached.Count;
                    for (int i = 0; i < cached.Count; i++)
                    {
                        polygonCollider.SetPath(i, cached[i]);
                    }

                    return;
                }
            }

            PopulateCollider(polygonCollider, ref p);
        }

        public int VerticesCount
        {
            get { return (polygonCollider == null ? 0 : polygonCollider.GetTotalPointCount()); }
        }

        /// <summary>
        /// Populate the vertices of a collider
        /// </summary>
        /// <param name="collider">Collider to setup vertices in.</param>
        /// <param name="p">Polygon creation parameters</param>
        public void PopulateCollider(PolygonCollider2D collider, ref PolygonParameters p)
        {
            try
            {
                if (p.Texture.format != TextureFormat.ARGB32 && p.Texture.format != TextureFormat.BGRA32 && p.Texture.format != TextureFormat.RGBA32 &&
                    p.Texture.format != TextureFormat.RGB24 && p.Texture.format != TextureFormat.Alpha8 && p.Texture.format != TextureFormat.RGBAFloat &&
                    p.Texture.format != TextureFormat.RGBAHalf && p.Texture.format != TextureFormat.RGB565)
                {
                    Debug.LogWarning("Advanced Polygon Collider works best with a non-compressed texture in ARGB32, BGRA32, RGB24, RGBA4444, RGB565, RGBAFloat or RGBAHalf format");
                }
                int width = (int)p.Rect.width;
                int height = (int)p.Rect.height;
                int x = (int)p.Rect.x;
                int y = (int)p.Rect.y;
                UnityEngine.Color[] pixels = p.Texture.GetPixels(x, y, width, height, 0);
                List<Vertices> verts = geometryDetector.DetectVertices(pixels, width, p.AlphaTolerance);
                int pathIndex = 0;
                List<Vector2[]> list = new List<Vector2[]>();

                for (int i = 0; i < verts.Count; i++)
                {
                    ProcessVertices(collider, verts[i], list, ref p, ref pathIndex);
                }

#if UNITY_EDITOR

                if (Application.isPlaying)
                {

#endif

                    if (p.UseCache)
                    {
                        AdvancedPolygonCollider.CacheKey key = new AdvancedPolygonCollider.CacheKey();
                        key.Texture = p.Texture;
                        key.Rect = p.Rect;
                        cache[key] = list;
                    }

#if UNITY_EDITOR

                }
                else if (p.UseCache)
                {
                    AddEditorCache(ref p, list);
                }

#endif

                //Debug.Log("Updated polygon.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error creating collider: " + ex);
            }
        }

        private List<Vector2[]> ProcessVertices(PolygonCollider2D collider, Vertices v, List<Vector2[]> list, ref PolygonParameters p, ref int pathIndex)
        {
            Vector2 offset = p.Offset;
            float flipXMultiplier = (spriteRenderer.flipX ? -1.0f : 1.0f);
            float flipYMultiplier = (spriteRenderer.flipY ? -1.0f : 1.0f);
            //float flipXMultiplier = 1.0f;
            //float flipYMultiplier = 1.0f;

            if (p.DistanceThreshold > 1)
            {
                v = SimplifyTools.DouglasPeuckerSimplify(v, p.DistanceThreshold);
            }

            if (p.Decompose)
            {
                List<List<Vector2>> points = BayazitDecomposer.ConvexPartition(v);
                for (int j = 0; j < points.Count; j++)
                {
                    List<Vector2> v2 = points[j];
                    for (int i = 0; i < v2.Count; i++)
                    {
                        float xValue = (2.0f * (((v2[i].x - offset.x) + 0.5f) / p.Rect.width));
                        float yValue = (2.0f * (((v2[i].y - offset.y) + 0.5f) / p.Rect.height));
                        v2[i] = new Vector2(xValue * p.XMultiplier * Scale * flipXMultiplier, yValue * p.YMultiplier * Scale * flipYMultiplier);
                    }
                    Vector2[] arr = v2.ToArray();
                    collider.pathCount = pathIndex + 1;
                    collider.SetPath(pathIndex++, arr);
                    list.Add(arr);
                }
            }
            else
            {
                collider.pathCount = pathIndex + 1;
                for (int i = 0; i < v.Count; i++)
                {
                    float xValue = (2.0f * (((v[i].x - offset.x) + 0.5f) / p.Rect.width));
                    float yValue = (2.0f * (((v[i].y - offset.y) + 0.5f) / p.Rect.height));
                    v[i] = new Vector2(xValue * p.XMultiplier * Scale * flipXMultiplier, yValue * p.YMultiplier * Scale * flipYMultiplier);
                }
                Vector2[] arr = v.ToArray();
                collider.SetPath(pathIndex++, arr);
                list.Add(arr);
            }

            return list;
        }
    }
}

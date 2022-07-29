using CustomEditors.CustomAttributes;
using UnityEngine;
using Utils.BaseUtils;

//https://answers.unity.com/questions/682285/editor-script-for-setting-the-sorting-layer-of-an.html

namespace Views.GameViews
{
    public class LineView : MonoBehaviour
    {
        private static readonly AnimationCurve DefaultCurve = AnimationCurve.Constant(0f, 0f, 1f);
        private static Material _defaultMaterial = null;

        private LineRenderer _lineRenderer;

        [field: SerializeField] 
        public Vector2[] Points { get; set; }

        private Vector2[] _drawnPoints;

        [field: SerializeField] 
        public int CornerVertices { get; set; } = 5;

        [field: SerializeField] 
        public int EndVertices { get; set; } = 5;
        
        [field: SerializeField]
        public Color LineColor { get; set; } = Color.white;
        
        [field: SerializeField] 
        public AnimationCurve WidthCurve { get; private set; }

        [field: SerializeField] 
        public Material LineMaterial { get; private set; } = null;

        [field: SerializeField] 
        public int ZIndex { get; set; } = -5;

        [field: SerializeField]
        [field: SortingLayer]
        public string SortingLayerName { get; private set; } = "TilemapOn1";
        
        [SerializeField] 
        private bool smooth = false;
        public bool Smooth
        {
            get => smooth; 
            set => smooth = value;
        }

        [SerializeField] 
        [DrawIf("smooth", true)]
        private float smoothness = 3.0f;
        public float Smoothness
        {
            get => smoothness; 
            set => smoothness = value;
        }
        
        [field: SerializeField]
        public bool Loop { get; set; } = false;

        private void Awake()
        {
            if (_defaultMaterial == null)
                _defaultMaterial = (Material)Resources.Load("Default-Line", typeof(Material));

            if (!CheckUtil.ElementInArray(SortingLayerName, SortingLayer.layers, layer => layer.name == SortingLayerName))
            {
                throw new MissingComponentException(
                    $"MissingComponentException: {GetType()} - Sorting Layer with name '{SortingLayerName}' not found!");
            }
            
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

            WidthCurve = CheckUtil.CheckNullAndReturnDefault(WidthCurve, DefaultCurve);
            LineMaterial = CheckUtil.CheckNullAndReturnDefault(LineMaterial,_defaultMaterial);
        }

        private void Start()
        {
            SetLineRendererAttributes();
            DoLinePositionChange();
        }

        public void SetLineRendererAttributes()
        {
            _lineRenderer.numCornerVertices = CornerVertices;
            _lineRenderer.numCapVertices = EndVertices;
            _lineRenderer.loop = Loop;
            _lineRenderer.material = LineMaterial;
            _lineRenderer.sortingLayerName = SortingLayerName;
            _lineRenderer.widthCurve = WidthCurve;
            var position = _lineRenderer.transform.position;
            position.Set(position.x, position.y, ZIndex);

            SetColorGradient();
        }

        public void DisableLine()
        {
            _lineRenderer.enabled = false;
        }
        
        public void EnableLine()
        {
            _lineRenderer.enabled = true;
        }
        
        private void SetColorGradient()
        {
            Gradient lineGradient = new Gradient();
            lineGradient.mode = GradientMode.Fixed;

            GradientColorKey[] colorKey = new GradientColorKey[2];
            colorKey[0].color = LineColor;
            colorKey[0].time = 0f;
            colorKey[1].color = LineColor;
            colorKey[1].time = 1f;

            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1f;
            
            lineGradient.SetKeys(colorKey, alphaKey);
            _lineRenderer.colorGradient = lineGradient;
        }

        public void DoLinePositionChange()
        {
            _drawnPoints = Points;
            if (smooth)
            {
                _drawnPoints = MathUtil.ToBezierCurve(_drawnPoints, smoothness);
            }
            
            Vector3[] vector3S = new Vector3[_drawnPoints.Length];
            for (int i = 0; i < _drawnPoints.Length; i++)
            {
                vector3S[i] = new Vector3(_drawnPoints[i].x, _drawnPoints[i].y, ZIndex);
            }

            _lineRenderer.positionCount = _drawnPoints.Length;
            _lineRenderer.SetPositions(vector3S);
        }
    }
}
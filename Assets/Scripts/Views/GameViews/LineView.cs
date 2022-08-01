using CustomEditors.CustomAttributes;
using UnityEngine;
using Utils.BaseUtils;

namespace Views.GameViews
{
    /// <summary>
    /// View for displaying a line on the map between given points. The line is drawn using a line renderer.
    /// It can be curved or not and provides other ways of changing the line's looks.
    /// </summary>
    public class LineView : MonoBehaviour
    {
        /// Default Animation curve for the width.
        private static readonly AnimationCurve DefaultCurve = AnimationCurve.Constant(0f, 0f, 1f);
        /// Default material.
        private static Material _defaultMaterial;

        /// Line renderer which is used to draw the line.
        private LineRenderer _lineRenderer;

        /// Points of the line.
        [field: SerializeField] 
        public Vector2[] Points { get; set; }

        /// Points after preprocessing has been done.
        private Vector2[] _drawnPoints;

        /// Whether the corner of the line is curved or not.
        [field: SerializeField] 
        public int CornerVertices { get; set; } = 5;
        
        /// Whether the end of the line is curved or not.
        [field: SerializeField] 
        public int EndVertices { get; set; } = 5;
        
        /// Color of the line.
        [field: SerializeField]
        public Color LineColor { get; set; } = Color.white;
        
        /// Width of the line as a curve.
        [field: SerializeField] 
        public AnimationCurve WidthCurve { get; private set; }
        
        /// Material of the line.
        [field: SerializeField] 
        public Material LineMaterial { get; private set; }
        
        /// Z-Index of the line (for 3D sorting).
        [field: SerializeField] 
        public int ZIndex { get; set; } = -5;

        /// Sort order of the line (for 2D sorting).
        [field: SerializeField]
        [field: SortingLayer]
        public string SortingLayerName { get; private set; } = "TilemapOn1";
        
        /// Should the line be smoothed? Uses bezier curves.
        [SerializeField] 
        private bool smooth;
        public bool Smooth
        {
            get => smooth; 
            set => smooth = value;
        }

        /// How smooth should the line be?
        [SerializeField] 
        [DrawIf("smooth", true)]
        private float smoothness = 3.0f;
        public float Smoothness
        {
            get => smoothness; 
            set => smoothness = value;
        }
        
        /// Should the line be closed?
        [field: SerializeField]
        public bool Loop { get; set; } = false;

        /// <summary>
        /// Initializes the line view.
        /// </summary>
        /// <exception cref="MissingComponentException"></exception>
        private void Awake()
        {
            if (_defaultMaterial == null)
                _defaultMaterial = (Material)Resources.Load("Default-Line", typeof(Material));

            // Check if the sorting layer is valid.
            if (!CheckUtil.ElementInArray(SortingLayerName, SortingLayer.layers, layer => layer.name == SortingLayerName))
            {
                throw new MissingComponentException(
                    $"MissingComponentException: {GetType()} - Sorting Layer with name '{SortingLayerName}' not found!");
            }
            
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

            WidthCurve = CheckUtil.CheckNullAndReturnDefault(WidthCurve, DefaultCurve);
            LineMaterial = CheckUtil.CheckNullAndReturnDefault(LineMaterial,_defaultMaterial);
        }

        /// <summary>
        /// Starts the line view.
        /// </summary>
        private void Start()
        {
            SetLineRendererAttributes();
            DoLinePositionChange();
        }

        /// <summary>
        /// Sets / Resets attributes of the line renderer. Must be called every time the line is changed.
        /// </summary>
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
        
        /// <summary>
        /// Disables the line.
        /// </summary>
        public void DisableLine()
        {
            _lineRenderer.enabled = false;
        }
        
        /// <summary>
        /// Enables the line.
        /// </summary>
        public void EnableLine()
        {
            _lineRenderer.enabled = true;
        }
        
        /// <summary>
        /// Sets the color of the line.
        /// </summary>
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

        /// <summary>
        /// Changes the position of the line.
        /// </summary>
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
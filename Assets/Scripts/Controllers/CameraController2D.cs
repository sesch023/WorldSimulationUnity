using System;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    /// <summary>
    /// Controller MonoBehaviour for a Camera. It requires a Player Input which messages the methods OnMoveHorizontal,
    /// OnMoveVertical and OnZoom. It then controls the required Camera Component, which is clamped to the TileMap given in
    /// the MapController of the MapManager.
    /// </summary>
    [RequireComponent(typeof(Camera), typeof(PlayerInput))]
    public class CameraController2D : MonoBehaviour
    {
        /// Camera Component of the CameraController.
        private Camera _camera;
        /// Transform of the Camera.
        private Transform _cameraTransform;
        
        /// Movement Speed of the Camera.
        [SerializeField] 
        private float cameraSpeed = 2f;
        
        /// Border around the Map that the Camera can move in.
        [SerializeField]
        private float borderCameraSpan = 0.2f;
        
        /// Maximum Zoom Level of the Camera (lowest absolute size of the camera field of view).
        [SerializeField]
        [Tooltip("The zoom level is the orthographic size of the Camera. This means that the maximum zoom must have a lower number" +
                 "than the minimum zoom level.")]
        private float maxZoom = 2.0f;
        
        /// Minimum Zoom Level of the Camera (highest absolute size of the camera field of view).
        [SerializeField]
        [Tooltip("The zoom level is the orthographic size of the Camera. This means that the maximum zoom must have a lower number" +
                 "than the minimum zoom level.")]
        private float minZoom = 200.0f;
        
        /// Half height of the Camera viewport.
        private float _halfHeight;
        /// Half width of the Camera viewport.
        private float _halfWidth;
        
        /// Lower border of the Camera viewport.
        private float _lowerBound;
        /// Upper border of the Camera viewport.
        private float _upperBound;
        /// Left border of the Camera viewport.
        private float _leftBound;
        /// Right border of the Camera viewport.
        private float _rightBound;
        
        // Base Position of the tilemap.
        private Vector2 _baseTilemapPos;
        /// Base Scale of the tilemap.
        private Vector2 _tilemapScale;
        
        /// Is a button pressed?
        private bool _pressed = false;
        /// Movement Vector of the Camera.
        private Vector3 _input = Vector3.zero;
        
        /// <summary>
        /// Start of the camera. It is called before the first frame update.
        /// </summary>
        /// <exception cref="MissingComponentException">If camera component is missing.</exception>
        /// <exception cref="ArgumentException">If MaxZoom Value is bigger than MinZoom</exception>
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _cameraTransform = gameObject.transform;

            if (_camera == null)
            {
                throw new MissingComponentException($"MissingComponentException: {GetType().Name}. CameraComponent missing!");
            }

            if (maxZoom > minZoom)
            {
                throw new ArgumentException($"ArgumentException: {GetType().Name}. MaxZoom Value is bigger than MinZoom!");
            }
            
            _baseTilemapPos = MapManager.Instance.MapController.TileMapPositionToGlobalPosition(new(0, 0));
            _tilemapScale = MapManager.Instance.MapController.TileMap.transform.localScale;
            CalculateLimits();
            
            _cameraTransform.position = new Vector3(_leftBound, _lowerBound, _cameraTransform.position.z);
        }
        
        /// <summary>
        /// Update of the camera. It is called every frame and updates the camera position if a button is pressed.
        /// </summary>
        private void Update()
        {
            if (_pressed)
            {
                var newPosition = _cameraTransform.position + (_input * (Time.deltaTime * _camera.orthographicSize * cameraSpeed));
                var clampedPosition = ClampedPosition(newPosition);
                _cameraTransform.position = clampedPosition;
            }
        }
        
        /// <summary>
        /// Method for the PlayerInput to call via messages when the MoveHorizontal button is pressed.
        /// </summary>
        /// <param name="input">Input value of the MoveHorizontal button.</param>
        public void OnMoveHorizontal(InputValue input)
        {
            _input.Set(input.Get<Single>(), _input.y, _input.z);
            // No movement?
            _pressed = _input != Vector3.zero;
        }
        
        /// <summary>
        /// Method for the PlayerInput to call via messages when the MoveVertical button is pressed.
        /// </summary>
        /// <param name="input">Input value of the MoveVertical button.</param>
        public void OnMoveVertical(InputValue input)
        {
            _input.Set(_input.x, input.Get<Single>(), _input.z);
            // No movement?
            _pressed = _input != Vector3.zero;
        }
        
        /// <summary>
        /// Method for the PlayerInput to call via messages when the Zoom button is pressed.
        /// </summary>
        /// <param name="input">Input value of the Zoom button.</param>
        public void OnZoom(InputValue input)
        {
            Single value = input.Get<Single>();
            if (value != 0.0)
            {
                var orthographicSize = _camera.orthographicSize;
                // Scale the change of the orthographic size with the zoom power of 1/3 of the orthographic size.
                float newSize = orthographicSize - (value * Mathf.Pow(orthographicSize, 1.0f / 3.0f));
                _camera.orthographicSize = Math.Clamp(newSize, maxZoom, minZoom);
                // Calculate the new bounds of the camera.
                CalculateLimits();
                // Clamp the position of the camera.
                ClampToLimits();
            }
        }
        
        /// <summary>
        /// Calculates the new bounds of the camera.
        /// </summary>
        private void CalculateLimits()
        {
            _halfHeight = _camera.orthographicSize;
            _halfWidth = _camera.aspect * _halfHeight;

            _lowerBound = _halfHeight - borderCameraSpan + _baseTilemapPos.y;
            _leftBound = _halfWidth - borderCameraSpan + _baseTilemapPos.x;
            _rightBound = _tilemapScale.x * MapManager.Instance.MapController.UnitMap.SizeX - _halfWidth + borderCameraSpan + _baseTilemapPos.x;
            _upperBound = _tilemapScale.y * MapManager.Instance.MapController.UnitMap.SizeY - _halfHeight + borderCameraSpan + _baseTilemapPos.y;
        }
        
        /// <summary>
        /// Clamps the position of the camera after it has been moved already.
        /// </summary>
        private void ClampToLimits()
        {
            _cameraTransform.position = ClampedPosition(_cameraTransform.position);
        }
        
        /// <summary>
        /// Calculates a clamped position of the camera.
        /// </summary>
        /// <param name="newPosition">Position to clamp.</param>
        /// <returns>Clamped Position.</returns>
        private Vector3 ClampedPosition(Vector3 newPosition)
        {
            return new Vector3(
                Math.Clamp(newPosition.x, _leftBound, _rightBound),
                Math.Clamp(newPosition.y, _lowerBound, _upperBound),
                newPosition.z
            );
        }
    }
}
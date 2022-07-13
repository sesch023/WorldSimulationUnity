using System;
using Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;
        private Transform _cameraTransform;
        
        [SerializeField] 
        private float cameraSpeed = 2f;
        [SerializeField]
        private float borderCameraSpan = 0.2f;
        [SerializeField]
        [Tooltip("The zoom level is the orthographic size of the Camera. This means that the maximum zoom must have a lower number" +
                 "than the minimum zoom level.")]
        private float maxZoom = 2.0f;
        [SerializeField]
        [Tooltip("The zoom level is the orthographic size of the Camera. This means that the maximum zoom must have a lower number" +
                 "than the minimum zoom level.")]
        private float minZoom = 200.0f;

        private float _halfHeight;
        private float _halfWidth;

        private float _lowerBound;
        private float _upperBound;
        private float _leftBound;
        private float _rightBound;

        private bool _pressed = false;
        private Vector3 _input = Vector3.zero;
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
            
            CalculateLimits();
            
            _cameraTransform.position = new Vector3(_leftBound, _lowerBound, _cameraTransform.position.z);
        }

        private void Update()
        {
            if (_pressed)
            {
                var newPosition = _cameraTransform.position + (_input * Time.deltaTime * _camera.orthographicSize * cameraSpeed);
                var clampedPosition = ClampedPosition(newPosition);
                _cameraTransform.position = clampedPosition;
            }
        }
        
        public void OnMoveHorizontal(InputValue input)
        {
            _input.Set(input.Get<Single>(), _input.y, _input.z);
            _pressed = _input != Vector3.zero;
        }
        
        public void OnMoveVertical(InputValue input)
        {
            _input.Set(_input.x, input.Get<Single>(), _input.z);
            _pressed = _input != Vector3.zero;
        }

        public void OnZoom(InputValue input)
        {
            Single value = input.Get<Single>();
            if (value != 0.0)
            {
                var orthographicSize = _camera.orthographicSize;
                float newSize = orthographicSize - (value * Mathf.Pow(orthographicSize, 1.0f / 3.0f));
                _camera.orthographicSize = Math.Clamp(newSize, maxZoom, minZoom);
                CalculateLimits();
                ClampToLimits();
            }
        }

        private void CalculateLimits()
        {
            _halfHeight = _camera.orthographicSize;
            _halfWidth = _camera.aspect * _halfHeight;
            
            _lowerBound = _halfHeight - borderCameraSpan;
            _leftBound = _halfWidth - borderCameraSpan;
            _rightBound = MapManager.Instance.MapController.UnitMap.SizeX - _halfWidth + borderCameraSpan;
            _upperBound = MapManager.Instance.MapController.UnitMap.SizeY - _halfHeight + borderCameraSpan;
        }

        private void ClampToLimits()
        {
            _cameraTransform.position = ClampedPosition(_cameraTransform.position);
        }

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
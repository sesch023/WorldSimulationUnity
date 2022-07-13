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
        private float cameraSpeed = 10f;

        [SerializeField]
        private float borderCameraSpan = 0.2f;

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
            
            _halfHeight = _camera.orthographicSize;
            _halfWidth = _camera.aspect * _halfHeight;

            _lowerBound = _halfHeight - borderCameraSpan;
            _leftBound = _halfWidth - borderCameraSpan;
            _rightBound = MapManager.Instance.MapController.UnitMap.SizeX - _halfWidth + borderCameraSpan;
            _upperBound = MapManager.Instance.MapController.UnitMap.SizeY - _halfHeight + borderCameraSpan;
            
            _cameraTransform.position = new Vector3(_leftBound, _lowerBound, _cameraTransform.position.z);
        }

        private void Update()
        {
            if (_pressed)
            {
                var newPosition = _cameraTransform.position + (_input * Time.deltaTime);
                var clampedPosition = new Vector3(
                    Math.Clamp(newPosition.x, _leftBound, _rightBound),
                    Math.Clamp(newPosition.y, _lowerBound, _upperBound),
                    newPosition.z
                );
                
                _cameraTransform.position = clampedPosition;
            }
        }
        
        public void OnMoveHorizontal(InputValue input)
        {
            _input.Set(input.Get<Single>() * cameraSpeed, _input.y, _input.z);
            _pressed = _input != Vector3.zero;
        }
        
        public void OnMoveVertical(InputValue input)
        {
            _input.Set(_input.x, input.Get<Single>() * cameraSpeed, _input.z);
            _pressed = _input != Vector3.zero;
        }
    }
}
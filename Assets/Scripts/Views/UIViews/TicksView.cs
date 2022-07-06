using System;
using Manager;
using TMPro;
using UnityEngine;

namespace Views
{
    public class TicksView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;

        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException("MissingComponentException: Illegal TicksView. Text Mesh missing!");
            }
        }

        private void Update()
        {
            _textMesh.text = "Tick: " + $"{TimeManager.Instance.PassedTicks:0}";
        }
    }
}
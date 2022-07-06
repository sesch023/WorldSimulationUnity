using System;
using Manager;
using TMPro;
using UnityEngine;

namespace Views
{
    public class TpsView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        private VerboseTimeManager _instance;
        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException("MissingComponentException: Illegal TPS View. Text Mesh missing!");
            }

            if (TimeManager.Instance.GetType() != typeof(VerboseTimeManager))
            {
                Debug.LogWarning("ApplicationStateWarning: No VerboseTimeManager found. TPSView is disabled.");
                gameObject.SetActive(false);
            }
            else
            {
                _instance = TimeManager.Instance as VerboseTimeManager;
            }
        }

        private void Update()
        {
            _textMesh.text = "TPS: " + $"{_instance.GetTps():0.00}";
        }
    }
}
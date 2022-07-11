using Manager;
using TMPro;
using UnityEngine;

namespace Views.UIViews
{
    public class TpsTargetView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;

        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException("MissingComponentException: Illegal TPS View. Text Mesh missing!");
            }
        }

        private void Update()
        {
            _textMesh.text = "TPS Target: " + $"{TimeManager.Instance.Tps:0.00}";
        }
    }
}
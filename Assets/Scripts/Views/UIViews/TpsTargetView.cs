using Manager;
using TMPro;
using UnityEngine;

namespace Views.UIViews
{
    /// <summary>
    /// UIView for displaying the target tps.
    /// </summary>
    public class TpsTargetView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;

        /// <summary>
        /// Starts the tps target view with a text mesh.
        /// </summary>
        /// <exception cref="MissingComponentException">If the text mesh is missing.</exception>
        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException($"MissingComponentException: {GetType()} - Illegal TPS View. Text Mesh missing!");
            }
        }

        /// <summary>
        /// Updates the text mesh with the current tps target.
        /// </summary>
        private void Update()
        {
            _textMesh.text = "TPS Target: " + $"{TimeManager.Instance.Tps:0.00}";
        }
    }
}
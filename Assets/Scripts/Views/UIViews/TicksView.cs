using Manager;
using TMPro;
using UnityEngine;

namespace Views.UIViews
{
    /// <summary>
    /// UIView for displaying the current ticks.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TicksView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        
        /// <summary>
        /// Starts the tick view with a text mesh.
        /// </summary>
        /// <exception cref="MissingComponentException">If the text mesh is missing.</exception>
        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException($"MissingComponentException: {GetType()} - Illegal TicksView. Text Mesh missing!");
            }
        }

        /// <summary>
        /// Updates the text mesh with the current ticks.
        /// </summary>
        private void Update()
        {
            _textMesh.text = "Tick: " + $"{TimeManager.Instance.PassedTicks:0}";
        }
    }
}
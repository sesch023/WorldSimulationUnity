using Manager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Views.UIViews
{
    /// <summary>
    /// UIView for the current tps.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TpsView : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        private VerboseTimeManager _instance;
        
        /// <summary>
        /// Displays the current tps the simulation is running at. This requires a VerboseTimeManager to be present
        /// in the simulation, otherwise it will not display anything since the basic TimeManager does not
        /// calculate average tps.
        /// </summary>
        /// <exception cref="MissingComponentException">If the text mesh is missing.</exception>
        void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                throw new MissingComponentException($"MissingComponentException: {GetType()} - Illegal TPS View. Text Mesh missing!");
            }

            if (TimeManager.Instance.GetType() != typeof(VerboseTimeManager))
            {
                LoggingManager.GetInstance().LogWarning("ApplicationStateWarning: No VerboseTimeManager found. TPSView is disabled.");
                gameObject.SetActive(false);
            }
            else
            {
                _instance = TimeManager.Instance as VerboseTimeManager;
            }
        }

        /// <summary>
        /// Updates the text mesh with the current tps.
        /// </summary>
        private void Update()
        {
            _textMesh.text = "TPS: " + $"{_instance.GetTps():0.00}";
        }
    }
}
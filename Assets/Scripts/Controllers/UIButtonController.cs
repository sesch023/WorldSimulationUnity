using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controllers
{
    /// <summary>
    /// Controller for Main UI elements.
    /// </summary>
    public class UIButtonController : MonoBehaviour
    {
        /// Select button for tile selection.
        [SerializeField]
        private Button selectButton;
        // Button for a tile group selection.
        [FormerlySerializedAs("heightLineButton")] [SerializeField]
        private Button tileGroupButton;
        // Button for a slope selection.
        [SerializeField]
        private Button slopeButton;
        
        /// <summary>
        /// When awake disables buttons depending on the current state.
        /// </summary>
        private void Awake()
        {
            if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.SelectTile)
                SelectButtonClick();
            else if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.GroupSelection)
                TileGroupButtonClick();
            else if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.SlopeSelection)
                SlopeButtonClick();
        }
        
        /// <summary>
        /// If the select button is clicked, the simulation manager will change to select tile mode and disable the select button,
        /// after enabling the other buttons.
        /// </summary>
        public void SelectButtonClick()
        {
            EnableAll();
            selectButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.SelectTile;
        }
        
        /// <summary>
        /// If the tile group button is clicked, the simulation manager will change to tile group selection mode and disable the tile group button,
        /// after enabling the other buttons.
        /// </summary>
        public void TileGroupButtonClick()
        {
            EnableAll();
            tileGroupButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.GroupSelection;
        }
        
        /// <summary>
        /// If the slope button is clicked, the simulation manager will change to slope selection mode and disable the slope button,
        /// after enabling the other buttons.
        /// </summary>
        public void SlopeButtonClick()
        {
            EnableAll();
            slopeButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.SlopeSelection;
        }
        
        /// <summary>
        /// Enables all buttons.
        /// </summary>
        private void EnableAll()
        {
            tileGroupButton.interactable = true;
            slopeButton.interactable = true;
            selectButton.interactable = true;
        }
    }
}
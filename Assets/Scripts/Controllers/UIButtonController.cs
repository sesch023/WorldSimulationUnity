using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class UIButtonController : MonoBehaviour
    {
        [SerializeField]
        private Button selectButton;
        [SerializeField]
        private Button heightLineButton;
        [SerializeField]
        private Button slopeButton;

        private void Awake()
        {
            if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.SelectTile)
                SelectButtonClick();
            else if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.ValleySelection)
                HeightLineButtonClick();
            else if(SimulationManager.Instance.CurrentInteractionMode == SimulationManager.InteractionMode.SlopeSelection)
                SlopeButtonClick();
        }
        
        public void SelectButtonClick()
        {
            EnableAll();
            selectButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.SelectTile;
        }

        public void HeightLineButtonClick()
        {
            EnableAll();
            heightLineButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.ValleySelection;
        }

        public void SlopeButtonClick()
        {
            EnableAll();
            slopeButton.interactable = false;
            SimulationManager.Instance.CurrentInteractionMode = SimulationManager.InteractionMode.SlopeSelection;
        }

        private void EnableAll()
        {
            heightLineButton.interactable = true;
            slopeButton.interactable = true;
            selectButton.interactable = true;
        }
    }
}
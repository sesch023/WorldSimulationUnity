using UnityEngine;

namespace Model.Map.UnitBehaviors
{
    [CreateAssetMenu(fileName = "Vegetation", menuName = "ScriptableObjects/Vegetation", order = 4)]
    public class VegetationBehavior : BaseUnitBehavior
    {
        [SerializeField]
        private int updateInterval = 10;

        private int current = 0;
        
        public override string GetBehaviorDescription()
        {
            return "Vegetation";
        }

        private bool CheckCondition(MapUnit unit)
        {
            return (unit.Temperature > 273.15f && unit.GroundMaterial.Soil > 0.1f);
        }
        
        public override void TriggerBehavior(MapUnit unit)
        {
            if (current == updateInterval)
            {
                current = 0;
            }
            else
            {
                current++;
            }
        }
    }
}
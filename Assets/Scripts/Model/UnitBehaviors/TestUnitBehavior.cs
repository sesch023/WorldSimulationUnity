using Manager;
using UnityEngine;

namespace Model.UnitBehaviors
{
    public class TestUnitBehavior : BaseUnitBehavior
    {
        public override string GetBehaviorDescription()
        {
            return "TestUnitBehavior";
        }

        public override void TriggerBehavior()
        {
            LoggingManager.GetInstance().LogInfo("TestUnitBehavior triggered!");
        }
    }
}
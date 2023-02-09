﻿using Manager;
using UnityEngine;

namespace Model.Map.UnitBehaviors
{
    public class TestUnitBehavior : BaseUnitBehavior
    {
        public override string GetBehaviorDescription()
        {
            return "TestUnitBehavior";
        }

        public override void TriggerBehavior(MapUnit unit)
        {
            LoggingManager.GetInstance().LogInfo("TestUnitBehavior triggered!");
        }
    }
}
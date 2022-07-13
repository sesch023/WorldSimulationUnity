using System.Collections.Generic;

namespace Model.UnitBehaviors
{
    public interface IBehaviorsRule
    {
        public bool CheckBehaviorCondition();
        public void TriggerBehaviors(MapUnit unit);
        public List<BaseUnitBehavior> GetBehaviors();
        public IBehaviorCondition GetCondition();
    }
}
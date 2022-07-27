using System.Collections.Generic;
using Model.Map;

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
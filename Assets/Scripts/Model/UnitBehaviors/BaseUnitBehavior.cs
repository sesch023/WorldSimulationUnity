using Base;

namespace Model.UnitBehaviors
{
    public abstract class BaseUnitBehavior : ConditionalUpdate, IUnitBehavior
    {
        public override bool Condition()
        {
            return true;
        }
    }
}
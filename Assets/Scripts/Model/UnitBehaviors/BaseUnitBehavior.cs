using Base;

namespace Model.UnitBehaviors
{
    public abstract class BaseUnitBehavior : IUpdatable, IUnitBehavior
    {
        public abstract string GetBehaviorDescription();
        public abstract void TriggerBehavior();

        public virtual void Update()
        {
            TriggerBehavior();
        }
    }
}
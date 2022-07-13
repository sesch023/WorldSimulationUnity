namespace Model.UnitBehaviors
{
    public interface IUnitBehavior
    {
        public string GetBehaviorDescription();
        public void TriggerBehavior();
    }
}
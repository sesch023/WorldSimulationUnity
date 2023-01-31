using Model.Map;

namespace Model.UnitBehaviors
{
    /// <summary>
    /// Interface for a unit behavior. A unit behavior changes the behavior of a unit and can be added
    /// under certain conditions.
    /// </summary>
    public interface IUnitBehavior
    {
        /// <summary>
        /// Gets a description of the behavior for display purposes.
        /// </summary>
        /// <returns>Description of the behavior.</returns>
        public string GetBehaviorDescription();
        
        /// <summary>
        /// Triggers the behavior.
        /// </summary>
        public void TriggerBehavior(MapUnit unit);
    }
}
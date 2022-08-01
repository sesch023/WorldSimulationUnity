using Base;

namespace Model.UnitBehaviors
{
    /// <summary>
    /// Base class for all unit behaviors. A unit behavior changes the behavior of a unit and can be added
    /// under certain conditions. The behavior is triggered in the update method.
    /// </summary>
    public abstract class BaseUnitBehavior : IUpdatable, IUnitBehavior
    {
        /// <summary>
        /// Gets a description of the behavior for display purposes.
        /// </summary>
        /// <returns>Description of the behavior.</returns>
        public abstract string GetBehaviorDescription();
        
        /// <summary>
        /// Triggers the behavior.
        /// </summary>
        public abstract void TriggerBehavior();

        /// <summary>
        /// Updates the behavior.
        /// </summary>
        public virtual void Update()
        {
            TriggerBehavior();
        }
    }
}
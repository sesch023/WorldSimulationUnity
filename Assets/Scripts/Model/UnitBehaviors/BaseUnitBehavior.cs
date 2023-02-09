using Base;
using UnityEngine;

namespace Model.Map.UnitBehaviors
{
    /// <summary>
    /// Base class for all unit behaviors. A unit behavior changes the behavior of a unit and can be added
    /// under certain conditions. The behavior is triggered in the update method.
    /// </summary>
    public abstract class BaseUnitBehavior : ScriptableObject, IUnitBehavior
    {
        public abstract string GetBehaviorDescription();

        public abstract void TriggerBehavior(MapUnit unit);
    }
}
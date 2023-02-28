using Model.Map;

namespace Views.GameViews
{
    /// <summary>
    /// Condition of a Map Unit.
    /// </summary>
    public interface IMapUnitCondition
    {
        /// <summary>
        /// Checks if the condition is met.
        /// </summary>
        /// <param name="unit">Unit to check.</param>
        /// <returns>True, if condition is met.</returns>
        public bool CheckCondition(MapUnit unit);
    }
}
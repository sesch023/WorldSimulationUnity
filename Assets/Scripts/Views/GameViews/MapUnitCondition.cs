using System;
using System.Collections.Generic;
using Model.Map;
using UnityEngine;
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ConstantNullCoalescingCondition

namespace Views.GameViews
{
    /// <summary>
    /// Delegate for a comparison function.
    /// </summary>
    public delegate bool CheckComparision(IComparable valueA, IComparable valueB);
    
    /// <summary>
    /// Delegate for returning a comparable value.
    /// </summary>
    public delegate IComparable GetUnitValue(MapUnit unit);
    
    /// <summary>
    /// Type of a comparison.
    /// </summary>
    [Serializable]
    public enum ComparisionType
    {
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        LessThan,
        LessThanOrEqual,
    }

    /// <summary>
    /// Types of values that can be put into a condition. This cannot be done in a dynamic way, since
    /// the unity editor does not support it.
    /// </summary>
    [Serializable]
    public enum UnitValueType
    {
        TileMaterialClay,
        TileMaterialSand,
        TileMaterialSoil,
        TileMaterialRock,
        TileMaterialGravel,
        TileHeight,
        TileWaterLevel,
        TileTemperature,
        TileHumidity
    }

    /// <summary>
    /// A condition for map units, which contain multiple hard and soft conditions.
    /// </summary>
    [Serializable]
    public class MapUnitCondition : IMapUnitCondition
    {
        /// <summary>
        /// All comparision types and their corresponding functions.
        /// </summary>
        private static Dictionary<ComparisionType, CheckComparision> _comparisionTypes = new()
        {
            {ComparisionType.GreaterThan, (a, b) => a.CompareTo(b) > 0},
            {ComparisionType.GreaterThanOrEqual, (a, b) => a.CompareTo(b) >= 0},
            {ComparisionType.Equal, (a, b) => a.CompareTo(b) == 0},
            {ComparisionType.LessThan, (a, b) => a.CompareTo(b) < 0},
            {ComparisionType.LessThanOrEqual, (a, b) => a.CompareTo(b) <= 0},
        };
        
        /// <summary>
        /// All value types and their corresponding getter functions.
        /// </summary>
        private static Dictionary<UnitValueType, GetUnitValue> _unitValueTypes = new()
        {
            {UnitValueType.TileMaterialClay, unit => unit.GroundMaterial.Clay},
            {UnitValueType.TileMaterialSand, unit => unit.GroundMaterial.Sand},
            {UnitValueType.TileMaterialSoil, unit => unit.GroundMaterial.Soil},
            {UnitValueType.TileMaterialRock, unit => unit.GroundMaterial.Rock},
            {UnitValueType.TileMaterialGravel, unit => unit.GroundMaterial.Gravel},
            {UnitValueType.TileHeight, unit => unit.Position.Elevation},
            {UnitValueType.TileWaterLevel, unit => unit.WaterLevel},
            {UnitValueType.TileTemperature, unit => unit.Temperature},
            {UnitValueType.TileHumidity, unit => unit.Humidity},
        };

        /// <summary>
        /// A condition between a dynamic value and a static float value.
        /// This class cannot have a abstract base class, since unity does not support serializing abstract classes.
        /// </summary>
        [Serializable]
        public class HardCondition : IMapUnitCondition
        {
            [SerializeField]
            private UnitValueType unitValueTypeLeft;
            [SerializeField]
            private ComparisionType comparisionType;
            [SerializeField]
            private float valueRight;

            /// <summary>
            /// Constructor for a hard condition.
            /// </summary>
            /// <param name="unitValueTypeLeft">Type of the left dynamic value.</param>
            /// <param name="comparisionType">Type of the comparision.</param>
            /// <param name="valueRight">Value of the static float value.</param>
            public HardCondition(UnitValueType unitValueTypeLeft, ComparisionType comparisionType, float valueRight)
            {
                this.unitValueTypeLeft = unitValueTypeLeft;
                this.comparisionType = comparisionType;
                this.valueRight = valueRight;
            }

            /// <summary>
            /// Checks if the condition is true for the given unit.
            /// </summary>
            /// <param name="unit">Given unit.</param>
            /// <returns>True, if condition is true.</returns>
            public bool CheckCondition(MapUnit unit)
            {
                var valueLeft = _unitValueTypes[unitValueTypeLeft](unit);
                return _comparisionTypes[comparisionType](valueLeft, valueRight);
            }
        }

        /// <summary>
        /// A condition between a dynamic value and multiple other dynamic values.
        /// This class cannot have a abstract base class, since unity does not support serializing abstract classes.
        /// </summary>
        [Serializable]
        public class SoftCondition : IMapUnitCondition
        {
            [SerializeField]
            private UnitValueType unitValueTypeLeft;
            [SerializeField]
            private ComparisionType comparisionType;
            [SerializeField]
            private UnitValueType[] unitValueTypeRight;
            
            /// <summary>
            /// Constructor for a soft condition.
            /// </summary>
            /// <param name="unitValueTypeLeft">Type of the left values to compare all right values to.</param>
            /// <param name="comparisionType">Type of the comparision.</param>
            /// <param name="unitValueTypeRight">Type of the right values to compare the single left value to.</param>
            public SoftCondition(UnitValueType unitValueTypeLeft, ComparisionType comparisionType, UnitValueType[] unitValueTypeRight)
            {
                this.unitValueTypeLeft = unitValueTypeLeft;
                this.comparisionType = comparisionType;
                this.unitValueTypeRight = unitValueTypeRight;
            }
            
            /// <summary>
            /// Checks if the condition is true for the given unit.
            /// </summary>
            /// <param name="unit">Given unit.</param>
            /// <returns>True, if condition is true.</returns>
            public bool CheckCondition(MapUnit unit)
            {
                var valueLeft = _unitValueTypes[unitValueTypeLeft](unit);
                bool ret = true;
                
                foreach (var unitValueType in unitValueTypeRight)
                {
                    var valueRight = _unitValueTypes[unitValueType](unit);
                    ret &= _comparisionTypes[comparisionType](valueLeft, valueRight);
                }
                
                return ret;
            }
        }
        
        [SerializeField]
        private SoftCondition[] softConditions;
        [SerializeField]
        private HardCondition[] hardConditions;

        /// <summary>
        /// Constructor for a map unit condition.
        /// </summary>
        public MapUnitCondition()
        {
            softConditions ??= Array.Empty<SoftCondition>();
            hardConditions ??= Array.Empty<HardCondition>();
        }
        
        /// <summary>
        /// Checks all Soft and Hard conditions.
        /// </summary>
        /// <param name="unit">Unit to check.</param>
        /// <returns>True, if all soft and hard conditions are true.</returns>
        public bool CheckCondition(MapUnit unit)
        {
            bool result = true;

            foreach (var con in softConditions)
            {
                result &= con.CheckCondition(unit);
            }

            foreach (var con in hardConditions)
            {
                result &= con.CheckCondition(unit);
            }
            
            return result;
        }
    }
}
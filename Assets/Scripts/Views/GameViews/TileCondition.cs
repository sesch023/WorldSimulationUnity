using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ConstantNullCoalescingCondition

namespace Views.GameViews
{
    public delegate bool CheckComparision(IComparable valueA, IComparable valueB);
    public delegate IComparable GetUnitValue(MapUnit unit);
    
    [Serializable]
    public enum ComparisionType
    {
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        LessThan,
        LessThanOrEqual,
    }

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
        TileHumidity,
    }
    
    [Serializable]
    public class TileCondition
    {
        private static Dictionary<ComparisionType, CheckComparision> _comparisionTypes = new()
        {
            {ComparisionType.GreaterThan, (a, b) => a.CompareTo(b) > 0},
            {ComparisionType.GreaterThanOrEqual, (a, b) => a.CompareTo(b) >= 0},
            {ComparisionType.Equal, (a, b) => a.CompareTo(b) == 0},
            {ComparisionType.LessThan, (a, b) => a.CompareTo(b) < 0},
            {ComparisionType.LessThanOrEqual, (a, b) => a.CompareTo(b) <= 0},
        };
        
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
    
        // Keine Generalisierung in Serializable Klassen möglich
        [Serializable]
        public class HardCondition
        {
            [SerializeField]
            private UnitValueType unitValueTypeLeft;
            [SerializeField]
            private ComparisionType comparisionType;
            [SerializeField]
            private float valueRight;

            public HardCondition(UnitValueType unitValueTypeLeft, ComparisionType comparisionType, float valueRight)
            {
                this.unitValueTypeLeft = unitValueTypeLeft;
                this.comparisionType = comparisionType;
                this.valueRight = valueRight;
            }

            public bool CheckCondition(MapUnit unit)
            {
                var valueLeft = _unitValueTypes[unitValueTypeLeft](unit);
                return _comparisionTypes[comparisionType](valueLeft, valueRight);
            }
        }

        // Keine Generalisierung in Serializable Klassen möglich
        [Serializable]
        public class SoftCondition
        {
            [SerializeField]
            private UnitValueType unitValueTypeLeft;
            [SerializeField]
            private ComparisionType comparisionType;
            [SerializeField]
            private UnitValueType[] unitValueTypeRight;
            
            public SoftCondition(UnitValueType unitValueTypeLeft, ComparisionType comparisionType, UnitValueType[] unitValueTypeRight)
            {
                this.unitValueTypeLeft = unitValueTypeLeft;
                this.comparisionType = comparisionType;
                this.unitValueTypeRight = unitValueTypeRight;
            }
            
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

        public TileCondition()
        {
            softConditions ??= Array.Empty<SoftCondition>();
            hardConditions ??= Array.Empty<HardCondition>();
        }
        
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
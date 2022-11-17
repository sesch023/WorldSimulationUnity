using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Map
{
    public enum GroundMaterialType
    {
        Soil,
        Rock,
        Sand,
        Gravel,
        Clay
    } 
        
    /// <summary>
    /// Represents the Ground makeup of the unit.
    /// </summary>
    public class MapUnitGroundMaterial
    {
        /// <summary>
        /// Creates a new MapUnitMaterial.
        /// </summary>
        /// <param name="soil">Relative amount of soil.</param>
        /// <param name="rock">Relative amount of rock.</param>
        /// <param name="sand">Relative amount of sand.</param>
        /// <param name="gravel">Relative amount of gravel.</param>
        /// <param name="clay">Relative amount of clay.</param>
        public MapUnitGroundMaterial(float soil = 0.0f, float rock = 1.0f, float sand = 0.0f, float gravel=0.0f, float clay=0.0f)
        {
            Material = new Dictionary<GroundMaterialType, float>();
            Material.Add(GroundMaterialType.Soil, soil);
            Material.Add(GroundMaterialType.Rock, rock);
            Material.Add(GroundMaterialType.Sand, sand);
            Material.Add(GroundMaterialType.Gravel, gravel);
            Material.Add(GroundMaterialType.Clay, clay);
        }

        public Dictionary<GroundMaterialType, float> Material { get; }
        public float Soil
        {
            get => Material[GroundMaterialType.Soil];
            set => Material[GroundMaterialType.Soil] = value;
        }

        public float Rock
        {
            get => Material[GroundMaterialType.Rock];
            set => Material[GroundMaterialType.Rock] = value;
        }
        public float Sand
        {
            get => Material[GroundMaterialType.Sand];
            set => Material[GroundMaterialType.Sand] = value;
        }
        
        public float Gravel
        {
            get => Material[GroundMaterialType.Gravel];
            set => Material[GroundMaterialType.Gravel] = value;
        }
        
        public float Clay
        {
            get => Material[GroundMaterialType.Clay];
            set => Material[GroundMaterialType.Clay] = value;
        }

        /// <summary>
        /// Gets the makeup of the ground normalized.
        /// </summary>
        /// <returns>Tuple of the normalized ground types.</returns>
        public (float soil, float rock, float sand, float gravel, float clay) GetNormalized()
        {
            float total = Soil + Rock + Sand + Gravel + Clay;
            return (Soil / total, Rock / total, Sand / total, Gravel / total, Clay / total);
        }

        public GroundMaterialType FindMostSignificantMaterial()
        {
            return Material.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            var (soil, rock, sand, gravel, clay) = GetNormalized();
            
            stringBuilder.Append($"Soil: {soil:0.00}\n");
            stringBuilder.Append($"Rock: {rock:0.00}\n");
            stringBuilder.Append($"Sand: {sand:0.00}\n");
            stringBuilder.Append($"Gravel: {gravel:0.00}\n");
            stringBuilder.Append($"Clay: {clay:0.00}\n");
            
            return stringBuilder.ToString();
        }
    }
}
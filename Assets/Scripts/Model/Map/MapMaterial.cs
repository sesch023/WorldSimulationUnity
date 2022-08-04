using System.Text;

namespace Model.Map
{
    /// <summary>
    /// Represents the Ground makeup of the unit.
    /// </summary>
    public struct MapUnitMaterial
    {
        /// <summary>
        /// Creates a new MapUnitMaterial.
        /// </summary>
        /// <param name="soil">Relative amount of soil.</param>
        /// <param name="rock">Relative amount of rock.</param>
        /// <param name="sand">Relative amount of sand.</param>
        public MapUnitMaterial(float soil=1.0f, float rock=1.0f, float sand=1.0f)
        {
            Soil = soil;
            Rock = rock;
            Sand = sand;
        }

        public float Soil { get; set; } 
        public float Rock { get; set; }
        public float Sand { get; set; }
            
        /// <summary>
        /// Gets the makeup of the ground normalized.
        /// </summary>
        /// <returns></returns>
        public (float soil, float rock, float sand) GetNormalized()
        {
            float total = Soil + Rock + Sand;
            return (Soil / total, Rock / total, Sand / total);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            var (soil, rock, sand) = GetNormalized();
            
            stringBuilder.Append($"Soil: {soil:0.00}\n");
            stringBuilder.Append($"Rock: {rock:0.00}\n");
            stringBuilder.Append($"Sand: {sand:0.00}\n");
            
            return stringBuilder.ToString();
        }
    }
}
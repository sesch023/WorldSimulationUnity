namespace Model.Generators
{
    public interface IGenerator
    {
        public float[,] GenerateElevation(int sizeX, int sizeY);
        public (int sizeX, int sizeY) LimitMapSizes(int sizeX, int sizeY);
    }
}
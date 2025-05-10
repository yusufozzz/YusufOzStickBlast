using System.Collections.Generic;

namespace GridSystem.Shapes
{
    public class ShapeStructure
    {
        public List<StickPoints> StickPoints { get; private set; }
        public int Width { get; }
        public int Height { get; }

        public ShapeStructure(List<StickPoints> stickPoints, int width, int height)
        {
            StickPoints = stickPoints;
            Width = width;
            Height = height;
        }
    }
}
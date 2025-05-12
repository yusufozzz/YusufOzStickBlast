using System;

namespace GridSystem.Shapes
{
    public class ShapeEvents
    {
        public static Action<Shape> OnShapePlaced;
        public static Action RefreshGridMap;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class ShapeStructure
    {
        public List<StickTransform> StickTransforms { get; }
        public int Width { get; }
        public int Height { get; }

        public ShapeStructure(List<StickTransform> stickTransforms, int width, int height)
        {
            StickTransforms = stickTransforms;
            Width = width;
            Height = height;
        }
    }
}
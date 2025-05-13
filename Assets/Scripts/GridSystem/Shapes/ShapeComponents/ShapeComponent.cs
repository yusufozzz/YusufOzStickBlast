using GameManagement;
using UnityEngine;

namespace GridSystem.Shapes.ShapeComponents
{
    public abstract class ShapeComponent : MonoBehaviour
    {
        protected Shape Shape;
        protected ShapeSettings ShapeSettings;

        public virtual void Initialize(Shape shape, ShapeSettings shapeSettings)
        {
            ShapeSettings = shapeSettings;
            Shape = shape;
        }
    }
}
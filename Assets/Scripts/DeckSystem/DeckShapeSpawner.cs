using GridSystem;
using GridSystem.GridSpecific;
using GridSystem.Shapes;
using UnityEngine;

namespace DeckSystem
{
    public class DeckShapeSpawner: MonoBehaviour
    {
        [SerializeField]
        private Shape[] shapes;
        
        private GridPlacement _gridPlacement;
        private GridPlacement GridPlacement => _gridPlacement ??= FindObjectOfType<GridPlacement>();
        
        public Shape[] GenerateDeck(int deckSlotsLength)
        {
            Shape[] deckShapes = new Shape[deckSlotsLength];
            for (int i = 0; i < deckSlotsLength; i++)
            {
                var placeableShape = RandomShape();
                var shape = Instantiate(placeableShape, transform);
                deckShapes[i] = shape;
            }

            return deckShapes;
        }
        
        private Shape RandomShape()
        {
            int randomIndex = Random.Range(0, shapes.Length);
            return shapes[randomIndex];
        }
        
        private Shape FindPlaceableShapeFromShapeArray()
        {
            foreach (var shape in shapes)
            {
                if (GridPlacement.CanShapeBePlacedUsingStickPoints(shape.StickPoints))
                {
                    return shape;
                }
            }

            return null;
        }
    }
}
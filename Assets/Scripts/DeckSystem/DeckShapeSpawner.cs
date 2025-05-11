using GridSystem.Shapes;
using UnityEngine;

namespace DeckSystem
{
    public class DeckShapeSpawner: MonoBehaviour
    {
        [SerializeField]
        private Shape[] shapes;
        
        public Shape[] GenerateDeck(int deckSlotsLength)
        {
            Shape[] deckShapes = new Shape[deckSlotsLength];
            for (int i = 0; i < deckSlotsLength; i++)
            {
                int randomIndex = Random.Range(0, shapes.Length);
                deckShapes[i] = Instantiate(shapes[randomIndex]);
            }

            return deckShapes;
        }
    }
}
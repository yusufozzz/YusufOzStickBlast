using System.Collections.Generic;
using UnityEngine;

namespace GridSystem.Shapes
{
    public class Shape : MonoBehaviour
    {
        [SerializeField]
        private List<StickPoints> stickPoints = new List<StickPoints>();

        public void SetStickPoints(List<StickPoints> points)
        {
            stickPoints = points;
        }

        public void ClearSticks()
        {
            
        }

        public void Initialize(Transform deckSlot)
        {
            transform.position = deckSlot.position;
            transform.rotation = deckSlot.rotation;
        }
    }
}
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
    }
}
using UnityEngine;

namespace Utilities
{
    public static class VectorUtilities
    {
        public static float GetDistance(this Vector3 from, Vector3 to)
        {
            from.y = 0;
            to.y = 0;
            return (from - to).sqrMagnitude;
        }
        
        public static float GetDistance2D(this Vector3 from, Vector3 to)
        {
            from.z = 0;
            to.z = 0;
            return (from - to).magnitude;
        }
    }
}
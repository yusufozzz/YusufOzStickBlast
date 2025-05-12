#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GridSystem.Dots
{
    public class DotGizmos : MonoBehaviour
    {
        private Dot _dot;

        [SerializeField]
        private Color textColor = Color.green;

        private void OnEnable()
        {
            // Ensure we have a reference to the Dot component
            if (_dot == null)
            {
                _dot = GetComponent<Dot>();
            }
        }

        private void OnDrawGizmos()
        {
            if (_dot == null) return;

            int occupiedLineCount = CountOccupiedLines();

            // Draw only if there are occupied lines
            Gizmos.color = textColor;

            // Position the text above the dot
            Vector3 textPosition = transform.position;

            // Create a style for the text
            GUIStyle style = new GUIStyle();
            style.normal.textColor = textColor;
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            // Draw the text showing the occupied line count
            Handles.Label(textPosition, occupiedLineCount.ToString(), style);
        }

        private int CountOccupiedLines()
        {
            if (_dot == null || _dot.Lines == null)
                return 0;

            int count = 0;
            foreach (var line in _dot.Lines)
            {
                if (line != null && line.IsOccupied)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
#endif
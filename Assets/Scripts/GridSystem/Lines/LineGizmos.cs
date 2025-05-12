#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GridSystem.Lines
{
    public class LineGizmos : MonoBehaviour
    {
        private Line _line;
        
        [SerializeField]
        private Color textColor = Color.blue;
   
        private void OnEnable()
        {
            // Ensure we have a reference to the Line component
            if (_line == null)
            {
                _line = GetComponent<Line>();
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_line == null) return;
            
            int memberCount = _line.MemberOfCompletedSquares.Count;
            Gizmos.color = textColor;
                
            // Position the text directly on the line
            Vector3 textPosition = transform.position;
                
            // Create a style for the text
            GUIStyle style = new GUIStyle();
            style.normal.textColor = textColor;
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
                
            // Draw the text showing the count
            Handles.Label(textPosition, memberCount.ToString(), style);
        }
    }
}
#endif
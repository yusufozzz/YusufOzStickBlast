using UnityEngine;

namespace GridSystem
{
    public class GridGizmos : MonoBehaviour
    {
        [SerializeField] 
        private GridMapper gridMapper;
        
        [SerializeField]
        private bool showGizmos = true;
        
        [SerializeField]
        private Color emptyLineColor = new Color(0f, 1f, 0f, 0.5f); // Yarı-saydam yeşil
        
        [SerializeField]
        private Color occupiedLineColor = new Color(1f, 0f, 0f, 0.5f); // Yarı-saydam kırmızı
        
        [SerializeField] 
        private float lineThickness = 0.2f;
        
        [SerializeField]
        private bool showOnlyEmptyLines = false; // Sadece boş lineları göster
        
        private void OnDrawGizmos()
        {
            if (!showGizmos || gridMapper == null)
                return;
            
            DrawGridLinesGizmos();
        }
        
        private void DrawGridLinesGizmos()
        {
            // GridMapper'ın linelarına erişim sağla
            Line[,] horizontalLines = gridMapper.GetHorizontalLines();
            Line[,] verticalLines = gridMapper.GetVerticalLines();
            
            if (horizontalLines == null || verticalLines == null)
                return;
            
            int horizontalWidth = horizontalLines.GetLength(0);
            int horizontalHeight = horizontalLines.GetLength(1);
            
            int verticalWidth = verticalLines.GetLength(0);
            int verticalHeight = verticalLines.GetLength(1);
            
            // Yatay çizgileri çiz
            for (int y = 0; y < horizontalHeight; y++)
            {
                for (int x = 0; x < horizontalWidth; x++)
                {
                    Line line = horizontalLines[x, y];
                    if (line != null)
                    {
                        // Eğer sadece boş lineları gösteriyorsak ve bu line dolu ise atla
                        if (showOnlyEmptyLines && line.IsOccupied)
                            continue;
                            
                        DrawLineGizmo(line, true);
                    }
                }
            }
            
            // Dikey çizgileri çiz
            for (int y = 0; y < verticalHeight; y++)
            {
                for (int x = 0; x < verticalWidth; x++)
                {
                    Line line = verticalLines[x, y];
                    if (line != null)
                    {
                        // Eğer sadece boş lineları gösteriyorsak ve bu line dolu ise atla
                        if (showOnlyEmptyLines && line.IsOccupied)
                            continue;
                            
                        DrawLineGizmo(line, false);
                    }
                }
            }
        }
        
        private void DrawLineGizmo(Line line, bool isHorizontal)
        {
            // Line doğru şekilde oluşturulmuş mu kontrol et
            if (line == null) return;
            
            Vector3 position = line.transform.position;
            
            // Çizgi rengini belirle
            Gizmos.color = line.IsOccupied ? occupiedLineColor : emptyLineColor;
            
            // Çizgi boyutunu hesapla (Grid Generator ile benzer)
            float length = 0.96f; // Grid Generator'daki spacing değeri
            
            // Çizgi başlangıç ve bitiş noktaları
            Vector3 start, end;
            
            if (isHorizontal)
            {
                start = position + new Vector3(-length / 2f, 0f, 0f);
                end = position + new Vector3(length / 2f, 0f, 0f);
            }
            else
            {
                start = position + new Vector3(0f, -length / 2f, 0f);
                end = position + new Vector3(0f, length / 2f, 0f);
            }
            
            // Çizgiyi çiz
            Gizmos.DrawLine(start, end);
            
            // Çizgi kalınlığını göstermek için kutu çiz
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f);
            
            Vector3 thickness = perpendicular * lineThickness * 0.5f;
            
            Vector3[] corners = new Vector3[4]
            {
                start + thickness,
                start - thickness,
                end - thickness,
                end + thickness
            };
            
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
        
        // Runtime'da gizmos toggle için
        public void ToggleGizmos(bool show)
        {
            showGizmos = show;
        }
        
        // Sadece boş lineları gösterme toggle
        public void ToggleOnlyEmptyLines(bool onlyEmpty)
        {
            showOnlyEmptyLines = onlyEmpty;
        }
        
        // Play Mode'da da çalışması için
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && showGizmos && gridMapper != null)
            {
                DrawGridLinesGizmos();
            }
        }
    }
}
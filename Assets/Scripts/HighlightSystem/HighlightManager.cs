using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace HighlightSystem
{
    public class HighlightManager: ManagerBase
    {
        [SerializeField]
        private Highlight prefab;
        
        private readonly Queue<Highlight> _highlightPool = new ();
        private readonly HashSet<Highlight> _activeHighlights = new ();
        
        public Highlight GetHighlight()
        {
            if (_highlightPool.Count > 0)
            {
                var highlight = _highlightPool.Dequeue();
                highlight.gameObject.SetActive(true);
                return highlight;
            }
            else
            {
                var highlight = Instantiate(prefab, transform);
                highlight.gameObject.SetActive(true);
                _activeHighlights.Add(highlight);
                return highlight;
            }
        }
        
        private void ReturnHighlight(Highlight highlight)
        {
            highlight.gameObject.SetActive(false);
            _highlightPool.Enqueue(highlight);
        }

        public void ClearAllHighlights()
        {
            foreach (var highlight in _activeHighlights)
            {
                highlight.StopAnimation();
                ReturnHighlight(highlight);
            }
        }
    }
}
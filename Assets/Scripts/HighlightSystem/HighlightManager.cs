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
            var highlight = _highlightPool.Count > 0 ? _highlightPool.Dequeue() : Instantiate(prefab, transform);
            _activeHighlights.Add(highlight);
            highlight.gameObject.SetActive(true);
            return highlight;
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
            _activeHighlights.Clear();
        }
    }
}
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
        
        public override void SetUp()
        {
            base.SetUp();
            InitializeHighlightPool();
        }
        
        private void InitializeHighlightPool()
        {
            for (int i = 0; i < 10; i++)
            {
                var highlight = Instantiate(prefab,transform);
                highlight.gameObject.SetActive(false);
                _highlightPool.Enqueue(highlight);
            }
        }
        
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
                return highlight;
            }
        }
        
        public void ReturnHighlight(Highlight highlight)
        {
            highlight.gameObject.SetActive(false);
            _highlightPool.Enqueue(highlight);
        }
    }
}
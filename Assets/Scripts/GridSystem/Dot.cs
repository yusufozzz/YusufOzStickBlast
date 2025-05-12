using GridSystem.Visuals;
using UnityEngine;

namespace GridSystem
{
    public class Dot : MonoBehaviour
    {
        private ItemVisual _itemVisual;
        public void SetUp()
        {
            _itemVisual ??= GetComponent<ItemVisual>();
        }

        public void Preview(Color color)
        {
            if (_itemVisual.IsPreviewed) return;
            _itemVisual.Preview(color);
        }

        public void ResetPreview()
        {
            if (!_itemVisual.IsPreviewed) return;
            _itemVisual.ResetPreview();
        }

        public void SetColor(Color color)
        {
            _itemVisual.SetColor(color);
        }
    }
}
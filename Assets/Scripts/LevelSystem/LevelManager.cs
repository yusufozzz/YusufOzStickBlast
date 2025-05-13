using GameManagement;
using UnityEngine;

namespace LevelSystem
{
    public class LevelManager: ManagerBase
    {
        [SerializeField]
        private Color[] colorList;

        public Color ActiveShapeColor { get; private set; }
        
        public override void SetUp()
        {
            base.SetUp();
            SetActiveShapeColor();
        }

        private void SetActiveShapeColor()
        {
            if (colorList.Length == 0)
            {
                Debug.LogError("Color list is empty. Cannot set active shape color.");
                return;
            }

            int randomIndex = Random.Range(0, colorList.Length);
            ActiveShapeColor = colorList[randomIndex];
        }
    }
}
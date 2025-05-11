using GameManagement;
using GridSystem;
using GridSystem.Shapes;
using UnityEngine;

namespace DeckSystem
{
    public class ShapePlacementValidator: MonoBehaviour
    {
        private GridManager GridManager => ManagerType.Grid.GetManager<GridManager>();
        public bool CanShapeBePlacedAnywhere(Shape arg)
        {
            var lineGrid = GridManager.GridGenerator.GetLineGrid();
            return false;
        }
    }
}
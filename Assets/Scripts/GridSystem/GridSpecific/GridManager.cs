using DeckSystem;
using GameManagement;
using LevelSystem;
using UnityEngine;

namespace GridSystem.GridSpecific
{
    public class GridManager : ManagerBase
    {
        [field: SerializeField]
        public GridSettingsSo GridSettings { get; private set; }

        [field: SerializeField]
        public GridGenerator GridGenerator { get; private set; }

        [field: SerializeField]
        public GridSquareChecker GridSquareChecker { get; private set; }

        private DeckManager DeckManager => ManagerType.Deck.GetManager<DeckManager>();
        private LevelManager LevelManager => ManagerType.Level.GetManager<LevelManager>();

        [field: SerializeField]
        public GridPlacement GridPlacement { get; private set; }

        public override void SetUp()
        {
            base.SetUp();
            GridGenerator.Generate(GridSettings);
            GridPlacement.Initialize(GridGenerator.HorizontalLines, GridGenerator.VerticalLines);
            GridSquareChecker.Initialize(GridGenerator.Squares, LevelManager.ActiveShapeColor);
            DeckManager.GenerateDeck();
        }
    }
}
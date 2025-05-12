using DeckSystem;
using GameManagement;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : ManagerBase
    {
        [field: SerializeField]
        public GridSettingsSo GridSettings { get; private set; }

        [field: SerializeField]
        public GridGenerator GridGenerator { get; private set; }

        [field: SerializeField]
        public GridChecker GridChecker { get; private set; }

        private DeckManager DeckManager => ManagerType.Deck.GetManager<DeckManager>();

        [field: SerializeField]
        public GridPlacement GridPlacement { get; private set; }

        public override void SetUp()
        {
            base.SetUp();
            GridGenerator.Generate(GridSettings);
            GridPlacement.Initialize(GridGenerator.HorizontalLines, GridGenerator.VerticalLines);
            DeckManager.GenerateDeck();
        }
    }
}
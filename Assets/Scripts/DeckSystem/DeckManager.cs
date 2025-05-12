using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameManagement;
using GridSystem.GridSpecific;
using GridSystem.Shapes;
using UnityEngine;
using Utilities;

namespace DeckSystem
{
    public class DeckManager : ManagerBase
    {
        [SerializeField]
        private Transform[] deckSlots;

        [SerializeField]
        private DeckShapeSpawner deckShapeSpawner;
        
        [SerializeField]
        private float gameOverCheckDelay = 0.5f;

        public readonly List<Shape> ActiveShapes = new();

        private bool _gameEnded;
        private Coroutine _checkGameLostRoutine;
        private Coroutine _refreshDeckRoutine;
        private GridManager GridManager => ManagerType.Grid.GetManager<GridManager>();

        public override void SetUp()
        {
            base.SetUp();
        }

        public override void SubscribeEvents()
        {
            base.SubscribeEvents();
            ShapeEvents.OnShapePlaced += OnShapePlaced;
        }

        public override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            ShapeEvents.OnShapePlaced -= OnShapePlaced;
        }

        public void GenerateDeck()
        {
            if (_gameEnded) return;
            ClearDeck();

            var shapes = deckShapeSpawner.GenerateDeck(deckSlots.Length);

            for (int i = 0; i < deckSlots.Length; i++)
            {
                var shape = shapes[i];
                shape.Initialize(deckSlots[i]);
                ActiveShapes.Add(shape);
            }

            CheckIfGameIsLost();
        }

        private void ClearDeck()
        {
            foreach (var shape in ActiveShapes)
            {
                if (shape != null)
                    Destroy(shape.gameObject);
            }

            ActiveShapes.Clear();
        }

        private void OnShapePlaced(Shape shape)
        {
            if (_refreshDeckRoutine != null)
                StopCoroutine(_refreshDeckRoutine);

            _refreshDeckRoutine = StartCoroutine(RefreshDeckRoutine(shape));
            CheckIfGameIsLost();
        }
        
        private IEnumerator RefreshDeckRoutine(Shape shape)
        {
            if (_gameEnded) yield break;

            ActiveShapes.Remove(shape);
            shape.ClearSticks();
            Destroy(shape.gameObject);

            GridManager.GridSquareChecker.CheckForCompletedLines();

            yield return new WaitForEndOfFrame();
            
            if (ActiveShapes.Count == 0)
                GenerateDeck();
        }

        private void CheckIfGameIsLost()
        {
            if (_checkGameLostRoutine != null)
                StopCoroutine(_checkGameLostRoutine);

            _checkGameLostRoutine = StartCoroutine(CheckGameLostRoutine());
        }

        private IEnumerator CheckGameLostRoutine()
        {
            yield return CoroutineExtensions.WaitForSeconds(gameOverCheckDelay);
            bool canPlace = ActiveShapes.Any(shape =>
                GridManager.GridPlacement.CanShapeBePlacedUsingStickPoints(shape.StickPoints));

            if (!canPlace)
            {
                Debug.Log("No available moves - Game Over");
                _gameEnded = true;
                GameEvents.OnGameOver?.Invoke();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioSystem;
using GameManagement;
using GridSystem;
using GridSystem.GridSpecific;
using GridSystem.Shapes;
using LevelSystem;
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

        private readonly List<Shape> _activeShapes = new();
        private bool _gameEnded;
        private Coroutine _checkGameLostRoutine;
        private GridManager GridManager => ManagerType.Grid.GetManager<GridManager>();
        private LevelManager LevelManager => ManagerType.Level.GetManager<LevelManager>();

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
                var deckSlot = deckSlots[i];
                shape.Initialize(deckSlot, LevelManager.ActiveShapeColor);
                shape.SendToDeck(deckSlot);
                _activeShapes.Add(shape);
            }

            CheckIfGameIsLost();
        }

        private void ClearDeck()
        {
            foreach (var shape in _activeShapes)
            {
                if (shape != null)
                    Destroy(shape.gameObject);
            }

            _activeShapes.Clear();
        }

        private void OnShapePlaced(Shape shape)
        {
            if (_gameEnded) return;

            _activeShapes.Remove(shape);
            shape.ClearSticks();
            Destroy(shape.gameObject);

            GridManager.GridSquareChecker.ProcessMatching();

            if (_activeShapes.Count == 0)
                GenerateDeck();

            CheckIfGameIsLost();
        }

        private void CheckIfGameIsLost()
        {
            if (_checkGameLostRoutine != null)
                StopCoroutine(_checkGameLostRoutine);

            _checkGameLostRoutine = StartCoroutine(CheckGameLostRoutine());
        }

        private IEnumerator CheckGameLostRoutine()
        {
            yield return new WaitForEndOfFrame();
            bool canPlace = _activeShapes.Any(shape =>
                GridManager.GridPlacement.CanShapeBePlacedUsingStickPoints(shape.StickPoints));

            if (!canPlace)
            {
                SetGameOver();
            }
        }

        private void SetGameOver()
        {
            Debug.Log("No available moves - Game Over");
            _gameEnded = true;
            GameEvents.OnGameOver?.Invoke();
            PlaySfx();
            gameObject.SetActive(false);
        }

        private void PlaySfx()
        {
            SoundType.GameLose.PlaySfx();
        }
    }
}
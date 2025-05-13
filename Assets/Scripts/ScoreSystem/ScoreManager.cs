using DG.Tweening;
using GameManagement;
using SaveSystem;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
  public class ScoreManager: ManagerBase
  {
      [SerializeField]
      private TMP_Text scoreText;
      
      [SerializeField]
      private TMP_Text highScoreText;
      
      [SerializeField]
      private float scoreTweenDuration = 0.5f;
      
      [SerializeField]
      private Ease scoreEase = Ease.OutCubic;

      private Tween _scoreTween;
      private Tween _highScoreTween;
      private int _score;
      private int Score
      {
          get => _score;
          set
          {
              int oldScore = _score;
              _score = value;
              UpdateScoreText(oldScore, _score);
              CheckHighScore();
          }
      }
      
      private SaveManager SaveManager => ManagerType.Save.GetManager<SaveManager>();

      private void UpdateScoreText(int fromScore, int toScore)
      {
          _scoreTween?.Kill();
          _scoreTween = DOTween.To(
              () => fromScore,
              x => scoreText.text = x.ToString(),
              toScore,
              scoreTweenDuration
          ).SetEase(scoreEase);
      }
      
      private void CheckHighScore()
      {
          if (Score > SaveManager.GameData.HighScore)
          {
              int oldHighScore = SaveManager.GameData.HighScore;
              SaveManager.GameData.HighScore = Score;
              SaveManager.SaveGameData();
              UpdateHighScoreText(oldHighScore, SaveManager.GameData.HighScore);
          }
      }
      
      private void UpdateHighScoreText(int fromHighScore, int toHighScore)
      {
          if (highScoreText != null)
          {
              _highScoreTween?.Kill();
              _highScoreTween = DOTween.To(
                  () => fromHighScore,
                  x => highScoreText.text = x.ToString(),
                  toHighScore,
                  scoreTweenDuration
              ).SetEase(scoreEase);
          }
      }
      
      private void UpdateHighScoreText()
      {
          if (highScoreText != null)
          {
              UpdateHighScoreText(SaveManager.GameData.HighScore, SaveManager.GameData.HighScore);
          }
      }

      public override void SetUp()
      {
          base.SetUp();
          Score = 0;
          UpdateHighScoreText();
      }

      public void AddScore(int score)
      {
          Score += score;
      }
      
      public void ResetScore()
      {
          Score = 0;
      }
      
      private void OnDestroy()
      {
          _scoreTween?.Kill();
          _highScoreTween?.Kill();
      }
  }
}
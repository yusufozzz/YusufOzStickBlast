using UnityEngine.SceneManagement;

namespace UISystem.Buttons
{
    public class RestartButton: ButtonBase
    {
        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            RestartGame();
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  public void QuitApplication() {
    Application.Quit();
  }
  public void ReturnToMenu() {
    Time.timeScale = 1f;
    SceneManager.LoadScene("Menu");
  }
}

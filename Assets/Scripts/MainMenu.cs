using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  public void QuitApplication() {
    Application.Quit();
  }
  public void ReturnToMenu() {
    SceneManager.LoadScene("Menu");
  }
}

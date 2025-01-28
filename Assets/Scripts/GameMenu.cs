using UnityEngine;

public class GameMenu : MonoBehaviour {
  public GameObject pause;
  public GameObject defeat;
  public GameObject victory;

  private bool isPaused = false;

  // Update is called once per frame
  void Update() {
    if(Input.GetKeyDown(KeyCode.Escape)) {
      pause.SetActive(!pause.activeSelf);
      isPaused = !isPaused;
      if(isPaused)
        Time.timeScale = 0.01f;
      else
        Time.timeScale = 1f;
    }
  }

  public void Victory() {
    Time.timeScale = 0.01f;
    victory.SetActive(true);
  }
  public void Defeat() {
    Time.timeScale = 0.01f;
    defeat.SetActive(true);
    }
}

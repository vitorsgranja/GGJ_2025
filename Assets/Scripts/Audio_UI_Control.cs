using UnityEngine;
using UnityEngine.UI;

public class Audio_UI_Control : MonoBehaviour {
  [SerializeField] private Slider masterVolumeSlider;
  [SerializeField] private Slider musicVolumeSlider;
  [SerializeField] private Slider soundVolumeSlider;

  private void Start() {
    // Initialize sliders' values based on AudioManager's current volume settings
    masterVolumeSlider.value = AudioManager.instance.masterVolume;
    musicVolumeSlider.value = AudioManager.instance.musicVolume;
    soundVolumeSlider.value = AudioManager.instance.soundVolume;
  }

  // Update AudioManager's volume settings based on slider values
  public void UpdateVolumes() {
    AudioManager.instance.SetMasterVolume(masterVolumeSlider.value);
    AudioManager.instance.SetMusicVolume(musicVolumeSlider.value);
    AudioManager.instance.SetSoundVolume(soundVolumeSlider.value);
  }

  public void PlaySound(AudioClip clip) {
    AudioManager.instance.PlaySound(clip);
  }
}
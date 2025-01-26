using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
  public static AudioManager instance;

  [SerializeField] private AudioMixer audioMixer; // Reference to the audio mixer

  private AudioSource musicSource; // Source for background music

  private List<AudioSource> soundSources = new List<AudioSource>(); // List of sources for sporadic sounds

  public float masterVolume = 1f; // Master volume multiplier
  public float musicVolume = 1f; // Music volume multiplier
  public float soundVolume = 1f; // Sound volume multiplier

  [SerializeField] private int initialSoundSources = 5; // Initial number of sound sources to instantiate

  [Tooltip("music 0 - theme music")]
  public List<AudioClip> musicList = new List<AudioClip>(); // List of Musics
  [Tooltip("sound 0 - button\nsound 1 - coins\nsound 2 - cloth")]
  public List<AudioClip> effectList = new List<AudioClip>(); // List of Effects
  private void Awake() {
    // Ensure only one instance of AudioManager exists
    if(instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
      return;
    }

    // Instantiate initial sound sources
    for(int i = 0; i < initialSoundSources; i++) {
      AudioSource source = gameObject.AddComponent<AudioSource>();
      source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2]; // set audiosource to EffectsVolume mixer
      soundSources.Add(source);
    }
    PlayMusic(musicList[0]);
  }

  // Play background music
  public void PlayMusic(AudioClip music) {
    if(musicSource == null) {
      musicSource = gameObject.AddComponent<AudioSource>();
      musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[1]; // set audiosource to MusicVolume mixer
      musicSource.loop = true;
    }
    musicSource.clip = music;
    musicSource.Play();
  }

  // Play a sporadic sound
  public void PlaySound(AudioClip sound) {
    foreach(AudioSource source in soundSources) {
      if(!source.isPlaying) {
        source.clip = sound;
        source.Play();
        return;
      }
    }

    // If no available source, instantiate a new one
    AudioSource newSource = gameObject.AddComponent<AudioSource>();
    newSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2]; // set audiosource to EffectsVolume mixer
    newSource.clip = sound;
    newSource.Play();
    soundSources.Add(newSource);
  }

  // Set master volume
  public void SetMasterVolume(float volume) {
    masterVolume = volume;
    UpdateVolume();
  }

  // Set music volume
  public void SetMusicVolume(float volume) {
    musicVolume = volume;
    UpdateVolume();
  }

  // Set sound volume
  public void SetSoundVolume(float volume) {
    soundVolume = volume;
    UpdateVolume();
  }

  // Update volume levels
  private void UpdateVolume() {
    audioMixer.SetFloat("Master",Mathf.Log10(masterVolume) * 20);
    audioMixer.SetFloat("MusicVolume",Mathf.Log10(musicVolume) * 20);
    audioMixer.SetFloat("SoundVolume",Mathf.Log10(soundVolume) * 20);
  }
}
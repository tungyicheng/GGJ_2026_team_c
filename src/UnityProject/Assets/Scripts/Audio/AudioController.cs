using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio {
    public sealed class AudioController : MonoBehaviour {
        public AudioMixer AudioMixer;
        public int DefaultVolume = 70;
        public string SoundEffectVolumePlayerPrefKey = "sfxVolume";
        public string SoundEffectVolumeParameterName = "sfxVolume";
        public string MusicVolumePlayerPrefKey = "musicVolume";
        public string MusicVolumeParameterName = "musicVolume";

        int soundEffectsVolume;
        int musicVolume;

        public int SoundEffectsVolume {
            get => soundEffectsVolume;
            set {
                if (value == soundEffectsVolume) return;
                soundEffectsVolume = value;
                PlayerPrefs.SetInt(SoundEffectVolumePlayerPrefKey, soundEffectsVolume);
                ApplyVolume(SoundEffectVolumeParameterName, soundEffectsVolume);
            }
        }

        public int MusicVolume {
            get => musicVolume;
            set {
                if (value == musicVolume) return;
                musicVolume = value;
                PlayerPrefs.SetInt(MusicVolumePlayerPrefKey, musicVolume);
                ApplyVolume(MusicVolumeParameterName, musicVolume);
            }
        }

        void Awake() {
            soundEffectsVolume = PlayerPrefs.GetInt(SoundEffectVolumePlayerPrefKey, DefaultVolume);
            musicVolume = PlayerPrefs.GetInt(MusicVolumePlayerPrefKey, DefaultVolume);
        }

        void Start() {
            ApplyVolume(SoundEffectVolumeParameterName, soundEffectsVolume);
            ApplyVolume(MusicVolumeParameterName, musicVolume);
        }

        void ApplyVolume(string parameterName, int volume) {
            var value = volume > 0 ? Mathf.Log10(Mathf.Clamp01(volume / 100f)) * 20f : -80f;
            AudioMixer.SetFloat(parameterName, value);
        }
    }
}

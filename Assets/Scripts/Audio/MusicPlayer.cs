// Copyright Olli Etuaho 2018

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

namespace LPUnityUtils
{

    // A class that makes sure only one piece of music is playing at a time.
    // The music can still crossfade (one can start while another is fading out).
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer FirstInstance { get; private set; }

        [FormerlySerializedAs("Sounds")]
        [SerializeField] private List<Sound> MusicList;

        private AudioSourcePlayer.PlayingSound PlayingMusic;
        public string PlayingMusicName => (PlayingMusic != null && PlayingMusic.State == AudioSourcePlayer.PlayingSound.SoundState.Playing) ? PlayingMusic.SoundName : "";

        private AudioSourcePlayer Player;

        [SerializeField] private float DefaultFadeOutPreviousTime = 0.15f;

        [SerializeField] private string MuteMixerChannelOnApplicationPause = "MasterVolume";

        [SerializeField] private UnityEngine.Audio.AudioMixerGroup _DefaultOutputAudioMixerGroup;
        public UnityEngine.Audio.AudioMixerGroup DefaultOutputAudioMixerGroup => _DefaultOutputAudioMixerGroup;

        [SerializeField] private AudioMixer _Mixer;
        public AudioMixer Mixer => _Mixer;

        private void Awake()
        {
            if ( FirstInstance == null )
            {
                FirstInstance = this;
            }
            Player = GetComponent<AudioSourcePlayer>();
            if ( Player == null )
            {
                Player = gameObject.AddComponent<AudioSourcePlayer>();
            }
        }

        public static float ConvertLinearToDB(float linearValue)
        {
            return Mathf.Log10(Mathf.Clamp(linearValue, 0.0001f, 1f)) * 20.0f;
        }

        public static float ConvertDBToLinear(float dbValue)
        {
            return Mathf.Pow(10.0f, dbValue / 20.0f);
        }

        private void OnApplicationFocus(bool focus)
        {
            if ( !string.IsNullOrEmpty(MuteMixerChannelOnApplicationPause) )
            {
                Mixer?.SetFloat(MuteMixerChannelOnApplicationPause, ConvertLinearToDB(focus ? 1.0f : 0.0f));
            }
        }

        public void TransitionToMixerSnapshot(AudioMixerSnapshot snapshot, float time)
        {
            if ( snapshot == null )
            {
                return;
            }
            AudioMixerSnapshot[] snapshots = { snapshot };
            float[] weights = { 1.0f };
            Mixer?.TransitionToSnapshots(snapshots, weights, time);
        }

        public bool HasMusic(string musicName)
        {
            return MusicList.Find(s => s.Name == musicName) != null;
        }

        public void Play(string musicName)
        {
            Play(musicName, DefaultFadeOutPreviousTime);
        }

        public void Play(SoundVariants sound)
        {
            Play(sound, DefaultFadeOutPreviousTime);
        }

        public void Play(string musicName, float fadeOutPreviousTime)
        {
            if ( string.IsNullOrEmpty(musicName) )
            {
                return;
            }
            if ( PlayingMusicName == musicName )
            {
                return;
            }
            Sound sound = MusicList.Find(s => s.Name == musicName);
            if ( sound == null )
            {
                Debug.LogWarning("Sound not found! " + musicName, this);
                return;
            }
            Play(sound, fadeOutPreviousTime);
        }

        public void Play(SoundVariants sound, float fadeOutPreviousTime)
        {
            if ( sound == null )
            {
                Debug.LogWarning("SoundVariants not set!", this);
                return;
            }
            FadeOutAndStopIfNeeded(fadeOutPreviousTime);
            PlayingMusic = Player.Play(sound);
        }

        public void Play(Sound sound, float fadeOutPreviousTime)
        {
            if ( sound == null )
            {
                Debug.LogWarning("Sound not set!", this);
                return;
            }
            if ( PlayingMusic != null && PlayingMusic.Equals(sound) )
            {
                return;
            }
            FadeOutAndStopIfNeeded(fadeOutPreviousTime);
            PlayingMusic = Player.Play(sound);
        }

        // Returns true if there was some music playing and it needed to be stopped.
        public bool FadeOutAndStopIfNeeded(float durationSeconds, System.Action<AudioSourcePlayer.PlayingSound> callbackOnStop = null)
        {
            if ( PlayingMusic == null )
            {
                return false;
            }
            return PlayingMusic.FadeOutAndStopUnscaledTime(durationSeconds, callbackOnStop);
        }

        public void DuckWhileSoundPlays(AudioSourcePlayer.PlayingSound other, float fadeDuration, float lowLevel = 0.0f)
        {
            if ( PlayingMusic == null )
            {
                return;
            }
            PlayingMusic.DuckWhileSoundPlays(other, fadeDuration, lowLevel);
        }

        public void DuckForDuration(System.Object duckId, float fadeOutDuration, float totalDuration, float fadeInDuration, float lowLevel)
        {
            if ( PlayingMusic == null )
            {
                return;
            }
            PlayingMusic.DuckForDuration(duckId, fadeOutDuration, totalDuration, fadeInDuration, lowLevel);
        }
    }

}  // namespace LPUnityUtils

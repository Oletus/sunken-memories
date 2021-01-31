using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

namespace LPUnityUtils
{

    // This class helps with instantiating multiple audio sources if playing sounds with different pitch/volume/etc. settings on the same transform.
    // It also makes it easier to fade out a specific playing sound.
    public class AudioSourcePlayer : MonoBehaviour
    {
        private static AudioSourcePlayer _GlobalPlayer;

        public static AudioSourcePlayer GlobalPlayer
        {
            get
            {
                if ( _GlobalPlayer == null )
                {
                    if ( MusicPlayer.FirstInstance == null )
                    {
                        Debug.Log("Using global AudioSourcePlayer requires MusicPlayer instance to be initialized");
                        return null;
                    }
                    GameObject child = MusicPlayer.FirstInstance.transform.CreateEmptyChild("GlobalAudioSourcePlayer");
                    _GlobalPlayer = child.AddComponent<AudioSourcePlayer>();
                }
                return _GlobalPlayer;
            }
        }

        private class ManagedAudioSource
        {
            internal AudioSource Source { get; private set; }
            internal int PlayCallIndex;
            internal int Index { get; private set; }

            internal ManagedAudioSource(AudioSource source, int index)
            {
                Source = source;
                PlayCallIndex = 0;
                Index = index;
            }

            internal PlayingSound Play(AudioSourcePlayer owner, Sound sound, float volumeScale)
            {
                sound.SetToSource(Source, volumeScale);
                Source.Play();
                ++PlayCallIndex;
                return new PlayingSound(owner, Index, PlayCallIndex, sound);
            }
        }

        private List<ManagedAudioSource> AudioSources;
        private Dictionary<SoundVariants, SoundVariantPlayer> SoundVariantPlayers;

        [SerializeField] public List<float> MinPlayIntervalForVoice = new List<float>();

        private List<float> LastPlayTimeForVoice = new List<float>();

        public void SetMinPlayIntervalForVoice(int voiceIndex, float minPlayInterval)
        {
            while (MinPlayIntervalForVoice.Count <= voiceIndex)
            {
                MinPlayIntervalForVoice.Add(0.0f);
            }
            MinPlayIntervalForVoice[voiceIndex] = minPlayInterval;
        }

        public class PlayingSound
        {
            public enum SoundState
            {
                Playing,
                FadingOut,
                Stopped
            }
            private SoundState _State;
            public SoundState State
            {
                get
                {
                    GetSource();  // This will set the sound to the stopped state if needed.
                    return _State;
                }
                private set
                {
                    _State = value;
                }
            }

            public string SoundName => Sound.Name;

            private AudioSourcePlayer Owner;
            private int SourceIndex;
            private int PlayCallIndex;
            private Sound Sound;

            private float FadeMultiplier = 1.0f;
            private float DuckingMultiplier = 1.0f;

            // The key is the object that requested the audio to duck.
            private Dictionary<object, float> DuckingMultipliers;

            public float Progress
            {
                get
                {
                    ManagedAudioSource source = GetSource();
                    if ( source == null )
                    {
                        return 1.0f;
                    }
                    return (float)(source.Source.timeSamples) / source.Source.clip.samples;
                }
            }

            public PlayingSound(AudioSourcePlayer owner, int sourceIndex, int playCallIndex, Sound sound)
            {
                State = SoundState.Playing;
                Owner = owner;
                SourceIndex = sourceIndex;
                PlayCallIndex = playCallIndex;
                Sound = sound;
            }

            private ManagedAudioSource GetSource()
            {
                if ( _State == SoundState.Stopped )
                {
                    return null;
                }
                ManagedAudioSource source = Owner.AudioSources[SourceIndex];
                if ( !source.Source.isPlaying || source.PlayCallIndex != PlayCallIndex )
                {
                    // This sound has finished playing.
                    State = SoundState.Stopped;
                    return null;
                }
                return source;
            }

            private void SetFadeMultiplier(float fadeMultiplier)
            {
                FadeMultiplier = fadeMultiplier;
                UpdateVolume();
            }

            private void SetDuckingMultiplier(object requester, float duckingMultiplier)
            {
                if ( DuckingMultipliers == null )
                {
                    DuckingMultipliers = new Dictionary<object, float>();
                }
                if ( duckingMultiplier == 1.0f )
                {
                    DuckingMultipliers.Remove(requester);
                }
                else
                {
                    DuckingMultipliers[requester] = duckingMultiplier;
                }
                float totalMultiplier = 1.0f;
                foreach (float multiplier in DuckingMultipliers.Values)
                {
                    totalMultiplier *= multiplier;
                }
                DuckingMultiplier = totalMultiplier;
                UpdateVolume();
            }

            private void UpdateVolume()
            {
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    source.Source.volume = Sound.Volume * FadeMultiplier * DuckingMultiplier;
                }
            }

            public void Stop()
            {
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    State = SoundState.Stopped;
                    source.Source.Stop();
                }
            }

            // Note that this is not the ideal method for fading out, since the volume will be set in steps.
            // But it works well if the game is running at high framerate and the fade is long enough.
            // Returns true if really started to fade out.
            public bool FadeOutAndStop(float durationSeconds, System.Action<PlayingSound> callbackOnStop = null)
            {
                if ( State != SoundState.Playing )
                {
                    return false;
                }
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    Owner.StartCoroutine(FadeOutAndStopCoroutine(false, durationSeconds, callbackOnStop));
                    return true;
                }
                return false;
            }

            public bool FadeOutAndStopUnscaledTime(float durationSeconds, System.Action<PlayingSound> callbackOnStop = null)
            {
                if ( State != SoundState.Playing )
                {
                    return false;
                }
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    Owner.StartCoroutine(FadeOutAndStopCoroutine(true, durationSeconds, callbackOnStop));
                    return true;
                }
                return false;
            }

            private IEnumerator FadeOutAndStopCoroutine(bool unscaledTime, float durationSeconds, System.Action<PlayingSound> callbackOnStop = null)
            {
                State = SoundState.FadingOut;
                float startTime = unscaledTime ? Time.unscaledTime : Time.time;
                while ( (unscaledTime ? Time.unscaledTime : Time.time) - startTime < durationSeconds )
                {
                    float progress = ((unscaledTime ? Time.unscaledTime : Time.time) - startTime) / durationSeconds;
                    SetFadeMultiplier(1.0f - progress);
                    yield return null;
                }
                Stop();
                callbackOnStop?.Invoke(this);
            }

            // TODO: Handle multiple simultaneous ducks.
            public void DuckWhileSoundPlays(PlayingSound other, float fadeDuration, float lowLevel = 0.0f)
            {
                if ( State != SoundState.Playing )
                {
                    return;
                }
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    Owner.StartCoroutine(DuckWhileSoundPlaysCoroutine(other, fadeDuration, lowLevel));
                }
            }

            public void DuckForDuration(object requester, float fadeOutDuration, float totalDuration, float fadeInDuration, float lowLevel)
            {
                if ( State != SoundState.Playing )
                {
                    return;
                }
                ManagedAudioSource source = GetSource();
                if ( source != null )
                {
                    Owner.StartCoroutine(DuckForDurationCoroutine(requester, fadeOutDuration, totalDuration, fadeInDuration, lowLevel));
                }
            }

            private IEnumerator DuckWhileSoundPlaysCoroutine(PlayingSound other, float fadeDuration, float lowLevel)
            {
                float startTime = Time.unscaledTime - (1.0f / 60.0f);
                while ( Time.unscaledTime - startTime < fadeDuration )
                {
                    float fadeOutProgress = (Time.unscaledTime - startTime) / fadeDuration;
                    SetDuckingMultiplier(other, Mathf.Lerp(1.0f, lowLevel, fadeOutProgress));
                    yield return null;
                }
                SetDuckingMultiplier(other, lowLevel);
                while (other.State == SoundState.Playing)
                {
                    yield return null;
                }
                startTime = Time.unscaledTime - (1.0f / 60.0f);
                while ( Time.unscaledTime - startTime < fadeDuration )
                {
                    float fadeInProgress = (Time.unscaledTime - startTime) / fadeDuration;
                    SetDuckingMultiplier(other, Mathf.Lerp(lowLevel, 1.0f, fadeInProgress));
                    yield return null;
                }
                SetDuckingMultiplier(other, 1.0f);
            }

            private IEnumerator DuckForDurationCoroutine(object requester, float fadeOutDuration, float totalDuration, float fadeInDuration, float lowLevel)
            {
                float startTime = Time.unscaledTime - (1.0f / 60.0f);
                while ( Time.unscaledTime - startTime < fadeOutDuration )
                {
                    float fadeOutProgress = (Time.unscaledTime - startTime) / fadeOutDuration;
                    SetDuckingMultiplier(requester, Mathf.Lerp(1.0f, lowLevel, fadeOutProgress));
                    yield return null;
                }
                SetDuckingMultiplier(requester, lowLevel);
                while ( Time.unscaledTime - startTime < totalDuration - fadeInDuration )
                {
                    yield return null;
                }
                startTime = Time.unscaledTime - (1.0f / 60.0f);
                while ( Time.unscaledTime - startTime < fadeInDuration )
                {
                    float fadeInProgress = (Time.unscaledTime - startTime) / fadeInDuration;
                    SetDuckingMultiplier(requester, Mathf.Lerp(lowLevel, 1.0f, fadeInProgress));
                    yield return null;
                }
                SetDuckingMultiplier(requester, 1.0f);
            }

            public bool Equals(Sound sound)
            {
                return (sound == Sound);
            }
        }

        private void Awake()
        {
            AudioSources = new List<ManagedAudioSource>();
            AudioSource defaultAudioSource = GetComponent<AudioSource>();
            if (!defaultAudioSource)
            {
                defaultAudioSource = gameObject.AddComponent<AudioSource>();
                if ( MusicPlayer.FirstInstance != null )
                {
                    defaultAudioSource.outputAudioMixerGroup = MusicPlayer.FirstInstance.DefaultOutputAudioMixerGroup;
                }
            }
            AudioSources.Add(new ManagedAudioSource(defaultAudioSource, 0));
        }

        public static void CopyAudioSourceSettings(AudioSource from, AudioSource target)
        {
            target.outputAudioMixerGroup = from.outputAudioMixerGroup;
            target.bypassEffects = from.bypassEffects;
            target.bypassListenerEffects = from.bypassListenerEffects;
            target.bypassReverbZones = from.bypassReverbZones;
            target.loop = from.loop;

            target.priority = from.priority;

            target.volume = from.volume;
            target.pitch = from.pitch;
            target.panStereo = from.panStereo;
            target.spatialBlend = from.spatialBlend;
            target.reverbZoneMix = from.reverbZoneMix;

            target.dopplerLevel = from.dopplerLevel;
            target.spread = from.spread;
            target.rolloffMode = from.rolloffMode;
            target.minDistance = from.minDistance;
            target.maxDistance = from.maxDistance;
        }

        private ManagedAudioSource GetFreeAudioSource()
        {
            for ( int i = 1; i < AudioSources.Count; ++i )
            {
                if ( !AudioSources[i].Source.isPlaying )
                {
                    return AudioSources[i];
                }
            }
            AudioSource source = gameObject.AddComponent<AudioSource>();
            CopyAudioSourceSettings(AudioSources[0].Source, source);
            source.playOnAwake = false;
            source.loop = false;
            int index = AudioSources.Count;
            ManagedAudioSource managedSource = new ManagedAudioSource(source, index);
            AudioSources.Add(managedSource);
            return managedSource;
        }

        // This function uses whatever pitch and volume have been set on the first AudioSource component.
        public void PlayWithAudioSourceSettings(Sound sound, float volumeScale = 1f)
        {
            sound.PlayOneShot(AudioSources[0].Source, volumeScale);
        }

        private bool MarkVoicePlaying(int voiceIndex)
        {
            if ( voiceIndex < 0 || MinPlayIntervalForVoice == null || voiceIndex >= MinPlayIntervalForVoice.Count )
            {
                return true;
            }
            while ( voiceIndex >= LastPlayTimeForVoice.Count )
            {
                LastPlayTimeForVoice.Add(-1.0f);
            }
            if ( Time.unscaledTime > LastPlayTimeForVoice[voiceIndex] + MinPlayIntervalForVoice[voiceIndex] )
            {
                LastPlayTimeForVoice[voiceIndex] = Time.unscaledTime;
                return true;
            }
            return false;
        }

        public PlayingSound Play(Sound sound, int voiceIndex = -1, float volumeScale = 1.0f)
        {
            if ( !MarkVoicePlaying(voiceIndex) )
            {
                return null;
            }
            ManagedAudioSource source = GetFreeAudioSource();
            if ( source == null )
            {
                return null;
            }
            return source.Play(this, sound, volumeScale);
        }

        public PlayingSound Play(SoundVariants variants, int voiceIndex = -1, float volumeScale = 1.0f)
        {
            if ( variants == null )
            {
                return null;
            }
            if ( !MarkVoicePlaying(voiceIndex) )
            {
                return null;
            }
            if ( SoundVariantPlayers == null )
            {
                SoundVariantPlayers = new Dictionary<SoundVariants, SoundVariantPlayer>();
            }
            if ( !SoundVariantPlayers.ContainsKey(variants) )
            {
                SoundVariantPlayers.Add(variants, new SoundVariantPlayer(this, variants));
            }
            return SoundVariantPlayers[variants].Play(volumeScale);
        }
    }

}

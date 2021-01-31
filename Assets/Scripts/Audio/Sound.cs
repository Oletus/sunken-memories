// Copyright Olli Etuaho 2018

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LPUnityUtils
{

    [System.Serializable]
    public class Sound
    {
        public string Name;

        [SerializeField] private AudioClip Clip = null;

        [Range(0.0f, 1.0f)]
        [SerializeField] [FormerlySerializedAs("Volume")] private float _Volume = 0.5f;
        public float Volume
        {
            get => _Volume;
            set => _Volume = value;
        }

        [Range(0.1f, 3.0f)]
        [SerializeField] public float Pitch = 1.0f;
        [SerializeField] private bool Loop = false;

        public void PlayOneShot(AudioSource source, float volumeScale = 1.0f)
        {
            // Note that we can't set the pitch or other parameters of the source here - those affect all sound effects currently playing on the source.
            source.PlayOneShot(Clip, volumeScale);
        }

        public void SetToSource(AudioSource source)
        {
            source.clip = Clip;
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
        }
    }

}  // namespace LPUnityUtils

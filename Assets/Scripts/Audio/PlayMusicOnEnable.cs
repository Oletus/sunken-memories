// Copyright Lockpickle Oy 2020

using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

namespace LPUnityUtils
{
    public class PlayMusicOnEnable : MonoBehaviour
    {
        [SerializeField] private string MusicName;
        
        private void OnEnable()
        {
            MusicPlayer.FirstInstance.Play(MusicName);
        }
    }

}  // namespace LPUnityUtils

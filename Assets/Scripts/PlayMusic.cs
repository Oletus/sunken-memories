using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LPUnityUtils;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class PlayMusic : MonoBehaviour
{
    void Start ()
    {
        if (MusicPlayer.FirstInstance != null)
        {
            DontDestroyOnLoad(MusicPlayer.FirstInstance);
            MusicPlayer.FirstInstance.Play("main");
        }
    }
}

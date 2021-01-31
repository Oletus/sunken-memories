
// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

using UnityEngine;

namespace LPUnityUtils
{

    public struct LazyInitAudioSourcePlayer
    {
        private AudioSourcePlayer Player;

        public AudioSourcePlayer GetAsChild(Transform parent)
        {
            if ( Player == null )
            {
                GameObject child = parent.CreateEmptyChild();
                Player = child.AddComponent<AudioSourcePlayer>();
            }
            return Player;
        }

    }

}

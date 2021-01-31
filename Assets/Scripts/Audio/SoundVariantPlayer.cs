// Copyright Olli Etuaho 2018

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LPUnityUtils
{

    [System.Serializable]
    public class SoundVariantPlayer
    {
        [SerializeField] private AudioSourcePlayer Player = null;
        [SerializeField] private SoundVariants Variants = null;
        private const int MinCycleLength = 3;

        private List<int> LastPlayedVariants;

        public SoundVariantPlayer(AudioSourcePlayer player, SoundVariants variants)
        {
            Player = player;
            Variants = variants;
        }

        public AudioSourcePlayer.PlayingSound Play()
        {
            if ( LastPlayedVariants == null )
            {
                LastPlayedVariants = new List<int>();
            }
            int playedIndex = -1;
            AudioSourcePlayer.PlayingSound sound = Variants?.PlayOn(Player, LastPlayedVariants, out playedIndex);
            if ( sound != null )
            {
                LastPlayedVariants.Add(playedIndex);
                if ( LastPlayedVariants.Count >= MinCycleLength || LastPlayedVariants.Count > Variants.Count - 1 )
                {
                    LastPlayedVariants.RemoveAt(0);
                }
            }
            return sound;
        }
    }

}

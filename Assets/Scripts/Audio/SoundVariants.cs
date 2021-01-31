// Copyright Olli Etuaho 2018

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace LPUnityUtils
{

    [CreateAssetMenu(fileName = "Sound", menuName = "LPUnityUtils/Sound Variants")]
    public class SoundVariants : ScriptableObject
    {
        [SerializeField] [FormerlySerializedAs("sounds")] private List<Sound> Sounds = null;

        public int Count => Sounds != null ? Sounds.Count : 0;

        public AudioSourcePlayer.PlayingSound PlayOn(AudioSourcePlayer player, ICollection<int> avoidVariantIndexes, out int soundIndex)
        {
            if ( Count == 0 )
            {
                Debug.Log("No sounds set as variants", this);
                soundIndex = -1;
                return null;
            }
            if ( player == null )
            {
                Debug.LogWarning("No sound player", this);
                soundIndex = -1;
                return null;
            }
            soundIndex = 0;
            if ( Sounds.Count > avoidVariantIndexes.Count )
            {
                int randomCount = Sounds.Count - avoidVariantIndexes.Count;
                soundIndex = Mathf.FloorToInt(Random.Range(0.0f, randomCount - Mathf.Epsilon));
                avoidVariantIndexes = avoidVariantIndexes.OrderBy(index => index).ToList();
                foreach ( int avoidIndex in avoidVariantIndexes )
                {
                    if ( soundIndex >= avoidIndex )
                    {
                        ++soundIndex;
                    }
                }
            }
            return player.Play(Sounds[soundIndex]);
        }
    }
}

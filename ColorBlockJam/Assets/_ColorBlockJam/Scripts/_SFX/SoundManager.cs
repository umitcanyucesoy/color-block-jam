using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ColorBlockJam.Scripts._SFX
{
    public class SoundManager : MonoBehaviour
    {
        public enum SoundType
        {
            Game,
            Drag,
            Drop,
            Vacuum,
            LoseSound,
            WinSound,
            TimeFreeze
        }

        [Serializable]
        public class SoundList
        {
            public SoundType soundType;
            public AudioSource audio;
        }
        
        public static SoundManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        public List<SoundList> soundList;

        public void PlaySound(SoundType soundType)
        {
            foreach (var sound in soundList)
            {
                if (sound.soundType == soundType)
                {
                    sound.audio.Play();
                    break;
                }
            }
        }
        
        public void StopSound(SoundType soundType)
        {
            foreach (var sound in soundList)
            {
                if (sound.soundType == soundType)
                {
                    sound.audio.Stop();
                    break;
                }
            }
        }

    }
}
using System;
using UnityEngine;

namespace Assets.Project.Scripts
{
    [Serializable]
    public struct SFXData
    {
        public string Id;
        [Range(0.0f, 1.0f)]
        public float volume;
        public AudioClip[] AudioClips;
    }
}

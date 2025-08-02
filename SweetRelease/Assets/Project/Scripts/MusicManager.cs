using DG.Tweening;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        private AudioClip mainMenuMusic;

        [SerializeField]
        private AudioClip gameplayMusic;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private float transitionInDuration;

        [SerializeField]
        private float transitionOutDuration;


        [SerializeField]
        private AudioLowPassFilter lowPassFilter;

        private float mainMenuMusicTime;
        private float originalVolume;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
            originalVolume = audioSource.volume;
        }

        public void TransitionToMainMenu()
        {
            if (audioSource.clip == mainMenuMusic)
            {
                return;
            }

            TransitionToMusic(mainMenuMusic, mainMenuMusicTime);
        }

        private void TransitionToMusic(AudioClip music, float playTime = 0)
        {
            UnMuffle();
            Sequence sequence = DOTween.Sequence();
            _ = sequence.Append(audioSource.DOFade(0.0f, transitionOutDuration));
            _ = sequence.AppendCallback(() =>
            {
                audioSource.Stop();
                audioSource.clip = music;
                audioSource.time = playTime;
                audioSource.Play();
            });
            _ = sequence.Append(audioSource.DOFade(originalVolume, transitionOutDuration));
            _ = sequence.Play();
        }

        public void TransitionToGamplay()
        {
            mainMenuMusicTime = audioSource.time;
            TransitionToMusic(gameplayMusic);
        }

        public void Muffle()
        {
            lowPassFilter.enabled = true;
            audioSource.volume = originalVolume / 2.0f;
        }

        public void UnMuffle()
        {
            lowPassFilter.enabled = false;
            audioSource.volume = originalVolume;
        }
    }
}

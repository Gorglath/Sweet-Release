using System.Linq;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class SFXManager : MonoBehaviour
    {
        public static SFXManager instance;

        [SerializeField]
        private SFXData[] sfxData;

        [SerializeField]
        private AudioSource templateAudioSource;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }

            instance = this;
        }

        public void PlaySFX(string Id)
        {
            SFXData targetSfx = sfxData.First(sfxData => sfxData.Id == Id);
            AudioSource audioSource = Instantiate(templateAudioSource, transform);
            audioSource.gameObject.name = Id;

            audioSource.clip = targetSfx.AudioClips[Random.Range(0, targetSfx.AudioClips.Length)];
            audioSource.volume = targetSfx.volume;
            audioSource.Play();

            Destroy(audioSource.gameObject, 5.0f);
        }
    }
}

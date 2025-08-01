using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class CinematicCamera : MonoBehaviour
    {
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private Transform[] cineamticLocations;

        [SerializeField]
        private float timeBetweenCinematicLocations;

        [SerializeField]
        private float transitionDuration;

        private float timeBetweenCinematicLocationsCounter;
        private float transitionDurationCounter;
        private int currentCinematicLocationIndex = 0;
        private Transform m_cacheTransform;

        private void Awake()
        {
            m_cacheTransform = transform;
            m_cacheTransform.position = cineamticLocations[currentCinematicLocationIndex].position;
            m_cacheTransform.rotation = cineamticLocations[currentCinematicLocationIndex].rotation;
        }

        public async UniTask Play()
        {
            while (true)
            {
                timeBetweenCinematicLocationsCounter += Time.deltaTime;
                if (timeBetweenCinematicLocationsCounter / timeBetweenCinematicLocations < 1.0f)
                {
                    await UniTask.Yield();
                    continue;
                }

                timeBetweenCinematicLocationsCounter = 0;
                bool isComplete = await TransitionToNextCinematicLocation();
                if (isComplete)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        private async UniTask<bool> TransitionToNextCinematicLocation()
        {
            Transform currentCinematicLocation = cineamticLocations[currentCinematicLocationIndex];
            Transform targetCinematicLocation = cineamticLocations[currentCinematicLocationIndex + 1];
            transitionDurationCounter = 0;
            while (true)
            {
                float interpolation01 = transitionDurationCounter == 0 ? 0 : transitionDurationCounter / transitionDuration;
                Vector3 newPosition = Vector3.Lerp(currentCinematicLocation.position, targetCinematicLocation.position, interpolation01);
                Quaternion newRotation = Quaternion.Slerp(currentCinematicLocation.rotation, targetCinematicLocation.rotation, interpolation01);

                m_cacheTransform.position = newPosition;
                m_cacheTransform.rotation = newRotation;

                if (interpolation01 >= 1)
                {
                    break;
                }

                await UniTask.Yield();

                transitionDurationCounter += Time.deltaTime;
            }

            currentCinematicLocationIndex++;
            return currentCinematicLocationIndex == cineamticLocations.Length - 1;
        }
    }
}

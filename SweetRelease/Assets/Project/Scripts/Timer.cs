using TMPro;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text timerLabel;

        [SerializeField]
        private string timeFormat = "Time : {0}";

        private bool isActive = false;

        public float TotalTime { get; private set; }
        private void Awake()
        {
            TotalTime = 0;
            isActive = false;
        }

        public void StartTimer()
        {
            isActive = true;
        }

        public void StopTimer()
        {
            isActive = false;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            TotalTime += Time.deltaTime;
            timerLabel.text = string.Format(timeFormat, TotalTime.ToString("F1"));
        }
        public void ClearTimer()
        {
            timerLabel.text = string.Format(timeFormat, "0");
            TotalTime = 0;
            isActive = false;
        }
    }
}

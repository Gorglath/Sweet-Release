using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public abstract class UIAnimation : MonoBehaviour
    {
        public abstract UniTask Play();
    }
}

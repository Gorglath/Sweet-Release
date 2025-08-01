using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public abstract class UITransitionAnimation : MonoBehaviour
    {
        public abstract UniTask Play();
    }
}

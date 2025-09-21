using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialogs {
    [RequireComponent(typeof(Animation))]
    public class DialogAnimationController : MonoBehaviour {
        [SerializeField]
        private AnimationClip _showClip, _hideClip;

        private Animation _animation;

        private Animation Animation => _animation ??= GetComponent<Animation>();

        private bool _isShown;

        public async UniTask Show() {
            if (!_isShown) {
                EnsureClip(_showClip);

                Animation.Play(_showClip.name);
            }

            _isShown = true;
            await UniTask.WaitWhile(() => Animation.isPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        private void EnsureClip(AnimationClip clip) {
            if (Animation.GetClip(clip.name) == null) {
                Animation.AddClip(clip, clip.name);
            }
        }

        public async UniTask Hide() {
            if (_isShown) {
                EnsureClip(_hideClip);
                Animation.Play(_hideClip.name);
            }

            _isShown = false;
            await UniTask.WaitWhile(() => Animation.isPlaying, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}
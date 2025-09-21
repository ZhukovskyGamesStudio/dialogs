using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialogs {
    public abstract class DialogBase : MonoBehaviour {
        private DialogAnimationController _animationController;
        private DialogAnimationController AnimationController => _animationController ??= GetComponent<DialogAnimationController>();
        protected event Action OnClose;

        protected virtual bool IsHideProfile => false;

        public virtual async UniTask Show(Action onClose) {
            gameObject.SetActive(true);
            OnClose = onClose;
            if (AnimationController) {
                await AnimationController.Show();
            }

            //if (IsHideProfile) {
            //    UIHud.Instance.ProfileView.Hide();
            //}
        }

        public void CloseByButton() {
            Close().Forget();
        }

        protected virtual async UniTask Close() {
            // if (IsHideProfile) {
            //     UIHud.Instance.ProfileView.Show();
            // }

            if (AnimationController) {
                await AnimationController.Hide();
            }

            OnClose?.Invoke();
            Destroy(gameObject);
        }
    }
}
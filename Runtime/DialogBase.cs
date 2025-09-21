using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dialogs {
    public abstract class DialogBase : MonoBehaviour {
        private DialogAnimationController _animationController;
        private DialogAnimationController AnimationController => _animationController ??= GetComponent<DialogAnimationController>();
        protected event Action OnClose;
        protected event Action<bool> OnHideUI;

        public bool IsHidingUIOnOpen => IsHideProfile;
        protected virtual bool IsHideProfile => false;

        public virtual async UniTask Show(Action onClose, Action<bool> onHideUI) {
            gameObject.SetActive(true);
            OnClose = onClose;
            OnHideUI = onHideUI;
            if (AnimationController) {
                await AnimationController.Show();
            }

            if (IsHidingUIOnOpen) {
                OnHideUI?.Invoke(true);
                //UIHud.Instance.ProfileView.Hide();
            }
        }

        public void CloseByButton() {
            Close().Forget();
        }

        protected virtual async UniTask Close() {
            if (IsHidingUIOnOpen) {
                OnHideUI?.Invoke(false);
                // UIHud.Instance.ProfileView.Show();
            }

            if (AnimationController) {
                await AnimationController.Hide();
            }

            OnClose?.Invoke();
            Destroy(gameObject);
        }
    }
}
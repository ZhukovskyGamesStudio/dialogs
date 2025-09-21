using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogs {
    public class DialogsManager : MonoBehaviour {
        [SerializeField]
        private List<DialogBase> _dialogs = new List<DialogBase>();

        private Queue<DialogBase> _dialogQueue = new Queue<DialogBase>();
        public static DialogsManager Instance { get; private set; }
        private DialogBase _shownDialog;
        public bool IsDialogShown => _shownDialog != null;

        public Action<bool> OnHideUI;

        private void Awake() {
            Instance = this;
            LoadDialogs();
        }

        private void LoadDialogs() {
#if UNITY_EDITOR
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Dialogs")) {
                Debug.LogError("DialogsManager: Папка Assets/Resources/Dialogs отсутствует!");
                return;
            }
#endif

            var loadedDialogs = Resources.LoadAll<DialogBase>("Dialogs");
            foreach (var dialog in loadedDialogs) {
                if (!_dialogs.Contains(dialog)) {
                    _dialogs.Add(dialog);
                }
            }
        }

        public void ShowDialog(Type dialogType) {
            if (_shownDialog != null && _shownDialog.GetComponent(dialogType) != null) {
                return;
            }

            if (_dialogQueue.Any(d => d.GetComponent(dialogType) != null)) {
                return;
            }

            DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
            DialogBase dialogInstance = Instantiate(prefab, transform);
            DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
            dialogBase.gameObject.SetActive(false);
            AddToQueue(dialogBase);
        }

        public DialogBase ShowDialogWithData<T>(Type dialogType, T data) {
            if (_shownDialog != null && _shownDialog.GetComponent(dialogType) != null) {
                return null;
            }

            if (_dialogQueue.Any(d => d.GetComponent(dialogType) != null)) {
                return null;
            }

            DialogBase prefab = _dialogs.First(d => d.GetComponent(dialogType) != null);
            DialogBase dialogInstance = Instantiate(prefab, transform);
            //TODO remake via async
            DialogBase dialogBase = dialogInstance.GetComponent(dialogType) as DialogBase;
            if (dialogBase is DialogWithData<T> dialogWithData) {
                dialogWithData.SetData(data);
            }

            dialogBase.gameObject.SetActive(false);
            AddToQueue(dialogBase);
            return dialogBase;
        }

        private void AddToQueue(DialogBase dialog) {
            _dialogQueue.Enqueue(dialog);
            TryShowFromQueue();
        }

        private void TryShowFromQueue() {
            if (_shownDialog != null || _dialogQueue.Count == 0) {
                return;
            }

            _shownDialog = _dialogQueue.Dequeue();
            _shownDialog.Show(OnClosedDialog, OnHideUI);
        }

        private void OnClosedDialog() {
            _shownDialog = null;
            TryShowFromQueue();
        }
    }
}
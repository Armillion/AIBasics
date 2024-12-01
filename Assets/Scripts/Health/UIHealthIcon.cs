using System;
using KBCore.Refs;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Health {
    public class UIHealthIcon : MonoBehaviour {
        [SerializeField, Self]
        private Image _icon;

        [SerializeField, Child(Flag.ExcludeSelf)]
        private Image _flashFill;

        private void OnValidate() => this.ValidateRefs();

        public void Show(bool show) => _icon.color = _icon.color.WithAlpha(show ? 1f : 0f);

        public void Flash(float duration) {
            if (duration <= 0f)
                return;

            _flashFill.fillAmount = 0f;
            Tween.UIFillAmount(_flashFill, 1f, duration, Ease.OutCubic);
            Tween.UIFillAmount(_flashFill, 0f, float.Epsilon, Ease.OutCubic, startDelay: duration);
        }
    }
}

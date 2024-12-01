using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Health {
    public class UIPlayerHealth : MonoBehaviour {
        [SerializeField]
        private Health _playerHealth;

        [SerializeField, Child]
        private UIHealthIcon _healthIconTemplate;

        private UIHealthIcon[] _healthIcons;

        private void OnValidate() => this.ValidateRefs();

        private void Start() {
            _healthIcons = new UIHealthIcon[_playerHealth.MaxHealth];
            
            for (var i = 0; i < _playerHealth.MaxHealth; i++)
                _healthIcons[i] = Instantiate(_healthIconTemplate, transform);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            
            _playerHealth.onDamageTaken.AddListener(UpdateHealth);
            _healthIconTemplate.gameObject.SetActive(false);
            UpdateHealth();
        }

        private void UpdateHealth() {
            for (var i = 0; i < _healthIcons.Length; i++) {
                _healthIcons[i].Show(i < _playerHealth.CurrentHeath);
                _healthIcons[i].Flash(_playerHealth.InvulnerabilityPeriod);
            }
        }
    }
}
using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour {
    private const int MAX_ICONS = 10;
    
    [SerializeField]
    private Health _playerHealth;

    [SerializeField, Child]
    private UIHealthIcon _healthIconTemplate;

    private UIHealthIcon[] _healthIcons;

    private void OnValidate() => this.ValidateRefs();

    private void Start() {
        _healthIcons = new UIHealthIcon[Mathf.Min(_playerHealth.MaxHealth, MAX_ICONS)];
        
        for (var i = 0; i < _playerHealth.MaxHealth && i < MAX_ICONS; i++)
            _healthIcons[i] = Instantiate(_healthIconTemplate, transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        
        _playerHealth.onDamageTaken.AddListener(UpdateHealth);
        _healthIconTemplate.gameObject.SetActive(false);
        UpdateHealth();
    }

    private void UpdateHealth(object dealer = null) {
        for (var i = 0; i < _healthIcons.Length; i++) {
            _healthIcons[i].Show(i < _playerHealth.CurrentHealth);
            _healthIcons[i].Flash(_playerHealth.InvulnerabilityPeriod);
        }
    }
}

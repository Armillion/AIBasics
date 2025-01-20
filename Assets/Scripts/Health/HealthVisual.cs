using System;
using KBCore.Refs;
using UnityEngine;

public class HealthVisual : MonoBehaviour {
    [SerializeField, Parent]
    private Health _health;
    
    [SerializeField]
    private SpriteRenderer _healthSpriteRenderer;
    
    [SerializeField]
    private SpriteRenderer _armorSpriteRenderer;
    
    private static readonly int _spinnerPropertyId = Shader.PropertyToID("_Arc2");
    private Material RespawnSpinnerMaterial => _armorSpriteRenderer.material;
    
    private Vector3 _initialHealthScale;
    
    private void OnValidate() => this.ValidateRefs();

    private void OnEnable() {
        _initialHealthScale = _healthSpriteRenderer.transform.localScale;
        _health.onHealthChanged.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    private void OnDisable() => _health.onHealthChanged.RemoveListener(UpdateVisuals);

    private void UpdateVisuals() {
        _healthSpriteRenderer.transform.localScale = new Vector3(_initialHealthScale.x, _initialHealthScale.y, _initialHealthScale.z) * _health.HealthNormalized;
        RespawnSpinnerMaterial.SetFloat(_spinnerPropertyId, 360f * (1f - _health.ArmorNormalized));
    }
}

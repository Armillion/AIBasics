using System;
using KBCore.Refs;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField, Self]
    private CinemachineBasicMultiChannelPerlin _cameraNoise;

    [SerializeField, Min(0f)]
    private float _shakeAmplitudeIncrease = 0.5f;

    [SerializeField, Min(0f)]
    private float _shakeDuration = 0.2f;
    
    private float _originalAmplitudeGain;
    
    private void OnValidate() => this.ValidateRefs();

    private void Start() {
        _originalAmplitudeGain = _cameraNoise.AmplitudeGain;
    }

    public void Shake() {
        _cameraNoise.AmplitudeGain = _originalAmplitudeGain + _shakeAmplitudeIncrease;
        
        Tween.Custom(
            _cameraNoise.AmplitudeGain,
            _originalAmplitudeGain,
            duration: _shakeDuration,
            onValueChange: newVal => _cameraNoise.AmplitudeGain = newVal,
            Ease.InCubic
        );
    }
}
using System;
using System.Collections.Generic;
using ImprovedTimers;
using JetBrains.Annotations;
using KBCore.Refs;
using Physics;
using PrimeTween;
using Shooter.Agents;
using Shooter.Pickups.PickupStrategies;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils;

namespace Shooter.Pickups {
    [RequireComponent(typeof(SimpleCircleCollider))]
    public class Pickup : MonoBehaviour {
        public static IReadOnlyList<Pickup> AllPickups => _allPickups;
        
        [field: SerializeField]
        public PickupStrategy Strategy { get; private set; }
        
        [SerializeField, Min(0)]
        private int _effectValue = 10;
        
        [SerializeField, Min(float.Epsilon)]
        private float _respawnDuration = 10f;
        public bool _isRespawning = false;
        
        [SerializeField, Self]
        private SimpleCircleCollider _collider;

        [SerializeField]
        private SpriteRenderer _respawnSpinner;
        
        [SerializeField]
        private SpriteRenderer[] _visuals;
        
        private static List<Pickup> _allPickups = new();
        private static readonly int _spinnerPropertyId = Shader.PropertyToID("_Arc2");
        
        private Material RespawnSpinnerMaterial => _respawnSpinner.material;
        
        public bool Enabled {
            get => _collider.enabled;
            
            private set {
                _collider.enabled = value;
                
                foreach (SpriteRenderer visual in _visuals)
                    visual.color = visual.color.SetAlpha(Enabled ? 1f : 0.1f);
            }
        }
        
        private CountdownTimer _respawnTimer;

        private void OnValidate() {
            this.ValidateRefs();
            _collider.isTrigger = true;
        }

        private void Awake() {
            _respawnTimer = new CountdownTimer(_respawnDuration);
            _respawnTimer.OnTimerStop += () => { Enabled = true; _isRespawning = false; };
        }

        private void Start() => _allPickups.Add(this);

        private void AnimateRespawnSpinner() {
            RespawnSpinnerMaterial.SetFloat(_spinnerPropertyId, 360f);
            Tween.MaterialProperty(RespawnSpinnerMaterial, _spinnerPropertyId, 0f, _respawnDuration, Ease.OutSine);
        }

        [UsedImplicitly]
        public void OnSimpleTrigger(SimpleCircleCollider other) {
            if (!Strategy) {
                Debug.LogWarning("No strategy assigned to pickup", this);
                return;
            }
            
            if (!other.TryGetComponent(out Agent agent) || !Strategy.ApplyEffect(agent, _effectValue))
                return;

            _respawnTimer.Start();
            Enabled = false;
            _isRespawning = true;
            AnimateRespawnSpinner();
        }
    }
}
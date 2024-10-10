using System;
using ImprovedTimers;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons {
    public class Gun : MonoBehaviour {
        [Header("Config")]
        [SerializeField]
        private InputActionReference _shootAction;
        
        [SerializeField, Min(0f)]
        private float _fireRate = 0.1f;
        
        [Header("Visuals")]
        [SerializeField, Child]
        private Bullet _bulletTemplate;
        
        [SerializeField, Child]
        private Casing _casingTemplate;
        
        [SerializeField, FloatRangeSlider(-180f, 180f)]
        private FloatRange _casingsEjectionAngle = new(-30f, 30f);
        
        [SerializeField]
        private float _casingsEjectionForce = 1f;
        
        [Header("Shoot Effects")]
        [SerializeField]
        private CameraController _cameraController;

        [SerializeField, Self]
        private AudioSource _audioSource;
        
        private CountdownTimer _fireRateTimer;
        private bool _isShooting;
        private bool _isCooldownActive;

        private void OnValidate() {
            this.ValidateRefs();
            SetupTimer();
        }

        private void OnEnable() {
            _shootAction.action.started += StartShooting;
            _shootAction.action.canceled += StopShooting;
        }

        private void OnDisable() {
            _shootAction.action.started -= StartShooting;
            _shootAction.action.canceled -= StopShooting;
        }

        private void Start() {
            _bulletTemplate.gameObject.SetActive(false);
            _casingTemplate.gameObject.SetActive(false);
            SetupTimer();
        }

        private void Update() {
            if (_isShooting) Shoot();
        }

        private void StartShooting(InputAction.CallbackContext _ = default) {
            _isShooting = true;
            
            if (_fireRateTimer.IsRunning) return;
            
            Shoot();
            _fireRateTimer.Start();
        }
        
        private void StopShooting(InputAction.CallbackContext _ = default) => _isShooting = false;

        private void Shoot() {
            if (_isCooldownActive)
                return;
            
            Bullet bullet = Instantiate(_bulletTemplate, transform.position, transform.rotation);
            bullet.gameObject.SetActive(true);
            bullet.Fire();
            
            ShootVisuals();
            _fireRateTimer.Start();
        }

        private void ShootVisuals() {
            Vector2 ejectionDirection = Quaternion.Euler(0, 0, UnityEngine.Random.Range(_casingsEjectionAngle.min, _casingsEjectionAngle.max)) * transform.right;
            
            Casing casing = Instantiate(_casingTemplate, transform.position, transform.rotation);
            casing.gameObject.SetActive(true);
            casing.Eject(ejectionDirection, _casingsEjectionForce);
            
            _cameraController.Shake();
            _audioSource.Play();
        }

        private void SetupTimer() {
            _fireRateTimer = new CountdownTimer(_fireRate);
            _fireRateTimer.OnTimerStart += () => _isCooldownActive = true;
            _fireRateTimer.OnTimerStop += () => _isCooldownActive = false;
        }

        private void OnDrawGizmosSelected() {
            Vector3 ejectionDirection = Quaternion.Euler(0, 0, _casingsEjectionAngle.min) * transform.right;
            Gizmos.DrawRay(transform.position, ejectionDirection);
            
            ejectionDirection = Quaternion.Euler(0, 0, _casingsEjectionAngle.max) * transform.right;
            Gizmos.DrawRay(transform.position, ejectionDirection);
        }
    }
}
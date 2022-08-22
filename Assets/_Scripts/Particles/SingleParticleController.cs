using UnityEngine;
using ARLuft.UI;
using Lean.Touch;

namespace ARLuft
{
    [RequireComponent(typeof(Rigidbody))]
    public class SingleParticleController : MonoBehaviour
    {
        [SerializeField] private ParticlesManager particlesManager;
        public ParticleType Type;

        private Rigidbody _rigidbody;
        private LeanFinger _currentFinger;
        private void OnEnable()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            LeanTouch.OnFingerDown += HandleInteraction;
        }
        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= HandleInteraction;
        }

        private void HandleInteraction(LeanFinger finger)
        {
            if (ARUIController.Instance.GamePaused) return;

            _currentFinger = finger;

            if (Physics.Raycast(_currentFinger.GetRay(), out RaycastHit hit) && hit.transform.Equals(transform))
            {
                Destroy(gameObject);
                ARUIController.Instance.DecreaseCounter();
            }
        }

        private void Update()
        {
            if (ARUIController.Instance.GamePaused) return;

            if (_currentFinger!=null && _currentFinger.IsActive)
                HandleInteraction(_currentFinger);
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.CompareTag(particlesManager.tag))
            {
                Vector3 direction = Vector3.Reflect(_rigidbody.velocity.normalized, coll.ClosestPoint(gameObject.transform.position).normalized);
                _rigidbody.velocity = direction * Mathf.Max(_rigidbody.velocity.magnitude, 1f);
                _rigidbody.AddTorque(direction, ForceMode.Acceleration);
            }
        }
    }
}

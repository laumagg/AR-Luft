using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLuft.UI;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace ARLuft
{
    public class SecurityZone : MonoBehaviour
    {
        [Range(1, 10)]
        [SerializeField] private float securityRadius = 5;
        [SerializeField] private Image backgroundOverlay;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Color warningColor;

        [HideInInspector] public bool OutOfSecurityZone = false;
        [HideInInspector] public UnityEvent OnOutOfSecurityZone = new();

        private string _cameraTag;
        private Color _normalBackgroundColor;

        private void Start()
        {
            transform.localScale = securityRadius * Vector3.one;
            _cameraTag = Camera.main.gameObject.tag;
            _normalBackgroundColor = backgroundOverlay.color;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_cameraTag))
            {
                warningText.gameObject.SetActive(true);
                backgroundOverlay.color = warningColor;
                OutOfSecurityZone = true;
                OnOutOfSecurityZone?.Invoke();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_cameraTag))
            {
                warningText.gameObject.SetActive(false);
                ARUIController.Instance.Play();
                OutOfSecurityZone = false;
                WaitToChangeColor();
            }
        }

        private async void WaitToChangeColor()
        {
            while (backgroundOverlay.gameObject.activeSelf)
            {
                await Task.Delay(50);
            }
            backgroundOverlay.color = _normalBackgroundColor;
        }

        private void OnDestroy()
        {
            OnOutOfSecurityZone.RemoveAllListeners();
        }
    }
}

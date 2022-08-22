using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLuft.Data;
using TMPro;

namespace ARLuft.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ParticlesCounter : MonoBehaviour
    {
        private TextMeshProUGUI _counterText;

        private int _counter;
        private string _units = "";

        private void Start()
        {
            _counterText = GetComponent<TextMeshProUGUI>();

            SetCounterValue();
            ShowCounter(false);
        }

        //Counter
        public void ShowCounter(bool enable)
        {
            if (enable)
            {
                _counterText.gameObject.SetActive(true);
                _counterText.gameObject.LeanMoveLocalY(-500, 0.5f).setEaseInExpo();
                return;
            }

            _counterText.gameObject.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo().setOnComplete(() =>
            {
                _counterText.gameObject.SetActive(false);
            });
        }
        public void DecreaseCounterByOne()
        {
            _counter--;
            _counterText.text = $"{_counter} {_units}";
            if (_counter == 0)
                ARUIController.Instance.ShowWinUI();
        }
        public void SetCounterValue()
        {
            _counter = DataManager.Instance.SelectedPollutantForAR.value;

            if (DataManager.Instance.SelectedPollutantComponents != null)
                _units = DataManager.Instance.SelectedPollutantComponents.unit;
            _counterText.text = $"{_counter} {_units}";
        }

    } 
}

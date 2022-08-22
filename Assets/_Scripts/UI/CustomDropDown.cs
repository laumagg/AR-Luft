using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class CustomDropDown : TMP_Dropdown
    {
        public UnityEvent OnShowDropdownContent = new();
        private ScrollRect m_scroll;
        private RectTransform m_rectTransform;
        private PointerEventData _data;
        protected override void Start()
        {
            base.Start();

            m_rectTransform = gameObject.GetComponent<RectTransform>();
            if (TryGetComponent<StationListEntry>(out _))
                m_scroll = UIController.Instance.StationListUIController.ListScrollRect;
            else
                m_scroll = UIController.Instance.PollListUIController.ListScrollRect;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            _data = eventData;
            if (m_rectTransform.position.y < (Screen.height / 3))
            {
                LeanTween.moveY(m_scroll.content.gameObject,
                    m_scroll.content.gameObject.transform.position.y + 400f,
                    0.2f);
                Invoke(nameof(FinishOnPointerClick), .2f);
            }
            else
            {
                FinishOnPointerClick();
            }
        }
        private void FinishOnPointerClick()
        {
            base.OnPointerClick(_data);
            OnShowDropdownContent?.Invoke();
        }

        public void AssignTextAsSingleOption(string text)
        {
            options.Clear();
            options.Add(new OptionData(text));
            RefreshShownValue();
        }
    }
}

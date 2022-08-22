using ARLuft.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class ListEntry : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] protected TextMeshProUGUI _titleText;
        [SerializeField] protected TextMeshProUGUI _descriptionText;

        [Header("Dropdown elements")]
        [SerializeField] protected bool _hasDropdown = true;
        [SerializeField] protected CustomDropDown _dropdown;
        [SerializeField] protected RectTransform _arrowImage;
        [SerializeField] protected RectTransform _siblingsParent;

        protected int _siblingsIndex;
        protected readonly List<RectTransform> _tempSiblingsList = new();
        protected  List<RectTransform> _entriesList = new();

        protected float _rotation;
        protected readonly WaitForSeconds _wait = new(0.1f);

        private void OnEnable()
        {
            if (_hasDropdown)
            {
                _siblingsIndex = transform.GetSiblingIndex();
                _dropdown.OnShowDropdownContent.AddListener(UIAnimationsOnShow);
            }
        }
        private void OnDisable()
        {
            if (_hasDropdown)
                _dropdown.OnShowDropdownContent.RemoveListener(UIAnimationsOnShow);
        }

        private void SetEntriesList()
        {
            if (TryGetComponent<StationListEntry>(out _))
                _entriesList = UIController.Instance.StationListUIController.GetEntriesList();
            else
                _entriesList = UIController.Instance.PollListUIController.GetEntriesList();
        }

        //UI animation methods on show dropdown
        private void UIAnimationsOnShow()
        {
            _siblingsIndex = transform.GetSiblingIndex();
            SetEntriesList();
            RotateArrow(true);
            PushSiblings();
            StartCoroutine(WaitForHide());
        }
        private void RotateArrow(bool open)
        {
            LeanTween.cancel(_arrowImage);

            if (open)
                _rotation = 180f;
            else
                _rotation = 0f;

            _arrowImage.LeanRotateZ(_rotation, 0.2f);
        }
        private void PushSiblings()
        {
            int sibIndex;
            foreach (RectTransform sibling in _entriesList)
            {
                sibIndex = sibling.GetSiblingIndex();

                if (sibIndex > _siblingsIndex)
                {
                    _tempSiblingsList.Add(sibling);
                    sibling.transform.SetParent(_siblingsParent);
                }
            }

            LeanTween.cancel(_siblingsParent);
            LeanTween.moveY(_siblingsParent, _siblingsParent.anchoredPosition.y - 300f, 0.2f);
        }
        private void PullSiblings()
        {
            LeanTween.cancel(_siblingsParent);
            LeanTween.moveY(_siblingsParent, _siblingsParent.anchoredPosition.y + 300f, 0.2f).setOnComplete(() =>
            {
                //Reset
                foreach (RectTransform tempEntry in _tempSiblingsList)
                    tempEntry.transform.SetParent(_siblingsParent.parent);
                _tempSiblingsList.Clear();
            });
        }
        private IEnumerator WaitForHide()
        {
            while (_dropdown.IsExpanded)
                yield return _wait;

            RotateArrow(false);
            PullSiblings();
        }
    }
}

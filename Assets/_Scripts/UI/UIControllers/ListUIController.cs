using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARLuft.Data;

namespace ARLuft.UI
{
    public class ListUIController : MonoBehaviour
    {
        public Transform listViewUI;

        [Header("Dynamic list view")]
        public ScrollRect ListScrollRect;
        [SerializeField] protected RectTransform _listEntriesParent;
        [Tooltip("Station template")]
        [SerializeField] protected RectTransform _templateListEntry;


        protected List<RectTransform> m_EntriesList = new();

        protected virtual void Start()
        {
            ListScrollRect.verticalNormalizedPosition = 1;
        }
        protected GameObject CreateListEntry(Vector2 position)
        {
            GameObject go = Instantiate(_templateListEntry.gameObject, _listEntriesParent);
            go.SetActive(true);
            go.tag = "Untagged";
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            m_EntriesList.Add(rectTransform);
            return go;
        }
        public List<RectTransform> GetEntriesList() { return m_EntriesList; }
        protected void DestroyOldListEntries()
        {
            if (_listEntriesParent.transform.childCount == 2) return;
            Transform temp;
            for (int i = 0; i < _listEntriesParent.transform.childCount; i++)
            {
                temp = _listEntriesParent.GetChild(i);
                if (temp.CompareTag(_listEntriesParent.tag)) continue;

                Destroy(_listEntriesParent.GetChild(i).gameObject);
            }
            m_EntriesList.Clear();
        }
    }
}


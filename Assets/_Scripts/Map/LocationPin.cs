using ARLuft.Data;
using ARLuft.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using Lean.Common;

namespace ARLuft.Map
{
    [RequireComponent(typeof(LeanSelectableByFinger))]
    public class LocationPin : MonoBehaviour
    {
        private LeanSelectableByFinger leanSelectable;
        [HideInInspector] public StationsData StationData { get; set; }
        private void OnEnable()
        {
            leanSelectable = gameObject.GetComponent<LeanSelectableByFinger>();
            leanSelectable.OnSelected.AddListener(Show);
        }
        private void OnDisable()
        {
            leanSelectable.OnSelected.RemoveListener(Show);
        }

        private void Show(LeanSelect action)
        {            
            UIController.Instance.StartUIController.ShowStationData(StationData);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean.Touch;
using Lean.Common;

namespace ARLuft
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(LeanSelectableByFinger))]
    public class GraphLabels : MonoBehaviour
    {
        [SerializeField] private TextMeshPro label;

        public GameObject labelContainer;
        public MeshRenderer PointMeshRenderer;


        public string LabelText
        {
            get { return labelValue; }
            set
            {
                labelValue = value;
                label.text = labelValue;
            }
        }

        //For selection
        private bool selectablePoint;
        public LeanSelectableByFinger leanSelectable;
        private string labelValue;

        public void SetSelectable()
        {
            selectablePoint = true;
            leanSelectable = GetComponent<LeanSelectableByFinger>();
            
            //Listeners
            leanSelectable.OnSelected.AddListener((LeanSelect action) => GraphManager.Instance.HighlightStation(labelValue));
            leanSelectable.OnDeselected.AddListener((LeanSelect action) => GraphManager.Instance.ResetGraphColors());
        }

        private void OnDisable()
        {
            if (selectablePoint)
            {
                leanSelectable.OnSelected.RemoveAllListeners();
                leanSelectable.OnDeselected.RemoveAllListeners();
            }
        }
    }
}
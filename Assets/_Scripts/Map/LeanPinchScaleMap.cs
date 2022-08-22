namespace ARLuft.Map
{
    using Mapbox.Unity.Map;
    using UnityEngine;
    using System;
    using Lean.Touch;
    using CW.Common;

    public class LeanPinchScaleMap : LeanPinchScale
    {
        [SerializeField] AbstractMap _mapManager;
        private void Start()
        {
            _mapManager = gameObject.GetComponent<AbstractMap>();
        }

        protected override void Update()
        {
            // Store
            var oldScale = transform.localPosition;

            // Get the fingers we want to use
            var fingers = Use.UpdateAndGetFingers();

            // Calculate pinch scale, and make sure it's valid
            var pinchScale = LeanGesture.GetPinchScale(fingers);

            if (pinchScale != 1.0f)
            {
                pinchScale = Mathf.Pow(pinchScale, sensitivity);

                // Perform the translation if this is a relative scale
                if (relative == true)
                {
                    var pinchScreenCenter = LeanGesture.GetScreenCenter(fingers);

                    if (transform is RectTransform)
                    {
                        TranslateUI(pinchScale, pinchScreenCenter);
                    }
                    else
                    {
                        Translate(pinchScale, pinchScreenCenter);
                    }
                }

                transform.localScale *= pinchScale;

                remainingScale += transform.localPosition - oldScale;

                //var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + pinchScale * _zoomSpeed, 21.0f));
                var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom * pinchScale, 21.0f));
                if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
                {
                    _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
                }
            }

            // Get t value
            var factor = CwHelper.DampenFactor(damping, Time.deltaTime);

            // Dampen remainingDelta
            var newRemainingScale = Vector3.Lerp(remainingScale, Vector3.zero, factor);

            // Shift this transform by the change in delta
            transform.localPosition = oldScale + remainingScale - newRemainingScale;

            // Update remainingDelta with the dampened value
            remainingScale = newRemainingScale;
        }
    }
}
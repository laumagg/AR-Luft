namespace ARLuft.Map
{
    using Lean.Touch;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LeanDragTranslateMap : LeanDragTranslate
    {
        [SerializeField] AbstractMap _mapManager;
        private void Start()
        {
            _mapManager = gameObject.GetComponent<AbstractMap>();
        }
        protected override void Translate(Vector2 screenDelta)
        {
            //Debug.Log($"translate {screenDelta.x} and {screenDelta.y} ");

            base.Translate(screenDelta);

            float factor = Conversions.GetTileScaleInMeters(0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(-screenDelta.x * factor, -screenDelta.y * factor));
            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
        }
    }
}


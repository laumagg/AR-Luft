using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARLuft.Map
{
    public class ReloadMap : MonoBehaviour
    {
        private AbstractMap _map;
        private Camera _camera;
        private Vector3 _cameraStartPos;
        private Coroutine _reloadRoutine;
        private WaitForSeconds _wait;

        private void Awake()
        {
            //_map = MapManager.Instance.Map;
            _camera = Camera.main;
            _cameraStartPos = _camera.transform.position;
            _wait = new WaitForSeconds(.3f);
            _map.OnUpdated += () => Reload(_map.Zoom);

        }
        private void OnDisable()
        {
            _map.OnUpdated -= () => Reload(_map.Zoom);
        }

        private void Reload(float value)
        {
            if (_reloadRoutine != null)
            {
                StopCoroutine(_reloadRoutine);
                _reloadRoutine = null;
            }
            _reloadRoutine = StartCoroutine(ReloadAfterDelay());
        }
        private IEnumerator ReloadAfterDelay()
        {
            yield return _wait;
            _camera.transform.position = _cameraStartPos;
            _map.UpdateMap(_map.CenterLatitudeLongitude, _map.Zoom);
            _reloadRoutine = null;
        }
    }

}


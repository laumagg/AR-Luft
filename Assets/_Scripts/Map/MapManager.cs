using System.Collections.Generic;
using UnityEngine;
using ARLuft.Data;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;

namespace ARLuft.Map
{
    public class MapManager : MonoBehaviour
    {
        public AbstractMap Map;
        [SerializeField] private Transform _markersParent;
        [SerializeField] private GameObject _markerPrefab;
        [SerializeField] private float _scale = 1;

        [Header("Stations' colors")]
        [SerializeField] private Color trafficStationColor;
        [SerializeField] private Color suburbStationColor;
        [SerializeField] private Color backgroundStationColor;


        private string[] _locationStrings;
        private Vector2d[] _locations;
        private List<GameObject> _spawnedObjects;
        private StationsData[] _stationsData;
        private Vector3 _scaleVec = new();
        private void OnEnable()
        {
            DataManager.Instance.StationsDataAvailable.AddListener(SpawnDataOnMap);
        }
        private void OnDisable()
        {
            DataManager.Instance.StationsDataAvailable?.RemoveListener(SpawnDataOnMap);
        }

        private void SpawnDataOnMap()
        {
            _stationsData = DataManager.Instance.RootStations.data;

            //Setup
            _locationStrings = new string[_stationsData.Length];
            _locations = new Vector2d[_locationStrings.Length];
            _spawnedObjects = new List<GameObject>();

            SpawnMarkers();
        }
        private void SpawnMarkers()
        {
            LocationPin tempPin;
            _scaleVec.x = _scale;
            _scaleVec.y = 0.1f;
            _scaleVec.z = _scale;

            for (int i = 0; i < _stationsData.Length; i++)
            {
                _locationStrings[i] = LatLngToString(_stationsData[i]);
                _locations[i] = Conversions.StringToLatLon(_locationStrings[i]);

                //Create location pin
                GameObject tempInstance;
                tempInstance = Instantiate(_markerPrefab, _markersParent);
                tempInstance.transform.localPosition = Map.GeoToWorldPosition(_locations[i], true);
                tempInstance.transform.localScale = _scaleVec;
                tempInstance.GetComponent<Renderer>().material.color = SetPinColor(_stationsData[i].stationgroups[0]);
                
                //Set data
                tempPin = tempInstance.GetComponent<LocationPin>();
                tempPin.StationData = _stationsData[i];

                //Save
                _spawnedObjects.Add(tempInstance);
            }
        }
        private Color SetPinColor(string stationType)
        {
            Color color = stationType switch
            {
                "traffic" => trafficStationColor,
                "background" => backgroundStationColor,
                _ => suburbStationColor,
            };
            return color;
        }

        private string LatLngToString(StationsData station)
        {
            return $"{station.lat}, {station.lng}";
        }

        private void Update()
        {
            if (_spawnedObjects == null || !gameObject.activeSelf) return;

            //Update markers position 
            for (int i = 0; i < _spawnedObjects.Count; i++)
            {
                _spawnedObjects[i].transform.localPosition = Map.GeoToWorldPosition(_locations[i], true);
            }
        }
    }
}


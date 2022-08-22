using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLuft.Data;

namespace ARLuft
{
    public class GraphManager : Singleton<GraphManager>
    {
        [Header("Managers")]
        [SerializeField] private GraphCoordinates coordinates;
        [SerializeField] private GraphDataConnector dataConnector;
        [SerializeField] private GraphDataPoints dataPoints;

        [Header("Visualization")]
        [SerializeField] private Transform graphParent;
        [Range(0, 1)]
        [SerializeField] private float graphScale = 0.1f;
        [Range(1, 10)]
        [SerializeField] private float graphOffset = 3f;
        public bool GraphActive { get; private set; }
        public Color DefaultColor = Color.black;

        private readonly Dictionary<string, Color> _stationColors = new();
        private readonly List<GraphStationOptics> _stationOptics = new();

        //General
        private void Start()
        {
            GraphActive = false;

            graphParent.localScale = Vector3.one * graphScale;
            graphParent.gameObject.SetActive(false);

            GameManager.Instance.OnSwitchToGraph.AddListener(() => ChangeState(true));
            DataManager.Instance.HistoricalDataAvailable.AddListener(CreateGraf);
        }
        public void ChangeState(bool enable)
        {
            GraphActive = enable;

            if (enable)
                StartCoroutine(DataManager.Instance.GetHistorialData());
        }
        public void ShowGraph()
        {
            graphParent.gameObject.SetActive(true);
        }


        //Graph creation
        private void CreateGraf()
        {
            //Creation of coordinate system
            coordinates.UpdateCoordinateSystem();

            //handle random colors
            SetStationColors();
            coordinates.SetColorToStationPoints(_stationColors);

            //Create data points
            dataPoints.SetDataPoints();
            dataConnector.ConnectData(_stationOptics);

            //Update position and activate
            UpdateGraphPosition();
        }
        private void SetStationColors()
        {
            foreach (GraphData pointData in DataManager.Instance.GraphDataList)
            {
                if (!_stationColors.ContainsKey(pointData.Station))
                {
                    _stationColors[pointData.Station] = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                }
            }
        }
        public Vector3 GetPositionDataPoint(GraphData data)
        {
            return coordinates.FindCoordinates(data);
        }
        public void FillStationOpticsList(Dictionary<string, List<Renderer>> dataPoints)
        {
            foreach (string station in dataPoints.Keys)
            {
                GraphStationOptics optics = new(
                stationName: station,
                stationColor: _stationColors[station],
                pointsList: dataPoints[station]
                );

                _stationOptics.Add(optics);
            }
        }

        //Colors
        public Color GetStationColor(string stationName)
        {
            return _stationColors[stationName];
        }
        public void HighlightStation(string selectedLabel)
        {
            ResetGraphColors();

            coordinates.HighlightLabelPoint(selectedLabel);
            dataPoints.HighlightDataPoints(_stationOptics, selectedLabel);
            dataConnector.HighlightConnectors(selectedLabel);
        }
        public void ResetGraphColors()
        {
            coordinates.SetColorToStationPoints(_stationColors);
            dataPoints.SetStationColors(_stationOptics);
            dataConnector.SetConnectorsColors(_stationColors);
        }


        private void UpdateGraphPosition()
        {
            Transform playerPos = Camera.main.transform;
            Vector3 offset = (playerPos.forward * graphOffset) + playerPos.position;
            graphParent.localPosition = offset;
        }
        private void OnDestroy()
        {
            DataManager.Instance.GraphDataList = new();
            DataManager.Instance.RootYearPollution = new();
        }
    }

    public class GraphStationOptics
    {
        public string StationName;
        public Color StationColor { get; set; }
        public List<Renderer> StationDataPoints = new();
        public GraphStationOptics(string stationName, Color stationColor, List<Renderer> pointsList)
        {
            StationName = stationName;
            StationColor = stationColor;
            StationDataPoints = pointsList;
        }
    }
}

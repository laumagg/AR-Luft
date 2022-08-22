using ARLuft.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARLuft
{
    public class GraphDataPoints : MonoBehaviour
    {
        [SerializeField] private GameObject dataPointPrefab;
        [SerializeField] private Transform dataParent;

        //Creation
        public void SetDataPoints()
        {
            Dictionary<string, List<Renderer>> dataPoints = new();

            Vector3 pos;
            foreach (GraphData pointData in DataManager.Instance.GraphDataList)
            {
                //Position
                pos = GraphManager.Instance.GetPositionDataPoint(pointData);
                if (pos == Vector3.one * -1)
                    continue;

                //Instantiation
                GameObject dataPoint = Instantiate(dataPointPrefab,
                    transform.position, transform.rotation, dataParent);
                dataPoint.transform.localPosition = pos;

                //Optics
                dataPoint.GetComponent<GraphLabels>().LabelText = pointData.PollutantValue.ToString();
                Renderer rend = dataPoint.GetComponent<Renderer>();
                rend.material.color = GraphManager.Instance.GetStationColor(pointData.Station);


                //Add to list
                if (!dataPoints.ContainsKey(pointData.Station))
                    dataPoints[pointData.Station] = new();
                dataPoints[pointData.Station].Add(rend);
            }

            GraphManager.Instance.FillStationOpticsList(dataPoints);
        }


        //Colors
        public void HighlightDataPoints(List<GraphStationOptics> dataPoints, string stationName)
        {
            Color defaultColor = GraphManager.Instance.DefaultColor;
            foreach (GraphStationOptics station in dataPoints)
            {
                if (station.StationName == stationName) continue;

                foreach (Renderer rend in station.StationDataPoints)
                {
                    rend.material.color = defaultColor;
                }
            }
        }
        public void SetStationColors(List<GraphStationOptics> dataPoints)
        {
            Color tempColor;
            foreach (GraphStationOptics station in dataPoints)
            {
                tempColor = station.StationColor;

                foreach (Renderer rend in station.StationDataPoints)
                {
                    rend.material.color = tempColor;
                }
            }
        }

    }
}
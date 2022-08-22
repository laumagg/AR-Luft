using ARLuft.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Lean.Common;

namespace ARLuft
{
    public class GraphCoordinates : MonoBehaviour
    {
        [Header("Axes")]
        [SerializeField] private GameObject[] XYZAxis;
        [SerializeField] private TextMeshPro[] XYZAxisTitle;
        [SerializeField] private Transform[] XYZPosition;

        [Header("Points prefabs")]
        [SerializeField] private GraphLabels[] XYZPoint;

        [Header("Planes")]
        [SerializeField] private Transform XYPlane;
        [SerializeField] private Transform XZPlane;
        [SerializeField] private Transform YZPlane;

        [Header("Others")]
        [SerializeField] private int valueRangeQuantity = 10;

        private int _xAxisLength, _yAxisLength, _zAxisLength;
        private List<string> _xAxisLabels, _yAxisLabels, _zAxisLabels;
        private List<GraphLabels> _xAxisPointsList, _yAxisPointsList, _zAxisPointsList;
        private int _rangeDistance;
        public void UpdateCoordinateSystem()
        {
            //Get data for coordinates
            SaveAxisData();
            _xAxisLength = _xAxisLabels.Count;
            _yAxisLength = _yAxisLabels.Count;
            _zAxisLength = _zAxisLabels.Count;

            //Create box
            ScaleGraphBox();

            //Update axes
            UpdateXAxis();
            UpdateYAxis();
            UpdateZAxis();
            SetAxisTitles();
        }

        // Labels data
        private void SaveAxisData()
        {
            _xAxisLabels = new();
            _yAxisLabels = new();
            _zAxisLabels = new();

            List<int> yValues = new();

            //Labels of axes 
            foreach (GraphData graphData in DataManager.Instance.GraphDataList)
            {
                //x axis
                if (!_xAxisLabels.Contains(graphData.Year))
                    _xAxisLabels.Add(graphData.Year);

                //y values
                yValues.Add(graphData.PollutantValue);

                //z values
                if (!_zAxisLabels.Contains(graphData.Station))
                    _zAxisLabels.Add(graphData.Station);
            }

            //Show only every fifth value if too many years
            _xAxisLabels.Sort();
            if (_xAxisLabels.Count > 10)
                _xAxisLabels = SaveOnlyEveryFifthValue(_xAxisLabels);

            //Labels of Y axis -> Number ranges
            yValues.Sort();
            if (yValues.Last() > 10)
                _yAxisLabels = CreateValueRanges(yValues.Last());
            else
                yValues.ForEach(x => _yAxisLabels.Add(x.ToString()));
        }
        private List<string> SaveOnlyEveryFifthValue(List<string> values)
        {
            List<string> newList = new();

            for (int i = values.Count - 1; i >= 0; i -= 5)
            {
                if (i >= 0)
                    newList.Add(values[i]);
            }
            newList.Reverse();
            return newList;
        }
        private List<string> CreateValueRanges(int maxValue, int minValue = 0)
        {
            List<string> rangesList = new();

            //Setup
            int[] rangeDivisions = new int[valueRangeQuantity + 1];
            _rangeDistance = Mathf.RoundToInt(maxValue / valueRangeQuantity) + 1;

            for (int i = 0; i < rangeDivisions.Length; i++)
            {
                if (i == 0)
                    rangeDivisions[i] = minValue;
                else
                    rangeDivisions[i] = rangeDivisions[i - 1] + _rangeDistance;

                rangesList.Add(rangeDivisions[i].ToString());
            }

            return rangesList;
        }
        private void SetAxisTitles()
        {
            //X Axis
            XYZAxisTitle[0].text = "Jahr";
            //pos = XAxis.GetComponentInChildren<MeshRenderer>().bounds.extents;
            XYZAxisTitle[0].gameObject.transform.position = XYZPosition[0].position;

            //Y Axis
            XYZAxisTitle[1].text = $"Jahresmittelwert " +
                $"{DataManager.Instance.SelectedPollutantForAR.core.ToUpper()} " +
                $"[{DataManager.Instance.SelectedPollutantComponents.unit}]";
            XYZAxisTitle[1].gameObject.transform.position = XYZPosition[1].position;

            //Z Axis
            XYZAxisTitle[2].text = "Station";
            XYZAxisTitle[2].gameObject.transform.position = XYZPosition[2].position;
        }

        // Coordinates
        private void ScaleGraphBox()
        {
            XYPlane.localScale = new Vector3(_xAxisLength + 1, _yAxisLength + 1, XYPlane.localScale.z);
            XZPlane.localScale = new Vector3(_xAxisLength + 1, XZPlane.localScale.y, _zAxisLength + 1);
            YZPlane.localScale = new Vector3(YZPlane.localScale.x, _yAxisLength + 1, _zAxisLength + 1);
        }
        private void UpdateXAxis()
        {
            _xAxisPointsList = new();

            //Scale of axis
            XYZAxis[0].transform.localScale = new Vector3(
                1 + (_xAxisLength - 1),
                XYZAxis[0].transform.localScale.y,
                XYZAxis[0].transform.localScale.z);

            //Creation of label point
            Vector3 position;

            for (int i = 0; i < _xAxisLength; i++)
            {
                //position
                if (i == 0)
                    position = transform.localPosition + Vector3.right;
                else
                    position = _xAxisPointsList[i - 1].transform.localPosition + Vector3.right;
                position.z = 0;

                //instantiation
                GraphLabels temp = Instantiate(XYZPoint[0], transform.position, transform.rotation, transform);
                temp.transform.localPosition = position;
                
                //Save to list
                _xAxisPointsList.Add(temp);
            }

            AssignAxisLabels(_xAxisLabels, _xAxisPointsList);
        }
        private void UpdateYAxis()
        {
            _yAxisPointsList = new();

            //Scale of axis
            XYZAxis[1].transform.localScale = new Vector3(
                XYZAxis[1].transform.localScale.x,
                1 + (_yAxisLength - 1),
                XYZAxis[1].transform.localScale.z);

            //Creation of label point
            Vector3 position;
            for (int i = 0; i < _yAxisLength; i++)
            {
                if (i == 0)
                    position = transform.localPosition;
                else
                    position = _yAxisPointsList[i - 1].transform.localPosition + Vector3.up;
                position.z = 0;

                GraphLabels temp = Instantiate(XYZPoint[1], transform.position, transform.rotation, transform);
                temp.transform.localPosition = position;

                _yAxisPointsList.Add(temp);
            }

            AssignAxisLabels(_yAxisLabels, _yAxisPointsList);
        }
        private void UpdateZAxis()
        {
            _zAxisPointsList = new();

            //Scale of axis
            XYZAxis[2].transform.localScale = new Vector3(
                XYZAxis[2].transform.localScale.x,
                XYZAxis[2].transform.localScale.y,
                1 + (_zAxisLength - 1));

            //Creation of label point
            Vector3 position;
            for (int i = 0; i < _zAxisLength; i++)
            {
                if (i == 0)
                    position = transform.localPosition + Vector3.back;
                else
                    position = _zAxisPointsList[i - 1].transform.localPosition + Vector3.back;

                GraphLabels temp = Instantiate(XYZPoint[2], transform.position, transform.rotation, transform);
                temp.transform.localPosition = position;
                temp.SetSelectable();

                _zAxisPointsList.Add(temp);
            }

            AssignAxisLabels(_zAxisLabels, _zAxisPointsList);
        }
        private void AssignAxisLabels(List<string> axisDataList, List<GraphLabels> labelsList)
        {
            for (int i = 0; i < axisDataList.Count; i++)
            {
                labelsList[i].LabelText = axisDataList[i];
            }
        }

        // Find relative position
        public Vector3 FindCoordinates(GraphData data)
        {
            Vector3 pos = new();

            //X coordinate
            int xPos = _xAxisLabels.FindIndex(x => x == data.Year);
            if (xPos == -1)
                return Vector3.one * -1;

            pos.x = _xAxisPointsList[xPos].transform.localPosition.x;

            //Y coordinate
            pos.y = FindYPosition(data.PollutantValue);

            //Z coordinate
            int zPos = _zAxisLabels.FindIndex(z => z == data.Station);
            pos.z = _zAxisPointsList[zPos].transform.localPosition.z;

            return pos;
        }
        private float FindYPosition(int value)
        {
            float distanceRange = Mathf.Abs(_yAxisPointsList[0].transform.localPosition.y - _yAxisPointsList[1].transform.localPosition.y);
            distanceRange /= _rangeDistance;

            return value * distanceRange;
        }

        // Colors
        public void SetColorToStationPoints(Dictionary<string, Color> stationColors)
        {
            int index;
            foreach (string station in stationColors.Keys)
            {
                index = _zAxisPointsList.FindIndex(x => x.LabelText == station);

                //Set color
                if (index != -1)
                    _zAxisPointsList[index].PointMeshRenderer.material.color = stationColors[station];
            }
        }
        public void HighlightLabelPoint(string label)
        {
            Color defaultColor = GraphManager.Instance.DefaultColor;

            foreach (GraphLabels stationLabel in _zAxisPointsList)
            {
                if (stationLabel.LabelText != label)
                    stationLabel.PointMeshRenderer.material.color = defaultColor;
            }
        }
    }

    public enum XYZ
    {
        x = 0,
        y = 1,
        z = 2
    }
}

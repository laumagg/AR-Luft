using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARLuft
{
    [RequireComponent(typeof(GraphCoordinates))]
    public class GraphDataConnector : MonoBehaviour
    {
        [SerializeField] private GameObject connectorPrefab;
        [SerializeField] private Transform connectorParent;
        [SerializeField] private float scaleOffset = 1f;
        private Renderer connectorRenderer;
        private readonly Dictionary<string, List<Renderer>> connectors = new();

        private void Start()
        {
            connectorPrefab.SetActive(false);
            connectorRenderer = connectorPrefab.GetComponent<Renderer>();
        }

        //Creation
        public void ConnectData(List<GraphStationOptics> stationPoints)
        {
            Vector3 pos, scale;
            Quaternion rot;
            Transform temp, nextTemp;

            if (!connectorRenderer)
                connectorRenderer = connectorPrefab.GetComponent<Renderer>();

            foreach (GraphStationOptics station in stationPoints)
            {
                connectorRenderer.material.color = station.StationColor;

                //Create new renderers list
                connectors[station.StationName] = new();

                //First value
                nextTemp = station.StationDataPoints[0].transform;
                for (int i = 0; i < station.StationDataPoints.Count - 1; i++)
                {
                    temp = nextTemp;
                    nextTemp = station.StationDataPoints[i + 1].transform;

                    GameObject conn = Instantiate(connectorPrefab, connectorParent);
                    Renderer connRenderer = conn.GetComponent<Renderer>();
                    connRenderer.material.color = connectorRenderer.material.color;

                    // Scale connector
                    scale = temp.localScale * scaleOffset;
                    scale.z = Vector3.Distance(temp.localPosition, nextTemp.localPosition);
                    conn.transform.localScale = scale;

                    // Position and rotation connector
                    pos = GetAverageValue(temp.position, nextTemp.position);
                    rot = Quaternion.LookRotation(temp.position - nextTemp.position, Vector3.forward);
                    conn.transform.SetPositionAndRotation(pos, rot);

                    //Save and activate
                    connectors[station.StationName].Add(connRenderer);

                    conn.SetActive(true);
                }
            }
        }
        private Vector3 GetAverageValue(Vector3 a, Vector3 b)
        {
            return (a + b) / 2;
        }

        //Colors
        public void HighlightConnectors(string stationName)
        {
            Color defaultColor = GraphManager.Instance.DefaultColor; 
            foreach(string station in connectors.Keys)
            {
                if (station == stationName) continue;
                foreach(Renderer rend in connectors[station])
                {
                    rend.material.color = defaultColor;
                }
            }
        }

        public void SetConnectorsColors(Dictionary<string, Color> stationColors)
        {
            foreach (string station in connectors.Keys)
            {
                foreach (Renderer rend in connectors[station])
                {
                    rend.material.color = stationColors[station];
                }
            }
        }

    }
}


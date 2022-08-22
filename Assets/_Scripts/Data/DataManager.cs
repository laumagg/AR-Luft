using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ARLuft.Data
{
    public class DataManager : Singleton<DataManager>
    {
        //Events
        [HideInInspector] public UnityEvent StationsDataAvailable;
        [HideInInspector] public UnityEvent PollutionDataAvailable;
        [HideInInspector] public UnityEvent NoPollutionDataAvailable;
        [HideInInspector] public UnityEvent HistoricalDataAvailable;

        //Root data
        [HideInInspector] public RootStationsData RootStations;
        [HideInInspector] public RootPollutantData RootPollution;
        [HideInInspector] public RootCorePollutionData RootCorePollution;
        [HideInInspector] public RootCoreComponentsData RootCoreComponents;

        //Saved data between scenes
        [HideInInspector] public PollutantData SelectedPollutantForAR;
        [HideInInspector] public CoreComponentsData SelectedPollutantComponents;

        //For graf
        [HideInInspector] public int routinesRunning;
        [HideInInspector] public List<RootPollutantData> RootYearPollution;
        [HideInInspector] public List<GraphData> GraphDataList;

        private readonly DataRequestManager requestManager = new();
        private void Start()
        {
            gameObject.transform.parent = null;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(requestManager.GetStationsList());
            StartCoroutine(requestManager.GetPollutantsList());
        }

        //API requests
        public void GetPollutantsData(string stationCode)
        {
            StartCoroutine(requestManager.GetStationData(stationCode, "1h"));
        }
        public void SetPollutantForAR(PollutantData pollutant)
        {
            SelectedPollutantForAR = pollutant;
            StartCoroutine(requestManager.GetCoreComponents(true));
        }

        public IEnumerator GetHistorialData()
        {
            routinesRunning = RootStations.data.Length;

            foreach (StationsData data in RootStations.data)
            {
                StartCoroutine(requestManager.GetYearData(data.code, SelectedPollutantForAR.core));
            }

            while (routinesRunning > 0)
                yield return null;

            SimplifyGraphData();
            HistoricalDataAvailable?.Invoke();
        }


        //Helpers
        public void SaveCoreComponents()
        {
            SelectedPollutantComponents = RootCoreComponents.data.FirstOrDefault(x => x.core == SelectedPollutantForAR.core);
        }
        public CorePollutionData GetCorePollutant(PollutantData pollutant)
        {
            foreach (CorePollutionData core in RootCorePollution.data)
            {
                if (core.code == pollutant.core)
                    return core;
            }

            return null;
        }
        public string GetStationName(string stationCode)
        {
            return RootStations.data.FirstOrDefault(x => x.code == stationCode).name;
        }
        private void SimplifyGraphData()
        {
            GraphDataList = new();
            foreach (RootPollutantData rootData in RootYearPollution)
            {
                foreach (PollutantData pollData in rootData.data)
                {
                    GraphData data = new();
                    data.Station = GetStationName(pollData.station);
                    data.PollutantValue = pollData.value;
                    data.Year = ChangeDateTimeToYear(pollData.datetime);

                    GraphDataList.Add(data);
                }
            }
        }
        private string ChangeDateTimeToYear(string dateTime)
        {
            //Get only year of datetime string
            return dateTime[..4];
        }


        private void OnDestroy()
        {
            StationsDataAvailable?.RemoveAllListeners();
            PollutionDataAvailable?.RemoveAllListeners();
            NoPollutionDataAvailable?.RemoveAllListeners();
            HistoricalDataAvailable?.RemoveAllListeners();
        }
    }
}


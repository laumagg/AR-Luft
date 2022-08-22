using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ARLuft.Data
{
    public class DataRequestManager
    {
        public IEnumerator GetStationsList()
        {
            //API Request GET/stations
            UnityWebRequest request = CreateRequest("https://luftdaten.berlin.de/api/stations?active=true");
            yield return request.SendWebRequest();

            //Data deserialisation
            DataManager.Instance.RootStations = JsonUtility.FromJson<RootStationsData>(BuildJsonPass(request));

            if (DataManager.Instance.RootStations.data != null)
                DataManager.Instance.StationsDataAvailable?.Invoke();
        }
        public IEnumerator GetStationData(string stationCode, string period)
        {
            DataManager.Instance.RootPollution = null;

            DateTime dateTime = DateTime.Now;

            //If it is midnight
            if (dateTime.Hour == DateTime.Today.Hour)
                dateTime.AddHours(-1);

            string startDate;
            int lastHour = int.Parse(dateTime.ToString("HH")) - 1;
            startDate = $"{dateTime:dd.MM.yyyy}%20{lastHour}%3A00";


            //API Request GET/stations/{code}/data
            string url = $"https://luftdaten.berlin.de/api/stations/{stationCode}/data?period={period}&timespan=custom&start={startDate}";
            UnityWebRequest request = CreateRequest(url);
            yield return request.SendWebRequest();

            //Data update
            DataManager.Instance.RootPollution = JsonUtility.FromJson<RootPollutantData>(BuildJsonPass(request));


            if (DataManager.Instance.RootPollution.data.Length != 0)
                DataManager.Instance.PollutionDataAvailable?.Invoke();
            else
                DataManager.Instance.NoPollutionDataAvailable?.Invoke();
        }
        public IEnumerator GetPollutantsList()
        {
            DataManager.Instance.RootCorePollution = null;

            //API Request GET/api/cores
            UnityWebRequest request = CreateRequest("https://luftdaten.berlin.de/api/cores?active=true");
            yield return request.SendWebRequest();

            //Data update
            DataManager.Instance.RootCorePollution = JsonUtility.FromJson<RootCorePollutionData>(BuildJsonPass(request));
        }
        public IEnumerator GetCoreComponents(bool saveComponentsAfterwards)
        {
            DataManager.Instance.RootCoreComponents = null;

            //API Request GET/api/components
            UnityWebRequest request = CreateRequest(
                "https://luftdaten.berlin.de/api/components?active=true&group_code=pollution");

            yield return request.SendWebRequest();

            //Data update
            DataManager.Instance.RootCoreComponents = JsonUtility.FromJson<RootCoreComponentsData>(BuildJsonPass(request));

            if (saveComponentsAfterwards)
                DataManager.Instance.SaveCoreComponents();
        }
        public IEnumerator GetYearData(string stationCode, string pollutantCode)
        {
            string url = $"https://luftdaten.berlin.de/api/stations/{stationCode}/data?core={pollutantCode}&period=1y";

            //API Request GET/api/stations/{code}/data
            UnityWebRequest request = CreateRequest(url);
            yield return request.SendWebRequest();

            RootPollutantData poll = JsonUtility.FromJson<RootPollutantData>(BuildJsonPass(request));
            if (poll != null && poll.data.Length != 0)
                DataManager.Instance.RootYearPollution.Add(poll);

            DataManager.Instance.routinesRunning--;
        }

        private UnityWebRequest CreateRequest(string url)
        {
            UnityWebRequest request = new(url);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }
        private string BuildJsonPass(UnityWebRequest request)
        {
            return "{\"data\":" + request.downloadHandler.text + "}";
        }

    }
}


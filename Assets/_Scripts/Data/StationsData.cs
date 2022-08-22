using System;

namespace ARLuft.Data
{
    //API API stations
    [Serializable]
    public class StationsData
    {
        public string name;
        public string code;
        public string codeEu;
        public string address;
        public string lat;
        public string lng;
        public bool active;
        public string[] stationgroups;
        public string measuringStart;
        public string measuringEnd;
        public string measuringHeight;
        public string url;
        public string information;
        public string[] components;
        public string[] activeComponents;
        public string[] partials;
        public string[] lqis;
        public string[] exceeds;
    }

    [Serializable]
    public class RootStationsData
    {
        public StationsData[] data;
    }
}


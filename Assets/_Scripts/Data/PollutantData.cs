using System;

namespace ARLuft.Data
{
    //API stations/{code}/data
    [Serializable]
    public class PollutantData
    {
        public string datetime;
        public string station;
        public string core;
        public string component;
        public string period;
        public int value;
    }
    [Serializable]
    public class RootPollutantData
    {
        public PollutantData[] data;
    }

    //API cores
    [Serializable]
    public class CorePollutionData
    {
        public string code;
        public bool active;
        public string group;
        public string name;
        public string shortName;
        public string description;
        public string defaultPeriod;
        public string[] periods;
        public float decimalPoints;
        public string[] components;
    }
    [Serializable]
    public class RootCorePollutionData
    {
        public CorePollutionData[] data;
    }

    //API components
    [Serializable]
    public class CoreComponentsData
    {
        public string code;
        public string core;
        public string period;
        public string description;
        public string unit;
        public int min;
        public int max;
        public int decimalPoints;
    }
    [Serializable]
    public class RootCoreComponentsData
    {
        public CoreComponentsData[] data;
    }
}

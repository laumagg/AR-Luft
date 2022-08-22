using System;
using ARLuft.Data;
using System.Linq;

namespace ARLuft.UI
{
    public class StationListEntry : ListEntry
    {
        private StationsData _data = new();

        public void SetDataInUI(StationsData data)
        {
            _data = data;

            DateTime startDate;
            try
            {
                startDate = DateTime.Parse(data.measuringStart, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                startDate = DateTime.Today;
            }

            _titleText.text = data.name;
            _descriptionText.text = $"{data.stationgroups[0]} \n" +
                $"Messbegin: {startDate.ToShortDateString()} \n" +
                $"Adresse: {data.address} \n" +
                $"{data.information}";

            if (_hasDropdown)
                _dropdown.AssignTextAsSingleOption(_descriptionText.text);
        }

        public void ShowPollutantsList()
        {
            if(_hasDropdown)
                _dropdown.Hide();

            if (_data.code == null)
                _data = GetStation(_titleText.text);

            DataManager.Instance.GetPollutantsData(_data.code);
            UIController.Instance.PollListUIController.ShowPollutantsListUI(_data.code);
        }
        private StationsData GetStation(string title)
        {
            return DataManager.Instance.RootStations.data.FirstOrDefault(x => x.name == title);
        }
    }
}

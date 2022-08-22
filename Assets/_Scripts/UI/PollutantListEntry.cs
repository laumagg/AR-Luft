using ARLuft.Data;
using TMPro;
using UnityEngine;

namespace ARLuft.UI
{
    public class PollutantListEntry : ListEntry
    {
        private PollutantData _data = new();

        public void SetDataInUI(PollutantData data)
        {
            _data = data;
            CorePollutionData pollData = DataManager.Instance.GetCorePollutant(_data);

            _titleText.text = pollData.name;
            _descriptionText.text = pollData.description;
           
            if (_hasDropdown)
                _dropdown.AssignTextAsSingleOption(_descriptionText.text);
        }

        public void StartParticlesGame()
        {
            if (_hasDropdown)
                _dropdown.Hide();

            UIController.Instance.PollListUIController.SwitchToARParticlesScene(_data);
        }
    }
}


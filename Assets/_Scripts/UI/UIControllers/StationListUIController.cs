using UnityEngine;
using UnityEngine.UI;
using ARLuft.Data;

namespace ARLuft.UI
{
    public class StationListUIController : ListUIController
    {
        [SerializeField] private Button _mapViewButton;

        private void OnEnable()
        {
            _mapViewButton.onClick.AddListener(() => UIController.Instance.StartUIController.SwitchStationView(false));
            DataManager.Instance.StationsDataAvailable.AddListener(CreateStationsList);
        }

        private void CreateStationsList()
        {
            //Temporary objects
            StationsData[] stationsData = DataManager.Instance.RootStations.data;

            Vector2 tempPos = new(0f, _templateListEntry.rect.position.y);

            //List entries creation
            for (int i = 0; i < stationsData.Length; i++)
            {
                //Fill list entry with data
                StationListEntry tempListEntry = _templateListEntry.GetComponent<StationListEntry>();
                tempListEntry.SetDataInUI(stationsData[i]);

                //Instantiation in position
                CreateListEntry(tempPos);
                tempPos.y -= 80f;
            }

            _templateListEntry.gameObject.SetActive(false);
        }
    }
}



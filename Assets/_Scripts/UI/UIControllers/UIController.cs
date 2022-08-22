using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class UIController : Singleton<UIController>
    {
        [Header("UIs")]
        public StartUIController StartUIController;
        public StationListUIController StationListUIController;
        public PollutantsListUIController PollListUIController;
    }
}

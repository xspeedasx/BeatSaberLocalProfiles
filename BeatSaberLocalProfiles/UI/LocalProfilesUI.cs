using CustomUI.MenuButton;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberLocalProfiles.UI
{
    public class LocalProfilesUI : MonoBehaviour
    {
        public bool initialized = false;
        private static LocalProfilesUI _instance;
        public static LocalProfilesUI Instance {
            get {
                if (!_instance)
                {
                    _instance = new GameObject("LocalProfilesUIGO").AddComponent<LocalProfilesUI>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
            private set {
                _instance = value;
            }
        }

        public LocalProfilesFlowCoordinator localProfilesFlowCoordinator;
        private MenuButton LocalProfilesMenuButton;

        public void OnLoad()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            if (initialized) return;

            RectTransform mainMenu = (Resources.FindObjectsOfTypeAll<MainMenuViewController>().First().rectTransform);
            LocalProfilesMenuButton = MenuButtonUI.AddButton("Local Profiles", "Configure local profiles.", LocalProfilesButtonPressed);
            //try
            //{
            //    var MainScreen = GameObject.Find("MainScreen");
            //    var MainScreenPosition = MainScreen.transform.position;
            //    MenuButtonUI.AddButton("Local Profiles", "Configure local profiles.", () => {
            //        //Plugin.Log("IIIIISSSS CLIXED!!!!!");
            //        //MainScreen.transform.position = new Vector3(0, -100, 0); //"If it works it's not stupid"

            //    });



            //    //ToggleOption toggle = GameplaySettingsUI.CreateToggleOption("Test Option", "This is a short description of the option, which will be displayed as a tooltip when you hover over it", null);
            //    //toggle.AddConflict("Another Gameplay Option");

            //    //Plugin.Log("CREATXXX MENUUUUUUUUUUUUUUUUUUUUUU");
            //    //toggle.GetValue = toggleValue;
            //    //toggle.OnToggle += ((bool e) =>
            //    //{
            //    //    toggleValue = e;
            //    //});

            //    //SubMenu settingsSubmenu = SettingsUI.CreateSubMenu("Test Submenu");
            //    //IntViewController testInt = settingsSubmenu.AddInt("Test Int", 0, 100, 1);
            //    //testInt.GetValue += delegate { return ModPrefs.GetInt(this.Name, "Test Int", 0, true); };
            //    //testInt.SetValue += delegate (int value) { ModPrefs.SetInt(this.Name, "Test Int", value); };

            //    //MenuButtonUI.AddButton("Test Button", delegate { Console.WriteLine("Pushed test button!"); });
            //    Plugin.Log("CREATEDEDED MENUUUUUUUUUUUUUUUUUUUUUU");
            //}
            //catch (Exception ex)
            //{
            //    Plugin.Log("COULD NOT CREATE MENUUUUUUUUUUUUUUUUUU");
            //    Plugin.Log(ex + "", LogLevel.Error);
            //}
            initialized = true;
        }


        public void LocalProfilesButtonPressed()
        {
            if (localProfilesFlowCoordinator == null)
                localProfilesFlowCoordinator = new GameObject("MoreSongsFlowCoordinator").AddComponent<LocalProfilesFlowCoordinator>();

            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();

            mainFlow.InvokeMethod("PresentFlowCoordinator", localProfilesFlowCoordinator, null, false, false);
        }
    }
}

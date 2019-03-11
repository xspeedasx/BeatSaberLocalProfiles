using CustomUI.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace BeatSaberLocalProfiles.UI
{
    class LocalProfilesViewController : VRUIViewController
    {
        private Button _profileButton;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            var profilesData = Resources.FindObjectsOfTypeAll<LocalProfilesData>().FirstOrDefault();

            _profileButton = CreateButton();
        }

        private Button CreateButton()
        {
            var profilesData = Resources.FindObjectsOfTypeAll<LocalProfilesData>().FirstOrDefault();
            return BeatSaberUI.CreateUIButton(rectTransform, "CreditsButton", new Vector2(0f, 0f), new Vector2(80f, 8f), ProfileChangeButtonClicked, "Profile: " + profilesData?.CurrentProfile ?? "---");
        }

        private void ProfileChangeButtonClicked()
        {
            var profilesData = Resources.FindObjectsOfTypeAll<LocalProfilesData>().FirstOrDefault();
            profilesData.CurrentProfile = profilesData.CurrentProfile == "xspeedasx" ? "meowtastic" : "xspeedasx";

            //Destroy(_profileButton);
            //_profileButton = CreateButton();
            _profileButton.SetButtonText("Profile: " + profilesData?.CurrentProfile ?? "---");

            //ThisButton.GetComponentsInChildren.< Text >.text
            //Plugin.Log("Clicked batton.! " + this.name);
            //Plugin.Log("Profile: " + profilesData?.CurrentProfile ?? "---");
            //_profileButton.GetComponentInChildren<Text>().text = "Profile: " + profilesData?.CurrentProfile ?? "---";
        }

        protected override void DidDeactivate(DeactivationType type)
        {
        }
    }
}

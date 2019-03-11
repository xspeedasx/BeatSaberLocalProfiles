using BeatSaberLocalProfiles.UI;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;

namespace BeatSaberLocalProfiles
{
    public class LocalProfilesFlowCoordinator : FlowCoordinator
    {
        private BackButtonNavigationController _backButtonNavigationController;
        private LocalProfilesViewController _localProfilesViewController;
        internal static LocalProfilesFlowCoordinator Instance;
        private SettingsMenuFillerForMainViewController placeholder;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                Instance = this;
                title = "Beat Saber Local Profiles";

                _backButtonNavigationController = BeatSaberUI.CreateViewController<BackButtonNavigationController>();
                _backButtonNavigationController.didFinishEvent += _BackButtonNavigationController_didFinishEvent;


                _localProfilesViewController = BeatSaberUI.CreateViewController<LocalProfilesViewController>();

                GameObject _songDetailGameObject = Instantiate(Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First(), _backButtonNavigationController.rectTransform, false).gameObject;
                Destroy(_songDetailGameObject.GetComponent<StandardLevelDetailViewController>());
            }


            SetViewControllersToNavigationConctroller(_backButtonNavigationController, new VRUIViewController[]
            {
                _localProfilesViewController
            });
            ProvideInitialViewControllers(_backButtonNavigationController, null, null);
        }


        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            if (deactivationType == DeactivationType.RemovedFromHierarchy)
            {
                PopViewControllerFromNavigationController(_backButtonNavigationController);
            }
        }

        private void _BackButtonNavigationController_didFinishEvent()
        {
            MainFlowCoordinator mainFlow = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            mainFlow.InvokeMethod("DismissFlowCoordinator", this, null, false);
        }


        class SettingsMenuFillerForMainViewController : VRUIViewController
        {
            protected override void DidActivate(bool firstActivation, ActivationType activationType)
            {
                base.DidActivate(firstActivation, activationType);
            }
        }
    }
}

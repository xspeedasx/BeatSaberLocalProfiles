using CustomUI.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using VRUI;

namespace BeatSaberLocalProfiles.UI
{
    class BackButtonNavigationController : VRUINavigationController
    {
        public event Action didFinishEvent;

        private Button _backButton;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                _backButton = BeatSaberUI.CreateBackButton(rectTransform, () => { didFinishEvent?.Invoke(); });
            }
        }
    }
}

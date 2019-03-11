using BeatSaberLocalProfiles.DataModels;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberLocalProfiles.UI
{
    class ProfileSongResultUI : MonoBehaviour
    {
        public bool initialized = false;

        private static ProfileSongResultUI _instance;
        public static ProfileSongResultUI Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = new GameObject("ProfileSongResultUIGO").AddComponent<ProfileSongResultUI>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }


        private ResultsViewController _standardLevelResultsViewController;
        private TextMeshProUGUI _resultText;
        private IBeatmapLevel _lastLevel;


        internal void OnLoad()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            if (initialized) return;
            _standardLevelResultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().First(x => x.name == "StandardLevelResultsViewController");
            _standardLevelResultsViewController.didActivateEvent += _standardLevelResultsViewController_didActivateEvent;

            _resultText = _standardLevelResultsViewController.CreateText("song results", new Vector2(-45f, -15f));
            _resultText.alignment = TextAlignmentOptions.Left;
            _resultText.fontSize = 6f;
            _resultText.lineSpacing = -50f;
            _resultText.color = new Color(0.8f, 0.8f, 0.8f);
            initialized = true;
        }


        private void _standardLevelResultsViewController_didActivateEvent(bool firstActivation, VRUI.VRUIViewController.ActivationType activationType)
        {
            try
            {
                var profilesData = Resources.FindObjectsOfTypeAll<LocalProfilesData>().FirstOrDefault();
                IDifficultyBeatmap diffBeatmap = _standardLevelResultsViewController.GetPrivateField<IDifficultyBeatmap>("_difficultyBeatmap");
                _lastLevel = diffBeatmap.level;

                var bestScore = -1;
                var bestRank = -1f;
                var completed = -1f;
                Score bestScoreO = null;

                foreach (var score in profilesData.lastProfile.Scores)
                {
                    if (score.score > bestScore)
                    {
                        bestScoreO = score;
                        bestScore = score.score;
                        var acc = score.hitNotes / (score.passedNotes == 0 ? 1 : score.passedNotes);
                        var rankPercent = 1.0f * score.score / profilesData.lastDiff.MaxScore;
                        bestRank = rankPercent;
                        completed = 1.0f * score.timePlayed / profilesData.lastSong.Length;
                    }
                }

                //_resultText.SetText($"profile: {profilesData.CurrentProfile}\r\nPlayed: {profilesData.lastProfile.Scores.Count} times");
                var resultText = $"Profile: {profilesData.CurrentProfile}\n";
                resultText += $"Played: {profilesData.lastProfile.Scores.Count} times\n";
                resultText += $"Best score: {bestScore} ({GetRank(bestRank)} {(bestRank * 100f).ToString("0.0")}%) ";
                var mods = GetMods(bestScoreO);
                if (mods != null)
                {
                    resultText += mods;
                }

                if (completed < 1)
                {
                    resultText += $"\nCompleted: {(completed * 100f).ToString("0.0")}%";
                }

                string bestPlayer = null;
                var bestPlayerScore = -1;
                var bestPlayerRank = -1f;
                var bestPlayerCompleted = -1f;
                string bestPlayerMods = null;
                foreach(var prof in profilesData.lastDiff.Profiles)
                {
                    var bestSc = -1;
                    var bestRn = -1f;
                    var bestComp = -1f;
                    string bestMod = null;

                    foreach (var sc in prof.Scores)
                    {
                        if (sc.score > bestSc)
                        {
                            bestSc = sc.score;
                            bestRn = 1.0f* sc.hitNotes / (sc.passedNotes == 0 ? 1 : sc.passedNotes);
                            bestComp = 1.0f*sc.timePlayed / profilesData.lastSong.Length;
                            bestMod = GetMods(sc);
                        }
                    }
                    if (bestSc > bestPlayerScore)
                    {
                        bestPlayerScore = bestSc;
                        bestPlayer = prof.Name;
                        bestPlayerRank = bestRn;
                        bestPlayerCompleted = bestComp;
                        bestPlayerMods = bestMod;
                    }
                }

                if (bestPlayer != null)
                {
                    resultText += $"\n\nBest result for song: {bestPlayer}: {bestPlayerScore} ({GetRank(bestPlayerRank)} {(bestPlayerRank* 100f).ToString("0.0")}%) ";
                    if (bestPlayerMods != null) resultText += " " + bestPlayerMods;
                }

                _resultText.text = resultText;
            }catch(Exception ex)
            {
                Plugin.Log("Error setting up result display: " + ex);
            }
        }

        public string GetMods(Score score)
        {
            if (score == null) return null;
            var mods = new List<string>();
            if (score.modDisappearingArrows) mods.Add("DA");
            if (score.modSongSpeed == "Faster") mods.Add("FS");
            if (score.modSongSpeed == "Slower") mods.Add("SS");
            if (score.modNoFail) mods.Add("NF");

            if (score.modObstacles != "All") mods.Add("NO");
            if (score.modNoBombs) mods.Add("NB");
            if (score.modInstaFail) mods.Add("IF");
            if (score.modBatteryEnergy) mods.Add("BE");

            return mods.Count == 0 ? null : $"[{mods.Aggregate((a, b) => a + "," + b)}]";
        }

        public string GetRank(float prec)
        {
            if (prec >= 1f) return "SSS";
            if (prec > 0.9f) return "SS";
            if (prec > 0.8f) return "S";
            if (prec > 0.65f) return "A";
            if (prec > 0.5f) return "B";
            if (prec > 0.35f) return "C";
            if (prec > 0.2f) return "D";
            return "E";
        }
    }
}

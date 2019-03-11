using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BeatSaberLocalProfiles.DataModels;
using BeatSaberLocalProfiles.UI;
using CustomUI.GameplaySettings;
using CustomUI.MenuButton;
using CustomUI.Settings;
using CustomUI.Utilities;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberLocalProfiles
{
    public class Plugin : IPlugin
    {
        public const string PluginName = "BeatSaberLocalProfiles";
        static LocalProfilesFlowCoordinator settingsFC;

        public string Name => PluginName;

        public string Version => "0.0.1";
        public static Plugin Instance = null;
        private static bool debug = true;


        private GameStatusTracker gameStatusTracker;
        private LocalProfilesData _localProfilesData;
        public LocalProfilesData localProfilesData {
            get {
                if (!_localProfilesData) { 
                    _localProfilesData = new GameObject("BeatSaberLocalProfiles").AddComponent<LocalProfilesData>();
                    UnityEngine.Object.DontDestroyOnLoad(_localProfilesData.gameObject);
                }
                return _localProfilesData;
            }
            private set
            {
                _localProfilesData = value;
            }
        } 

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            localProfilesData.OnLoad();
        }

        public void OnApplicationQuit()
        {
            if (Plugin.Instance == null)
            {
                Plugin.Instance = this;
            }
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        bool toggleValue = false;

        public void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (gameStatusTracker == null)
            {
                gameStatusTracker = new GameStatusTracker();
                gameStatusTracker.LevelFinished += GameStatusTracker_LevelFinished;
                gameStatusTracker.LevelFailed += GameStatusTracker_LevelFailed;
            }

            gameStatusTracker.SceneManagerOnActiveSceneChanged(oldScene, newScene);

            if (newScene.name == "Menu")
            {
                LocalProfilesUI.Instance.OnLoad();
            }

        }

        private void GameStatusTracker_LevelFailed(object sender, EventArgs e)
        {
            var gameStatus = gameStatusTracker.gameStatus;
            Plugin.Log("LEVEL FAILED! Game status:");
            Plugin.Log(GameStatusTracker.PrintObjectFields(gameStatus));
            SaveScore(gameStatus);
        }

        private void GameStatusTracker_LevelFinished(object sender, EventArgs e)
        {
            var gameStatus = gameStatusTracker.gameStatus;
            Plugin.Log("LEVEL FINISHED! Game status:");
            Plugin.Log(GameStatusTracker.PrintObjectFields(gameStatus));
            SaveScore(gameStatus);
        }

        private void SaveScore(GameStatus gameStatus)
        {
            try
            {
                var songScores = localProfilesData.SongScores;
                var song = songScores.ContainsKey(gameStatus.songHash) ? songScores[gameStatus.songHash] : null;
                if (song == null)
                {
                    song = new SongInfo()
                    {
                        Hash = gameStatus.songHash,
                        BeatSaverID = gameStatus.songBeatSaverID,
                        FilePath = gameStatus.songFilePath,
                        Name = gameStatus.songName,
                        SubName = gameStatus.songSubName,
                        AuthorName = gameStatus.songAuthorName,
                        BombsCount = gameStatus.bombsCount,
                        BPM = gameStatus.songBPM,
                        Difficulty = gameStatus.difficulty,
                        Length = gameStatus.length,
                        MaxRank = gameStatus.maxRank,
                        MaxScore = gameStatus.maxScore,
                        NoteJumpSpeed = gameStatus.noteJumpSpeed,
                        NotesCount = gameStatus.notesCount,
                        ObstaclesCount = gameStatus.obstaclesCount
                    };
                    songScores.Add(song.Hash, song);
                }

                var profile = song.Profiles.FirstOrDefault(x => x.Name == localProfilesData.CurrentProfile);
                if (profile == null)
                {
                    profile = new Profile()
                    {
                        Name = localProfilesData.CurrentProfile
                    };
                    song.Profiles.Add(profile);
                }

                //var scores = profile.Scores.ContainsKey(gameStatus.songHash) ? profile.Scores[gameStatus.songHash] : null;
                //if (scores == null)
                //{
                //    profile.Scores.Add(gameStatus.songHash, new List<Score>());
                //}

                var score = new Score()
                {
                    score = gameStatus.score,
                    currentMaxScore = gameStatus.currentMaxScore,
                    rank = gameStatus.rank,
                    passedNotes = gameStatus.passedNotes,
                    hitNotes = gameStatus.hitNotes,
                    missedNotes = gameStatus.missedNotes,
                    passedBombs = gameStatus.passedBombs,
                    hitBombs = gameStatus.hitBombs,
                    maxCombo = gameStatus.maxCombo,
                    timePlayed = GameStatusTracker.GetCurrentTime() - gameStatus.start,
                    timestamp = (int)(GameStatusTracker.GetCurrentTime() / 1000)
                };

                profile.Scores.Add(score);


                //var js = Newtonsoft.Json.JsonConvert.SerializeObject(gameStatus, Newtonsoft.Json.Formatting.Indented);
                //File.WriteAllText(Path.Combine(ScoreDirPath, "score" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"), js);
                var js = Newtonsoft.Json.JsonConvert.SerializeObject(song, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine(localProfilesData.ScoreDirPath, song.Hash + ".json"), js);
            }catch(Exception ex)
            {
                Plugin.Log("ERROR SAVING SCORE!! " + ex);
            }
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnLevelWasInitialized(int level)
        {
            
        }

        public void OnLevelWasLoaded(int level)
        {
            
        }

        public void OnUpdate()
        {
            
        }
        public enum LogLevel { Debug, Info, Error };

        public static void Log(string text, LogLevel logLevel = LogLevel.Debug)
        {
            if (logLevel == LogLevel.Debug && debug)
            {
                Console.WriteLine($"[{PluginName}] {text}");
            }
            else
            {
                Console.WriteLine($"[{PluginName} - {logLevel}] {text}");
            }
        }
    }
}

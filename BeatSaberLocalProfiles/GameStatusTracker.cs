using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberLocalProfiles
{
    public class GameStatusTracker
    {
        public event EventHandler LevelFinished;
        public event EventHandler LevelFailed;

        public GameStatus gameStatus;
        private ScoreController scoreController;
        private StandardLevelSceneSetupDataSO levelSceneSetupData;
        //private GamePauseManager gamePauseManager;
        private GameplayModifiersModelSO gameplayModifiersSO;
        private AudioTimeSyncController audioTimeSyncController;
        //private PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction;
        private GameEnergyCounter gameEnergyCounter;
        private StandardLevelGameplayManager gameplayManager;
        //private BeatmapObjectCallbackController beatmapObjectCallbackController;

        public void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "GameCore")
            {
                gameStatus = new GameStatus();

                levelSceneSetupData = FindFirstOrDefault<StandardLevelSceneSetupDataSO>();
                //gamePauseManager = FindFirstOrDefault<GamePauseManager>();
                scoreController = FindFirstOrDefault<ScoreController>();
                gameplayModifiersSO = FindFirstOrDefault<GameplayModifiersModelSO>();
                audioTimeSyncController = FindFirstOrDefault<AudioTimeSyncController>();
                //playerHeadAndObstacleInteraction = FindFirstOrDefault<PlayerHeadAndObstacleInteraction>();
                gameEnergyCounter = FindFirstOrDefault<GameEnergyCounter>();
                gameplayManager = FindFirstOrDefault<StandardLevelGameplayManager>();
                //beatmapObjectCallbackController = FindFirstOrDefault<BeatmapObjectCallbackController>();

                // Register event listeners
                // private GameEvent GamePauseManager#_gameDidPauseSignal
                //AddSubscriber(gamePauseManager, "_gameDidPauseSignal", OnGamePause);
                // private GameEvent GamePauseManager#_gameDidResumeSignal
                //AddSubscriber(gamePauseManager, "_gameDidResumeSignal", OnGameResume);
                // public ScoreController#noteWasCutEvent<NoteData, NoteCutInfo, int multiplier> // called after AfterCutScoreBuffer is created
                scoreController.noteWasCutEvent += OnNoteWasCut;
                // public ScoreController#noteWasMissedEvent<NoteData, int multiplier>
                scoreController.noteWasMissedEvent += OnNoteWasMissed;
                // public ScoreController#scoreDidChangeEvent<int> // score

                scoreController.scoreDidChangeEvent += OnScoreDidChange;
                // public ScoreController#comboDidChangeEvent<int> // combo
                scoreController.comboDidChangeEvent += OnComboDidChange;
                // public ScoreController#multiplierDidChangeEvent<int, float> // multiplier, progress [0..1]
                scoreController.multiplierDidChangeEvent += OnMultiplierDidChange;
                // private GameEvent GameplayManager#_levelFinishedSignal
                AddSubscriber(gameplayManager, "_levelFinishedSignal", OnLevelFinished);
                // private GameEvent GameplayManager#_levelFailedSignal
                AddSubscriber(gameplayManager, "_levelFailedSignal", OnLevelFailed);
                // public event Action<BeatmapEventData> BeatmapObjectCallbackController#beatmapEventDidTriggerEvent
                //beatmapObjectCallbackController.beatmapEventDidTriggerEvent += OnBeatmapEventDidTrigger;

                IDifficultyBeatmap diff = levelSceneSetupData.difficultyBeatmap;
                IBeatmapLevel level = diff.level;
                
                GameplayModifiers gameplayModifiers = levelSceneSetupData.gameplayCoreSetupData.gameplayModifiers;
                PlayerSpecificSettings playerSettings = levelSceneSetupData.gameplayCoreSetupData.playerSpecificSettings;
                PracticeSettings practiceSettings = levelSceneSetupData.gameplayCoreSetupData.practiceSettings;

                float songSpeedMul = gameplayModifiers.songSpeedMul;
                if (practiceSettings != null) songSpeedMul = practiceSettings.songSpeedMul;
                float modifierMultiplier = gameplayModifiersSO.GetTotalMultiplier(gameplayModifiers);

                var songInfo = FindLevelInfancyWay(levelSceneSetupData.difficultyBeatmap.level.levelID);
                

                gameStatus.songHash = level.levelID.Substring(0, Math.Max(0, level.levelID.IndexOf('∎')));
                gameStatus.songBeatSaverID  = songInfo == null ? null : ParseIdFromSongPath(songInfo);
                gameStatus.songFilePath = songInfo?.path;
                gameStatus.songName = level.songName;
                gameStatus.songSubName = level.songSubName;
                gameStatus.songAuthorName = level.songAuthorName;
                gameStatus.songBPM = level.beatsPerMinute;
                gameStatus.noteJumpSpeed = diff.noteJumpMovementSpeed;
                gameStatus.songTimeOffset = (long)(level.songTimeOffset * 1000f / songSpeedMul);
                gameStatus.length = (long)(level.audioClip.length * 1000f / songSpeedMul);
                gameStatus.start = GetCurrentTime() - (long)(audioTimeSyncController.songTime * 1000f / songSpeedMul);
                if (practiceSettings != null) gameStatus.start -= (long)(practiceSettings.startSongTime * 1000f / songSpeedMul);
                gameStatus.paused = 0;
                gameStatus.difficulty = diff.difficulty.Name();
                gameStatus.notesCount = diff.beatmapData.notesCount;
                gameStatus.bombsCount = diff.beatmapData.bombsCount;
                gameStatus.obstaclesCount = diff.beatmapData.obstaclesCount;
                gameStatus.maxScore = ScoreController.GetScoreForGameplayModifiersScoreMultiplier(ScoreController.MaxScoreForNumberOfNotes(diff.beatmapData.notesCount), modifierMultiplier);
                gameStatus.maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(diff.beatmapData.notesCount);
                gameStatus.maxRank = RankModel.MaxRankForGameplayModifiers(gameplayModifiers, gameplayModifiersSO).ToString();
                
                gameStatus.ResetPerformance();

                gameStatus.modifierMultiplier = modifierMultiplier;
                gameStatus.songSpeedMultiplier = songSpeedMul;
                gameStatus.batteryLives = gameEnergyCounter.batteryLives;

                gameStatus.modObstacles = gameplayModifiers.enabledObstacleType.ToString();
                gameStatus.modInstaFail = gameplayModifiers.instaFail;
                gameStatus.modNoFail = gameplayModifiers.noFail;
                gameStatus.modBatteryEnergy = gameplayModifiers.batteryEnergy;
                gameStatus.modDisappearingArrows = gameplayModifiers.disappearingArrows;
                gameStatus.modNoBombs = gameplayModifiers.noBombs;
                gameStatus.modSongSpeed = gameplayModifiers.songSpeed.ToString();
                gameStatus.modFailOnSaberClash = gameplayModifiers.failOnSaberClash;
                gameStatus.modStrictAngles = gameplayModifiers.strictAngles;
            }
        }


        public void OnNoteWasMissed(NoteData noteData, int multiplier)
        {
            // Event order: combo, multiplier, scoreController.noteWasMissed, (LateUpdate) scoreController.scoreDidChange

            gameStatus.batteryEnergy = gameEnergyCounter.batteryEnergy;

            if (noteData.noteType == NoteType.Bomb)
            {
                gameStatus.passedBombs++;
            }
            else
            {
                gameStatus.passedNotes++;
                gameStatus.missedNotes++;
            }
        }

        public void OnNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
        {
            if (noteData.noteType == NoteType.Bomb)
            {
                gameStatus.passedBombs++;
                gameStatus.hitBombs++;
            }
            else
            {
                gameStatus.passedNotes++;

                if (noteCutInfo.allIsOK)
                {
                    gameStatus.hitNotes++;
                }
                else
                {
                    gameStatus.missedNotes++;
                }
            }
        }

        private void OnBeatmapEventDidTrigger(BeatmapEventData beatmapEventData)
        {
            gameStatus.beatmapEventType = (int)beatmapEventData.type;
            gameStatus.beatmapEventValue = beatmapEventData.value;
        }

        public void OnMultiplierDidChange(int multiplier, float multiplierProgress)
        {
            gameStatus.multiplier = multiplier;
            gameStatus.multiplierProgress = multiplierProgress;
        }

        public void OnComboDidChange(int combo)
        {
            try
            {
                gameStatus.combo = combo;
                gameStatus.maxCombo = scoreController.maxCombo;
            }catch(Exception ex)
            {
                Plugin.Log("EXception in score change: " + ex);
            }
}

        public void OnScoreDidChange(int scoreBeforeMultiplier)
        {
            try
            {
                gameStatus.score = ScoreController.GetScoreForGameplayModifiersScoreMultiplier(scoreBeforeMultiplier, gameStatus.modifierMultiplier);

                int currentMaxScoreBeforeMultiplier = ScoreController.MaxScoreForNumberOfNotes(gameStatus.passedNotes);
                gameStatus.currentMaxScore = ScoreController.GetScoreForGameplayModifiersScoreMultiplier(currentMaxScoreBeforeMultiplier, gameStatus.modifierMultiplier);

                RankModel.Rank rank = RankModel.GetRankForScore(scoreBeforeMultiplier, gameStatus.score, currentMaxScoreBeforeMultiplier, gameStatus.currentMaxScore);
                gameStatus.rank = RankModel.GetRankName(rank);
            }catch(Exception ex)
            {
                Plugin.Log("EXception in score change: " + ex);
            }

        }
        
        public void OnLevelFinished()
        {
            //statusManager.EmitStatusUpdate(ChangedProperties.Performance, "finished");
            var handler = LevelFinished;
            handler(this, null);
        }
        
        public void OnLevelFailed()
        {
            //statusManager.EmitStatusUpdate(ChangedProperties.Performance, "failed");
            var handler = LevelFailed;
            handler(this, null);
        }
        
        public static string PrintObjectFields(object obj)
        {
            var sb = new StringBuilder();
            foreach (var fi in obj.GetType().GetFields())
            {
                sb.Append($"    {fi.Name}: {fi.GetValue(obj)?.ToString() ?? "null"}\r\n");
            }
            return sb.ToString();
        }



        private void AddSubscriber(object obj, string field, Action action)
        {
            Type t = obj.GetType();
            FieldInfo gameEventField = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (gameEventField == null)
            {
                Plugin.Log("Can't subscribe to " + t.Name + "." + field);
                return;
            }

            MethodInfo methodInfo = gameEventField.FieldType.GetMethod("Subscribe");
            methodInfo.Invoke(gameEventField.GetValue(obj), new object[] { action });
        }

        private void RemoveSubscriber(object obj, string field, Action action)
        {
            Type t = obj.GetType();
            FieldInfo gameEventField = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (gameEventField == null)
            {
                Plugin.Log("Can't unsubscribe from " + t.Name + "." + field);
                return;
            }

            MethodInfo methodInfo = gameEventField.FieldType.GetMethod("Unsubscribe");
            methodInfo.Invoke(gameEventField.GetValue(obj), new object[] { action });
        }

        private SongLoaderPlugin.CustomSongInfo FindLevelInfancyWay(string levelID)
        {
            var customSongs = Resources.FindObjectsOfTypeAll<SongLoaderPlugin.OverrideClasses.CustomLevel>();//.FirstOrDefault();
            if (customSongs == null)
            {
                Plugin.Log("No songs found :(");
            }
            else
            {
                var song = customSongs.FirstOrDefault(x => x.levelID == levelID);
                if (song == null)
                {
                    Plugin.Log("No song found :c");
                }
                else
                {
                    return song.customSongInfo;
                    
                    //foreach (var f in songInfo.GetType().GetFields())
                    //{
                    //    Plugin.Log($"[F] {f.Name} - {f.GetValue(songInfo)}");
                    //}
                }
            }
            return null;
        }

        private string ParseIdFromSongPath(SongLoaderPlugin.CustomSongInfo songInfo)
        {
            Plugin.Log("Song found! " + songInfo.path);
            var rx = new Regex(@"(\d+-\d+)");
            var matches = rx.Matches(songInfo.path);
            var match = matches[matches.Count > 1 ? matches.Count - 1 : 0];
            var id = match.Groups[1];

            return id?.Value;
        }

        public static long GetCurrentTime()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks / TimeSpan.TicksPerMillisecond);
        }
        
        private static T FindFirstOrDefault<T>() where T : UnityEngine.Object
        {
            T obj = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            if (obj == null)
            {
                Plugin.Log("Couldn't find " + typeof(T).FullName);
                throw new InvalidOperationException("Couldn't find " + typeof(T).FullName);
            }
            return obj;
        }

    }
}

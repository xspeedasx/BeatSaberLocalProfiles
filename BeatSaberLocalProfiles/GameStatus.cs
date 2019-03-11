using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberLocalProfiles
{
    //stolen from https://github.com/opl-/beatsaber-http-status/blob/master/BeatSaberHTTPStatus/GameStatus.cs
    public class GameStatus
    {
        // Beatmap
        public string songHash = null;
        public string songBeatSaverID = null;
        public string songFilePath = null;
        public string songName = null;
        public string songSubName = null;
        public string songAuthorName = null;
        public string songCover = null;
        public float songBPM;
        public float noteJumpSpeed;
        public long songTimeOffset = 0;
        public long length = 0;
        public long start = 0;
        public long paused = 0;
        public string difficulty = null;
        public int notesCount = 0;
        public int bombsCount = 0;
        public int obstaclesCount = 0;
        public int maxScore = 0;
        public string maxRank = "E";

        // Performance
        public int score = 0;
        public int currentMaxScore = 0;
        public string rank = "E";
        public int passedNotes = 0;
        public int hitNotes = 0;
        public int missedNotes = 0;
        public int lastNoteScore = 0;
        public int passedBombs = 0;
        public int hitBombs = 0;
        public int combo = 0;
        public int maxCombo = 0;
        public int multiplier = 0;
        public float multiplierProgress = 0;
        public int batteryEnergy = 1;

        // Mods
        public float modifierMultiplier = 1f;
        public string modObstacles = "All";
        public bool modInstaFail = false;
        public bool modNoFail = false;
        public bool modBatteryEnergy = false;
        public int batteryLives = 1;
        public bool modDisappearingArrows = false;
        public bool modNoBombs = false;
        public string modSongSpeed = "Normal";
        public float songSpeedMultiplier = 1f;
        public bool modFailOnSaberClash = false;
        public bool modStrictAngles = false;

        // Beatmap event
        public int beatmapEventType = 0;
        public int beatmapEventValue = 0;

        public void ResetMapInfo()
        {
            this.songHash = null;
            this.songBeatSaverID = null;
            this.songName = null;
            this.songSubName = null;
            this.songAuthorName = null;
            this.songCover = null;
            this.songBPM = 0f;
            this.noteJumpSpeed = 0f;
            this.songTimeOffset = 0;
            this.length = 0;
            this.start = 0;
            this.paused = 0;
            this.difficulty = null;
            this.notesCount = 0;
            this.obstaclesCount = 0;
            this.maxScore = 0;
            this.maxRank = "E";
        }

        public void ResetPerformance()
        {
            this.score = 0;
            this.currentMaxScore = 0;
            this.rank = "E";
            this.passedNotes = 0;
            this.hitNotes = 0;
            this.missedNotes = 0;
            this.lastNoteScore = 0;
            this.passedBombs = 0;
            this.hitBombs = 0;
            this.combo = 0;
            this.maxCombo = 0;
            this.multiplier = 0;
            this.multiplierProgress = 0;
        }

    }
}

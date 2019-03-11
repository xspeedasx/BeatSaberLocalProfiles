using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberLocalProfiles.DataModels
{
    public class Score
    {
        // Performance
        public int score = 0;
        public int maxPossibleScore = 0; //after multipliers
        public string rank = "E";
        public int passedNotes = 0;
        public int hitNotes = 0;
        public int missedNotes = 0;
        public int passedBombs = 0;
        public int hitBombs = 0;
        public int maxCombo = 0;
        public long timePlayed = 0;
        public int timestamp = 0;


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
    }
}

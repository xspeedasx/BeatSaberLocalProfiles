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
        public int currentMaxScore = 0;
        public string rank = "E";
        public int passedNotes = 0;
        public int hitNotes = 0;
        public int missedNotes = 0;
        public int passedBombs = 0;
        public int hitBombs = 0;
        public int maxCombo = 0;
        public long timePlayed = 0;
        public int timestamp = 0;
    }
}

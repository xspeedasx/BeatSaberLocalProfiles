using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberLocalProfiles.DataModels
{
    public class SongDifficulty
    {
        public float NoteJumpSpeed;
        public string Difficulty = null;
        public int NotesCount = 0;
        public int BombsCount = 0;
        public int ObstaclesCount = 0;
        public int MaxScore = 0;
        public string MaxRank = "E";

        public List<Profile> Profiles;

        public SongDifficulty()
        {
            Profiles = new List<Profile>();
        }
    }
}

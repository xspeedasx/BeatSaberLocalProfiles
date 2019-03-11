using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberLocalProfiles.DataModels
{
    [Serializable]
    public class SongInfo
    {
        public string Hash = null;
        public string BeatSaverID = null;
        public string FilePath = null;
        public string Name = null;
        public string SubName = null;
        public string AuthorName = null;
        public float BPM;
        public float NoteJumpSpeed;
        public long Length = 0;
        public string Difficulty = null;
        public int NotesCount = 0;
        public int BombsCount = 0;
        public int ObstaclesCount = 0;
        public int MaxScore = 0;
        public string MaxRank = "E";

        public List<Profile> Profiles;

        public SongInfo()
        {
            Profiles = new List<Profile>();
        }
    }
}

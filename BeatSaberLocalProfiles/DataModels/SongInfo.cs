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
        public long Length = 0;
        public float BPM;

        public List<SongDifficulty> Difficulties;

        public SongInfo()
        {
            //Profiles = new List<Profile>();
            Difficulties = new List<SongDifficulty>();
        }
    }
}

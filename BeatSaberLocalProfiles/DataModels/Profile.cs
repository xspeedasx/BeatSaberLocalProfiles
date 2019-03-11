using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberLocalProfiles.DataModels
{
    public class Profile
    {
        public string Name = null;
        public List<Score> Scores;

        public Profile()
        {
            Scores = new List<Score>();
        }
    }
}

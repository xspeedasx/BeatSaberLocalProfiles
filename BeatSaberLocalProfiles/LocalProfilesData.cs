using BeatSaberLocalProfiles.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberLocalProfiles
{
    public class LocalProfilesData : MonoBehaviour
    {
        public Dictionary<string, SongInfo> SongScores;
        //private string CurrentProfile = "meowtastic";
        public string CurrentProfile = "xspeedasx";
        public string ScoreDirPath;

        //public LocalProfilesData()
        //{
        //    OnLoad();
        //}

        public void OnLoad()
        {
            SongScores = new Dictionary<string, SongInfo>();
            ScoreDirPath = Path.Combine(Environment.CurrentDirectory, "UserData", "scores");
            if (!Directory.Exists(ScoreDirPath)) Directory.CreateDirectory(ScoreDirPath);

            var cnt = 0;
            foreach (var songInfoFile in Directory.GetFiles(ScoreDirPath))
            {
                try
                {
                    var data = File.ReadAllText(songInfoFile);
                    var songInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<SongInfo>(data) as SongInfo;
                    if (songInfo == null)
                    {
                        Plugin.Log("Could not deserialize json for file " + songInfoFile);
                        continue;
                    }
                    SongScores.Add(songInfo.Hash, songInfo);
                    cnt++;
                }
                catch (Exception ex)
                {
                    Plugin.Log("Exception reading file " + songInfoFile + ": " + ex);
                }
            }
            Plugin.Log($"Loaded {cnt} song scores");
        }
    }
}

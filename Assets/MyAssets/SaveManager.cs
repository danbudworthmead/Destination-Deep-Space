using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public static class SaveManager
    {
        static readonly string Path = Application.persistentDataPath + "/save.dat";
        private static Level[] _levels;
        private const int LevelCount = 16;

        static SaveManager()
        {
            _levels = new Level[LevelCount];
            for (int i = 0; i < LevelCount; i++)
            {
                _levels[i] = new Level
                {
                    time = -1,
                };
            }
        }
        
        [Serializable]
        private class Level
        {
            public long time = -1;
        }

        public static void Load()
        {
            if (File.Exists(Path))
            {
                var formatter = new BinaryFormatter();
                using var fileStream = File.Open(Path, FileMode.Open);
                try
                {
                    _levels = (Level[])formatter.Deserialize(fileStream);
                    Debug.Log("Save data loaded.");
                }
                catch
                {
                    fileStream.Close();
                    File.Delete(Path);
                    Save();
                    Debug.Log("Save data corrupted. Created new.");
                }
            }
            else
            {
                Save();
                Debug.Log("No save data found. Created new.");
            }
        }

        public static void Save()
        {
            var formatter = new BinaryFormatter();
            using var fileStream = File.Create(Path);
            formatter.Serialize(fileStream, _levels);
            fileStream.Close();
            Debug.Log("Save data saved.");
        }

        public static int GetHighestUnlockedLevel()
        {
            var idx = 0;
            while (idx < LevelCount)
            {
                if (!HasCompletionTime(idx))
                {
                    return idx + 1;
                }

                idx++;
            }

            return 0;
        }

        public static void Unlock(int buildIndex, long time)
        {
            var idx = int.Parse(SceneManager.GetSceneByBuildIndex(buildIndex).name.Split(" ").Last());
            _levels[idx].time = time;
            Save();
        }

        public static bool HasCompletionTime(int level)
        {
            return _levels[level].time > -1;
        }
    }
}

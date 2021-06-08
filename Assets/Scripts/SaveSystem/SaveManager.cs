using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Factory.Saving {
    public static class SaveManager {
        public static readonly string SAVE_PATH = Application.dataPath + "/saves";

        private static Dictionary<string, string> objectSaves = new Dictionary<string, string>();

        public static void Save(string key, string value) {
            objectSaves.Add(key, value);
        }

        public static void Save(GameObject obj) {
            Debug.Log("saved " + obj.name);
            string value = SerializeGameObject(obj);
            string key = obj.GetType().ToString() + "|" + obj.GetInstanceID();
            SaveManager.Save(key, value);
        }

        public static string Load(string name) {
            string output = "";
            if (objectSaves.TryGetValue(name, out output)) return output;
            return null;
        }

        public static string GetRecentSave() {
            DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_PATH);
            FileInfo[] saveFiles = directoryInfo.GetFiles();
            FileInfo recentFile = null;
            foreach (FileInfo info in saveFiles) {
                if (recentFile == null) {
                    recentFile = info;
                } else {
                    if (info.LastAccessTime > recentFile.LastWriteTime) {
                        recentFile = info;
                    }
                }
            }
            return recentFile?.FullName;
        }
        public static void SaveGame(string fileName) {
            SaveGame(fileName, false);
        }

        public static void SaveGame(string fileName, bool uniqueName) {
            Debug.Log("save game");
            if (!Directory.Exists(SAVE_PATH)) {
                Directory.CreateDirectory(SAVE_PATH);
            }
            int nameingNum = 1;
            string saveName = $"{SAVE_PATH}/{fileName}{nameingNum}.txt";

            if (!uniqueName) {
                while (File.Exists(saveName)) {
                    nameingNum++;
                    saveName = $"{SAVE_PATH}/{fileName}{nameingNum}.txt";
                }
            } else {
                if (Directory.Exists($"{SAVE_PATH}/{fileName}.txt")) {
                    Debug.LogWarning("you can not save the object to " + fileName + " please enter a new fileName");
                    return;
                }
            }
            //create string array with key#json
            string[] saveFiles = new string[objectSaves.Count];
            int i = 0;
            foreach (string s in objectSaves.Keys) {
                saveFiles[i] += s;
                i++;
            }
            i = 0;
            foreach (string s in objectSaves.Values) {
                saveFiles[i] += "#" + s;
                Debug.Log("save files " + saveFiles[i]);
                i++;
            }

            File.WriteAllLines(saveName, saveFiles);
        }

        public static void LoadGame(string fileName) {
            objectSaves.Clear();
            string[] saves = File.ReadAllLines($"{SAVE_PATH}/{fileName}.txt");
            foreach (string s in saves) {
                int index = s.IndexOf('#');
                Debug.Log($"added file key {s.Substring(0, index)}: value{s.Substring(index)}");
                objectSaves.Add(s.Substring(0, index), s.Substring(index));
            }
        }

        private static string SerializeGameObject(GameObject obj) {
            string output = "";
            List<Component> components = new List<Component>();
            foreach (Component c in obj.GetComponents<Component>()) {
                output += JsonUtility.ToJson(c as Object) + ".";
            }

            return output;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Factory.Core;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Factory.Saving {
    public class SaveSystem : Singleton<SaveSystem> {
        public static readonly string SAVE_PATH = Application.dataPath + "/saves";


        Dictionary<string, object>[] saveTypes = new Dictionary<string, object>[Enum.GetValues(typeof(SavingType)).Cast<int>().Last()];

        private void Awake() {
            for(int i = 0; i < saveTypes.Length; i++) {
                saveTypes[i] = new Dictionary<string, object>();
            }
        }
        public void Save(string saveFile) {
            Dictionary<string, object>[] state = LoadFile(saveFile);
            CaptureState(state);

            SaveFile(saveFile, state);
           
        }



        public void Load(string saveFile) {
            RestoreState(LoadFile(saveFile));

        }

        private void RestoreState(Dictionary<string, object>[] state) {
         /*   saveTypes = state;
            foreach (SavingEntity saveable in GameObject.FindObjectsOfType<SavingEntity>()) {
                string id = saveable.GetUniqueIdentifier();
                if (saveTypes(id)) {
                    saveable.RestoreState(stateDict[id]);
                }
            }*/
        }

        private int GetIndex(string value) {
            return value[0] - Utils.CHAR_TO_INT;
        }

        public Dictionary<string, object>[] LoadFile(string saveFile) {
            string path = GetPathFromSaveFile(saveFile);
//            if (!File.Exists(path)) {
       //         return new Dictionary<string, object>()[ ];
         //   }*/

            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>[])formatter.Deserialize(stream);
            }
        }

       /* private object SeperateTypes(Dictionary<string, object> state) {
            foreach(Sav) {

            }

            return saveTypes;
        }*/

        private void SaveFile(string saveFile, object state) {
            string path = GetPathFromSaveFile(saveFile);
            using (FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object>[] state) {

            foreach (SavingEntity saveable in GameObject.FindObjectsOfType<SavingEntity>()) {
                foreach(KeyValuePair<string, object> pair in saveable.CaptureState()) {
                    saveTypes[GetIndex(pair.Key)][saveable.GetUniqueIdentifier() + pair.Key] = pair.Value;
                }
              //  state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
        }

        private string GetPathFromSaveFile(string saveFile) {
            return Path.Combine(Application.dataPath, saveFile + ".sav");
        }
    }
}
/*public void Save(string saveFile) {
    Dictionary<string, object> state = LoadFile(saveFile);
    CaptureState(state);
    SaveFile(saveFile, state);
}

public void Load(string saveFile) {
    RestoreState(LoadFile(saveFile));
}

public Dictionary<string, object> LoadFile(string saveFile) {
    string path = GetPathFromSaveFile(saveFile);
    if (!File.Exists(path)) {
        return new Dictionary<string, object>();
    }

    using (FileStream stream = File.Open(path, FileMode.Open)) {
        BinaryFormatter formatter = new BinaryFormatter();
        return (Dictionary<string, object>)formatter.Deserialize(stream);dd
    }
}

private void RestoreState(Dictionary<string, object> state) {
    Dictionary<string, object> stateDict = state;
    foreach (SavingEntity saveable in GameObject.FindObjectsOfType<SavingEntity>()) {
        string id = saveable.GetUniqueIdentifier();
        if (state.ContainsKey(id)) {
            saveable.RestoreState(stateDict[id]);
        }
    }
}

private void SaveFile(string saveFile, object state) {
    string path = GetPathFromSaveFile(saveFile);
    using (FileStream stream = File.Open(path, FileMode.Create)) {
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);
    }
}

//private void PrintDict(Dictionary<string, object> test) {
//    foreach(KeyValuePair<string, object> pair in test) {
//        print($"key = {pair.Key}, Value = {pair.Value}");
//    }
//}

private void CaptureState(Dictionary<string, object> state) {
    foreach (SavingEntity saveable in GameObject.FindObjectsOfType<SavingEntity>()) {
        state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
    }
    //  state["lastSceneBuildingIndex"] = SceneManager.GetActiveScene().buildIndex;
}

private string GetPathFromSaveFile(string saveFile) {
    return Path.Combine(Application.dataPath, saveFile + ".sav");
}*/
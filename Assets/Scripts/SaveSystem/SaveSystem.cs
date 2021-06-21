using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Factory.Core;
using Factory.Buildings;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Factory.Saving {
    public class SaveSystem : Singleton<SaveSystem> {

      //  Dictionary<string, object>[] saveTypes = new Dictionary<string, object>[Enum.GetValues(typeof(SavingType)).Cast<int>().Last()];
        Dictionary<string, object>[] saveTypes;

        private void Start() {
            print("load");
            Load();
        }

       /* private void OnApplicationQuit() {
            print("save");
            Save("test");
        }*/
       public void QuickSave() {
            Save(GetLastFile());
        }

        public void Save() {
            Save(Utils.DEFAULT_FILE_NAME + (GetFileCount() + 1));
        }
        
        public void Save(string saveFile) {
            saveTypes = LoadFile(saveFile);
            CaptureState(saveTypes);
            print("save count" + saveTypes[0].Count);
            PrintDict(saveTypes[0]);
            SaveFile(saveFile, saveTypes);
           
        }

        public void Load() {
            print("last file was " + GetLastFile());
            Load(GetLastFile());
        }

        public void Load(string saveFile) {
         //   print("loading file from " + saveFile);
            saveTypes = LoadFile(saveFile);
            print("load dict");
            RestoreBuildings(saveTypes[0]);
        }

     //   private void RestoreState(Dictionary<string, object>[] state) {
//            saveTypes = state;

            
       // }

        private void RestoreBuildings(Dictionary<string, object> buildings) {
           // print("first restore buildings");
            if (buildings == null) return;
            print("restore buildings with count of " + buildings.Count);
            PrintDict(buildings);
            foreach(object obj in buildings) {
                print("obj  " + obj + " with type of " + obj.GetType());
                Dictionary<string, object> buildingTypeDict = ((((KeyValuePair<string, object>)obj).Value) as Dictionary<string, object>);
                print("building type dict " + buildingTypeDict);
                var buildingType = (buildingTypeDict["type"]);
                print("buildingType " + buildingType);
                Building temp = Instantiate(GlobalPointers.buildingPrefabs[(int) buildingType], GlobalPointers.buildingParent);
                print("loaded building with type " + buildingType);
                temp.Load(obj);
            }
        }

        private int GetIndex(string value) {
            return value[0] - Utils.CHAR_TO_INT;
        }

        private string GetLastFile() {
        //    print("save path " + Utils.SAVE_PATH);
            DirectoryInfo directory = new DirectoryInfo(Utils.SAVE_PATH);
            if (GetFileCount() == 0) {
                print("no files");
                return null; 
            }

            //still getting the meta data file instead of just the .sv
            FileInfo file = directory.GetFiles().
                                 OrderBy(a => a.LastWriteTime).
                                 Where(a => a.Name.Substring(a.Name.Length - Utils.FILE_ENDING.Length).Equals(Utils.FILE_ENDING)).
                                 First();
            print("file name " + file.Name);
            //FileInfo file = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();
            return file.Name;
        }

        private int GetFileCount() {
            //string[] temp = Directory.GetFiles(Utils.SAVE_PATH, Utils.FILE_ENDING);
           /* string[] temp = Directory.GetFiles(Utils.SAVE_PATH, ("*" + Utils.FILE_ENDING.ToString()));
            foreach(string str in temp) {
                print("file is " + str);
            }*/
            return Directory.GetFiles(Utils.SAVE_PATH,("*" + Utils.FILE_ENDING.ToString())).Length;
        }

        public Dictionary<string, object>[] LoadFile(string saveFile) {
       //     print("start load file");
            string path = GetPathFromSaveFile(saveFile);
            print("path " + path);
            if (!File.Exists(path)) {
                int length = Enum.GetValues(typeof(SavingType)).Cast<int>().Last() + 1;
                Dictionary<string, object>[] file =  new Dictionary<string, object>[length];
                for(int i = 0; i < length; i++) {
                    file[i] = new Dictionary<string, object>();
                }
                //    print("End load file");
                print("created new dic");
                return file;
            }

            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                print("found dic");
          //      print("End load file");
                return (Dictionary<string, object>[])formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state) {
            print("save file " + saveFile + ": state " + state);
            string path = GetPathFromSaveFile(saveFile) + Utils.DEFAULT_FILE_NAME;
            using (FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void PrintDict(Dictionary<string, object> test) {
            foreach (KeyValuePair<string, object> pair in test) {
                print($"key = {pair.Key}, Value = {pair.Value}");
            }
        }

        private void CaptureState(Dictionary<string, object>[] state) {
            print("capute state");

            //6 PrintDict(SavingEntity.globalLookup);
            print("save entity num " + SavingEntity.globalLookup.Values.Count);
            foreach (SavingEntity saveable in SavingEntity.globalLookup.Values) {
                print("saveable Entity " + saveable.name);

                foreach(KeyValuePair<string, object> pair in saveable.CaptureState()) {
                    int index = GetIndex(pair.Key);
                    print("saved " + pair.Key);
                    //  print("index " + index + " length " + saveTypes.Length);
                    // print("saving types " + saveTypes);
                    if (pair.Value != null) {
                        saveTypes[GetIndex(pair.Key)][saveable.GetUniqueIdentifier().Substring(0, 5) + pair.Key] = pair.Value;
                    }
                }
              //  state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

        }

        private string GetPathFromSaveFile(string saveFile) {
            return Path.Combine(Utils.SAVE_PATH, saveFile);
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
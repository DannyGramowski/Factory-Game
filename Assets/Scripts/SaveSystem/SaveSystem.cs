using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Factory.Core;
using Factory.Buildings;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Factory.Saving {

    public class SaveSystem : Singleton<SaveSystem> {
        [SerializeField] bool loadOnStart;

        Dictionary<string, object>[] saveTypes;

        private void Start() {
            if (loadOnStart)Load();
        }

       public void QuickSave() {
            Save(GetLastFile());
        }

        public void Save() {
            Save(Utils.DEFAULT_FILE_NAME + (GetFileCount() + 1));
        }
        
        public void Save(string saveFile) {
            saveTypes = LoadFile(saveFile);
            OrderBuildings();
            CaptureState(saveTypes);
            PrintDict(saveTypes[1]);
            SaveFile(saveFile, saveTypes);
        }

        public void Load() {
            Load(GetLastFile());
        }

        public void Load(string saveFile) {
            saveTypes = LoadFile(saveFile);
            RestoreBuildings(saveTypes[0]);
            RestoreItems(saveTypes[1]);
        }

        [MenuItem("Utilities/ clear save files")]
        public static void ClearSaveFiles() {
            DirectoryInfo directory = new DirectoryInfo(Utils.SAVE_PATH);
            foreach(FileInfo file in directory.GetFiles()) {
                file.Delete();
            }

            var saves = Directory.GetFiles(Utils.SAVE_PATH);
            foreach(string str in saves) {
                File.Delete(str);
            }
        }

        private Dictionary<string, object>[] LoadFile(string saveFile) {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path)) {
                int length = Enum.GetValues(typeof(SavingType)).Cast<int>().Last() + 1;
                Dictionary<string, object>[] file =  new Dictionary<string, object>[length];
                for(int i = 0; i < length; i++) {
                    file[i] = new Dictionary<string, object>();
                }
                return file;
            }

            using (FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>[])formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state) {
            string path = GetPathFromSaveFile(saveFile) + Utils.FILE_ENDING;
            using (FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        public void OrderBuildings() {
            Transform parent = GlobalPointers.buildingParent;
            List<Transform> children = new List<Transform>();
            foreach (Transform t in parent) children.Add(t);
            parent.DetachChildren();

            var orderedChildren = children.OrderByDescending(s => s.GetComponent<IOrderable>().GetSavePriority());

            foreach(var c in orderedChildren) {
                c.transform.SetParent(parent);
            }
        }

        private void RestoreBuildings(Dictionary<string, object> buildings) {
            if (buildings == null) return;
           // PrintDict(buildings);
            foreach(object obj in buildings) {
                Dictionary<string, object> buildingTypeDict = ((((KeyValuePair<string, object>)obj).Value) as Dictionary<string, object>);
                var buildingType = (buildingTypeDict["type"]);
                Building temp = Instantiate(GlobalPointers.buildingPrefabs[(int) buildingType], GlobalPointers.buildingParent);
                temp.Load(obj);
            }
        }

        private void RestoreItems(Dictionary<string, object> items) {
            if (items == null) return;
            foreach(object obj in items) {
                print("restored item " + obj);
                Dictionary<string, object> itemTypeDict = ((((KeyValuePair<string, object>)obj).Value) as Dictionary<string, object>);
                int itemType = (int)itemTypeDict["itemType"];
                Vector3 pos = (itemTypeDict["pos"] as SVector3).ToVector();
                Item temp = Item.SpawnItem(itemType, pos);
                temp.Load(obj);
            }
        }

        private void CaptureState(Dictionary<string, object>[] state) {
            foreach (SavingEntity saveable in SavingEntity.globalLookup.Values) {
                foreach(KeyValuePair<string, object> pair in saveable.CaptureState()) {
                    if (pair.Value != null) {
                        saveTypes[GetIndex(pair.Key)][saveable.GetUniqueIdentifier().Substring(0, 5) + pair.Key] = pair.Value;
                    }
                }
            }

        }

        private void PrintDict(Dictionary<string, object> test) {
            foreach (KeyValuePair<string, object> pair in test) {
               // print($"key = {pair.Key}, Value = {pair.Value}");
            }
        }

        private int GetIndex(string value) {
            return value[0] - Utils.CHAR_TO_INT;
        }

        private string GetLastFile() {
            DirectoryInfo directory = new DirectoryInfo(Utils.SAVE_PATH);
            if (GetFileCount() == 0) {
                return Utils.DEFAULT_FILE_NAME; 
            }

            var files = directory.GetFiles().Where(a => a.Name.Substring(a.Name.Length - Utils.FILE_ENDING.Length).Equals(Utils.FILE_ENDING));

            FileInfo file = files.First();
            foreach(var f in files) {
                if (f.LastWriteTime < file.LastWriteTime) file = f;
            }

            return file.Name;
        }

        private int GetFileCount() {
            return Directory.GetFiles(Utils.SAVE_PATH,("*" + Utils.FILE_ENDING.ToString())).Length;
        }

        private string GetPathFromSaveFile(string saveFile) {
            return Path.Combine(Utils.SAVE_PATH, saveFile);
        }
    }

}
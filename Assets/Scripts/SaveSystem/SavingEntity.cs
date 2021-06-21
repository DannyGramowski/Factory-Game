using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Factory.Saving {
    public class SavingEntity : MonoBehaviour {

        [ExecuteAlways]
            [SerializeField] string uniqueIdentifier = "";
            public static Dictionary<string, SavingEntity> globalLookup = new Dictionary<string, SavingEntity>();

        private void Awake() {
            if (string.IsNullOrEmpty(uniqueIdentifier) || IsUnique(uniqueIdentifier)) {
                uniqueIdentifier = Guid.NewGuid().ToString();
                globalLookup[uniqueIdentifier] = this;
            }
        }

        private void OnDestroy() {
            globalLookup.Remove(uniqueIdentifier);
        }

        public string GetUniqueIdentifier() {
                return uniqueIdentifier;
        }

        public void SetUniqueIdentifier(string guid) {
            uniqueIdentifier = guid;
        }

            public Dictionary<string, object> CaptureState() {
                Dictionary<string, object> state = new Dictionary<string, object>();

                foreach (ISaveable save in GetComponents<ISaveable>()) {
                    state[((int)(save.SaveType())).ToString() + save.GetType().ToString()] = save.Save();//number is to add to different lists
                }
                return state;
            }

            public void RestoreState(string guid, object state) {
            uniqueIdentifier = guid;
                Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
                foreach (ISaveable save in GetComponents<ISaveable>()) {
                    string typeString = ((int)(save.SaveType())).ToString() + save.GetType().ToString();
                    if (stateDict.ContainsKey(typeString)) {
                        save.Load(stateDict[typeString]);
                    }
                }
                //SerializableVector3 position = (SerializableVector3)state;

                //GetComponent<NavMeshAgent>().enabled = false;
                //transform.position = position.ToVector();
                //GetComponent<NavMeshAgent>().enabled = true;
                //GetComponent<ActionScheduler>().CancelCurrentAction();
            }

#if UNITY_EDITOR
            private void Update() {
                if (Application.IsPlaying(gameObject))
                    return;

                if (string.IsNullOrEmpty(gameObject.scene.path))
                    return;

                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");


                if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue)) {
                    property.stringValue = System.Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
                }
                globalLookup[property.stringValue] = this;
            }
#endif

            private bool IsUnique(string candidate) {
                if (!globalLookup.ContainsKey(candidate) || candidate == uniqueIdentifier)
                    return true;
                if (globalLookup[candidate] == null) {
                    globalLookup.Remove(candidate);
                    return true;
                }
                if (globalLookup[candidate].GetUniqueIdentifier() != candidate) {
                    return true;
                }
                return false;
            }
        }
    }

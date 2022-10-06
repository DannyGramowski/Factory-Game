using System;
using System.Collections.Generic;
using System.Linq;
using Factory.Units.Actions;
using Factory.Units.BaseUnits;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Factory.Units {
    [SelectionBase]
    public abstract class Unit: SerializedMonoBehaviour {
        [SerializeField] protected string[] actionName;
        protected static readonly object[] EmptyArgs = Array.Empty<object>();
        [SerializeField] protected IAction _action = null;
        [SerializeField] protected ActionSet _actionSet = null;
        protected HashSet<string> validActionNames = new HashSet<string>();

        public void Awake() {
            foreach (var str in actionName) {
                if(!GlobalActionList.ActionNames.Contains(str)) Debug.LogError("not valid action name");
                validActionNames.Add(str);
            }
        }

        private void Update() {
            if (_action != null) {//if there is an action
                _action.OnTick();
                if (_action.IsFinished()) {
                    Debug.Log(_action.ActionName()+ "action finished");
                    _action.OnExit();
                    _action = null;
                }
                _actionSet?.OnTick();
            }
        }

        #region Action

        public void SetActionSet(ActionSet actionSet) {
            _actionSet?.Stop();
            _actionSet = actionSet;
        }
        
        public virtual bool ExecuteAction(IAction action) {
            if(!ValidAction(action)) return false;
            _action = action;
            print(name + " set action to " + _action.ActionName());
            _action.OnInitiate();
            return true;
        }

        public bool HasAction => _action is not null;

        public IAction GetAction => _action;
        
        public bool ValidAction(IAction action) {
            bool output = validActionNames.Contains(action.ActionName()); 
            Debug.Assert(output, action.ActionName() + " is not a valid action for " + GetType());
            return output;
        }
        #endregion
        
    }
}
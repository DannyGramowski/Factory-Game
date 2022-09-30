using System;
using System.Collections.Generic;
using System.Linq;
using Factory.Units.Actions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Factory.Units {
    [SelectionBase]
    public abstract class Unit: MonoBehaviour {
        [SerializeField] protected string[] actionName;
        protected static readonly object[] EmptyArgs = Array.Empty<object>();
        protected IAction _action = null;
        protected ActionSet _actionSet = null;
        protected HashSet<string> validActionNames = new HashSet<string>();
        protected object[] onTickParams = null;
        protected object[] onExitParams = null;

        public void Start() {
            foreach (var str in actionName) {
                if(!GlobalActionList.ActionNames.Contains(str)) Debug.LogError("not valid action name");
                validActionNames.Add(str);
            }
        }

        private void Update() {
            if (_action != null) {//if there is an action
                _action.OnTick();
                if (_action.IsFinished()) {
                    _action.OnExit();
                    _action = null;
                }
            }
        }

        #region Action

        public void SetActionSet(ActionSet actionSet) {
            _actionSet = actionSet;
        }
        
        public virtual bool ExecuteAction(IAction action) {
            if(!ValidAction(action)) return false;
            _action = action;
            _action.OnInitiate();
            return true;
        }

        public bool HasAction => _action is not null;

        public IAction GetAction => _action;
        
        public bool ValidAction(IAction action) {
            return validActionNames.Contains(action.ActionName());
        }
        #endregion
        
    }
}
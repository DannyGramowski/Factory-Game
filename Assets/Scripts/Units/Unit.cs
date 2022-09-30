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
        protected object[] onTickParams = null;
        protected object[] onExitParams = null;

        public void Start() {
            
        }

        private void Update() {
            if (_action != null) {//if there is an action
                _action.OnTick(onTickParams);
                if (_action.IsFinished()) {
                    _action.OnExit(onExitParams);
                    _action = null;
                }
            }
        }

        public virtual bool ExecuteAction(IAction action, object[][] param) {
            if(!ValidAction(action)) return false;
            _action = action;
            _action.OnInitiate(param[0]);
            onTickParams = param[1];
            onExitParams = param[2];
            return true;
        }

        public abstract bool ValidAction(IAction action);
    }
}
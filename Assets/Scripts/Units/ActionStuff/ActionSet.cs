using System.Collections.Generic;
using System;
using System.Drawing.Printing;
using UnityEngine;

namespace Factory.Units.Actions {
    public class ActionSet {
        public delegate void OnComplete();

        private readonly OnComplete _onComplete;
        [SerializeField] private IAction[] _actions;
        private readonly bool _looping;
        private int _index;
        private bool _isFinished = false;
        private Unit _unit;
        
        public ActionSet(Unit unit,IAction[] actions, OnComplete onComplete = null, bool looping = false) {
            Debug.Assert(onComplete is not null || looping, "you need to pass in one of the 2");
            _actions = actions;
            _looping = looping;
            _onComplete = onComplete;
            _index = 0;
            _unit = unit;
            unit.ExecuteAction(actions[_index]);
        }

        public void OnTick() {
            if(_isFinished) return;
            
            if (!_unit.HasAction) {
                _index++;
                if (_looping) {
                    _index %= _actions.Length;
                } else if (_index == _actions.Length) {
                    _onComplete?.Invoke();
                    _isFinished = true;
                }

                _unit.ExecuteAction(_actions[_index]);
            }
        }

        public void Stop() {
            _unit.GetAction.OnExit();
        }
    }
}
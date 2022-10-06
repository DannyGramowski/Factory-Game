using System;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Units.Actions {
    public class ADroneMove: IAction {
        const float MinDistance = 2f;
        private const float BreakingSpeed = 0.9f;
        private const float BreakingBuffer = 3;
        private Vector3 _targetPosition;
        private float _speed;
        private float _turnSpeed; //degrees per second
        private bool _isFinished;
        private Drone _drone;

        public ADroneMove(Drone drone, Vector3 targetPos) {
            _drone = drone;
            _targetPosition = targetPos;
            _speed = _drone.GetSpeed();
            _turnSpeed = drone.GetTurnSpeed();

        }
        public void OnInitiate() { }
        public void OnTick() {
            if(_isFinished) return;
            var position = _drone.transform.position;
            var dist = Vector3.Distance(position, _targetPosition);
            if (dist <= MinDistance) {
                _isFinished = true;
                return;
            }

            var wanted = (_targetPosition - position).normalized;
            if(_drone.transform.forward != wanted) _drone.transform.forward = ClampRotation(wanted, _drone.transform.forward, _turnSpeed);
            if (dist > _speed * Time.deltaTime * BreakingBuffer) {
                var transform = _drone.transform;
                transform.position = position + transform.forward * (dist * BreakingSpeed * Time.deltaTime);
            } else {
                var transform = _drone.transform;
                transform.position = position + transform.forward * (_speed * Time.deltaTime);
            }
        }

        private Vector3 ClampRotation(Vector3 wantedDirection, Vector3 currentDirection,float degreesPerSec) {
            var anglePercentage = Time.deltaTime * (degreesPerSec) / Vector3.Angle(currentDirection, wantedDirection);// how close the turn rate per delta time is to the turn rate
            return Vector3.Lerp(currentDirection, wantedDirection, Mathf.Clamp(anglePercentage, 0, 1)).normalized;
        }
        
        public bool IsFinished() => _isFinished;
        public string ActionName() => "ADroneMove";

        public void OnExit() { }
    }
}
using System;
using System.Drawing.Printing;
using System.Net;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Units.Actions {
    public class ADroneMove: IAction {
        const float MinDistance = 0.5f;
        private const float BreakingSpeed = 0.8f;
        private const float BreakingBuffer = 3;
        private Vector3 _targetPosition;
        private float _speed = 10;
        private float _turnSpeed = 500; //degrees per second
        private bool _isFinished = false;
        private Drone _drone;

        public ADroneMove(Drone drone, Vector3 targetPos) {
            _drone = drone;
            _targetPosition = targetPos;
            _speed = _drone.GetSpeed();
            _turnSpeed = drone.GetTurnSpeed();

        }
        //params = [droneTransform, targetTransfrom, speed, turnspeed]
        public void OnInitiate() {
        }
        //params = [targetTransform]
        public void OnTick() {
            if(_isFinished) return;
            var position = _drone.transform.position;
            var dist = Vector3.Distance(position, _targetPosition);
            if (dist <= MinDistance) {
                _isFinished = true;
                return;
            }

            var wanted = _targetPosition - position;
            var curr = _drone.transform.forward;
            /*Debug.Log("Dot " + Vector3.Dot(wanted, curr));
            Debug.Log("Angle" + Angle(wanted,curr));
            Debug.Log("their angle " + Vector3.Angle(curr, wanted));*/
            _drone.transform.forward = ClampRotation((_targetPosition - position).normalized, _drone.transform.forward, _turnSpeed);
            if (dist > _speed * Time.deltaTime * BreakingBuffer) {
                _drone.transform.position = position + _drone.transform.forward * (dist * BreakingSpeed * Time.deltaTime);
            } else {
                _drone.transform.position = position + _drone.transform.forward * (_speed * Time.deltaTime);
            }
        }

        private Vector3 ClampRotation(Vector3 wantedDirection, Vector3 currentDirection,float degreesPerSec) {
            var anglePercentage = Time.deltaTime * (degreesPerSec) / Vector3.Angle(currentDirection, wantedDirection);// how close the turnrate per delta time is to the turnrate
            return Vector3.Lerp(currentDirection, wantedDirection, Mathf.Clamp(anglePercentage, 0, 1)).normalized;
        }
        
        public bool IsFinished() => _isFinished;
        public string ActionName() => "ADroneMove";

        public void OnExit() { }
    }
}
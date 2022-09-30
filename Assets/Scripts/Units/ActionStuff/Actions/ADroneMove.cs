using System;
using System.Drawing.Printing;
using System.Net;
using UnityEngine;

namespace Factory.Units.Actions {
    public class ADroneMove: IAction {
        const float MinDistance = 0.5f;
        private const float BreakingSpeed = 0.8f;
        private const float BreakingBuffer = 3;
        private Transform _droneTransform;
        private Transform _targetTransform;
        private float _speed = 10;
        private float _turnSpeed = 500; //degrees per second
        private bool _isFinished = false;
        
        //params = [droneTransform, targetTransfrom, speed, turnspeed]
        public void OnInitiate(object[] param) {
            _droneTransform = (Transform)param[0];
            _targetTransform = (Transform)param[1];
            if (param.Length > 2) {
                _speed = (float)param[2];
                _turnSpeed = (float)param[3];
            }
            if(_droneTransform == null) Debug.LogError("drone transform is null");
            if(_droneTransform == null) Debug.LogError("target transform is null");
        }
        //params = [targetTransform]
        public void OnTick(object[] param) {
            if(_isFinished) return;
            var position = _droneTransform.position;
            var dist = Vector3.Distance(position, _targetTransform.position);
            if (dist <= MinDistance) {
                _isFinished = true;
                return;
            }

            var wanted = _targetTransform.position - position;
            var curr = _droneTransform.forward;
            /*Debug.Log("Dot " + Vector3.Dot(wanted, curr));
            Debug.Log("Angle" + Angle(wanted,curr));
            Debug.Log("their angle " + Vector3.Angle(curr, wanted));*/
            _droneTransform.forward = ClampRotation((_targetTransform.position - position).normalized, _droneTransform.forward, _turnSpeed);
            if (dist > _speed * Time.deltaTime * BreakingBuffer) {
                _droneTransform.position = position + _droneTransform.forward * (dist * BreakingSpeed * Time.deltaTime);
            } else {
                _droneTransform.position = position + _droneTransform.forward * (_speed * Time.deltaTime);
            }
        }

        private Vector3 ClampRotation(Vector3 wantedDirection, Vector3 currentDirection,float degreesPerSec) {
            var anglePercentage = Time.deltaTime * (degreesPerSec) / Vector3.Angle(currentDirection, wantedDirection);// how close the turnrate per delta time is to the turnrate
            Debug.Log($"angle {Vector3.Angle(currentDirection, wantedDirection)}, percentage {anglePercentage}, delta t {Time.deltaTime}, dps {degreesPerSec} ");
            Debug.Log($"curr {currentDirection}, wanted {wantedDirection}, lerp{Vector3.Lerp(currentDirection, wantedDirection, Mathf.Clamp(anglePercentage, 0, 1)).normalized}");
            return Vector3.Lerp(currentDirection, wantedDirection, Mathf.Clamp(anglePercentage, 0, 1)).normalized;
        }

        private float Angle(Vector3 wanted, Vector3 current) => (1 - Vector3.Dot(wanted, current)) * 90;//uses dot product to find how far it is from forward

        public bool IsFinished() => _isFinished;
        public string ActionName() => "ADroneMove";

        public void OnExit(object[] param) { }
    }
}
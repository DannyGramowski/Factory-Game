using System.Drawing.Printing;
using Factory.Core;
using Factory.MapFeatures;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Units.Actions {
    [System.Serializable]
    public class AHarvestResource : IAction {
        private ResourceNode _node;
        private IHarvester _harvester;
        private float currTime = 0;
        private float timerLength = 0;
        
        public void OnInitiate(object[] param) {
            Debug.Log("on inititate");
            _node = (ResourceNode)param[0];
            _harvester = (IHarvester)param[1];
            timerLength = _node.GetHarvestTime();
        }

        public void OnTick(object[] param) {
            Debug.Log("curr time " + currTime);
            currTime += Time.deltaTime;
            if (currTime >= timerLength) {
                Debug.Log("Timer done");
                currTime -= timerLength;
                _harvester.Harvest(_node.HarvestResource());
            }
        }

        public bool IsFinished() => _harvester.IsFinished();

        public string ActionName() => "AHarvestResource";
        
        public void OnExit(object[] param) { }
    }
}
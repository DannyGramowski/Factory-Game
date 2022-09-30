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


        public AHarvestResource(ResourceNode node, IHarvester harvester) {
            _node = node;
            _harvester = harvester;
            timerLength = _node.GetHarvestTime();            
        }
        public void OnInitiate() { }

        public void OnTick() {
            currTime += Time.deltaTime;
            if (currTime >= timerLength) {
                currTime -= timerLength;
                _harvester.Harvest(_node.HarvestResource());
            }
        }

        public bool IsFinished() => _harvester.IsFinished();

        public string ActionName() => "AHarvestResource";
        
        public void OnExit() { }
    }
}
using Factory.MapFeatures;
using Factory.Units.BaseUnits;
using UnityEngine;

namespace Factory.Units.Actions {
    public class AHarvestResource : IAction {
        private ResourceNode _node;
        private IHarvester _harvester;
        private float _currTime;
        private float _timerLength;


        public AHarvestResource(ResourceNode node, IHarvester harvester) {
            _node = node;
            _harvester = harvester;
            _timerLength = _node.GetHarvestTime();            
        }
        public void OnInitiate() { }

        public void OnTick() {
            _currTime += Time.deltaTime;
            if (_currTime >= _timerLength) {
                _currTime -= _timerLength;
                _harvester.Harvest(_node.HarvestResource());
            }
        }

        public bool IsFinished() => _harvester.IsFinished();

        public string ActionName() => "AHarvestResource";
        
        public void OnExit() { }
    }
}
using Factory.Core;
using UnityEngine;

namespace Factory.Buildings
{
    public class GatheringHub : Building, ISelectableBuilding {

        
        #region ISelectableBuilding
        public int UINum() => -1;

        public ProducableBuildings ProducableBuildingsType() => ProducableBuildings.gatherer;

        public void SetItem(Item item) { }
        #endregion

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Saving {
    public interface ISaveable {
        public object Save();
        public void Load(object state);

        public SavingType SaveType();//used to differentiate between save groups;
    }

    public enum SavingType {
        building
    }
}

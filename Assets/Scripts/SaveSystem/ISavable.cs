using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory.Saving {
    public interface ISavable {
        public void Save();
        public void Load();
    }
}

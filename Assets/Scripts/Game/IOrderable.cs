using System.Collections;
using UnityEngine;

namespace Factory.Core {
    public interface IOrderable {
        public int GetSavePriority();//used to prioritize which is saved and loaded first (high to low
    }
}
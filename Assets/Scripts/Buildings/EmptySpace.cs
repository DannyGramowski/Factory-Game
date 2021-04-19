using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EmptySpace : MonoBehaviour {
    new Collider collider;
    public bool isEmpty;
    public Item item;
    bool change;
    void Start() {
        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(change) isEmpty = true;
        change = true;
        item = null;

    }

    private void OnTriggerStay(Collider other) {
        isEmpty = false;
        change = false;
        item = other.GetComponent<Item>();
    }
}

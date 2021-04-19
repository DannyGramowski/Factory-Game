using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public Item[] recipe;
    public float productionCost;
    public string itemName;
    public int stackSize = 100;
    public ProducableBuildings[] producableBuildings;
    public Sprite sprite;

    [SerializeField] float errorDistance = 0.5f;

    BeltSystem beltSystem;
    Belt nextBelt;
    Belt currBelt;

    float time;
    bool onBelt;
    bool moving;

    public override bool Equals(object other) {
        Debug.Assert(itemName != null, "you need to give " + name + " an item name");
        Debug.Assert((other as Item).itemName != null, "you need to give " + (other as Item).name + " an item name");
        if (!(other is Item)) return false;
        return itemName.Equals((other as Item).itemName);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public void AddToBeltSystem(Belt belt) {
        this.beltSystem = belt.beltSystem;
        currBelt = belt;
        nextBelt = beltSystem.NextBelt(belt);
        onBelt = true;
        moving = true;
    }

    public void RemoveFromBeltSystem() {
        onBelt = false;
        moving = false;
    }

    public void Deactivate() {
        //transform.position = Vector3.zero;
        gameObject.SetActive(false);    
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    public bool ValidBuilding(ProducableBuildings type) {
        foreach(ProducableBuildings p in producableBuildings) {
            if (p == type) return true;
        }
        return false;
    }

    private void FixedUpdate() {
        if (onBelt && moving && nextBelt) {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, nextBelt.itemPos, nextBelt.speed / 60 * time);
            float distance = Vector3.Distance(transform.position, nextBelt.itemPos);
            if (distance < errorDistance) {
                currBelt = nextBelt;
                nextBelt = beltSystem.NextBelt(currBelt);
                time = 0;
            }
        }
        moving = true;
    }


    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<Item>()) {
            moving = false;
        }
    }
}

//type of building its produced in
public enum ProducableBuildings {
    miner,
    assember,
    all,
    none
}

﻿using UnityEngine;

public class Belt : Building {
    public BeltSystem beltSystem;
    public Vector3 itemPos;
    public float speed;
    public int beltNum => beltSystem.BeltNum(this);

    public override void Placed() {
        itemPos = Utils.Vector3SetY(transform.position, GlobalPointers.itemHeight);
    }
}

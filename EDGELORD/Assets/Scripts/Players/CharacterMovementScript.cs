﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementScript : MonoBehaviour {
    public float moveSpeed;
    public bool movementEnabled = true;

    PlayerInputManager inputs;
    Rigidbody2D rigid;

    // Use this for initialization
    void Start () {
        inputs = GetComponent<PlayerInputManager>();
        rigid = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (movementEnabled) {
            Vector2 direction = inputs.getMovementDirection().normalized * moveSpeed;
            rigid.velocity = direction;
        }
	}
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerRotation : MonoBehaviour {
    public PlayerInputManager inputs;
    public Players.PlayerID playerID;
    public float maxRotationRawInput = 75;

    private Transform hammerTransform;
    private BoxCollider2D collider;
    private bool smithing;
    private float maxRotation;
    private SfxPlayer sfxPlayer;

    private IEnumerator dropHammerCoroutineVar;


	// Use this for initialization
	void Start () {
        maxRotation = maxRotationRawInput;
        sfxPlayer = FindObjectOfType<SfxPlayer>();

        // hammerTransform starts at horizontal
        hammerTransform = GetComponent<Transform>();
        
        // get reference to the player's PlayerInputManager to read inputs
        string playerToFind = null;
        if (playerID == Players.PlayerID.Player_1) {
            maxRotation *= -1; // if attached to player2, positive is counterclockwise and need to reverse the number
            playerToFind = "Player1";
        } else if (playerID == Players.PlayerID.Player_2) {
            playerToFind = "Player2";
        }

        if (playerToFind != null) {
            inputs = (PlayerInputManager)GameObject.Find(playerToFind).GetComponent<PlayerInputManager>();
        }

        dropHammerCoroutineVar = dropHammerCoroutine(); // for use with StopCoroutine in OnCollisionEnter2D
        smithing = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!smithing && inputs.getActionDown()) {
            Debug.Log("got ActionDown");
            smithing = true;
            SwingHammer();
            // StartCoroutine(swingAndSetPower());
        }
	}

    private IEnumerator swingAndSetPower () {
        yield return swingHammerCoroutine();
        float deltaRotation = maxRotation * Time.deltaTime;
        while (!inputs.getActionDown()) {
            if (hammerTransform.eulerAngles.z < maxRotation) {
                Debug.Log("rotating hammer");
                RotateHammer(deltaRotation);
            }
            yield return null;
        }
        yield return dropHammerCoroutineVar;
    }

    void OnCollisionEnter2D (Collision2D coll) {
        if (coll.gameObject.name == "Anvil") {
            StopCoroutine(dropHammerCoroutineVar);
            dropHammerCoroutineVar = dropHammerCoroutine();
            smithing = false;
            sfxPlayer.PlaySoundEffect("anvil_hit_1");
        }
    }

    // quickly raises and then lowers the hammer
    void SwingHammer () {
        Debug.Log("swung the hammer");
        StartCoroutine(swingHammerCoroutine());
    }

    void RotateHammer (float degrees) {
        hammerTransform.Rotate(0, 0, degrees);
    }

    // swings hammer starting from its current rotation
    void DropHammer () {
        StartCoroutine(dropHammerCoroutineVar);
    }

    // raise hammer from horizontal
    private IEnumerator raiseHammerCoroutine (float degrees, float lengthInSeconds = 0.2f) {
        float startTime = Time.time;
        float deltaRotation = degrees / (lengthInSeconds / Time.deltaTime);

        float currentTime = startTime;
        while (currentTime < startTime + lengthInSeconds) {
            hammerTransform.Rotate(0, 0, deltaRotation);
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator dropHammerCoroutine (float lengthInSeconds = 0.1f) {
        Debug.Log("dropping hammer");
        float startTime = Time.time;
        float deltaRotation = (360 - hammerTransform.eulerAngles.z) / (lengthInSeconds / Time.deltaTime);

        float currentTime = startTime;
        while (currentTime < startTime + lengthInSeconds) {
            hammerTransform.Rotate(0, 0, deltaRotation);
            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator swingHammerCoroutine () {
        yield return raiseHammerCoroutine(maxRotation);
        yield return dropHammerCoroutineVar;
    }
}

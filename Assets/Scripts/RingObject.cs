using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject : MonoBehaviour
{
    private GameManager _gameManager;
    private float _timingError = 1f; //1 is biggest error, 0 is perfect
    private float _holdScore = 0f;
    private bool _canBePressed;
    private bool _hasBeenPressed;
    private bool _canBeHeld;
    private bool _circ;
    private GameObject _noteCollider;
    private KeyCode _key;
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int idx = Array.IndexOf(_gameManager._ringsPositions, transform.position.x);
        _key = _gameManager._keyList[idx];
    }
    void Update() {
        if (_canBePressed) {
            if (Input.GetKeyDown(_key)) {
                //Add sound
                _hasBeenPressed = true;
                _timingError = transform.position.y - _noteCollider.transform.position.y;
                //Write the timing error 
            }
            if (_hasBeenPressed) {
                if (_noteCollider.transform.position.y <= transform.position.y) {
                    Destroy(_noteCollider);
                    _hasBeenPressed = false;
                }
            }
        }
        if (_canBeHeld) {
            if (Input.GetKey(_key)) {
                _holdScore += Time.deltaTime;
                if (_circ) {
                    if (_noteCollider.transform.position.y <= transform.position.y) {
                        Destroy(_noteCollider);
                    }
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "CircleFront") {
            _canBePressed = true;
            _noteCollider = other.gameObject;
        }
        if (other.tag == "Square") {
            _canBeHeld = true;
        }
        if (other.tag == "CircleBack") {
            _canBeHeld = true;
            _circ = true;
            _noteCollider = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "CircleFront") {
            _canBePressed = false;
        }
        if (other.tag == "Square") {
            Destroy(other.gameObject);
        }
        if (other.tag == "CircleBack") {
            _canBeHeld = false;
            _circ = false;
            //Write the per note held score
            _holdScore = 0f;
        }
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingObject : MonoBehaviour
{
    private GameManager _gameManager;
    private bool _timingFlag = false;
    private float _timingError = 1f; //1 is biggest error, 0 is perfect
    private float _holdScore = 0f;
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Debug.Log(_gameManager._ringsPositions);
    }

    // Update is called once per frame
    void Update()
    {
    }
    // I need to check so it works with new notes on same ring
    // I should reset an amount of seconds after collision with circle fornt
    // Make it note based
    void OnCollisionEnter(Collision collision) {
        //Find which ring it is and which key we are looking for
        int idx = Array.IndexOf(_gameManager._ringsPositions, transform.position.x);
        KeyCode _key = _gameManager._keyList[idx];
        if (collision.gameObject.CompareTag("CircleFront")) {
            if (Input.GetKey(_key)) {
                if (!_timingFlag) {
                    _timingError = transform.position.y - collision.transform.position.y;
                    _timingFlag = true;
                }
            } else {
                if (_timingFlag) {
                    Destroy(collision.transform.Find("CircleFront"));
                    _timingFlag = false;
                }
            }
        }
        if (collision.gameObject.CompareTag("Square")) {
            if (Input.GetKey(_key)) {
                    _holdScore += Time.deltaTime;
            }
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("CircleFront")) {
        }
        if (collision.gameObject.CompareTag("Square")) {
        }
    }
    
}

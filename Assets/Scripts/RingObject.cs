using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    private Text[] _indicators = new Text[4];
    private Text _holdIndicator;
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int idx = Array.IndexOf(_gameManager._ringsPositions, transform.position.x);
        _key = _gameManager._keyList[idx];
        for (int i=0; i < 4; i++) {
            _indicators[i] = GameObject.Find("Canvas/indicator" + i.ToString()).GetComponent<Text>();
            _indicators[i].transform.localPosition = new Vector3(_gameManager._ringsPositions.Last() * 100 + 300,0,0);
            _indicators[i].enabled = false;
        }
        _holdIndicator = GameObject.Find("Canvas/holdIndicator").GetComponent<Text>();
        _holdIndicator.transform.localPosition = new Vector3(_gameManager._ringsPositions.Last() * 100 + 300,-150,0);
        _holdIndicator.enabled = false;
    }
    void Update() {
        if (_canBePressed) {
            if (Input.GetKeyDown(_key)) {
                //Add sound
                _canBePressed = false;
                _timingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                if (_timingError <= 0.25f) {
                    _indicators[3].enabled = true;
                } else if (_timingError > 0.25 && _timingError <= 0.5) {
                    // for (int i=0; i < 4; i++) {
                    //     _indicators[i].enabled = false;
                    // }
                    _indicators[2].enabled = true;
                } else if (_timingError > 0.5 && _timingError <= 0.75) {
                    // for (int i=0; i < 4; i++) {
                    //     _indicators[i].enabled = false;
                    // }
                    _indicators[1].enabled = true;
                } else {
                    // for (int i=0; i < 4; i++) {
                    //     _indicators[i].enabled = false;
                    // }
                    _indicators[0].enabled = true;
                }
                //Write the timing error 
            }
        }
        if (_canBeHeld) {
            if (Input.GetKey(_key)) {
                //Play charging sound
                _holdScore += Time.deltaTime;
                _holdIndicator.text = "HOLD SCORE : " + _holdScore.ToString().Substring(0,3);
                _holdIndicator.enabled = true;
            }

        }
        if (_noteCollider != null) {
            if (_noteCollider.transform.position.y <= transform.position.y) {
                Destroy(_noteCollider);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "CircleFront") {
            _canBePressed = true;
            for (int i=0; i < 4; i++) {
                _indicators[i].enabled = false;
            }
            _noteCollider = other.gameObject;
        }
        if (other.tag == "Square") {
            _canBeHeld = true;
        }
        if (other.tag == "CircleBack") {
            _canBeHeld = true;
            _noteCollider = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "CircleFront") {
            _canBePressed = false;
        }
        if (other.tag == "Square") {
            Destroy(other.gameObject);
        for (int i=0; i < 4; i++) {
            _indicators[i].enabled = false;
        }
        }
        if (other.tag == "CircleBack") {
            _canBeHeld = false;
            _holdIndicator.enabled = false;  
            //Write the per note held score
            _holdScore = 0f;
        }
    }
    
}

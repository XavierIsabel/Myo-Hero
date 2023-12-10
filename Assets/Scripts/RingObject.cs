using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    private float _lengthOfNote;
    private KeyCode _key;
    private int _classification;
    private Text[] _indicators = new Text[4];
    private Text _holdIndicator;
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        int idx = Array.IndexOf(_gameManager._ringsPositions, transform.position.x);
        _key = _gameManager._keyList[idx];
        _classification = _gameManager._classList[idx];
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
            if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
                if (Input.GetKeyDown(_key)) {
                    _canBePressed = false;
                    _timingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                    PrintTimingIndicator(_timingError);
                }
            } else {
                // Code up for BioPoint & BioArmBand
            }
        }
        if (_canBeHeld) {
            if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
                if (Input.GetKey(_key)) {
                    _holdScore += Time.deltaTime;
                    _holdIndicator.text = "HOLD SCORE : " + _holdScore.ToString().Substring(0,3);
                    _holdIndicator.enabled = true;
                }
            } else {
                if (true) { // Code up for BioPoint & BioArmBand
                    _holdScore += Time.deltaTime;
                    _holdIndicator.text = "HOLD SCORE : " + _holdScore.ToString().Substring(0,3);
                    _holdIndicator.enabled = true;
                }
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
            _lengthOfNote = other.gameObject.transform.localScale.x;
            if (_lengthOfNote != 0) {
                _canBeHeld = true;
            }
        }
        if (other.tag == "CircleBack") {
            if (_lengthOfNote != 0) {
                _canBeHeld = true;
            }
            _canBePressed = false;
            _noteCollider = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "CircleFront") {
            _canBePressed = false;
            _gameManager._timingsScore.Add(_timingError);
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
            _gameManager._holdsScore.Add(_holdScore / ( _lengthOfNote + 0.5f));
            _holdScore = 0f;
        }
    }
    
    void PrintTimingIndicator(float _timingError) {
        if (_timingError <= 0.25f) {
            _indicators[3].enabled = true;
        } else if (_timingError > 0.25 && _timingError <= 0.5) {
            _indicators[2].enabled = true;
        } else if (_timingError > 0.5 && _timingError <= 0.75) {
            _indicators[1].enabled = true;
        } else {
            _indicators[0].enabled = true;
        }
    }
}

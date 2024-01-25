using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RingObject : MonoBehaviour
{
    private GameManager _gameManager; // GameManager Gameobject
    private float _inTimingError = -1f; //1 is biggest error, 0 is perfect (-1 is never pressed)
    private float _outTimingError = -1f; //1 is biggest error, 0 is perfect (-1 is never released)
    private float _holdScore = 0f; // Score for holding note
    private bool _canBePressed;
    private bool _canBeReleased;
    private bool _canBeHeld;
    private GameObject _noteCollider;
    private KeyCode _key; // Key associated to note
    private int _classification; // classification associated to ring
    private Text[] _indicators = new Text[5]; // Text indicators
    private Text _holdIndicator;
    private float _startPosition = 0f;
    BioPointReader _reader;
    private string _p_classification = "-1";
    void Start()
    {
        if (PlayerPrefs.GetString("ControlDevice") != "Keyboard") {
            _reader = GameObject.Find("BioPointReader").GetComponent<BioPointReader>();
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Get idx of ring
        int idx = Array.IndexOf(_gameManager._ringsPositions, transform.position.x);
        // Set for different game
        _key = _gameManager._keyList[idx];
        _classification = idx;
        // Set all indicators
        for (int i=0; i < _indicators.Length; i++) {
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
                    StartCoroutine(ChangeColorCoroutine());
                    _canBePressed = false;
                    _inTimingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                    StartCoroutine(ShowTimingIndicatorCoroutine(_inTimingError));

                }
            } else {
                if (_classification.ToString() == "1" && _reader.readVal == _classification.ToString()) //Neutral Position
                {
                    // Code up for BioPoint & BioArmBand
                    StartCoroutine(ChangeColorCoroutine());
                    _canBePressed = false;
                    StartCoroutine(ShowTimingIndicatorCoroutine(0f));
                }
                else if (_reader.readVal == _classification.ToString() && _p_classification != _classification.ToString()) {
                    // Code up for BioPoint & BioArmBand
                    StartCoroutine(ChangeColorCoroutine());
                    _canBePressed = false;
                    _inTimingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                    StartCoroutine(ShowTimingIndicatorCoroutine(_inTimingError));
                } else {return;}
            }
        }
        if (_canBeHeld) {
            if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
                if (Input.GetKey(_key)) {
                    _holdScore += Time.deltaTime;
                    _holdIndicator.text = "HOLD SCORE : " + _holdScore.ToString()[..3];
                    _holdIndicator.enabled = true;
                }
            } else {
                if (_reader.readVal == _classification.ToString()) { // Code up for BioPoint & BioArmBand
                    _holdScore += Time.deltaTime;
                    _holdIndicator.text = "HOLD SCORE : " + _holdScore.ToString()[..3];
                    _holdIndicator.enabled = true;
                }
            }
            // Add time to start position to know for how much time the player could hold the note
            _startPosition += Time.deltaTime;
        }
        if (_canBeReleased) {
            if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
                if (Input.GetKeyUp(_key)) {
                    StartCoroutine(ChangeColorCoroutine());
                    _canBeReleased = false;
                    _outTimingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                    StartCoroutine(ShowTimingIndicatorCoroutine(_outTimingError));
                }
            } else {
                if (_classification.ToString() == "1" && _reader.readVal == _classification.ToString()) //Neutral Position
                {
                    // Code up for BioPoint & BioArmBand
                    StartCoroutine(ChangeColorCoroutine());
                    _canBeReleased = false;
                    StartCoroutine(ShowTimingIndicatorCoroutine(0f));
                }
                else if (_reader.readVal != _classification.ToString() && _p_classification == _classification.ToString()) {
                    // Code up for BioPoint & BioArmBand
                    StartCoroutine(ChangeColorCoroutine());
                    _canBeReleased = false;
                    _outTimingError = Mathf.Abs(transform.position.y - _noteCollider.transform.position.y);
                    StartCoroutine(ShowTimingIndicatorCoroutine(_outTimingError));
                }
                _p_classification = _reader.readVal;
            }
        }
        if (_noteCollider != null) {
            if (_noteCollider.transform.position.y <= -20) {
                Destroy(_noteCollider);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("CircleFront")) {
            if (other.transform.parent.childCount == 4 && Array.IndexOf(_gameManager._ringsPositions, transform.position.x) == 0) {
            _canBePressed = false;
            } else {_canBePressed = true;}
            // Tag is used for single notes and start of long notes
            // Set gameObject to calculate in-timing score
            _noteCollider = other.gameObject;
        }
        if (other.CompareTag("Square")) {
            if (other.transform.parent.childCount == 4 && Array.IndexOf(_gameManager._ringsPositions, transform.position.x) != 0) {
            _canBeHeld = false;
            } else {_canBeHeld = true;}
        }
        if (other.CompareTag("CircleBack")) {
            _canBeReleased = true;
            // Set gameObject to calculate out-timing score
            _noteCollider = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("CircleFront")) {
            _canBePressed = false;
            // If ring has never been pressed
            if (_inTimingError == -1f) {
                // Activate X indicator
                StartCoroutine(ShowTimingIndicatorCoroutine(_inTimingError));
            }
            // Check if one time note
            if (other.transform.parent.childCount == 2) {
                _gameManager._outTimingsScore.Add(_outTimingError);
                _gameManager._holdsScore.Add(_holdScore);
            }
            // Add in-timing-score to list for stats for long notes
            _gameManager._inTimingsScore.Add(_inTimingError);
            // Add in-timing-score to live game score
            _gameManager._scoreInt += Mathf.RoundToInt((1f - _inTimingError) * 100f);
            // Set in-timing-score value back to default
            _inTimingError = -1f;
        }
        if (other.CompareTag("Square")) {
            _canBeHeld = false;
            //Shut down visual feedback for holding
            _holdIndicator.enabled = false;
        }
        if (other.CompareTag("CircleBack")) {
            // This if checks the end of current long note
            _canBeReleased = false;
            // If ring has never been released
            if (_outTimingError == -1f) {
                // Activate X indicator
                StartCoroutine(ShowTimingIndicatorCoroutine(_outTimingError));
            }
            // Calculate hold score for current long note
            float _tempHoldScore = _holdScore / _startPosition;
            // Add out-timing-score to list for stats for current long note
            _gameManager._outTimingsScore.Add(_outTimingError);
            // Add hold-score to list for stats for current long note
            _gameManager._holdsScore.Add(_tempHoldScore);
            // Add scores to game live score
            _gameManager._scoreInt += Mathf.RoundToInt(_tempHoldScore * 100f);
            _gameManager._scoreInt += Mathf.RoundToInt((1f - _outTimingError) * 100f);
            // Set note-specific scores to their default value
            _outTimingError = -1f;
            _holdScore = 0f;
            _startPosition = 0f;
        }
    }

    IEnumerator ChangeColorCoroutine()
    {
        gameObject.transform.Find("Circle (2)").GetComponent<SpriteRenderer>().color = Color.white;
        // Interpolate between initial color and target color over the specified duration
        float elapsed_time = 0f;
        while (elapsed_time < 0.35f)
        {
            // Wait for the next frame
            yield return null;
            // Update the elapsed time
            elapsed_time += Time.deltaTime;
        }
        // Ensure the final color is exactly the target color
        gameObject.transform.Find("Circle (2)").GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
    }
    IEnumerator ShowTimingIndicatorCoroutine(float _timingError)
    {
        if (_timingError >= 0f && _timingError <= 0.15f) {
            _indicators[4].enabled = true;
        } else if (_timingError > 0.15f && _timingError <= 0.3f) {
            _indicators[3].enabled = true;
        } else if (_timingError > 0.3f && _timingError <= 0.45f) {
            _indicators[2].enabled = true;
        } else if (_timingError > 0.45f && _timingError <= 0.6f) {
            _indicators[1].enabled = true;
        } else {
            _indicators[0].enabled = true;
        }
        float elapsed_time = 0f;
        while (elapsed_time < 0.35f)
        {
            // Wait for the next frame
            yield return null;
            // Update the elapsed time
            elapsed_time += Time.deltaTime;
        }
        for (int i=0; i < _indicators.Length; i++) {
                if (_indicators[i].IsActive()) {
                    _indicators[i].enabled = false;

                }
            }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int _classes = 4;
    private StatsTracker _statstracker;
    private float _time = 60f;
    private int _songs = 0;
    private int x = 0;
    private int _tempo = 120;
    public GameObject _notePrefab;
    private string[][] _notes;
    private List<GameObject> _liveNotes = new List<GameObject>();
    public GameObject _ringPrefab;
    private GameObject[] _rings;
    public float[] _ringsPositions;
    private Color[] _colors = {new Color(0,0.39f,1), Color.red, Color.yellow, Color.green};
    private Color[] _pressedColors = {new Color(0,0.23f,1), new Color(0.74f,0,0), new Color(0.82f,0.82f,0), new Color(0,0.74f,0)};
    public KeyCode[] _keyList = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};
    void Awake()
    {
        _tempo /= 60;
        NoteGenerator();
        GeneratePlayArea();
        PlayerPrefs.SetString("ControlDevice","Keyboard"); // To REMOVE
    }

    // Update is called once per frame
    void Update()
    {
        GameGenerator();
        Controller();
        if (_time - Time.timeSinceLevelLoad <= 0) {
            //WriteStats
            SceneManager.LoadScene("MenuScene");
        }
    }

    void GeneratePlayArea() {
        _rings = new GameObject[_classes];
        _ringsPositions = new float[_classes];
        float _ringPosition = 0f;
        for (int i=0;i<_classes;i++) {
            if (i == 0) {_ringPosition += -0.75f * (_classes - 1);}
            else {_ringPosition += 1.5f;}
            _ringsPositions[i] = _ringPosition;
            GameObject _ring = Instantiate (_ringPrefab, new Vector3(_ringPosition,-4, 0), Quaternion.identity);
            _ring.transform.Find("Circle").GetComponent<SpriteRenderer>().color = _colors[i];
            _rings[i] = _ring;
        }
    }

    void Controller() {
        if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
            for (int i=0;i<_classes;i++) {
                if (Input.GetKey(_keyList[i])) {
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }

        if(PlayerPrefs.GetString("ControlDevice") == "BioPoint") {
            for (int i=0;i<_classes;i++) {
                if (Input.GetKey(_keyList[i])) { //Should be changed to match BioPoint Controls
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }
    }

    void NoteGenerator() {
        String path = "Assets/Resources/song" + UnityEngine.Random.Range(0,_songs).ToString() + ".txt";
        string fileContents = File.ReadAllText(path);
        string[] _tempNotes = fileContents.Split('\n');
        _notes = new string[_tempNotes.Length][];
        for (int i=0; i<_tempNotes.Length;i++) {
            _notes[i] = _tempNotes[i].Split(':');
        }
    }

    void GameGenerator() {
        if (x < _notes.Length) {
            //Spawn in note at right time
            if (float.Parse(_notes[x][0]) <= Time.timeSinceLevelLoad) {
                //Instantiate
                GameObject _note = Instantiate(_notePrefab, new Vector3(_ringsPositions[int.Parse(_notes[x][1])],5,0), Quaternion.identity);
                _liveNotes.Add(_note);
                //Set size
                SizeNote(_note, float.Parse(_notes[x][2]));
                //Set Color
                int _alley = int.Parse(_notes[x][1]);
                ColorNote(_note, _alley);
                //Set Speed
                x++;
            }
        }
        for (int i=0;i<_liveNotes.Count;i++) {
            _liveNotes[i].transform.Translate(Vector3.down * _tempo * Time.deltaTime);
        }
        //Check if collision with ring
        //Save timing for stats
        //(Take extremity of capsule compared to end of ring)
        //You know speed so you know perfect timing and actual timing
        //Once it is out of frame. Remove Instance of note

    }

    void SizeNote(GameObject _note, float _size) {
        _note.transform.Find("CircleBack").localPosition = new Vector3(0,0.5f * _size, 0);
        _note.transform.Find("CircleFront").localPosition = new Vector3(0,-0.5f * _size, 0);
        _note.transform.Find("Square").localScale = new Vector3(1,_size, 1);
    }
    void ColorNote(GameObject _note, int _alley) {
        _note.transform.Find("CircleBack").GetComponent<SpriteRenderer>().color = _colors[_alley];
        _note.transform.Find("CircleFront").GetComponent<SpriteRenderer>().color = _colors[_alley];
        _note.transform.Find("Square").GetComponent<SpriteRenderer>().color = _colors[_alley];
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int _classes = 4; // Number of classes
    private RectTransform _g_indicator;
    private float _time; // Time of song
    private int _songs = 0; // Number of .txt files we have (Should disappear)
    private int _tempo = 144 / 60; // Tempo of song (to fix)
    public GameObject _notePrefab; // Note prefab
    private string[][] _notes; // array of all notes of song
    private List<GameObject> _notesGameobjects = new(); // List of all live notes

    private List<string> _notesLengthList = new(); // List of length of all notes
    private List<string> _notesList = new(); // List of all notes
    public GameObject _ringPrefab; // Ring Prefab
    private GameObject[] _rings; // Array of all ring objects
    public float[] _ringsPositions; // Array of all ring objects positions
    private readonly Color[] _colors = { new(0,0.39f,1),
                                Color.red,
                                Color.yellow,
                                Color.green,
                                Color.white}; // Colors of all possible rings
    private readonly Color[] _pressedColors = {  new(0,0.23f,1),
                                        new(0.74f,0,0),
                                        new(0.82f,0.82f,0),
                                        new(0,0.74f,0),
                                        Color.grey}; // Colors of all possible pressed rings
    public readonly KeyCode[] _keyList = {  KeyCode.Space,
                                            KeyCode.H,
                                            KeyCode.J,
                                            KeyCode.K,
                                            KeyCode.L}; // All Keys
    public List<float> _holdsScore = new(); // List of hold scores
    public List<float> _inTimingsScore = new(); // List of IN timings scores
    public List<float> _outTimingsScore = new(); // List of OUT timings scores
    public List<int> _classifications = new(); // List of classifications made
    public List<float> _timestamps = new(); // List of classifications made
    private int _classification = -1;
    private Text _scoreText; // Score Text component
    public int _scoreInt = 0; // Score value

    BioPointReader _reader;
    void Awake()
    {
        if (PlayerPrefs.GetString("ControlDevice") != "Keyboard") {
            _reader = GameObject.Find("BioPointReader").GetComponent<BioPointReader>();
        }
        // Generate Play Area components like rings
        GeneratePlayArea();
        // Generate all notes of song in the game
        NoteGenerator();
    }

    void Update()
    {
        Debug.Log(_reader.velocity);
        for (int i=0;i<_notesGameobjects.Count;i++) {
            _notesGameobjects[i].transform.Translate(_tempo * Time.deltaTime * Vector3.down);
        }
        // Check if player is interacting with game
        Controller();
        // Update Score
        _scoreText.text = "Score : " + _scoreInt.ToString();
        // Check if note is out of bound and destroy
        if (_notesGameobjects.Count == 0) {
            if (_notesGameobjects[0].transform.position.y <= -10) {
                Destroy(_notesGameobjects[0]);
                _notesGameobjects.RemoveAt(0);
            }
        }
        // Check if game has ended and end game
        if (_time - Time.timeSinceLevelLoad <= 0) {
            GameObject.Find("StatsTracker").GetComponent<StatsTracker>().WriteStats(_notesList,
                                                                                    _notesLengthList,
                                                                                    _holdsScore,
                                                                                    _inTimingsScore,
                                                                                    _outTimingsScore,
                                                                                    _classifications,
                                                                                    _timestamps);
            SceneManager.LoadScene("MenuScene");
        }
    }

    void GeneratePlayArea() {
        // This function sets the position of all rings
        // based on the number of rings there are
        // Set length of array of ring objects and ring positions
        _rings = new GameObject[_classes];
        _ringsPositions = new float[_classes];
        // Set initial ring position
        float _ringPosition = 0f;
        // Set positions of all rings
        for (int i=0;i<_classes;i++) {
            if (i == 0) {_ringPosition += -0.75f * (_classes - 1);}
            else {_ringPosition += 1.5f;}
            _ringsPositions[i] = _ringPosition;
            GameObject _ring = Instantiate (_ringPrefab, new Vector3(_ringPosition,-3, 0), Quaternion.identity);
            _ring.transform.Find("Circle").GetComponent<SpriteRenderer>().color = _colors[i];
            _rings[i] = _ring;
            //     _g_indicator = GameObject.Find("Canvas/G (" + i.ToString() + ")").GetComponent<RectTransform>();
            // if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
            //     Destroy(_g_indicator.gameObject);
            // } else {
            //     _g_indicator.anchoredPosition3D = new Vector3(_ringPosition*110,-400, 0);
            // }
        }
        // Get Score component and start song
        _scoreText = GameObject.Find("Canvas/ScoreText").GetComponent<Text>();
        gameObject.GetComponent<AudioSource>().Play();
    }

    void Controller() {
        _classification = -1;
        if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
            for (int i=0;i<_classes;++i) {
                if (Input.GetKey(_keyList[i])) {
                    _classification = i;
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            // Add classification to stats
            _classifications.Add(_classification);
            _timestamps.Add(Time.time);

        } else { //Put BioPoint and BioArmBand in same category. Assumes same number of classes
            for (int i=0;i<_classes;i++) {
                if (_reader.readVal == i.ToString()) { //Should be changed to match BioPoint Controls
                    _classification = i;
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle (1)").GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            // Add classification to stats
            _classifications.Add(_classification);
            _timestamps.Add(Time.time);
        }
    }

    void NoteGenerator() {
        // This function reads the song .txt file and splits it in unity list.
        // It also instantiates all notes in game
        String path = "./song" + UnityEngine.Random.Range(0,_songs).ToString() + ".txt";
        string fileContents = File.ReadAllText(path);
        string[] _tempNotes = fileContents.Split('\n');
        _notes = new string[_tempNotes.Length][];
        for (int i=0; i<_tempNotes.Length;i++) {
            _notes[i] = _tempNotes[i].Split(':');
            string _noteTime = _notes[i][0];
            string _note = _notes[i][1];
            string _noteLength = _notes[i][2];
            _notesLengthList.Add(_noteLength);
            _notesList.Add(_note);
            //Instantiate notes
            GameObject _noteObject = Instantiate(   _notePrefab, 
                                                    new Vector3(_ringsPositions[int.Parse(_note)], //Align with good ring
                                                                int.Parse(_noteTime) + 10, //Give 5 seconds for the song to start
                                                                1f), //Layer of collision
                                                    Quaternion.identity);
            _notesGameobjects.Add(_noteObject);
            //Set Size and Color
            SizeNColorNote(_noteObject, float.Parse(_noteLength), int.Parse(_note));
        }
        // Set time according to length of song
        _time = (int.Parse(_notes[^1][0]) + int.Parse(_notes[^1][2]) + 15f) / _tempo;
    }

    void SizeNColorNote(GameObject _note,float _size, int _alley) {
        Transform _noteFront = _note.transform.Find("CircleFront");
        Transform _noteMid = _note.transform.Find("Square");
        Transform _noteEnd = _note.transform.Find("CircleBack");
        Transform _noteFrontS = _note.transform.Find("SmallCircleFront");
        Transform _noteMidS = _note.transform.Find("SmallSquare");
        Transform _noteEndS = _note.transform.Find("SmallCircleBack");
        if (_alley == 0) { // Thats a silence
            // Rotate full note
            Destroy(_noteEnd.gameObject);
            Destroy(_noteEndS.gameObject);
            _noteMid.transform.Rotate(0f, 0f, 90f);
            _noteMidS.transform.Rotate(0f, 0f, 90f);
            _noteMid.localScale = new Vector3(_size - 0.5f, _ringsPositions[^1] * 2 , 1);
            _noteMidS.localScale = new Vector3(_size - 1f, (_ringsPositions[^1] * 2) - 0.5f , 1);
            _noteMid.localPosition = new Vector3(_ringsPositions[^1],0.5f*_size - 1.25f, -0.3f);
            _noteMidS.localPosition = new Vector3(_ringsPositions[^1],0.5f*_size - 1.25f, -0.4f);
            _noteMid.GetComponent<SpriteRenderer>().color = _colors[_alley];
            _noteFront.GetComponent<SpriteRenderer>().color = Color.black;

        } else {
            if (_size == 1) {
                Destroy(_noteMid.gameObject);
                Destroy(_noteEnd.gameObject);
                Destroy(_noteMidS.gameObject);
                Destroy(_noteEndS.gameObject);
            } else {
                // This code changes the size of the note
                _noteMid.localScale = new Vector3(0.5f,_size - 1, 1);
                _noteMidS.localScale = new Vector3(0.25f,_size - 1, 1);
                _noteMid.localPosition = new Vector3(0,0.5f * (_size - 1f), 0);
                _noteMidS.localPosition = new Vector3(0,0.5f * (_size - 1f), -0.1f);
                _noteEnd.localPosition = new Vector3(0,_size - 1f, 0);
                _noteEndS.localPosition = new Vector3(0,_size - 1f, -0.1f);
                _noteEnd.GetComponent<SpriteRenderer>().color = _colors[_alley];
                _noteMid.GetComponent<SpriteRenderer>().color = _colors[_alley];
            }
        _noteFront.localPosition = new Vector3(0,0, 0);
        _noteFrontS.localPosition = new Vector3(0,0, -0.1f);
        _noteFront.GetComponent<SpriteRenderer>().color = _colors[_alley];
        }
    }
}

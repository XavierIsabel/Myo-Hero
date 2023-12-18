using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private int _classes = 4; // Number of classes
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
    private Color[] _colors = { new(0,0.39f,1),
                                Color.red,
                                Color.yellow,
                                Color.green}; // Colors of all possible rings
    private Color[] _pressedColors = {  new(0,0.23f,1),
                                        new(0.74f,0,0),
                                        new(0.82f,0.82f,0),
                                        new(0,0.74f,0)}; // Colors of all possible pressed rings
    public KeyCode[] _keyList = {   KeyCode.A,
                                    KeyCode.S,
                                    KeyCode.D,
                                    KeyCode.F,
                                    KeyCode.G}; // All Keys
    public List<float> _holdsScore = new(); // List of hold scores
    public List<float> _inTimingsScore = new(); // List of IN timings scores
    public List<float> _outTimingsScore = new(); // List of OUT timings scores

    private Text _scoreText; // Score Text component
    public int _scoreInt = 0; // Score value

    void Awake()
    {
        // Generate Play Area components like rings
        GeneratePlayArea();
        // Generate all notes of song in the game
        NoteGenerator();
    }

    void Update()
    {
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
            Debug.Log(_holdsScore.Count);
            Debug.Log(_inTimingsScore.Count);
            Debug.Log(_outTimingsScore.Count);
            GameObject.Find("StatsTracker").GetComponent<StatsTracker>().WriteStats(_notesList,
                                                                                    _notesLengthList,
                                                                                    _holdsScore,
                                                                                    _inTimingsScore,
                                                                                    _outTimingsScore);
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
            GameObject _ring = Instantiate (_ringPrefab, new Vector3(_ringPosition,-4, 0), Quaternion.identity);
            _ring.transform.Find("Circle").GetComponent<SpriteRenderer>().color = _colors[i];
            _rings[i] = _ring;
        }
        // Get Score component and start song
        _scoreText = GameObject.Find("Canvas/ScoreText").GetComponent<Text>();
        gameObject.GetComponent<AudioSource>().Play();
    }

    void Controller() {
        if (PlayerPrefs.GetString("ControlDevice") == "Keyboard") {
            for (int i=0;i<_classes;i++) {
                if (Input.GetKey(_keyList[i])) {
                    _rings[i].transform.Find("Circle").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle").GetComponent<SpriteRenderer>().color = _colors[i];
                }
            }
        } else { //Put BioPoint and BioArmBand in same category. Assumes same number of classes
            for (int i=0;i<_classes;i++) {
                if (Input.GetKey(_keyList[i])) { //Should be changed to match BioPoint Controls
                    _rings[i].transform.Find("Circle").GetComponent<SpriteRenderer>().color = _pressedColors[i];
                } else {
                    _rings[i].transform.Find("Circle").GetComponent<SpriteRenderer>().color = _colors[i];
                }
            }
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
                                                                int.Parse(_noteTime) + 5, //Give 5 seconds for the song to start
                                                                1f), //Layer of collision
                                                    Quaternion.identity);
            _notesGameobjects.Add(_noteObject);
            //Set Size and Color
            SizeNColorNote(_noteObject, float.Parse(_noteLength), int.Parse(_note));
        }
        // Set time according to length of song
        _time = (int.Parse(_notes[^1][0]) + int.Parse(_notes[^1][2]) + 10f) / _tempo;
    }

    void SizeNColorNote(GameObject _note, float _size, int _alley) {
        Transform _noteFront = _note.transform.Find("CircleFront");;
        Transform _noteMid = _note.transform.Find("Square");;
        Transform _noteEnd = _note.transform.Find("CircleBack");;
        Transform _noteFrontS = _note.transform.Find("SmallCircleFront");;
        Transform _noteMidS = _note.transform.Find("SmallSquare");;
        Transform _noteEndS = _note.transform.Find("SmallCircleBack");;
        if (_size == 1) {
            Destroy(_noteMid.gameObject);
            Destroy(_noteEnd.gameObject);
            Destroy(_noteMidS.gameObject);
            Destroy(_noteEndS.gameObject);
        } else {
            // This code changes the size of the note
            _noteEnd.localPosition = new Vector3(0,0.5f * (_size - 1), 0);
            _noteMid.localScale = new Vector3(0.5f,_size - 1, 1);
            _noteEndS.localPosition = new Vector3(0,0.5f * (_size - 1), -0.1f);
            _noteMidS.localScale = new Vector3(0.25f,_size - 1, 1);
            _noteEnd.GetComponent<SpriteRenderer>().color = _colors[_alley];
            _noteMid.GetComponent<SpriteRenderer>().color = _colors[_alley];
        }
        _noteFront.localPosition = new Vector3(0,-0.5f * (_size - 1), 0);
        _noteFrontS.localPosition = new Vector3(0,-0.5f * (_size - 1), -0.1f);
        _noteFront.GetComponent<SpriteRenderer>().color = _colors[_alley];
    }
}

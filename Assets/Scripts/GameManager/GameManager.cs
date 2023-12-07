using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int _classes = 4;
    private StatsTracker _statstracker;
    private float _time = 0f;
    private int _songs = 0;
    public GameObject _ringPrefab;
    private GameObject[] _rings;
    private Color[] _colors = {new Color(0,0.39f,1), Color.red, Color.yellow, Color.green};
    private Color[] _pressedColors = {new Color(0,0.23f,1), new Color(0.74f,0,0), new Color(0.82f,0.82f,0), new Color(0,0.74f,0)};
    private KeyCode[] _keyList = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};
    void Awake()
    {
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
        float _ringPosition = 0f;
        for (int i=0;i<_classes;i++) {
            if (i == 0) {_ringPosition += -0.75f * (_classes - 1);}
            else {_ringPosition += 1.5f;}
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
        //Read all notes
        String path = "Assets/Resources/song" + UnityEngine.Random.Range(0,_songs).ToString() + ".txt";
        StreamReader reader = new StreamReader(path);
        //Generate notes

        //Add instances to notes buffer
        //Add times to times buffer
        reader.Close();
    }

    void GameGenerator() {
        //Spawn in note at right time
        //Make it go down
        //Check if collision with ring
        //Save timing for stats 
        //(Take extremity of capsule compared to end of ring)
        //You know speed so you know perfect timing and actual timing
        //Once it is out of frame. Remove Instance of note

    }
}

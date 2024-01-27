using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;


public class StatsTracker : MonoBehaviour
{
    private static StatsTracker playerInstance;

    void Awake(){
        DontDestroyOnLoad (this);
         
        if (playerInstance == null) {
            playerInstance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void WriteStats(List<string> notes, List<string> notesLengths, List<float> holdsAccuracy, List<float> inTimingsAccuracy, List<float> outTimingsAccuracy, List<int> classifications, List<float> timestamps) {
        Debug.Log("Writing Stats ...");
         DateTime timeStamp = DateTime.Now;
        string datetime = timeStamp.ToString("yyyy-MM-dd\\THH-mm-ss\\Z");
        string directory = "./Subject_" + PlayerPrefs.GetString("ID");
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        Debug.Log(notes.Count);
        Debug.Log(notesLengths.Count);
        Debug.Log(holdsAccuracy.Count);
        Debug.Log(inTimingsAccuracy.Count);
        Debug.Log(outTimingsAccuracy.Count);
        Debug.Log(classifications.Count);
        Debug.Log(timestamps.Count);
        string path = directory + "/Stats_" + PlayerPrefs.GetString("ControlDevice") + "_"+ PlayerPrefs.GetString("ControlMechanism") + "_" + datetime + ".txt";
        if (!System.IO.File.Exists(path)) {
            StreamWriter writer = new(path, true);
            string _notes = "";
            string _notesLength = "";
            string _holdsAccuracy = "";
            string _inTimingsAccuracy = "";
            string _outTimingsAccuracy = "";
            string _classifications = "";
            string _timestamps = "";

            for(int y=0; y<notes.Count; y++) {
                _notes += notes[y] + " ";
                _notesLength += notesLengths[y][0] + " ";
                _holdsAccuracy += holdsAccuracy[y] + " ";
                _inTimingsAccuracy += inTimingsAccuracy[y] + " ";
                _outTimingsAccuracy += outTimingsAccuracy[y] + " ";
            }
            for (int x=0; x<classifications.Count; x++) {
                _classifications += classifications[x] + " ";
                _timestamps += timestamps[x] + " ";
            }
            writer.WriteLine("Notes: " + _notes);
            writer.WriteLine("Notes_lengths: " + _notesLength);
            writer.WriteLine("Hold_Accuracy_For_Notes: " + _holdsAccuracy);
            writer.WriteLine("IN-Timing_Accuracy_For_Notes: " + _inTimingsAccuracy);
            writer.WriteLine("OUT-Timing_Accuracy_For_Notes: " + _outTimingsAccuracy);
            writer.WriteLine("Classifications: " + _classifications);
            writer.WriteLine("Timestamps: " + _timestamps);
            writer.Close();
            Debug.Log("Done Writing to File");
        } else {
            Debug.Log("File already exists...");
        }
    }
}
// Hold score per note
// Timing score per note
// size of each note
// class of each note
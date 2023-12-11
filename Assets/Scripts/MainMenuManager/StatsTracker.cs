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

    public void WriteStats(List<string> notes, List<string> notesLengths, List<float> holdsAccuracy, List<float> timingsAccuracy) {
        Debug.Log("Writing Stats ...");
         DateTime timeStamp = DateTime.Now;
        string datetime = timeStamp.ToString("yyyy-MM-dd\\THH-mm-ss\\Z");
        string directory = "./Subject_" + PlayerPrefs.GetString("ID");
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        string path = directory + "/Stats_" + PlayerPrefs.GetString("ControlDevice") + "_"+ PlayerPrefs.GetString("ControlMechanism") + "_" + datetime + ".txt";
        if (!System.IO.File.Exists(path)) {
            StreamWriter writer = new(path, true);
            string _notes = "";
            string _notesLength = "";
            string _holdsAccuracy = "";
            string _timingsAccuracy = "";

            for(int y=0; y<notes.Count; y++) {
                    _notes += notes[y] + " ";
                    _notesLength += notesLengths[y][0] + " ";
                    _holdsAccuracy += holdsAccuracy[y] + " ";
                    _timingsAccuracy += timingsAccuracy[y] + " ";
            }
            writer.WriteLine("Notes: " + _notes);
            writer.WriteLine("Notes_lengths: " + _notesLength);
            writer.WriteLine("Hold_Accuracy_For_Notes: " + _holdsAccuracy);
            writer.WriteLine("Timing_Accuracy_For_Notes: " + _timingsAccuracy);
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
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

    public void WriteStats(List<float> notes, List<float> notesLengths, List<float> holdsAccuracy, List<float> timingsAccuracy) {
        Debug.Log("Writing Stats ...");
         DateTime timeStamp = DateTime.Now;
        string datetime = timeStamp.ToString("yyyy-MM-dd\\THH-mm-ss\\Z");
        string path = "Assets/Resources/Stats_" + datetime + ".txt";
        if (!System.IO.File.Exists(path)) {
            StreamWriter writer = new StreamWriter(path, true);
            string _notes = "";
            string _notesLength = "";
            string _holdsAccuracy = "";
            string _timingsAccuracy = "";

            for(int y=0; y<notes.Count; y++) {
                    _notes += notes[y] + " ";
                    _notesLength += notesLengths[y] + " ";
                    _holdsAccuracy += holdsAccuracy[y] + " ";
                    _timingsAccuracy += timingsAccuracy[y] + " ";
            }
            writer.WriteLine("Notes: " + _notes);
            writer.WriteLine("notes_lengths: " + _notesLength);
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
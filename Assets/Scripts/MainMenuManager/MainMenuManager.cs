using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject playPanel;
    public InputField _inputField;
    //private SensorReader _sensorReader;

    
    void Start()
    {
        //_sensorReader = FindObjectOfType<SensorReader>;
        _inputField = _inputField.GetComponent<InputField>();
    }

    public void ExitClicked() {
        // Should tell streamer to quit
        Application.Quit();
    }

    public void ExitPlayPanelClicked() {
        playPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void BioPointPlayClicked() {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        PlayerPrefs.SetString("ControlDevice","BioPoint");
        PlayerPrefs.SetString("ID", _inputField.text);
    }

    public void BioArmBandPlayClicked() {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        PlayerPrefs.SetString("ControlDevice","BioArmBand");
        PlayerPrefs.SetString("ID", _inputField.text);
    }

    public void KeyboardPlayClicked() {
        PlayerPrefs.SetString("ControlDevice","Keyboard");
        PlayerPrefs.SetString("ControlMechanism","");
        PlayerPrefs.SetString("ID", _inputField.text);
        SceneManager.LoadScene("GameScene");
    }
    public void BackClicked() {
        mainPanel.SetActive(true);
        playPanel.SetActive(false);
    }

    public void EMGClicked() {
        PlayerPrefs.SetString("ControlMechanism","EMG");
        SceneManager.LoadScene("GameScene");
    }

    public void IMUClicked() {
        PlayerPrefs.SetString("ControlMechanism","IMU");
        SceneManager.LoadScene("GameScene");
    }
    public void ZClicked() {
        PlayerPrefs.SetString("ControlMechanism","Z");
        SceneManager.LoadScene("GameScene");
    }
    public void EMGIMUClicked() {
        PlayerPrefs.SetString("ControlMechanism","EMGIMU");
        SceneManager.LoadScene("GameScene");
    }
    public void EMGZClicked() {
        PlayerPrefs.SetString("ControlMechanism","EMGZ");
        SceneManager.LoadScene("GameScene");
    }
    public void IMUZClicked() {
        PlayerPrefs.SetString("ControlMechanism","IMUZ");
        SceneManager.LoadScene("GameScene");
    }
    public void EMGIMUZClicked() {
        PlayerPrefs.SetString("ControlMechanism","EMGIMUZ");
        SceneManager.LoadScene("GameScene");
    }
}

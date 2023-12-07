using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject playPanel;

    
    void Start()
    {
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
    }

    public void BioArmBandPlayClicked() {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        PlayerPrefs.SetString("ControlDevice","BioArmBand");
    }

    public void KeyboardPlayClicked() {
        PlayerPrefs.SetString("ControlMechanism","Keyboard");
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

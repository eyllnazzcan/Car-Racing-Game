using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel, settingsPanel, LoadingPanel, tutorialPanel;

    public Image trackSelectImage, racerSelectImage;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(RaceInfoManager.instance.enteredRace)
        {
            trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
            racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetup();
        }

        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
        PlayerPrefs.SetInt("Track 3_unlocked", 1);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Keys deleted");
        }
#endif
    }

    public void StartGame()
    {
        RaceInfoManager.instance.enteredRace = true;
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void OpenRaceSetup()
    {
        raceSetupPanel.SetActive(true);
    }

    public void OpenLoading()
    {
        LoadingPanel.SetActive(true);
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseLoading()
    {
        LoadingPanel.SetActive(false);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void CloseRaceSetup()
    {
        raceSetupPanel.SetActive(false);
    }

    public void OpenTrackSelect()
    {
        trackSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    public void OpenRacerSelect()
    {
        racerSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }


    public void CloseRacerSelect()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}

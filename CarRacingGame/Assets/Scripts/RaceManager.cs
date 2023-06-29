using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Dan.Demo; //sonradan eklendi


public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;

    public Checkpoint[] allCheckpoints;

    public int totalLaps;

    public LeaderBoardOnTrack1 leaderBoard; //sonradan eklendi

    public CarController playerCar;
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;
    public float timeBetweenPosCheck = .2f;
    private float posChkCounter;

    public float aiDefaultSpeed = 30f, playerDefaultSpeed = 30f, rubberBandSpeedMod = 3.5f, rubBandAccel = .5f;

    
    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    public int playerStartPosition, aiNumberToSpawn;
    public Transform[] startPoints;
    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceCompleted;

    public string raceCompleteScene;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        totalLaps = RaceInfoManager.instance.noOfLaps;
        aiNumberToSpawn = RaceInfoManager.instance.noOfAI;

        for(int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countDownText.text = countdownCurrent + "!";

        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1);

        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitcher.instance.SeTarget(playerCar);

        //playerCar.transform.position = startPoints[playerStartPosition].position;
        //playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        for(int i = 0; i < aiNumberToSpawn + 1; i++)
        {
            if(i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);

                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if (carsToSpawn.Count > aiNumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }

        UIManager.instance.positionText.text = (playerStartPosition + 1) + "/" + (allAICars.Count + 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if(startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countDownText.text = countdownCurrent + "!";

                if (countdownCurrent == 0)
                {
                    isStarting = false;

                    UIManager.instance.countDownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }
        }
        else
        {

            posChkCounter -= Time.deltaTime;
            if (posChkCounter <= 0)
            {

                playerPosition = 1;

                foreach (CarController aiCar in allAICars)
                {
                    if (aiCar.currentLap > playerCar.currentLap)
                    {
                        playerPosition++;
                    }
                    else if (aiCar.currentLap == playerCar.currentLap)
                    {
                        if (aiCar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPosition++;
                        }
                        else if (aiCar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            if (Vector3.Distance(aiCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position))
                            {
                                playerPosition++;
                            }
                        }
                    }
                }

                posChkCounter = timeBetweenPosCheck;

                UIManager.instance.positionText.text = playerPosition + "/" + (allAICars.Count + 1);
            }

            //manage rubber banding
            if (playerPosition == 1)
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeedMod, rubBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod, rubBandAccel * Time.deltaTime);
            }
            else
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubBandAccel * Time.deltaTime);
            }
        }


        //for testing
        if (Input.GetKeyDown(KeyCode.H))
        {
            UIManager.instance.gameObject.SetActive(!UIManager.instance.gameObject.activeInHierarchy);
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            playerCar.isAI = !playerCar.isAI;
            playerCar.SwitchToAI();
        }
    }

    public void FinishRace()
    {
       
            raceCompleted = true;

        switch(playerPosition)
        {
            case 1:
                UIManager.instance.raceResultText.text = "You finished 1st";

                if (RaceInfoManager.instance.trackToUnlock != "")
                {
                    if (!PlayerPrefs.HasKey(RaceInfoManager.instance.trackToUnlock + "_unlocked"))
                    {
                        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToUnlock + "_unlocked", 1);
                        UIManager.instance.trackUnlockedMessage.SetActive(true);
                    }
                }

                break;

            case 2:
                UIManager.instance.raceResultText.text = "You finished 2nd";

                break;

            case 3:
                UIManager.instance.raceResultText.text = "You finished 3rd";

                break;

            default:

                UIManager.instance.raceResultText.text = "You finished " + playerPosition + "th";

                break;
        }


        UIManager.instance.resultsScreen.SetActive(true);

        playerCar.bestLapTime = playerCar.lapTime; //3. lap timeı alıyor

        leaderBoard = GameObject.FindGameObjectWithTag("LeaderBoard").GetComponent<LeaderBoardOnTrack1>();
        leaderBoard.SubmitScore();
        
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompleteScene);

    }
}

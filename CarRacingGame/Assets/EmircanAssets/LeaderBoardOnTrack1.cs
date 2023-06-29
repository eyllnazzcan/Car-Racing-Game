using Dan.Main;
using Dan.Models;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
namespace Dan.Demo
{
    public class LeaderBoardOnTrack1 : MonoBehaviour
    {
        [SerializeField] private string _leaderboardPublicKey = "b7ff73b4b6d8d0a43c62aac4e8ca754b00386547a9fc8c0cb9214d71d45be63a";

        [SerializeField] private TextMeshProUGUI _playerScoreText;
        [SerializeField] private TextMeshProUGUI[] _entryFields;
        [SerializeField] private CarController carcontrol;
        [SerializeField] private RaceManager raceManage;
        [SerializeField] private GameObject raceManageerObject;
        [SerializeField] private TMP_InputField _playerUsernameInput;

        public int _playerScore;

        private void Start()
        {
            Load();
            _playerUsernameInput.text = PlayerPrefs.GetString("userName");
            StartCoroutine(StartCar());
        }

        IEnumerator StartCar()
        {
            yield return new WaitForSeconds(1);
            raceManage = raceManageerObject.GetComponent<RaceManager>();
            carcontrol = raceManage.playerCar.GetComponent<CarController>();
        }

        public void AddPlayerScore()
        {
            _playerScore++;
            _playerScoreText.text = "Your score: " + _playerScore;
        }

        public void Load() => LeaderboardCreator.GetLeaderboard(_leaderboardPublicKey, OnLeaderboardLoaded);

        private void OnLeaderboardLoaded(Entry[] entries)
        {
            foreach (var entryField in _entryFields)
            {
                entryField.text = "";
            }

            for (int i = 0; i < entries.Length; i++)
            {
                _entryFields[i].text = $"{i + 1}. {entries[i].Username} : {entries[i].Score}";
                
            }
        }

        public void Submit()
        {
            LeaderboardCreator.UploadNewEntry(_leaderboardPublicKey, _playerUsernameInput.text, _playerScore, Callback);
        }

        public void SubmitName()
        {
            LeaderboardCreator.UploadNewEntry(_leaderboardPublicKey, _playerUsernameInput.text, Convert.ToInt32(PlayerPrefs.GetFloat("Score")), Callback);
            PlayerPrefs.SetString("userName", _playerUsernameInput.text);
        }

        public void SubmitScore()
        {
            LeaderboardCreator.UploadNewEntry(_leaderboardPublicKey, PlayerPrefs.GetString("userName"), Convert.ToInt32(carcontrol.bestLapTime), Callback);
            PlayerPrefs.SetInt("Score", Convert.ToInt32(carcontrol.bestLapTime));           
        }

        public void DeleteEntry()
        {
            LeaderboardCreator.DeleteEntry(_leaderboardPublicKey, Callback);
        }

        private void Callback(bool success)
        {
            if (success)
                Load();
        }

    }
}

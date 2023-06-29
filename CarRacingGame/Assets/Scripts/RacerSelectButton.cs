using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacerSelectButton : MonoBehaviour
{
    public Image racerImage;

    public CarController racerToSet;

    public void SelectRacer()
    {
        RaceInfoManager.instance.racerToUse = racerToSet;
        RaceInfoManager.instance.racerSprite = racerImage.sprite;

        MainMenu.instance.racerSelectImage.sprite = racerImage.sprite;

        MainMenu.instance.CloseRacerSelect();
    }
}

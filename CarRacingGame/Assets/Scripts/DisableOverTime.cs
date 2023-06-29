using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    public float timeToDisable;

    // Update is called once per frame
    void Update()
    {
        timeToDisable -= Time.deltaTime;
        if(timeToDisable<= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

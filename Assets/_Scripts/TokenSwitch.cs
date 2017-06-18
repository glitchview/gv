using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenSwitch : MonoBehaviour {
    // Use this for initialization
    private bool isBlocked = false;
    private bool isVuforiaEnabled = true;
    private bool showImageTarget = true;
    private GameObject[] allChildren;

    private void Start()
    {
        allChildren = GetComponentsInChildren<GameObject>();
    }

    public void Update()
    {
        bool newStatus = isBlocked && isVuforiaEnabled;
        if (newStatus != showImageTarget)
        {
            showImageTarget = newStatus;
            foreach (GameObject child in allChildren)
            {
                Debug.Log("TS: Setting child status to=" + showImageTarget);
                child.SetActive(showImageTarget);
            }
        }
    }

    public void setBlocked(bool isBlocked)
    {   
        Debug.Log("setting block status=" + isBlocked);
        this.isBlocked = isBlocked;
    }

    public void setVuforiaEnabled(bool isVuforiaEnabled) {
        Debug.Log("setting vuforia enabled=" + isVuforiaEnabled);
        this.isVuforiaEnabled = isVuforiaEnabled;
    }
}

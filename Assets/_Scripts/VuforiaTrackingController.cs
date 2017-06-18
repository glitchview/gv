using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuforiaTrackingController : MonoBehaviour {

    public void setBlocked(bool isBlocked)
    {
        getTarget().setBlocked(isBlocked);
    }

    public void setVuforiaEnabled(bool isVuforiaEnabled)
    {
        foreach (TokenSwitch ts in getTokenSwitches())
        {
            ts.setVuforiaEnabled(isVuforiaEnabled);
        }
    }
    
    private TokenSwitch getTarget()
    {
        return getTokenSwitches()[0];
    }

    private TokenSwitch[] getTokenSwitches()
    {
        return FindObjectsOfType(typeof(TokenSwitch)) as TokenSwitch[];
    }
}

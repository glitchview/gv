using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBasedGlitchUpdater : MonoBehaviour {
   
    public bool isTokenEnabled = false;
    // Use this for initialization
    float elapsed = 0f;

    public GameObject glitcherObject;
  /*
   * void Start () {


    }
    */

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= 1f)
        {
            elapsed = elapsed % 1f;
            Debug.Log("TokenBasedGlitchUpdater... position=" + transform.position.ToString() + ", rotation=" + transform.rotation.eulerAngles.ToString());
            if (glitcherObject != null)
            {
                // set rotation/position

            }
        }
    }
}

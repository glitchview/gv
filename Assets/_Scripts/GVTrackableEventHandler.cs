using UnityEngine;
using System.Collections;
using Vuforia;

public class GVTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    public Transform myModelPrefab;
    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
   
   
    public void OnTrackableStateChanged(
      TrackableBehaviour.Status previousStatus,
      TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Detected object");
            OnTrackingFound();
        }
    }
    private void OnTrackingFound()
    {
        
        if (myModelPrefab != null)
        {
            Debug.Log("tracking found");
            Transform myModelTrf = GameObject.Instantiate(myModelPrefab) as Transform;
            myModelTrf.parent = mTrackableBehaviour.transform;
            myModelTrf.localPosition = new Vector3(0f, 0f, 0f);
            myModelTrf.localRotation = Quaternion.identity;
            myModelTrf.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
            myModelTrf.gameObject.SetActive(true);
        } else
        {
            Debug.Log("tracking lost!");
        }
    }
}
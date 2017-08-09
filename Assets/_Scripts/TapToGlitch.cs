using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
using HoloToolkit.Unity;
using System.Text;
using UnityEngine.VR.WSA.Input;

public class TapToGlitch : MonoBehaviour
{
    public Camera TargetCamera;
    public Glitcher Glitcher;
    private GestureRecognizer recognizer;
    private bool hasCaptured = false;

    public GameObject[] introUI;

    public bool GetHasCaptured()
    {
        return hasCaptured;
    }

    // Use this for initialization
    void Start()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);
        recognizer.TappedEvent += Recognizer_TappedEvent; ;
        recognizer.StartCapturingGestures();
    }

    private void Recognizer_HoldCompletedEvent(InteractionSourceKind source, Ray headRay)
    {
        Debug.Log("Recognizer_HoldCompletedEvent");
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        UpdatePositionToCapturePoint();
    }

    public void Recognizer_BlockUserSpeechCommand()
    {
        introUI[0].gameObject.SetActive(false);
        introUI[1].gameObject.SetActive(false);
        introUI[2].gameObject.SetActive(true);
        UpdatePositionToCapturePoint();
    }

    void UpdatePositionToCapturePoint()
    {
        this.transform.SetPositionAndRotation(TargetCamera.transform.position, TargetCamera.transform.rotation);

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, fwd, out hitInfo, 100))
        {
            hasCaptured = true;
            if (Glitcher != null)
            {
                Glitcher.HitPoint = hitInfo.point;
                Glitcher.EnableGlitch = true;
            }
        }
        else
        {
            Glitcher.EnableGlitch = false;
        }
    }

    void OnDestroy()
    {
        recognizer.TappedEvent -= Recognizer_TappedEvent;
    }
}

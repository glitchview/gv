using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using System;
using Vuforia;


public class VoiceControlledMethods : MonoBehaviour {

    string emotionsApiKey = "30915671aee241ddb4cf42d08b190ab9";
    string emotionApiEndpoint = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";

    PhotoCapture photoCaptureObject;
    Resolution cameraResolution;
    Texture2D targetTexture;
    CameraParameters cameraParameters;

    public VuforiaBehaviour vuforiaBehaviour;
    public Text t;

    public GameObject[] introUI;

    public GameObject HappyGameObject;
    public GameObject SadGameObject;
    public GameObject AngryGameObject;

    public void Start()
    {
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        cameraParameters = new CameraParameters();
    }

    public void ScanRoom()
    {
        //TODOs 
        if(!vuforiaBehaviour.enabled)
            vuforiaBehaviour.enabled = false;

        introUI[0].gameObject.SetActive(false);
        introUI[1].gameObject.SetActive(false);
        introUI[2].gameObject.SetActive(true);

        PhotoCapture.CreateAsync(false, OnPhotoCaptureReturned);
    }

    private void OnPhotoCaptureReturned(PhotoCapture captureObjected)
    {
        photoCaptureObject = captureObjected;
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            //TODO take photo
            string filename = string.Format(@"room_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedAndSavedPhoto);
        }
        else
        {
            //TODO mention error turning on photomode
        }
    }

    private void OnCapturedAndSavedPhoto(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"room_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            byte[] image = File.ReadAllBytes(filePath);
            StartCoroutine(RunEmotionDetection(image));
        }
        else
        {
            //TODO could not save to disk
        }

        photoCaptureObject.StopPhotoModeAsync(OnStopPhotoMode);
    }

    private void OnStopPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    IEnumerator RunEmotionDetection(byte[] image)
    {
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", emotionsApiKey },
            { "Content-Type", "application/octet-stream" }
        };

        WWW www = new WWW(emotionApiEndpoint, image, headers);
        yield return www;
        var jsonResults = www.text;
        t.text = jsonResults;

        if (!vuforiaBehaviour.enabled)
            vuforiaBehaviour.enabled = true;

        // parse json and find maximum represented emotion
        EmotionAnalysisResult[] listOfResults = parseFaces(jsonResults); // JsonHelper.FromJson<Face>(jsonResults);
        int emotionResult = MaxEmotion(listOfResults[0].scores);
        t.text = emotionResult.ToString();

        switch (emotionResult)
        {
            case 0:
                ShowAnger();
                break;
            case 4:
                ShowHappiness();
                break;
            case 6:
                ShowSadness();
                break;
            default:
                print("Default case");
                break;
        }
    }

    private void ShowAnger()
    {
        AngryGameObject.SetActive(true);
    }

    private void ShowHappiness()
    {
        HappyGameObject.SetActive(true);
    }

    private void ShowSadness()
    {
        SadGameObject.SetActive(true);
    }

    public int MaxEmotion(Scores s)
    {
        float[] scoreArray = { s.anger, s.contempt, s.disgust, s.fear, s.happiness, s.neutral, s.sadness, s.surprise };
        int maxScoreIndex = 0;
        float maxScore = scoreArray[0];

        for (int i = 1; i < 8; i++)
        {
            if(maxScore < scoreArray[i])
            {
                maxScoreIndex = i;
                maxScore = scoreArray[i];
            }
        }

        return maxScoreIndex;
    }

    FaceRectangle parseFaceRectangle(SimpleJSON.JSONObject faceJSON)
    {
        IEnumerator r = faceJSON.GetEnumerator();
        FaceRectangle fr = new FaceRectangle();
        while (r.MoveNext())
        {
            KeyValuePair<string, SimpleJSON.JSONNode> bar = (KeyValuePair<string, SimpleJSON.JSONNode>)r.Current;
            SimpleJSON.JSONNode val = bar.Value;
            string key = bar.Key;
            var castProp = fr.GetType().GetProperty(key);
            castProp.SetValue(fr, val.AsInt, null);
            Debug.Log("FR: " + bar.Key + "=" + val.AsInt);
        }
        return fr;
    }

    Scores parseScores(SimpleJSON.JSONObject scoreJSON)
    {
        IEnumerator r = scoreJSON.GetEnumerator();
        Scores fr = new Scores();
        while (r.MoveNext())
        {
            KeyValuePair<string, SimpleJSON.JSONNode> bar = (KeyValuePair<string, SimpleJSON.JSONNode>)r.Current;
            SimpleJSON.JSONNode val = bar.Value;
            string key = bar.Key;
            var castProp = fr.GetType().GetProperty(key);
            castProp.SetValue(fr, val.AsFloat, null);
            Debug.Log("FR: " + bar.Key + "=" + val.AsFloat);
        }
        return fr;
    }

    EmotionAnalysisResult[] parseFaces(string jsonResults)
    {
        var N = SimpleJSON.JSON.Parse(jsonResults);
        SimpleJSON.JSONArray a = N.AsArray;
       
        ArrayList faces = new ArrayList();
        foreach (SimpleJSON.JSONNode c in a.Children)
        {
           FaceRectangle fr = parseFaceRectangle(c["faceRectangle"].AsObject);
           Scores sc = parseScores(c["scores"].AsObject);
           EmotionAnalysisResult f = new EmotionAnalysisResult();
           f.scores = sc;
           f.faceRectangle = fr;

            faces.Add(f);
        }

        return (EmotionAnalysisResult[]) faces.ToArray(typeof (EmotionAnalysisResult));
    }

    public void RestartSystem()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }

#region earlymethods
    //private void ApplyImageToQuad(string filePath)
    //{
    //    byte[] image = File.ReadAllBytes(filePath);
    //    targetTexture.LoadImage(image);
    //    quadTest.material.SetTexture("_MainTex", targetTexture);
    //}
#endregion
}

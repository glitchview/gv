
using CognitiveServices;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.Windows;

public class Hud : MonoBehaviour
{


    PhotoCapture _photoCaptureObject = null;
    IEnumerator coroutine;

    public string _subscriptionKey = "cdd17e55188a409ebe672c6b174b63f5";
    string _computerVisionEndpoint = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";
    string _ocrEndpoint = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";

    public TextToSpeechManager textToSpeechManager;

    void Start()
    {

        Debug.Log("Starting Loop");
        StartCoroutine(CoroLoop());
    }

    IEnumerator CoroLoop()
    {
		int secondsInterval = 20;
		while (true) {
			AnalyzeScene();
			yield return new WaitForSeconds(secondsInterval);
		}
    }


    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);

    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"face_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            //doing this to get formatted image
            _photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);

        }
        else
        {
            Debug.Log("Unable to get camera photo thing");

        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"face_analysis.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            //byte[] image = File.ReadAllBytes(filePath);
            //GetTagsAndFaces(image);

        }
        else
        {
            Debug.Log("Failed to save photo to disk");
        }
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }


    // Update is called once per frame
    void Update()
    {

    }

    void AnalyzeScene()
    {
        Debug.Log("Trying to take Photo");
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    public void GetTagsAndFaces(byte[] image)
    {

        try
        {
            coroutine = RunComputerVision(image);
            StartCoroutine(coroutine);
        }
        catch (Exception)
        {

            Debug.Log("Computer vision error 1");
        }
    }

    
    IEnumerator RunComputerVision(byte[] image)
    {
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", _subscriptionKey },
            { "Content-Type", "application/octet-stream" }
        };

        WWW www = new WWW(_computerVisionEndpoint, image, headers);
        yield return www;

        List<string> tags = new List<string>();
        var jsonResults = www.text;
        var myObject = JsonUtility.FromJson<AnalysisResult>(jsonResults);
        
        List<string> faces = new List<string>();
        Debug.Log(jsonResults);
        Debug.Log("JSON results");

        foreach (var face in myObject.faces)
         { Debug.Log(string.Format("{0} scanned: age {1}.", face.gender, face.age));
  }
    }

    IEnumerator Read(byte[] image)
    {
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", _subscriptionKey },
            { "Content-Type", "application/octet-stream" }
        };

        WWW www = new WWW(_ocrEndpoint, image, headers);
        yield return www;

        List<string> words = new List<string>();
        var jsonResults = www.text;
        var ocrResults = JsonUtility.FromJson<OcrResults>(jsonResults);
        foreach (var region in ocrResults.regions)
        foreach (var line in region.lines)
        foreach (var word in line.words)
        {
            words.Add(word.text);
        }

        string textToRead = string.Join(" ", words.ToArray());

        if (textToRead.Length > 0)
        {
            if (ocrResults.language.ToLower() == "en")
            {
                textToSpeechManager.SpeakText(textToRead);
            }
        }else
        {
        }
       

    }
}

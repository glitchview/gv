using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EmotionAnalysisResult {
    public FaceRectangle faceRectangle;
    public Scores scores;    
}

[Serializable]
public class FaceRectangle
{
    public int left { get; set; }
    public int top { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

[Serializable]
public class Scores
{
    public float anger { get; set; }
    public float contempt { get; set; }
    public float disgust { get; set; }
    public float fear { get; set; }
    public float happiness { get; set; }
    public float neutral { get; set; }
    public float sadness { get; set; }
    public float surprise { get; set; }
}

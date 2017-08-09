using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitcher : MonoBehaviour
{

    public Vector3 HitPoint;
    public bool EnableGlitch = false;
    public float DisruptFadeStartDistance = 0.25f;
    public float DisruptFadeMaxDistance = 1.0f;
    public float BloatMeshAmount = 0.2f;
    public Texture2D GlitchTexture;

    HoloToolkit.Unity.SpatialMapping.SpatialMappingManager spatMan;

    void Start()
    {
        spatMan = GetComponent<HoloToolkit.Unity.SpatialMapping.SpatialMappingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EnableGlitch)
        {
            spatMan.SurfaceMaterial.SetTexture("_MainTex", GlitchTexture);
            spatMan.SurfaceMaterial.SetFloat("_FadeStartDist", DisruptFadeStartDistance);
            spatMan.SurfaceMaterial.SetFloat("_FadeMaxDist", DisruptFadeMaxDistance);
            spatMan.SurfaceMaterial.SetFloat("_BloatAmount", BloatMeshAmount);
            spatMan.SurfaceMaterial.SetVector("_TexOffset", new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
            spatMan.SurfaceMaterial.SetVector("_HitPoint", HitPoint);

            spatMan.DrawVisualMeshes = true;
        }
        else
        {
            spatMan.DrawVisualMeshes = false;
        }
    }
}

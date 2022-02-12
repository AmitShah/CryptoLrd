using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class ScreenCA : MonoBehaviour
{

    public int  sample_count=48;
    public float aberration = 1f;
    public float radial_intensity = 1f;
    public int auto = 0;

    public Material material;

    // Creates a private material used to the effect
    public void Awake()
    {
        //material = new Material(Shader.Find("Amit/CA"));
    }

    // Postprocess the image
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        

        material.SetFloat("_SampleCount", sample_count);
        material.SetFloat("_Aberration", aberration);
        material.SetFloat("AutoSample", auto);
        material.SetFloat("_RadialIntensity", radial_intensity);
        Graphics.Blit(source, destination, material);
    }
}
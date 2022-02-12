using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurlNoise : MonoBehaviour
{


	public Material mat;
	public Material blur;
	public Material quad;
	public Texture noiseTexture;
	public Texture stamp;
	public float StampSize = 0.4f;
	public RenderTexture buffer;
	public RenderTexture temp;
	public Material blend;
    public RenderTexture source;
    public RenderTexture destination;

    public void Awake()
    {
		mat = new Material(Shader.Find("Custom/CurlNoise"));
		mat.SetTexture(Shader.PropertyToID("_NoiseTex"), noiseTexture);
        source = new RenderTexture(Camera.main.pixelHeight, Camera.main.pixelWidth, 24);
        destination = new RenderTexture(Camera.main.pixelHeight, Camera.main.pixelWidth, 24);
        
    }

    // Use this for initialization
    void Start()
	{		

    }

	public void Update()
	{

        if (Input.GetMouseButton(0))
        {

            var currentActiveRT = RenderTexture.active;
            RenderTexture.active = source;                      //Set my RenderTexture active so DrawTexture will draw to it.
            GL.PushMatrix();                                //Saves both projection and modelview matrices to the matrix stack.
            GL.LoadPixelMatrix(0, source.width, source.height, 0);            //Setup a matrix for pixel-correct rendering.
                                                                              //Draw my stampTexture on my RenderTexture positioned by posX and posY.
            Graphics.DrawTexture(
            new Rect(Input.mousePosition.x - stamp.width * StampSize,
                (source.height - Input.mousePosition.y) - stamp.height * StampSize,
                stamp.width * StampSize,
                stamp.height * StampSize), stamp);
            GL.PopMatrix();                                //Restores both projection and modelview matrices off the top of the matrix stack.
            RenderTexture.active = currentActiveRT;

        }

        Graphics.Blit (source, destination,mat);
        source.DiscardContents();
        Graphics.Blit(destination, source);
        quad.mainTexture = source;
    }

    public void OnDestroy()
    {
       
        source.Release();
        destination.Release();

    }

    ////correctly stamping to a rendertexture
    ////http://answers.unity3d.com/questions/327984/graphicsdrawtexture-to-rendertexture-not-working.html
    //void Update()
    //{

    //	if (Input.GetMouseButton(0))
    //	{
    //		var currentActiveRT = RenderTexture.active;
    //		RenderTexture.active = buffer;                      //Set my RenderTexture active so DrawTexture will draw to it.
    //		GL.PushMatrix();                                //Saves both projection and modelview matrices to the matrix stack.
    //		GL.LoadPixelMatrix(0, buffer.width, buffer.height, 0);            //Setup a matrix for pixel-correct rendering.
    //																		  //Draw my stampTexture on my RenderTexture positioned by posX and posY.
    //		Graphics.DrawTexture(
    //		new Rect(Input.mousePosition.x - stamp.width * StampSize,
    //			(buffer.height - Input.mousePosition.y) - stamp.height * StampSize,
    //			stamp.width * StampSize,
    //			stamp.height * StampSize), stamp);
    //		GL.PopMatrix();                                //Restores both projection and modelview matrices off the top of the matrix stack.
    //		RenderTexture.active = currentActiveRT;


    //	}


    //	//De-activate my RenderTexture.
    //}


}
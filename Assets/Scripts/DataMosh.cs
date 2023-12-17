using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataMosh : MonoBehaviour
{

    public Material moshing;
    float moshControl;
    float rgbControl;
    bool toggleRGB;
    public Text showMosh;
    public Text showRGB;
    

    void Start()
    {
        // To simulate data moshing, motion vector is needed
        // so we get the motion vector running
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.MotionVectors;
        moshControl = 0;
        toggleRGB = false;
        rgbControl = 0.02f;
    }
    void Update()
    {
        // we need to tell the shader when to apply the effect, when to not.
        // the easiest way is to use a global variable and tie that to a button
        //############### Controling Mosh ###############
        if (Input.GetMouseButton(1))
        {
            // holding right click
            if (moshControl < 1)
            {
                moshControl += 0.1f;
            }
        } 
        else
        {
            if (moshControl > 0)
            {
                moshControl -= 0.00008f;
            }
            
        }
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.R))
        {
            // left click clears effect
            // or press R
            moshControl = 0.0f;
        }
        Shader.SetGlobalFloat("moshAmount", moshControl);

        //############### Controling RGB Split ###############
        if (Input.GetMouseButtonDown(0))
        {
            toggleRGB = !toggleRGB;
        }
        Shader.SetGlobalInt("isRGB", toggleRGB ? 1:0);
        if (Input.GetKey(KeyCode.E))
        {
            // increase the offset
            rgbControl += 0.001f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            // increase the offset
            rgbControl -= 0.001f;
        }
        Shader.SetGlobalFloat("rgbAmount", rgbControl);

        //############### UI display ###############
        showMosh.text = "mosh: " + moshControl.ToString();
        showRGB.text = "rgb: " + rgbControl.ToString();
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // function that gets called after rendering.
        // copy the render texture over using blit.
        // when blit is called, source gets passed on into moshing's _MainTex
        Graphics.Blit(source, destination, moshing);
    }


}

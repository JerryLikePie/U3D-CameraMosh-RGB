using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBshift : MonoBehaviour
{
    /*
     * This is the script for the RGB shift applied to the camera.
     * It's not.
     * I decieded to switch everything to GPU rendering
     * RGB shift is implemented in the shader.
     */

    Camera cam;
    public int RshiftAmount;
    public int GshiftAmount;
    public int BshiftAmount;


    void Start()
    {
        // get the main cam, it's the only cam I'll be using.
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    Texture2D SaveView()
    {
        // have a texture that acts as a buffer
        RenderTexture buffer = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = buffer;
        // generate a texture the size of the screen.
        Texture2D currentScreen = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
        cam.Render();
        RenderTexture.active = buffer;
        // save the current camera view to that texture
        currentScreen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        // deactivate
        cam.targetTexture = null;
        RenderTexture.active = null; 
        Destroy(buffer);
        return currentScreen;
    }

    void Update()
    {
        
    }

    
}

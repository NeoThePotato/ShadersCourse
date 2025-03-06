                                                 using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ComputeShaderMono : MonoBehaviour
{

    public ComputeShader shader;
    public int texResolution = 256;

    Renderer rend;
    RenderTexture outputTexture;

    int kernelHandle;

    // Use this for initialization
    void Start()                
    {
        outputTexture = new RenderTexture(texResolution, texResolution, 1);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InitShader();
    }

    private void InitShader()
    {
        kernelHandle = shader.FindKernel("CSMain");

        shader.SetTexture(kernelHandle, "Result", outputTexture);
 
        rend.sharedMaterial.SetTexture("_MainTex", outputTexture);

        DispatchShader(texResolution / 8, texResolution / 8);
    }

    private void DispatchShader(int x, int y)
    {
        shader.Dispatch(kernelHandle, x, y, 1);
    }

    void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            //Graphics.Blit(null, outputTexture, blitMaterial);
            DispatchShader(texResolution / 8, texResolution / 8);
        }
    }
}


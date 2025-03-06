using UnityEngine;

public class GPUComputeInfo : MonoBehaviour
{
    
    public  ComputeShader computeShader;
    void Start()
    {

        GetGropusBySystemInfo();
        GetGroupsByComputeShader();

    }

    void GetGropusBySystemInfo()
    {
        Debug.Log("Max Compute Shader Threads per Group: " + SystemInfo.maxComputeWorkGroupSize);
        Debug.Log("Max Threads Per Dimension: " + SystemInfo.maxComputeWorkGroupSizeX + " x " +
                  SystemInfo.maxComputeWorkGroupSizeY + " x " +
                  SystemInfo.maxComputeWorkGroupSizeZ);
    }

    void GetGroupsByComputeShader()
    {
       
        
        int kernelHandle = computeShader.FindKernel("CSMain");

        // Get the number of threads per group (this can vary based on the shader)
        uint threadGroupSizeX, threadGroupSizeY, threadGroupSizeZ;
        computeShader.GetKernelThreadGroupSizes(kernelHandle, out threadGroupSizeX, out threadGroupSizeY, out threadGroupSizeZ);

        Debug.Log($"Max Threads per Group (X: {threadGroupSizeX}, Y: {threadGroupSizeY}, Z: {threadGroupSizeZ})");
        
        // dispatch the compute shader
        computeShader.Dispatch(kernelHandle, 1, 1, 1);
    }
    
    
}
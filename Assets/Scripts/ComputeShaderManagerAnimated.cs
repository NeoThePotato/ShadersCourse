using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ComputeShaderManagerAnimated : MonoBehaviour
{

    
    //public Material blitMaterial; // Assign the Shader to it material
    public ComputeShader shader;
    public int texResolution = 1024;

    
    public Renderer targetRenderer;
    RenderTexture outputTexture;
    Material targetMaterial;
    int circlesHandle;
    int clearHandle;

    public Color clearColor = new Color();
    public Color circleColor = new Color();

    int count = 10;

    private string mainTexName = "_BaseMap";
    // Use this for initialization
    void Start()
    {
        outputTexture = new RenderTexture(texResolution, texResolution, 0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();



		targetRenderer.enabled = true;

		InitData();

		InitBufferData();
		InitShaderData();
	}

	private void InitData()
	{
		circlesHandle = shader.FindKernel("Circles");
        targetMaterial = targetRenderer.material;
	}

	// execute shader!
	private void DispatchKernels(int count)
    {
		shader.SetFloat("time", Time.time);
		shader.Dispatch(circlesHandle, count, 1, 1);
    }
    

    void Update()
	{
		DispatchKernels(count);
        shader.SetTexture( circlesHandle, "Result", outputTexture );
        targetMaterial.SetTexture("_BaseMap", outputTexture);
    }
    
    void OnDestroy()
    {
        if (outputTexture != null)
        {
            outputTexture.Release();
        }
    }


    Circle[] circleData;
    ComputeBuffer buffer;

    void InitShaderData()
    {
        int stride = (2 + 2 + 1) * 4; //2 floats origin, 2 floats velocity, 1 float radius - 4 bytes per float
        buffer = new ComputeBuffer(circleData.Length, stride);
        buffer.SetData(circleData);
        shader.SetBuffer(circlesHandle, "circlesBuffer", buffer);

    }
    
    void InitBufferData()
    {
        uint threadGroupSizeX;

        shader.GetKernelThreadGroupSizes(circlesHandle, out threadGroupSizeX, out _, out _);

        int total = (int)threadGroupSizeX * count;
        circleData = new Circle[total];

        float speed = 100;
        float halfSpeed = speed * 0.5f;
        float minRadius = 10.0f;
        float maxRadius = 30.0f;
        float radiusRange = maxRadius - minRadius;

        for(int i=0; i<total; i++)
        {
            Circle circle = circleData[i];
            circle.origin.x = Random.value * texResolution;
            circle.origin.y = Random.value * texResolution;
            circle.velocity.x = (Random.value * speed) - halfSpeed;
            circle.velocity.y = (Random.value * speed) - halfSpeed;
            circle.radius = Random.value * radiusRange + minRadius;
            circleData[i] = circle;
        }

    }
    struct Circle
    {
        public Vector2 origin;
        public Vector2 velocity;
        public float radius;
    }
    

}


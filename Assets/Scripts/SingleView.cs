using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SingleView : MonoBehaviour
{
    private void Awake()
    {
        cam = GetComponentInChildren<Camera>(true);
        cam.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Camera cam;
    public Shader viewshader;
    public Color solidColor = Color.white;
    public Color wireColor = Color.red;

    private void OnEnable()
    {
        cam = GetComponentInChildren<Camera>(true);
        cam.enabled = false;
    }

    private void OnDisable()
    {
        cam = GetComponentInChildren<Camera>(true);
        cam.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RenderSingleView()
    {
        if (viewshader != null && viewshader.isSupported)
        {
            Shader.SetGlobalColor("_SolidColor", solidColor);
            Shader.SetGlobalColor("_FrameColor", wireColor);
            cam.RenderWithShader(viewshader, "");
        }
        else
        {
            cam.Render();
        }
    }
}

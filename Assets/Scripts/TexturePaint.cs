using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TexturePaint : MonoBehaviour {

    // ======================================================================================================================
    // PARAMETERS -----------------------------------------------------------------------------------------------
    public Texture       baseTexture;                  // used to deterimne the dimensions of the runtime texture
    public Material      meshMaterial;                 // used to bind the runtime texture as the albedo of the mesh
    public GameObject    meshGameobject;
    public Shader        UVShader;                     // the shader usedto draw in the texture of the mesh
    public RenderTexture runTimeTexture;               // the actual shader the stuff are going to be drawn to
    public Mesh          meshToDraw;

    public static Vector3 mouseWorldPosition;

    // --------------------------------
    private Material        m;                          // material which the UV shader is binded to
    private CommandBuffer   cb;                         // commandbuffer used to run the UV shader on camera event
    private Camera          mainC;
    private int             clearTexture;
    private RenderTexture   paintedTexture;             // The runtime texture is always blited in to this for in take for next render loop
    // ======================================================================================================================
    // INITIALIZE -------------------------------------------------------------------

    void Start () {

        // Main cam initialization ---------------------------------------------------
                           mainC = Camera.main;
        if (mainC == null) mainC = this.GetComponent<Camera>();
        if (mainC == null) mainC = GameObject.FindObjectOfType<Camera>();


        // Texture and Mat initalization ---------------------------------------------
        runTimeTexture = new RenderTexture(baseTexture.width, baseTexture.height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false, 
            filterMode = FilterMode.Point
        };

        paintedTexture = new RenderTexture(baseTexture.width, baseTexture.height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false,
            filterMode = FilterMode.Point
        };

        m = new Material(UVShader);
        if (!m.SetPass(0)) Debug.LogError("Invalid Shader Pass: " + this.name);
        m.SetTexture("_MainTex", paintedTexture);


        meshMaterial.SetTexture("_MainTex", runTimeTexture);
        ClearTexture();

        // Command buffer inialzation ------------------------------------------------

        cb      =  new CommandBuffer();
        cb.name =  "TexturePainting";

        cb.SetRenderTarget(runTimeTexture);
        cb.DrawMesh(meshToDraw, Matrix4x4.identity, m);
        cb.Blit(runTimeTexture, paintedTexture);
        mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb);

  

    }
    // ======================================================================================================================
    // LOOP ---------------------------------------------------------------------------

    private void Update()
    {
        RaycastHit hit;
        Ray        ray = mainC.ScreenPointToRay(Input.mousePosition);
        Vector4    mwp = Vector3.positiveInfinity;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag != "PaintObject") return;
            mwp = hit.point;
        }

        mwp.w = Input.GetMouseButton(0)? 1 : 0;

        mouseWorldPosition = mwp;
        Shader.SetGlobalVector("_Mouse", mwp);
        m.SetMatrix("mesh_Object2World", meshGameobject.transform.localToWorldMatrix);


       
    }

    // ======================================================================================================================
    // HELPER FUNCTIONS ---------------------------------------------------------------------------

    void ClearTexture()
    {
        Graphics.SetRenderTarget(runTimeTexture);
        GL.Clear(false, true, Color.white);
        Graphics.SetRenderTarget(paintedTexture);
        GL.Clear(false, true, Color.white);
    }
    
}


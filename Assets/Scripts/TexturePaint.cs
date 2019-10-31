using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TexturePaint : MonoBehaviour {

    // ======================================================================================================================
    // PARAMETERS -----------------------------------------------------------------------------------------------
    public  Texture          baseTexture;                  // used to deterimne the dimensions of the runtime texture
    public  Material         meshMaterial;                 // used to bind the runtime texture as the albedo of the mesh
    public  GameObject       meshGameobject;
    public  Shader           UVShader;                     // the shader usedto draw in the texture of the mesh
    public  Mesh             meshToDraw;
    public  Shader           ilsandMarkerShader;
    public  Shader           fixIlsandEdgesShader;
    public  Shader           combineMetalicSmoothnes;                         
    public  static Vector3   mouseWorldPosition;

    // --------------------------------
  
    private Camera           mainC;
    private int              clearTexture;
    private RenderTexture    markedIlsandes;
    private CommandBuffer    cb_markingIlsdands;
    private int              numberOfFrames;
    private Material         fixEdgesMaterial;
    private Material         createMetalicGlossMap;
    private RenderTexture    metalicGlossMapCombined;

    // ---------------------------------
    private PaintableTexture albedo;
    private PaintableTexture metalic;
    public  PaintableTexture smoothness;
    // ======================================================================================================================
    // INITIALIZE -------------------------------------------------------------------

    void Start () {

        // Main cam initialization ---------------------------------------------------
                           mainC = Camera.main;
        if (mainC == null) mainC = this.GetComponent<Camera>();
        if (mainC == null) mainC = GameObject.FindObjectOfType<Camera>();


        // Texture and Mat initalization ---------------------------------------------
        markedIlsandes = new RenderTexture(baseTexture.width, baseTexture.height, 0, RenderTextureFormat.R8);
        albedo         = new PaintableTexture(Color.white, baseTexture.width, baseTexture.height, "_MainTex"
            ,UVShader, meshToDraw, fixIlsandEdgesShader,markedIlsandes);
        metalic        = new PaintableTexture(Color.white, baseTexture.width, baseTexture.height, "_MetallicGlossMap"
              , UVShader, meshToDraw, fixIlsandEdgesShader, markedIlsandes);

        smoothness     = new PaintableTexture(Color.black, baseTexture.width, baseTexture.height, "_GlossMap"
              , UVShader, meshToDraw, fixIlsandEdgesShader, markedIlsandes);

        metalicGlossMapCombined = new RenderTexture(metalic.runTimeTexture.descriptor)
        {
            format = RenderTextureFormat.ARGB32,
        };



        meshMaterial.SetTexture(albedo.id, albedo.runTimeTexture);
        meshMaterial.SetTexture(metalic.id, metalicGlossMapCombined);
        
        meshMaterial.EnableKeyword("_METALLICGLOSSMAP");


        createMetalicGlossMap = new Material(combineMetalicSmoothnes);


        // Command buffer inialzation ------------------------------------------------

        cb_markingIlsdands      = new CommandBuffer();
        cb_markingIlsdands.name = "markingIlsnads";

      
        cb_markingIlsdands.SetRenderTarget(markedIlsandes);
        Material mIlsandMarker  = new Material(ilsandMarkerShader);
        cb_markingIlsdands.DrawMesh(meshToDraw, Matrix4x4.identity, mIlsandMarker);
        mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);



        albedo.SetActiveTexture(mainC);
    }
    // ======================================================================================================================
    // LOOP ---------------------------------------------------------------------------

    private void Update()
    {
        if (numberOfFrames > 2) mainC.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);

        createMetalicGlossMap.SetTexture("_Smoothness", smoothness.runTimeTexture);
        createMetalicGlossMap.SetTexture("_MainTex", metalic.runTimeTexture);
        Graphics.Blit(metalic.runTimeTexture, metalicGlossMapCombined, createMetalicGlossMap);









        numberOfFrames++;

        // ----------------------------------------------------------------------------
        // This MUST be called to set up the painting with the mouse. 
        albedo    .UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);
        metalic   .UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);
        smoothness.UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);


        // ---------------------------------------------------------------------------
        // Setting up Mouse Parameters

        RaycastHit hit;
        Ray        ray = mainC.ScreenPointToRay(Input.mousePosition);
        Vector4    mwp = Vector3.positiveInfinity;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "PaintObject") 
            mwp = hit.point;
        }

        mwp.w = Input.GetMouseButton(0)? 1 : 0;

        mouseWorldPosition = mwp;
        Shader.SetGlobalVector("_Mouse", mwp);


        
    }

    // ======================================================================================================================
    // HELPER FUNCTIONS ---------------------------------------------------------------------------
    public void SetAlbedoActive()
    {
        metalic   .SetInactiveTexture(mainC);
        smoothness.SetInactiveTexture(mainC);
        albedo    .SetActiveTexture(mainC);
    }

    public void SetMetalicActive()
    {
        albedo    .SetInactiveTexture(mainC);
        smoothness.SetInactiveTexture(mainC);
        metalic   .SetActiveTexture(mainC);
    }

    public void SetGlossActive()
    {
        metalic   .SetInactiveTexture(mainC);
        albedo    .SetInactiveTexture(mainC);
        smoothness.SetActiveTexture(mainC);
    }
    
}


[System.Serializable]
public class PaintableTexture
{
    public  string        id;
    public  RenderTexture runTimeTexture;
    public  RenderTexture paintedTexture;

    public  CommandBuffer cb;

    private Material      mPaintInUV;
    private Material      mFixedEdges;
    private RenderTexture fixedIlsands;

    public PaintableTexture(Color clearColor, int width, int height, string id, 
        Shader sPaintInUV, Mesh mToDraw, Shader fixIlsandEdgesShader, RenderTexture markedIlsandes)
    {
        this.id        = id;

        runTimeTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false,
            filterMode = FilterMode.Bilinear
        };

        paintedTexture = new RenderTexture(width, height, 0)
        {
            anisoLevel = 0,
            useMipMap  = false,
            filterMode = FilterMode.Bilinear
        };


        fixedIlsands   = new RenderTexture(paintedTexture.descriptor);

        Graphics.SetRenderTarget(runTimeTexture);
        GL.Clear(false, true, clearColor);
        Graphics.SetRenderTarget(paintedTexture);
        GL.Clear(false, true, clearColor);


        mPaintInUV  = new Material(sPaintInUV);
        if (!mPaintInUV.SetPass(0)) Debug.LogError("Invalid Shader Pass: " );
        mPaintInUV.SetTexture("_MainTex", paintedTexture);

        mFixedEdges = new Material(fixIlsandEdgesShader);
        mFixedEdges.SetTexture("_IlsandMap", markedIlsandes);
        mFixedEdges.SetTexture("_MainTex", paintedTexture);

        // ----------------------------------------------

        cb      = new CommandBuffer();
        cb.name = "TexturePainting"+ id;


        cb.SetRenderTarget(runTimeTexture);
        cb.DrawMesh(mToDraw, Matrix4x4.identity, mPaintInUV);

        cb.Blit(runTimeTexture, fixedIlsands, mFixedEdges);
        cb.Blit(fixedIlsands, runTimeTexture);
        cb.Blit(runTimeTexture, paintedTexture);
    
    }

    public void SetActiveTexture(Camera mainC)
    {
        mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb);
    }
    
    public void SetInactiveTexture(Camera mainC)
    {
        mainC.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb);
    }

    public void UpdateShaderParameters(Matrix4x4 localToWorld)
    {
        mPaintInUV.SetMatrix("mesh_Object2World", localToWorld); // Mus be updated every time the mesh moves, and also at start
    }
}


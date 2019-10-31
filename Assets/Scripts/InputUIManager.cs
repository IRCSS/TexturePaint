using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputUIManager : MonoBehaviour, IPointerClickHandler, IDragHandler
{

    public  Shader        ColorPickerShader;
    public  Shader        brushShader;
    public  TexturePaint  texturePaintHardRef;                      

    private RawImage      valueSaturationPicker;
    private RawImage      huePicker;
    private RawImage      foreground;
    private RawImage      backGround;
                          
    private Button        switchButton;
    private Button        albedoButton;
    private Button        metalicButton;
    private Button        smotthnessButton;
                          
    private Slider        opacitySlider;
    private Slider        sizeSlider;
    private Slider        hardnessSlider;


    private GameObject[]  allUIElements;
    private RenderTexture valueSaturationImage;
    private RenderTexture huePickerImage;
    private Material      mColorPicker;

    private GameObject    mouseRepresentation;
    private GameObject    mouseHardnessRepresentation;
    private Material      mouseMaterial;
    private Material      mouseSoftnessMaterial;

    private Vector3       ColorPickerCurrentHSV = new Vector3(1f, 1f, 0.5f);
    private float         brushSize;
    private float         brushHardness;


    void OnDisable()
    {

        switchButton    .onClick.RemoveAllListeners();
        metalicButton   .onClick.RemoveAllListeners();
        smotthnessButton.onClick.RemoveAllListeners();
        albedoButton    .onClick.RemoveAllListeners();
    }

    void Start ()
    {
        allUIElements         = new GameObject[this.transform.childCount];
         for(int i = 0; i< allUIElements.Length; i++)
         {
             allUIElements[i] = this.transform.GetChild(i).gameObject;
         }
        
        valueSaturationPicker         = InitializeUIElement("SaturationValuePicker").GetComponent<RawImage>();
        valueSaturationImage          = new RenderTexture(1000, 1000, 0);
        valueSaturationPicker.texture = valueSaturationImage;
        
        huePicker             = InitializeUIElement("HuePicker").GetComponent<RawImage>();
        huePickerImage        = new RenderTexture(1000,200, 0);
        huePicker.texture     = huePickerImage;
        
        mColorPicker          = new Material(ColorPickerShader);
                             
        foreground            = InitializeUIElement("ForegroundColor").GetComponent<RawImage>();
        foreground.texture    = Texture2D.whiteTexture;
                             
        backGround            = InitializeUIElement("BackgroundColor").GetComponent<RawImage>();
        backGround.texture    = Texture2D.whiteTexture;
                              
        switchButton          = InitializeUIElement("SwitchBackForeGround").GetComponent<Button>();
        switchButton.onClick.AddListener(() => buttonCallBack(switchButton));

        albedoButton          = InitializeUIElement("Albedo").GetComponent<Button>();
        albedoButton.onClick.AddListener(() => buttonCallBack(albedoButton));

        metalicButton         = InitializeUIElement("Metalic").GetComponent<Button>();
        metalicButton.onClick.AddListener(() => buttonCallBack(metalicButton));

        smotthnessButton      = InitializeUIElement("Smoothness").GetComponent<Button>();
        smotthnessButton.onClick.AddListener(() => buttonCallBack(smotthnessButton));


        opacitySlider         = InitializeUIElement("Opacity").GetComponent<Slider>();
        sizeSlider            = InitializeUIElement("Size").GetComponent<Slider>();
        hardnessSlider        = InitializeUIElement("Hardness").GetComponent<Slider>();
        
        
        mouseRepresentation   = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mouseRepresentation.GetComponent<SphereCollider>().enabled = false;
        mouseMaterial         = new Material(brushShader);
        
        mouseRepresentation.GetComponent<Renderer>().material = mouseMaterial;
        
        mouseSoftnessMaterial       = new Material(brushShader);
        mouseHardnessRepresentation = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mouseHardnessRepresentation.GetComponent<SphereCollider>().enabled = false;
        mouseHardnessRepresentation.GetComponent<Renderer>().material = mouseSoftnessMaterial;
         
    }
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        mColorPicker.SetFloat("_Hue", ColorPickerCurrentHSV.x);
        Graphics.Blit(Texture2D.whiteTexture, valueSaturationImage, mColorPicker, 0);

        mColorPicker.SetFloat("_Saturation", 1.0f);
        mColorPicker.SetFloat("_Value", 1.0f);
        Graphics.Blit(Texture2D.whiteTexture, huePickerImage, mColorPicker, 1);

        Shader.SetGlobalColor("_BrushColor", foreground.color);
        
        Shader.SetGlobalFloat("_BrushOpacity", opacitySlider.value);
        brushSize = sizeSlider.value * .6f;
        Shader.SetGlobalFloat("_BrushSize", brushSize);
        brushHardness = hardnessSlider.value;
        Shader.SetGlobalFloat("_BrushHardness", brushHardness);

        if (TexturePaint.mouseWorldPosition.x == Mathf.Infinity) mouseRepresentation.transform.position = new Vector3(1000f, 1000f, 1000f);
        else mouseRepresentation.transform.position = TexturePaint.mouseWorldPosition;
        mouseRepresentation.transform.localScale = new Vector3(brushSize*2.0f, brushSize * 2.0f, brushSize * 2.0f);

        mouseHardnessRepresentation.transform.position = mouseRepresentation.transform.position;
        mouseHardnessRepresentation.transform.localScale = mouseRepresentation.transform.localScale * brushHardness;


        mouseMaterial.SetColor("_Color", foreground.color);
        mouseSoftnessMaterial.SetColor("_Color", new Color(1f-foreground.color.r, 1f - foreground.color.g, 1f - foreground.color.b));
    }

    private GameObject InitializeUIElement(string name)
    {
        for(int i = 0; i< allUIElements.Length; i++)
        {
            if (allUIElements[i].name == name) return allUIElements[i];
        }
        return null;
    }

    private void SetSaturationValueColorPicker(PointerEventData eventData)
    {
        Vector2 o;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(valueSaturationPicker.rectTransform, eventData.position, Camera.main, out o);
        o = new Vector2(o.x / valueSaturationPicker.rectTransform.rect.width + 0.5f, o.y / valueSaturationPicker.rectTransform.rect.height + 0.5f);
        ColorPickerCurrentHSV = new Vector3(ColorPickerCurrentHSV.x, o.x, o.y);
        foreground.color = Color.HSVToRGB(ColorPickerCurrentHSV.x, ColorPickerCurrentHSV.y, ColorPickerCurrentHSV.z);
    }

    private void SetHueColorPicker(PointerEventData eventData)
    {
        Vector2 o;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(valueSaturationPicker.rectTransform, eventData.position, Camera.main, out o);
        o = new Vector2(o.x / valueSaturationPicker.rectTransform.rect.width + 0.5f, o.y / valueSaturationPicker.rectTransform.rect.height + 0.5f);
        ColorPickerCurrentHSV = new Vector3(o.x, ColorPickerCurrentHSV.y, ColorPickerCurrentHSV.z);
        foreground.color = Color.HSVToRGB(ColorPickerCurrentHSV.x, ColorPickerCurrentHSV.y, ColorPickerCurrentHSV.z);
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        string objectName = eventData.pointerCurrentRaycast.gameObject.name;

        switch (objectName)
        {
            case "SaturationValuePicker":
                SetSaturationValueColorPicker(eventData);

                break;

            case "HuePicker":
                SetHueColorPicker(eventData);
                break;
        }
        
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null) return;
        string objectName = eventData.pointerCurrentRaycast.gameObject.name;

        switch (objectName)
        {
            case "SaturationValuePicker":

                SetSaturationValueColorPicker(eventData);

                break;

            case "HuePicker":
                SetHueColorPicker(eventData);
                break;
        }
    }


    private void buttonCallBack(Button buttonPressed)
    {
        string nameOfButton = buttonPressed.gameObject.name;
        switch (nameOfButton)
        {
            case "SwitchBackForeGround":
                Color c          = foreground.color;
                foreground.color = backGround.color;
                backGround.color = c;
                break;
            case "Albedo":
                texturePaintHardRef.SetAlbedoActive();
                break;

            case "Metalic":
                texturePaintHardRef.SetMetalicActive();
                break;
            case "Smoothness":
                texturePaintHardRef.SetGlossActive();
                break;

        }
    }

    private void sliderCallBack(float value, Slider s)
    {
        string nameOfSlider = s.gameObject.name;
        switch (nameOfSlider)
        {
            case "Opacity":

                break;
            case "Size":

                break;
            case "Hardness":

                break;
        }
    }



}

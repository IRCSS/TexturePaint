using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputUIManager : MonoBehaviour, IPointerClickHandler, IDragHandler
{

    public Shader ColorPickerShader;

    private RawImage valueSaturationPicker;
    private RawImage huePicker;
    private RawImage foreground;
    private RawImage backGround;

    private Button switchButton;

    private GameObject[] allUIElements;
    private RenderTexture valueSaturationImage;
    private RenderTexture huePickerImage;
    private Material mColorPicker;

    private Vector3 ColorPickerCurrentHSV = new Vector3(1f, 1f, 0.5f);



    void OnDisable()
    {
        switchButton.onClick.RemoveAllListeners();
    }

    void Start ()
    {
         allUIElements = new GameObject[this.transform.childCount];
         for(int i = 0; i< allUIElements.Length; i++)
         {
             allUIElements[i] = this.transform.GetChild(i).gameObject;
         }
         
         valueSaturationPicker= InitializeUIElement("SaturationValuePicker").GetComponent<RawImage>();
         valueSaturationImage = new RenderTexture(1000, 1000, 0);
         valueSaturationPicker.texture = valueSaturationImage;
         
         huePicker = InitializeUIElement("HuePicker").GetComponent<RawImage>();
         huePickerImage = new RenderTexture(1000,200, 0);
         huePicker.texture = huePickerImage;
         
         mColorPicker = new Material(ColorPickerShader);
         
         foreground = InitializeUIElement("ForegroundColor").GetComponent<RawImage>();
         foreground.texture = Texture2D.whiteTexture;

        backGround = InitializeUIElement("BackgroundColor").GetComponent<RawImage>();
        backGround.texture = Texture2D.whiteTexture;

        switchButton = InitializeUIElement("SwitchBackForeGround").GetComponent<Button>();
        switchButton.onClick.AddListener(() => buttonCallBack(switchButton));

    }
	
	// Update is called once per frame
	void Update () {


        mColorPicker.SetFloat("_Hue", ColorPickerCurrentHSV.x);
        Graphics.Blit(Texture2D.whiteTexture, valueSaturationImage, mColorPicker, 0);

        mColorPicker.SetFloat("_Saturation", 1.0f);
        mColorPicker.SetFloat("_Value", 1.0f);
        Graphics.Blit(Texture2D.whiteTexture, huePickerImage, mColorPicker, 1);

        Shader.SetGlobalColor("_BrushColor", foreground.color);

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
                Color c = foreground.color;
                foreground.color = backGround.color;
                backGround.color = c;
                break;
        }
    }



}

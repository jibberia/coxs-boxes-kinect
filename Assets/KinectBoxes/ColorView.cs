using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ColorView : MonoBehaviour
{
    public GameObject ColorSourceManager;
    private ColorManager _ColorManager;
    
    void Start ()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    
    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }
        
        _ColorManager = ColorSourceManager.GetComponent<ColorManager>();
        if (_ColorManager == null)
        {
            return;
        }
        
        gameObject.GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();

        float ratio = (float)_ColorManager.ColorWidth / _ColorManager.ColorHeight;
        Vector3 scale = this.transform.localScale;
        this.transform.localScale = new Vector3(scale.y * ratio, scale.y, scale.z);
    }
}
/*
camera y 1.22
colorview y 1.27, scale y 2

 */
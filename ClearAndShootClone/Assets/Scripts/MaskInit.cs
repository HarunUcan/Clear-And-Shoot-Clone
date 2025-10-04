using UnityEngine;

public class MaskInit : MonoBehaviour
{
    public RenderTexture maskRT;


    void Awake()
    {

        // Baþlangýçta maskeyi tamamen beyaz yap
        RenderTexture.active = maskRT;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = null;
    }


    void Start()
    {
        maskRT = new RenderTexture(Globals.DefaultMaskResolution, Globals.DefaultMaskResolution, 0, RenderTextureFormat.ARGB32);
        maskRT.Create();
    }
}


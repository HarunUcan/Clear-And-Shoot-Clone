using UnityEngine;
using UnityEngine.Rendering;

public class MaskTextureSpawner : MonoBehaviour
{
    public RenderTexture baseMask;

    [HideInInspector] public RenderTexture instanceMask;
    public float CleanPercent { get; private set; } = 0f;

    private Renderer _rend;
    private float timer = 0f;

    void Start()
    {
        _rend = GetComponent<Renderer>();

        if (baseMask != null)
        {
            instanceMask = new RenderTexture(baseMask.width, baseMask.height, 0, baseMask.format);
            instanceMask.Create();
            Graphics.Blit(baseMask, instanceMask);
        }
        else
        {
            instanceMask = new RenderTexture(Globals.DefaultMaskResolution, Globals.DefaultMaskResolution, 0, RenderTextureFormat.ARGB32);
            instanceMask.Create();

            var prev = RenderTexture.active;
            RenderTexture.active = instanceMask;
            GL.Clear(true, true, Color.white); // baþlangýç: kirli
            RenderTexture.active = prev;
        }

        var mpb = new MaterialPropertyBlock();
        _rend.GetPropertyBlock(mpb);
        mpb.SetTexture("_MaskTex", instanceMask);
        _rend.SetPropertyBlock(mpb);
    }

    void Update()
    {
        // her 0.2 sn'de bir GPU’dan oku
        timer += Time.deltaTime;
        if (timer >= 0.2f)
        {
            timer = 0f;
            if (instanceMask != null)
            {
                AsyncGPUReadback.Request(instanceMask, 0, request =>
                {
                    if (request.hasError) return;

                    var data = request.GetData<byte>();
                    int cleanCount = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        // kýrmýzý kanal kullanýlýyor (brush mask genelde R'de)
                        if (data[i] < 128) cleanCount++;
                    }

                    CleanPercent = (cleanCount / (float)data.Length) * 100f;
                    //Debug.Log($"{gameObject.name} temizlenme oraný: %{CleanPercent:F2}");
                });
            }
        }
    }

    void OnDestroy()
    {
        if (instanceMask != null)
        {
            instanceMask.Release();
            instanceMask = null;
        }
    }
}

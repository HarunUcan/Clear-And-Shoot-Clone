using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GradientImageGenerator : MonoBehaviour
{
    [Tooltip("Eklenecek UI Image (býrakýrsan bu GameObject'teki Image kullanýlýr)")]
    public Image targetImage;

    [Tooltip("Gradyan (Inspector'dan renkleri ayarla)")]
    public Gradient gradient = DefaultGradient();

    [Tooltip("Geniþlik/heigth (UI için yüksek çözünürlük gerekmeyebilir)")]
    public int width = 32;
    public int height = 256;

    [Tooltip("true = dikey gradyan, false = yatay")]
    public bool vertical = true;

    [Range(0f, 1f)]
    public float alphaMultiplier = 1f;

    void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    void Start()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        ApplyGradient();
    }

    public void ApplyGradient()
    {
        int w = Mathf.Max(1, width);
        int h = Mathf.Max(1, height);
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float t = vertical ? (float)y / (h - 1) : (float)x / (w - 1);
                Color c = gradient.Evaluate(t);
                c.a *= alphaMultiplier;
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();

        // Sprite oluþtur ve ata
        Sprite spr = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
        targetImage.sprite = spr;
        targetImage.type = Image.Type.Simple;
        targetImage.preserveAspect = false;
        // Eðer resmi ayrýca çarpmak istersen:
        targetImage.color = Color.white;
    }

    // Basit default gradient (siyah -> beyaz)
    static Gradient DefaultGradient()
    {
        Gradient g = new Gradient();
        g.colorKeys = new GradientColorKey[] {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(Color.black, 1f)
        };
        g.alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        };
        return g;
    }
}

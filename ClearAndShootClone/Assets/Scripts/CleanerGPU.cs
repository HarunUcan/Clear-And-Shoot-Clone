using UnityEngine;

public class CleanerGPU : MonoBehaviour
{
    public Material brushMat; // içinde brush texture var
    public RenderTexture maskRT;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 uv = GetUVFromMouse();

            brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0, 0));
            brushMat.SetFloat("_BrushSize", 0.05f);

            RenderTexture temp = RenderTexture.GetTemporary(maskRT.width, maskRT.height);
            Graphics.Blit(maskRT, temp);
            Graphics.Blit(temp, maskRT, brushMat);
            RenderTexture.ReleaseTemporary(temp);
        }
    }

    Vector2 GetUVFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);  
            Debug.Log("Hit UV: " + hit.textureCoord);
            return hit.textureCoord;
        }
        return Vector2.zero;
    }
}


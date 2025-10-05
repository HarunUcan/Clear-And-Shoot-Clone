using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class WaterGunController : MonoBehaviour
{
    [SerializeField] private Transform _raysStartPos;
    [SerializeField] private Transform _raysFinishPosLeft;
    [SerializeField] private Transform _raysFinishPosRight;
    [SerializeField] private int _rayCount = 10;
    [SerializeField] private int _rayLength = 5;
    [SerializeField] private float _brushSize = 0.05f; // UV-space 0..1

    public Material brushMat; // Brush shader material (MyBrush)
    // public RenderTexture maskRT; // art�k global mask kullanm�yoruz

    // Ge�ici; her renderer i�in tespit edilen UV listelerini tutar
    private Dictionary<Renderer, List<Vector2>> hitsPerRenderer = new Dictionary<Renderer, List<Vector2>>();

    // Toplanan silahlar�n aksiyon ba�layana kadar duracaklar� yerler
    [SerializeField] private List<Transform> _collectedWeaponPositions = new List<Transform>();
    public static int CurrentCollectedWeaponIndex = 0;

    void Update()
    {
        hitsPerRenderer.Clear();
        CastRays();

        // Her renderer i�in kendi mask'ine boya uygula
        foreach (var kvp in hitsPerRenderer)
        {
            Renderer rend = kvp.Key;
            List<Vector2> uvs = kvp.Value;

            if (rend == null || uvs.Count == 0) continue;

            // PaintableObject bile�eninden instanceMask al
            var paintable = rend.GetComponent<MaskTextureSpawner>();
            if (paintable == null || paintable.instanceMask == null) continue;

            RenderTexture objRT = paintable.instanceMask;

            // Kopya alma ve art arda �izme
            RenderTexture temp = RenderTexture.GetTemporary(objRT.descriptor);
            Graphics.Blit(objRT, temp);

            foreach (var uv in uvs)
            {
                brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0f, 0f));
                brushMat.SetFloat("_BrushSize", _brushSize);

                // temp -> objRT (brush shader kullan�larak)
                Graphics.Blit(temp, objRT, brushMat);

                // objRT �imdi g�ncellendi; temp'i objRT ile e�itleyelim ki bir sonraki uv de g�ncel mask �zerinden i�lesin
                Graphics.Blit(objRT, temp);
            }

            RenderTexture.ReleaseTemporary(temp);
        }
    }

    

    private void CastRays()
    {
        Vector3 startPos = _raysStartPos.position;
        Vector3 endPosLeft = _raysFinishPosLeft.position;
        Vector3 endPosRight = _raysFinishPosRight.position;
        Vector3 directionLeft = (endPosLeft - startPos).normalized;
        Vector3 directionRight = (endPosRight - startPos).normalized;

        for (int i = 0; i < _rayCount; i++)
        {
            float t = (_rayCount == 1) ? 0.5f : (float)i / (_rayCount - 1); // g�venli b�lme
            Vector3 direction = Vector3.Slerp(directionLeft, directionRight, t).normalized;

            Ray ray = new Ray(startPos, direction);
            Debug.DrawRay(ray.origin, ray.direction * _rayLength, Color.cyan);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, _rayLength))
            {
                Renderer hitRend = hitInfo.collider.GetComponent<Renderer>();
                if (hitRend == null) continue;

                // Ba�lang�� ve biti� noktas� boyand� m� kontrol�
                if (hitInfo.collider.tag == "StartPointOfCollectable" || hitInfo.collider.tag == "EndPointOfCollectable")
                {
                    var collectableObject = hitInfo.collider.GetComponentInParent<ICollectable>();
                    Debug.Log("Collectable Check Point Touched");
                    if (collectableObject != null)
                    {
                        if(hitInfo.collider.tag == "EndPointOfCollectable" && !collectableObject.IsEndPointPainted)
                        {
                            Debug.Log("End Point Touched");

                            collectableObject.IsEndPointPainted = true;
                            hitInfo.collider.enabled = false; // Collider'� devre d��� b�rak

                            if(collectableObject.IsStartPointPainted && collectableObject.IsCleaned)
                            {
                                collectableObject.Collect(_collectedWeaponPositions[CurrentCollectedWeaponIndex]);
                            }
                        }
                        else if(hitInfo.collider.tag == "StartPointOfCollectable" && !collectableObject.IsStartPointPainted)
                        {
                            collectableObject.IsStartPointPainted = true;
                            hitInfo.collider.enabled = false; // Collider'� devre d��� b�rak
                            Debug.Log("Start Point Touched");
                        }
                    }
                }

                if (hitInfo.collider.tag != "Floor" && hitInfo.collider.GetComponent<MaskTextureSpawner>() != null)
                {
                    var maskSpawner = hitInfo.collider.GetComponent<MaskTextureSpawner>();
                    var collectable = hitInfo.collider.GetComponentInParent<ICollectable>();
                    var cleanPercentTreshold = collectable.CleanPercentThreshold;
                    if(maskSpawner.CleanPercent >= cleanPercentTreshold && collectable.IsCleaned == false )
                    {
                        collectable.IsCleaned = true;
                    }
                }

                // UV al
                Vector2 uv = hitInfo.textureCoord;

                // Liste yoksa olu�tur
                if (!hitsPerRenderer.TryGetValue(hitRend, out var list))
                {
                    list = new List<Vector2>();
                    hitsPerRenderer[hitRend] = list;
                }

                list.Add(uv);
            }
        }
    }
}

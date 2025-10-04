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
    // public RenderTexture maskRT; // artýk global mask kullanmýyoruz

    // Geçici; her renderer için tespit edilen UV listelerini tutar
    private Dictionary<Renderer, List<Vector2>> hitsPerRenderer = new Dictionary<Renderer, List<Vector2>>();

    // Toplanan silahlarýn aksiyon baþlayana kadar duracaklarý yerler
    [SerializeField] private List<Transform> _collectedWeaponPositions = new List<Transform>();
    private int _currentCollectedWeaponIndex = 0;

    void Update()
    {
        hitsPerRenderer.Clear();
        CastRays();

        // Her renderer için kendi mask'ine boya uygula
        foreach (var kvp in hitsPerRenderer)
        {
            Renderer rend = kvp.Key;
            List<Vector2> uvs = kvp.Value;

            if (rend == null || uvs.Count == 0) continue;

            // PaintableObject bileþeninden instanceMask al
            var paintable = rend.GetComponent<MaskTextureSpawner>();
            if (paintable == null || paintable.instanceMask == null) continue;

            RenderTexture objRT = paintable.instanceMask;

            // Kopya alma ve art arda çizme
            RenderTexture temp = RenderTexture.GetTemporary(objRT.descriptor);
            Graphics.Blit(objRT, temp);

            foreach (var uv in uvs)
            {
                brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0f, 0f));
                brushMat.SetFloat("_BrushSize", _brushSize);

                // temp -> objRT (brush shader kullanýlarak)
                Graphics.Blit(temp, objRT, brushMat);

                // objRT þimdi güncellendi; temp'i objRT ile eþitleyelim ki bir sonraki uv de güncel mask üzerinden iþlesin
                Graphics.Blit(objRT, temp);
            }

            RenderTexture.ReleaseTemporary(temp);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.tag == "DangerZone")
        {
            Vector3 newCameraAngles = new Vector3(20f, 0f, 0f);
            Vector3 newCameraPosition = new Vector3(Camera.main.transform.position.x, 12f, Camera.main.transform.position.z);
            Camera.main.transform.DORotate(newCameraAngles, 1f);
            Camera.main.transform.DOMove(newCameraPosition, 1f);
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
            float t = (_rayCount == 1) ? 0.5f : (float)i / (_rayCount - 1); // güvenli bölme
            Vector3 direction = Vector3.Slerp(directionLeft, directionRight, t).normalized;

            Ray ray = new Ray(startPos, direction);
            Debug.DrawRay(ray.origin, ray.direction * _rayLength, Color.cyan);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, _rayLength))
            {
                Renderer hitRend = hitInfo.collider.GetComponent<Renderer>();
                if (hitRend == null) continue;

                // Baþlangýç ve bitiþ noktasý boyandý mý kontrolü
                if (hitInfo.collider.tag == "StartPointOfWeapon" || hitInfo.collider.tag == "EndPointOfWeapon")
                {
                    var weaponScript = hitInfo.collider.GetComponentInParent<Weapon>();
                    Debug.Log("Weapon Check Point Touched");
                    if (weaponScript != null)
                    {
                        if(hitInfo.collider.tag == "EndPointOfWeapon" && !weaponScript.IsEndPointPainted)
                        {
                            Debug.Log("End Point Touched");

                            weaponScript.WeaponCheckPointSetStatus(WeaponCheckPoint.EndPoint, true);
                            hitInfo.collider.enabled = false; // Collider'ý devre dýþý býrak

                            if(weaponScript.IsStartPointPainted && weaponScript.IsCollected)
                            {
                                /** TODO: TopMesh isimli kardeþ objenin collider'ýný devre dýþý býrak **/ 
                                hitInfo.collider.transform.parent.GetComponentInChildren<Collider>().enabled = false;
                                hitInfo.collider.transform.parent.parent = Camera.main.transform;

                                hitInfo.collider.transform.parent.DOMoveY(10f, 1.2f).SetEase(Ease.OutBounce).OnComplete(() =>
                                {
                                    Vector3 targetScale = hitInfo.collider.transform.parent.localScale * .7f;
                                    hitInfo.collider.transform.parent.DOScale(targetScale, 1f).SetEase(Ease.InOutSine);
                                    hitInfo.collider.transform.parent.DORotate(new Vector3(0f, 0f, 90f), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
                                    hitInfo.collider.transform.parent.DOMove(_collectedWeaponPositions[_currentCollectedWeaponIndex].position, .05f).SetEase(Ease.InOutSine).OnComplete(() =>
                                    {
                                        _currentCollectedWeaponIndex++;
                                    });
                                });
                            }
                        }
                        else if(hitInfo.collider.tag == "StartPointOfWeapon" && !weaponScript.IsStartPointPainted)
                        {
                            weaponScript.WeaponCheckPointSetStatus(WeaponCheckPoint.StartPoint, true);
                            hitInfo.collider.enabled = false; // Collider'ý devre dýþý býrak
                            Debug.Log("Start Point Touched");
                        }
                    }
                }

                if (hitInfo.collider.tag != "Floor" && hitInfo.collider.GetComponent<MaskTextureSpawner>() != null)
                {
                    var maskSpawner = hitInfo.collider.GetComponent<MaskTextureSpawner>();
                    var weaponScript = hitInfo.collider.GetComponentInParent<Weapon>();
                    var cleanPercentTreshold = weaponScript.CleanPercentThreshold;
                    if(maskSpawner.CleanPercent >= cleanPercentTreshold && weaponScript.IsCollected == false )
                    {
                        weaponScript.IsCollected = true;
                    }
                }

                // UV al
                Vector2 uv = hitInfo.textureCoord;

                // Liste yoksa oluþtur
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

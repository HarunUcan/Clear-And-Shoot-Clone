using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentGameState = GameState.Clear;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    [SerializeField] private GameObject _waterGun;

    public static List<GameObject> CollectedWeapons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        initialCameraPosition = Camera.main.transform.position;
        initialCameraRotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameState()
    {
        if (CurrentGameState == GameState.Clear)
        {
            CurrentGameState = GameState.Action;

            _waterGun.SetActive(false);

            Vector3 newCameraAngles = new Vector3(20f, 0f, 0f);
            Vector3 newCameraPosition = new Vector3(Camera.main.transform.position.x, 12f, Camera.main.transform.position.z);
            Camera.main.transform.DORotate(newCameraAngles, 1f);
            Camera.main.transform.DOMove(newCameraPosition, 1f);

            ChangeWeaponAnimState(true);
        }
        else
        {
            CurrentGameState = GameState.Clear;

            _waterGun.SetActive(true);

            Vector3 resetPosition = new Vector3(Camera.main.transform.position.x, initialCameraPosition.y, Camera.main.transform.position.z);

            Camera.main.transform.DORotateQuaternion(initialCameraRotation, 1f);
            Camera.main.transform.DOMove(resetPosition, 1f);

            ChangeWeaponAnimState(false);
        }
    }

    public void ChangeWeaponAnimState(bool isAction)
    {
        foreach (var weapon in CollectedWeapons)
        {
            if (weapon != null && weapon.TryGetComponent(out Animator weaponAnimator))
            {
                weaponAnimator.SetBool("IsAction", isAction);
            }
            weapon.GetComponent<Weapon>().ChangeState();
        }
    }
}

public enum GameState
{
    Clear,
    Action
}

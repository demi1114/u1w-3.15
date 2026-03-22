using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraMan : MonoBehaviour , InputSystem_Actions.ICameraManActions
{
    public bool CameraOn;

    public Camera cam;
    PlayerCamera plrCam;
    public string targetTag = "Subject";

    GameObject Player;

    public List<GameObject> targets = new List<GameObject>();

    InputSystem_Actions input;
    Vector2 mov;
    bool PressPicture;
    [SerializeField] Vector2 RangeLimit;
    Vector2 MultiplyRangeLimit;
    [SerializeField] float CamSpeed = 0.1f;

    [SerializeField] GameObject CameraManUI;
    [SerializeField] Slider slider;
    float ZoomValue;

    [SerializeField] GameObject AfterPictureUI;
    [SerializeField] Image AfterPicutreUIPicture;

    public bool CamEnd;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.CameraMan.SetCallbacks(this);
        cam = Camera.main;
        plrCam = cam.GetComponent<PlayerCamera>();
        Player = FindFirstObjectByType<PlayerMovement>().gameObject;
        AfterPictureUI.transform.localScale = Vector2.zero;
    }

    void Start()
    {
        //EnableCam(true); //テスト用
    }

    public void EnableCam(bool yes)
    {
        if(yes)
        {
            input.CameraMan.Enable();
            plrCam.CamMode = PCamMode.Fixed;
            plrCam.FixCam = true;
            CameraOn = true;
        }
        else
        {
            plrCam.CamMode = PCamMode.Player;
            input.CameraMan.Disable();
            plrCam.CamProjectionZoom = 1f;
            slider.value = 1f;
            CameraOn = false;
            StartCoroutine(CamExitCheckHelper());
        }
    }

    IEnumerator CamExitCheckHelper()
    {
        CamEnd = true;
        yield return null;
        CamEnd = false;
    }

    void Update()
    {
        if (CameraOn)
        {
            CameraManUI.transform.localScale = Vector2.one;
            
            plrCam.FixedCamPos = this.transform.position + new Vector3(0,0,-10);

            plrCam.CamProjectionZoom = slider.value;

            targets.Clear();

            this.transform.position += (Vector3)mov * CamSpeed * Time.deltaTime;

            GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);

            foreach (var obj in objs)//カメラ内のオブジェクトチェック
            {
                Vector3 viewPos = cam.WorldToViewportPoint(obj.transform.position);

                // カメラ前 & 画面内
                if (viewPos.z > 0 &&
                    viewPos.x >= 0 && viewPos.x <= 1 &&
                    viewPos.y >= 0 && viewPos.y <= 1)
                {
                    targets.Add(obj);
                }
            }

            MultiplyRangeLimit = RangeLimit * (slider.value* slider.value);
            //CamLimit
            if (this.transform.position.x > Player.transform.position.x + MultiplyRangeLimit.x * 0.5f)
            {
                this.transform.position = new Vector2(Player.transform.position.x + MultiplyRangeLimit.x * 0.5f, transform.position.y );
            }
            if (this.transform.position.x < Player.transform.position.x + MultiplyRangeLimit.x * -0.5f)
            {
                this.transform.position = new Vector2(Player.transform.position.x + MultiplyRangeLimit.x * -0.5f, transform.position.y);
            }
            if (this.transform.position.y > Player.transform.position.y + 0.3f + MultiplyRangeLimit.y * 0.5f)
            {
                this.transform.position = new Vector2(transform.position.x, Player.transform.position.y + 0.3f + MultiplyRangeLimit.y * 0.5f);
            }
            if (this.transform.position.y < Player.transform.position.y + 0.1f + MultiplyRangeLimit.y * -0.1f)
            {
                this.transform.position = new Vector2(transform.position.x, Player.transform.position.y + 0.1f + MultiplyRangeLimit.y * -0.1f);
            }

        }
        else
        {
            CameraManUI.transform.localScale = Vector2.zero;
            mov = Vector2.zero;
            this.transform.position = (Vector2)cam.transform.position;
        }
    }

    void LateUpdate()
    {
        PressPicture = false;
    }

    public void AddZoomValue(float value)
    {
        slider.value += value;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        mov = context.ReadValue<Vector2>();
    }
    public void OnPicture(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PressPicture = true;
            StartCoroutine(Pictured(CamScreenShot.CaptureAsSprite(cam, 1280, 720, true)));
        }
    }

    IEnumerator Pictured(Sprite pic)
    {
        input.CameraMan.Disable();
        AfterPicutreUIPicture.sprite = pic;
        AfterPictureUI.transform.localScale = Vector2.one;

        Album makeAlbum = new Album();
        makeAlbum.pict = pic;
        foreach (var obj in targets)
        {
            if (obj != null)
            {
                makeAlbum.targets.Add(obj.name);
            }
        }
        SaveDatas.instance.album.Add(makeAlbum);

        for (float t=0f; t< 1.0f; t += Time.unscaledDeltaTime)
        {
            yield return null;
        }

        input.CameraMan.Enable();
        AfterPictureUI.transform.localScale = Vector2.zero;
        yield break;
    }

    
}

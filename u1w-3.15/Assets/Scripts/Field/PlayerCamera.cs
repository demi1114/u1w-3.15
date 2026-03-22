using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 smoothVel;
    [SerializeField] float smoothAmount = 0.2f;

    [SerializeField] Vector2 PosShift;
    public Vector2 AdditionalShift;
    public bool FixCam;

    public PCamSpd CamSpd;

    //固定カメラモードとか
    public PCamMode CamMode;
    public Vector2 FixedCamPos;

    public float NoMoveRange = 1.0f;

    [SerializeField] private float DefaultCamProjection = 5.4f;
    public float CamProjectionZoom = 1.0f;

    private void Update()
    {
        //距離チェック
        if(Vector3.Distance(this.transform.position, target.position+(Vector3)PosShift + (Vector3)AdditionalShift + new Vector3(0, 0, -10)) > NoMoveRange)
        {
            FixCam = true;
        }
        switch(CamSpd)
        {
            case PCamSpd.Quick:

                transform.position = target.position + (Vector3)PosShift + (Vector3)AdditionalShift + new Vector3(0, 0, -10);
                break;
        }

        GetComponent<Camera>().orthographicSize = DefaultCamProjection / CamProjectionZoom;
    }

    void LateUpdate()
    {
        Vector3 targetpos = target.position + (Vector3)PosShift + (Vector3)AdditionalShift;
        if(CamMode == PCamMode.Fixed) targetpos = (Vector3)FixedCamPos;

        if (FixCam)
        {
            //滑らかに移動させる
            transform.position = Vector2.SmoothDamp(
                transform.position,
                targetpos,
                ref smoothVel,
                smoothAmount
            );
            transform.position += new Vector3(0, 0, -10);
            if (Vector3.Distance(this.transform.position, targetpos + new Vector3(0, 0, -10)) < 0.02f)
            {
                if(CamMode == PCamMode.Player)
                {
                    FixCam = false;
                }
            }
        }
    }
}

public enum PCamSpd
{
    Normal,
    Quick
}

public enum PCamMode
{
    Player,
    Fixed
}
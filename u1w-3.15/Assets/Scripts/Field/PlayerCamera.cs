using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 smoothVel;
    [SerializeField] float smoothAmount = 0.2f;

    public Vector2 PosShift;
    public bool FixCam;

    public float NoMoveRange = 1.0f;

    private void Update()
    {
        //距離チェック
        if(Vector3.Distance(this.transform.position, target.position+(Vector3)PosShift + new Vector3(0, 0, -10)) > NoMoveRange)
        {
            FixCam = true;
        }
    }

    void LateUpdate()
    {
        if(FixCam)
        {
            //滑らかに移動させる
            transform.position = Vector2.SmoothDamp(
                transform.position,
                target.position + (Vector3)PosShift,
                ref smoothVel,
                smoothAmount
            );
            transform.position += new Vector3(0, 0, -10);
            if (Vector3.Distance(this.transform.position, target.position + (Vector3)PosShift + new Vector3(0, 0, -10)) < 0.02f)
            {
                FixCam = false;
            }
        }
    }
}

using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 smoothVel;
    [SerializeField] float smoothAmount = 0.2f;

    void LateUpdate()
    {
        //ŠŠ‚ç‚©‚ÉˆÚ“®‚³‚¹‚é
        transform.position = Vector2.SmoothDamp(
            transform.position,
            target.position,
            ref smoothVel,
            smoothAmount
        );
        transform.position += new Vector3(0,0,-10);
    }
}

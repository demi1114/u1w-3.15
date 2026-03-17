using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    //INPUT
    InputSystem_Actions input;

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpPower = 10f;

    [SerializeField] Transform feet, left, right;

    [SerializeField] LayerMask obstacle;
    [SerializeField] float groundCheckDistance;

    Vector2 mov;

    bool JumpTask;

    Rigidbody2D rb;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    private void Update()
    {
        // ‰¡ˆÚ“®
        rb.linearVelocity = new Vector2(mov.x * speed, rb.linearVelocity.y);
        if (mov.x != 0) Debug.Log("Pressing");

        if(JumpTask&&IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            JumpTask = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        mov = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (IsGrounded()) JumpTask = true;
        }
    }

    bool IsGrounded()
    {
        if(Physics2D.Raycast(feet.position + new Vector3(left.localPosition.x,0), Vector2.down, groundCheckDistance, obstacle))
        {
            return true;
        }
        else if (Physics2D.Raycast(feet.position + new Vector3(right.localPosition.x, 0), Vector2.down, groundCheckDistance, obstacle))
        {
            return true;
        }

        return false;
    }
}

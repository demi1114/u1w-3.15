using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    //INPUT
    InputSystem_Actions input;

    [SerializeField] float speed = 5f;//移動力
    [SerializeField] float jumpPower = 10f;//ジャンプ力

    [SerializeField] Transform feet, left, right;

    [SerializeField] LayerMask obstacle;
    [SerializeField] float groundCheckDistance;

    Vector2 mov;

    bool JumpTask;
    bool Check;

    List<Eventer> targetEventers = new List<Eventer>();

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

    public void DisablePlayerInput(bool yes)
    {
        if (yes)
        {
            input.Player.Disable();
            mov = Vector2.zero;
        }
        else
        {
            input.Player.Enable();
        }
    }

    private void Update()
    {
        // 横移動
        rb.linearVelocity = new Vector2(mov.x * speed, rb.linearVelocity.y);

        if(JumpTask&&IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            JumpTask = false;
        }

        if (!input.Player.enabled) return;

        if (targetEventers != null && targetEventers.Count > 0)
        {
            for (int i = targetEventers.Count - 1; i >= 0; i--)
            {
                var targetEventer = targetEventers[i];

                if (targetEventer == null)
                {
                    targetEventers.RemoveAt(i);
                    continue;
                }

                if (targetEventer.ForceRunOnTouch)
                {
                    if (targetEventer.Run())
                    {
                        targetEventers.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    if (Check)
                    {
                        if (targetEventer.Run())
                        {
                            break; // 成功したら他はやらない
                        }
                    }
                }
            }
        }
        
    }
    private void LateUpdate()
    {
        Check = false;
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
    public void OnCheck(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Check = true;
        }
    }

    bool IsGrounded()//右端、左端 EmptyからXをとり、Feetから底辺を確認し、Raycastを飛ばして地面確認
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Eventer"))
        {
            var eventers = collision.GetComponents<Eventer>();

            foreach (var ev in eventers)
            {
                if (!targetEventers.Contains(ev))
                {
                    targetEventers.Add(ev);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Eventer"))
        {
            var eventers = collision.GetComponents<Eventer>();

            foreach (var ev in eventers)
            {
                targetEventers.Remove(ev);
            }
        }
    }
}

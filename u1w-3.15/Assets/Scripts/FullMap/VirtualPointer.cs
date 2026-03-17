//using UnityEngine;
//using UnityEngine.InputSystem;

//public class VirtualPointer : MonoBehaviour
//{
//    //INPUT
//    InputSystem_Actions input;

//    Vector2 mov;

//    private void Awake()
//    {
//        input = new InputSystem_Actions();
//        input.UIpointer.SetCallbacks(this);
//    }

//    void OnEnable()
//    {
//        input.UIpointer.Enable();
//    }

//    void OnDisable()
//    {
//        input.UIpointer.Disable();
//    }

//    public void OnLook(InputAction.CallbackContext context)
//    {
//        mov = context.ReadValue<Vector2>();
//    }
//}

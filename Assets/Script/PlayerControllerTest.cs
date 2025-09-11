using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTest : MonoBehaviour
{
    private Vector2 moveInput;
    public InputAction act;


    void OnEnable()
    {
        act.Enable();
    }

    void OnDisable()
    {
        act.Disable();
    }

    // PlayerInputコンポーネントから自動的に呼び出されるメソッド
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        // moveInputの値を使ってキャラクターを動かす
        // transform.position += new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * speed;
    }
}
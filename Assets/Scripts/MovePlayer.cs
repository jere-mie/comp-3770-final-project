using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MovePlayer : MonoBehaviour
{

    private Vector2 input;
    private CharacterController characterController;
    private Vector3 direction;

    [SerializeField] private float speed;

    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float velocity;

    // rotation 
    public Vector2 rot = new Vector2(0f, 0f);

    public float xRotation;

    public PlayerControls playerControls;

    private Animator animator;

    // private Data data;

    void Awake()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
        animator = transform.Find("PlayerModel").GetComponent<Animator>();
        // data = GameObject.Find("DataStorage").GetComponent<Data>();
    }

    // Update is called once per frame
    void Update()
    {
        // applying rotation
        Vector2 transVector = playerControls.Player.LookMouse.ReadValue<Vector2>();
        rot.x = transVector.y * 25f * Time.deltaTime;
        rot.y += transVector.x * 25f * Time.deltaTime;

        xRotation -= rot.x;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // cam focal point rotates via x
        gameObject.transform.Find("CamFocalPoint").gameObject.transform.localRotation = Quaternion.Euler(xRotation, 0f, -0f);

        // our actual player rotates via y
        transform.localRotation = Quaternion.Euler(0f, rot.y, -0f);

        ApplyGravity();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        direction.y = velocity;
    }
    private void ApplyMovement()
    {
        // animator.SetBool("isWalking", true);
        float angley = getAngleY();
        if(direction.x == 0 && direction.z == 0){
            Debug.Log("NOT Walking");
            Debug.Log(direction);
            animator.SetBool("isWalking", false);
        }else{
            Debug.Log("Walking");
            Debug.Log(direction);
            animator.SetBool("isWalking", true);
        }
        characterController.Move(Quaternion.Euler(0, angley, 0) * direction * speed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0.0f, input.y);
    }

    private float getAngleY()
    {
        float angley = transform.rotation.eulerAngles.y;
        if (angley > 180)
        {
            angley -= 360f;
        }
        else if (angley < -180)
        {
            angley += 360f;
        }
        return angley;
    }
    void OnTriggerEnter(Collider collider){
        if(collider.CompareTag("Finish")){
                SceneManager.LoadScene("Battle");
        }
    }
}

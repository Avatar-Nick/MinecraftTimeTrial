using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Inputs")]
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;

    [Header("Player Objects")]
    public Transform cam;

    [Header("Physics Variables")]
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController characterController;
    private bool isGrounded;

    //-----------------------------------------------------------------------------------//
    //PlayerController Initialization
    //-----------------------------------------------------------------------------------//
    public void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
    }

    private void Update()
    {
        GetPlayerInputs();

        velocity = (transform.forward * vertical + transform.right * horizontal) * Time.deltaTime * speed;
        velocity += Vector3.up * gravity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);


        /*
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(moveDirection * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        */
    }

    
}

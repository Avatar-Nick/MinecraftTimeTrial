using System;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Inputs")]
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;

    [Header("Player Variables")]
    public float playerRadius = 0.5f;
    public float playerHeight = 2f;

    [Header("Physics Variables")]
    public Transform cam;
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public CharacterController characterController;
    private bool isGrounded;

    [Header("Block Variables")]
    public Transform highlightBlock;
    public Transform placeBlock;
    public float cursorIncrement = 0.1f;
    public float reach = 8f;

    [Header("UI Variables")]
    public BlockType selectedBlockType;

    //-----------------------------------------------------------------------------------//
    //PlayerController Initialization and Update
    //-----------------------------------------------------------------------------------//
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;           
    }

    private void Update()
    {
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

        PlaceCursorBlocks();
        PlaceBlock();
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Block Functions
    //-----------------------------------------------------------------------------------//
    private void PlaceCursorBlocks()
    {
        float step = cursorIncrement;
        Vector3 previousPosition = new Vector3();        
        while (step < reach)
        {
            Vector3 currentPosition = cam.position + (cam.forward * step);
            if (Map.instance.IsSolid(currentPosition.x, currentPosition.y, currentPosition.z))
            {
                highlightBlock.position = new Vector3((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z);
                placeBlock.position = previousPosition;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);
                return;
            }
            previousPosition = new Vector3((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z);
            step += cursorIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    private void PlaceBlock()
    {
        if (highlightBlock != null && highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Chunk chunk = Map.instance.GetChunk(highlightBlock.position);
                if (chunk != null)
                {                    
                    chunk.UpdateVoxel(highlightBlock.position, BlockType.Air);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Chunk chunk = Map.instance.GetChunk(placeBlock.position);
                if (chunk != null)
                {
                    chunk.UpdateVoxel(placeBlock.position, selectedBlockType);               
                }
            }
        }
    }
    //-----------------------------------------------------------------------------------//
}

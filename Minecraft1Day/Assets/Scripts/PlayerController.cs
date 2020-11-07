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

    [Header("Player Variables")]
    public float playerRadius = 0.5f;
    public float playerHeight = 2f;

    [Header("Physics Variables")]
    public Transform cam;
    public float lookMultiplier = 400.0f;
    public float walkSpeed = 6.0f;
    public float sprintSpeed = 12.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private float verticalMomentum;
    private bool jumpRequest;
    private float xRotation = 0f;

    [Header("Block Variables")]
    public Transform highlightBlock;
    public Transform placeBlock;
    public float cursorIncrement = 0.1f;
    public float reach = 8f;

    [Header("Toolbar")]
    public Toolbar toolbar;

    [Header("Check Variables")]    
    private bool isGrounded;
    private bool isSprinting;

    //-----------------------------------------------------------------------------------//
    //PlayerController Initialization and Update
    //-----------------------------------------------------------------------------------//
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Framerates are different between editor and application
        lookMultiplier = 180;
        if (Application.isEditor)
        {
            lookMultiplier = 400;
        }            
    } 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Map.instance.ToggleUI(!Map.instance.viewingUI);
        }

        if (Map.instance.viewingUI) return;

        GetPlayerInputs();
        PlaceCursorBlocks();

        if (Mathf.Abs(mouseHorizontal) > 20 || Mathf.Abs(mouseVertical) > 20)
            return;

        xRotation -= mouseVertical;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseHorizontal);
    }

    private void FixedUpdate()
    {
        if (Map.instance.viewingUI) return;

        CalculateVelocity();
        if (jumpRequest)
        {
            Jump();
        }

        transform.Translate(velocity, Space.World);
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Physics Functions
    //-----------------------------------------------------------------------------------//
    private void GetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X") * lookMultiplier * Time.deltaTime;
        mouseVertical = Input.GetAxis("Mouse Y") * lookMultiplier * Time.deltaTime;

        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }

        if (highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Map.instance.GetChunk(highlightBlock.position).UpdateVoxel(highlightBlock.position, BlockType.Air);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (toolbar.itemSlots[toolbar.slotIndex].itemSlotData != null && toolbar.itemSlots[toolbar.slotIndex].itemSlotData.amount > 0)
                {
                    Map.instance.GetChunk(placeBlock.position).UpdateVoxel(placeBlock.position, toolbar.itemSlots[toolbar.slotIndex].itemSlotData.blockType);
                    toolbar.itemSlots[toolbar.slotIndex].itemSlotData.Remove(1);
                }
                
            }
        }
    }

    private void CalculateVelocity()
    {
        // Apply gravity to verticalMomentum
        if (verticalMomentum > gravity)
        {
            verticalMomentum += Time.fixedDeltaTime * gravity;
        }

        // Apply sprinting to velocity
        if (isSprinting)
        {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        }
        else
        {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;
        }

        // Apply vertical momentum to falling
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
        if (velocity.z > 0 && CheckFront() || velocity.z < 0 && CheckBack())
        {
            velocity.z = 0;
        }

        if (velocity.x > 0 && CheckRight() || velocity.x < 0 && CheckLeft())
        {
            velocity.x = 0;
        }

        if (velocity.y < 0)
        {
            isGrounded = CheckDown(velocity.y);
            if (isGrounded)
            {
                velocity.y = 0;
            }
        }
        else
        {
            if (CheckUp(velocity.y))
            {
                velocity.y = 0;
            }
        }
    }

    private void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }
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
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Check Variables, more efficient collision detection than using mesh colliders
    //because mesh colliders are computationally expensive to create and cause drops
    //in framerate
    //-----------------------------------------------------------------------------------//
    private bool CheckDown(float downSpeed)
    {
        // Need to check all 4 corners to ensure you can correctly determine if the player is on the ground even if they are standing on 
        // the intersection of multiple voxels.
        if (Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y + downSpeed, transform.position.z - playerRadius) ||
            Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y + downSpeed, transform.position.z - playerRadius) ||
            Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y + downSpeed, transform.position.z + playerRadius) ||
            Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y + downSpeed, transform.position.z + playerRadius))
        {
            return true;
        }
        return false;
    }

    private bool CheckUp(float upSpeed)
    {
        // Need to check all 4 corners to ensure you can correctly determine if the player is above a block even if they are standing on 
        // the intersection of multiple voxels.
        if (Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y + playerHeight + upSpeed, transform.position.z - playerRadius) ||
            Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y + playerHeight + upSpeed, transform.position.z - playerRadius) ||
            Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y + playerHeight + upSpeed, transform.position.z + playerRadius) ||
            Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y + playerHeight + upSpeed, transform.position.z + playerRadius))
        {
            return true;
        }
        return false;
    }

    private bool CheckFront()
    {
        if (Map.instance.IsSolid(transform.position.x, transform.position.y, transform.position.z + playerRadius) ||
            Map.instance.IsSolid(transform.position.x, transform.position.y + 1f, transform.position.z + playerRadius))
        {
            return true;
        }
        return false;
    }

    private bool CheckBack()
    {
        if (Map.instance.IsSolid(transform.position.x, transform.position.y, transform.position.z - playerRadius) ||
            Map.instance.IsSolid(transform.position.x, transform.position.y + 1f, transform.position.z - playerRadius))
        {
            return true;
        }
        return false;
    }

    private bool CheckLeft()
    {
        if (Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y, transform.position.z) ||
            Map.instance.IsSolid(transform.position.x - playerRadius, transform.position.y + 1f, transform.position.z))
        {
            return true;
        }
        return false;
    }

    private bool CheckRight()
    {
        if (Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y, transform.position.z) ||
            Map.instance.IsSolid(transform.position.x + playerRadius, transform.position.y + 1f, transform.position.z))
        {
            return true;
        }
        return false;
    }
    //-----------------------------------------------------------------------------------//

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;

    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Rigidbody rb;

    private NavMeshAgent navAgent;

    public bool isDesh = false;
    public int DeshSpeed = 10;




    private void Start()
    {
        // 필요한 컴포넌트들을 가져옵니다.
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();


        // 커서를 숨기고 고정시킵니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        MoveMent();
        DoDesh();



    }

    public void DoDesh()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            isDesh = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDesh = false;
        }


    }

    public void MoveMent()
    {

        // 마우스 이동으로 시점 회전을 처리합니다.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 플레이어 이동을 처리합니다.

        if (isDesh)
        {
            movementSpeed = DeshSpeed;
        }
        else
        {
            movementSpeed = 5.0f;
        }

        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;
        movement.y = rb.velocity.y;

       

        rb.velocity = movement;


    }
}

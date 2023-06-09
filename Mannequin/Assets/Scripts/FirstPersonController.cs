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
        // �ʿ��� ������Ʈ���� �����ɴϴ�.
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();


        // Ŀ���� ����� ������ŵ�ϴ�.
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

        // ���콺 �̵����� ���� ȸ���� ó���մϴ�.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // �÷��̾� �̵��� ó���մϴ�.

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

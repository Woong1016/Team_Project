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
        // ���콺 �̵����� ���� ȸ���� ó���մϴ�.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // �÷��̾� �̵��� ó���մϴ�.
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;
        movement.y = rb.velocity.y;
          
        rb.velocity = movement;

    }
}

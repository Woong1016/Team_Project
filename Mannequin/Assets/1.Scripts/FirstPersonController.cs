using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;
    public float warningDistance = 10f; // ������� ����� �Ÿ�

    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Rigidbody rb;
    private NavMeshAgent navAgent;

    public bool isDashing = false;
    public int dashSpeed = 10;

    public int maxHp = 100;
    public int currentHp;

    public Image uiHpimage;
    public Image uiBlureffect;

    public bool Key_01 = false;
    public bool Key_02 = false;

    public GameObject uiKey_01;
    public GameObject uiKey_02;

    public Transform monster;        // ���� ������Ʈ�� Transform ������Ʈ
    public AudioSource warningAudioSource;  // ����� ����� ���� AudioSource ������Ʈ

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        uiKey_01.SetActive(false);
        uiKey_02.SetActive(false);
        currentHp = maxHp; // ���� �� �ִ� ü������ �ʱ�ȭ
    }

    private void Update()
    {
        Movement();
        DoDash();
        UiUpdate();
        PlayWarningSound();
    }

    public void UiUpdate()
    {
        float viewHp = currentHp;
        float viewMaxHp = maxHp;
        uiHpimage.rectTransform.sizeDelta = new Vector2((viewHp / viewMaxHp) * 800.0f, 364.0f);

        float alpha = 0.5f - (viewHp / viewMaxHp);

        Color currentColor = uiBlureffect.color;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        uiBlureffect.color = newColor;

        if (Key_01) uiKey_01.SetActive(true);
        if (Key_02) uiKey_02.SetActive(true);
    }

    public void PlayWarningSound()
    {
        float distance = Vector3.Distance(transform.position, monster.position);

        if (distance <= warningDistance)
        {
            if (!warningAudioSource.isPlaying)
            {
                warningAudioSource.Play();
            }
        }
        else
        {
            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop();
            }
        }
    }

    public void DoDash()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isDashing = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isDashing = false;
        }
    }

    public void Movement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (isDashing)
        {
            movementSpeed = dashSpeed;
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

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHp += healAmount;

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    private void Die()
    {
        // �÷��̾��� ��� ó���� �����մϴ�.
        // ���� ���, ���� ���� ȭ���� �����ְų� ������ ������ ������ �� �ֽ��ϴ�.
        // �� �κ��� ���ӿ� ���� ���� ����� �޶��� �� �ֽ��ϴ�.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            HealthPickup healthPickup = other.GetComponent<HealthPickup>();

            if (healthPickup != null)
            {
                currentHp += healthPickup.healthAmount;

                if (maxHp <= currentHp)
                {
                    currentHp = maxHp;
                }
            }

            Destroy(other.gameObject);
        }

        if (other.CompareTag("KeyPickup_01"))
        {
            Key_01 = true;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("KeyPickup_02"))
        {
            Key_02 = true;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DoorObject"))
        {
            if (Key_01 && Key_02)
            {
                // ���� ������ ó�� 
            }
        }
    }
}

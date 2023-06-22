using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float jumpForce = 5f;
    public float warningDistance = 10f;

    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Rigidbody rb;
    private NavMeshAgent navAgent;

    public bool isDashing = false;
    public int dashSpeed = 10;

    public float maxStamina = 10f;
    public float currentStamina;

    public float staminaRecoveryRate = 0.5f;
    public float staminaRecoveryDelay = 2f;
    private float staminaRecoveryTimer;

    public int maxHp = 100;
    public int currentHp;

    public Image uiHpimage;
    public Image uiBlureffect;
    public Image uiStaminaBar;

    public bool Key_01 = false;
    public bool Key_02 = false;

    public GameObject uiKey_01;
    public GameObject uiKey_02;

    public Transform monster;
    public AudioSource warningAudioSource;
    public AudioSource lowHealthAudioSource; // 플레이어의 체력이 낮을 때 재생할 오디오 소스
    private bool isLowHealthAudioPlaying = false; // 체력이 낮은 상태에서 오디오 소스가 재생 중인지를 나타내는 변수



    public float interactDistance = 3f; // 상호작용 가능한 거리
    private bool nearDoor = false; // 문 근처에 있는지 여부를 나타내는 변수

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();

        uiKey_01.SetActive(false);
        uiKey_02.SetActive(false);
        currentHp = maxHp;
        currentStamina = maxStamina;
        staminaRecoveryTimer = staminaRecoveryDelay;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 씬 전환 이벤트에 함수 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // 씬이 로드될 때 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 마우스 커서 보이기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Movement();
        DoDash();
        UiUpdate();
        PlayWarningSound();

        if (nearDoor && Input.GetKeyDown(KeyCode.E))
        {
            if (Key_01 && Key_02)
            {
                StartCoroutine(StartGoodEnding());
            }
        }

        if (currentHp <= 0)
        {
            StartCoroutine(StartEndingScene());
        }

        // 스테미나 회복 로직
        if (!isDashing)
        {
            if (currentStamina < maxStamina)
            {
                staminaRecoveryTimer -= Time.deltaTime;

                if (staminaRecoveryTimer <= 0f)
                {
                    currentStamina += staminaRecoveryRate * Time.deltaTime;
                    currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
                }
            }
            else
            {
                staminaRecoveryTimer = staminaRecoveryDelay;
            }
        }
    }

    public void UiUpdate()
    {
        // HP UI 업데이트
        float viewHp = currentHp;
        float viewMaxHp = maxHp;
        uiHpimage.rectTransform.sizeDelta = new Vector2((viewHp / viewMaxHp) * 800.0f, 364.0f);

        // 스테미나 UI 업데이트
        float viewStamina = currentStamina;
        float viewMaxStamina = maxStamina;
        float newWidth = (viewStamina / viewMaxStamina) * 800.0f;
        uiStaminaBar.rectTransform.sizeDelta = new Vector2(newWidth, uiStaminaBar.rectTransform.sizeDelta.y);


        // 블러 효과 업데이트
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
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f)
        {
            isDashing = true;
            currentStamina -= Time.deltaTime;
        }
        else
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
    }

    private IEnumerator StartEndingScene()
    {
        float fadeDuration = 2f; // 페이드 인/아웃에 걸리는 시간 (초)
        float elapsedTime = 0f;
        Color startColor = Color.clear;
        Color endColor = Color.black;

        // 화면을 점점 어둡게 만듭니다.
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float alpha = Mathf.Lerp(startColor.a, endColor.a, t);

            uiBlureffect.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 일정 시간 대기
        yield return new WaitForSeconds(1f);

        // 엔딩 씬으로 전환합니다.
        SceneManager.LoadScene("EndingScene");
    }

    private IEnumerator StartGoodEnding()
    {
        float fadeDuration = 2f; // 페이드 인/아웃에 걸리는 시간 (초)
        float elapsedTime = 0f;
        Color startColor = uiBlureffect.color;
        Color endColor = Color.black;

        // 화면을 점점 어둡게 만듭니다.
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color newColor = Color.Lerp(startColor, endColor, t);

            uiBlureffect.color = newColor;

            yield return null;
        }

        // GoodEnding 씬으로 전환합니다.
        SceneManager.LoadScene("GoodEnding");
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
        // Death logic goes here
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealthPickup"))
        {
            HealthPickup healthPickup = other.GetComponent<HealthPickup>();

            if (healthPickup != null)
            {
                currentHp += healthPickup.healthAmount;

                if (currentHp > maxHp)
                {
                    currentHp = maxHp;
                }
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Monster"))
        {
            TakeDamage(20);
        }
        else if (other.CompareTag("KeyPickup_01"))
        {
            Key_01 = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("KeyPickup_02"))
        {
            Key_02 = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DoorObject"))
        {
            nearDoor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DoorObject"))
        {
            nearDoor = false;
        }
    }
}

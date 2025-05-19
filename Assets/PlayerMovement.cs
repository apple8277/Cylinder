using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rayDistance = 0.7f; // 벽 감지 거리
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded = true;
    public float fallMultiplier = 3.0f;
    public float runSpeed = 20f;
    private float currentSpeed;
    public float originalMoveSpeed;
    public Light sceneLight;
    public MouseLook mouseLookScript;
    private float originalLightIntensity;

    private bool isSlowed = false;
    private bool isDarkened = false;
    private bool isLocked = false;

    void Start()
    {
        originalMoveSpeed = moveSpeed;
        if(sceneLight != null)
        {
            originalLightIntensity = sceneLight.intensity;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveInput = transform.right * h + transform.forward * v;
        Vector3 moveDirection = moveInput.normalized * currentSpeed * Time.fixedDeltaTime;



        if (moveInput != Vector3.zero)
        {
            // 벽이 있는지 Ray로 감지
            Ray ray = new Ray(transform.position, moveInput);
            if (!Physics.Raycast(ray, rayDistance))
            {
                rb.MovePosition(rb.position + moveDirection);
            }
        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Wall1") && !isSlowed)
        {
            StartCoroutine(SlowMovementForSeconds(5F));
        }
        if (collision.gameObject.CompareTag("Wall2") && !isDarkened)
        {
            StartCoroutine(DarkenScreenForSeconds(5f));
        }
        if (collision.gameObject.CompareTag("Wall3") && !isLocked)
        {
            StartCoroutine(LockMouseLookForSeconds(5f));
        }
    }
    IEnumerator SlowMovementForSeconds(float duration)
    {
        isSlowed = true;
        moveSpeed = 3f;

        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isSlowed = false;
    }
    IEnumerator DarkenScreenForSeconds(float duration)
    {
        isDarkened = true;

        if (sceneLight != null)
            sceneLight.intensity = originalLightIntensity * 0.2f; // 20%로 줄이기

        yield return new WaitForSeconds(duration);

        if (sceneLight != null)
            sceneLight.intensity = originalLightIntensity; // 복구

        isDarkened = false;
    }

    IEnumerator LockMouseLookForSeconds(float duration)
    {
        isLocked = true;
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        yield return new WaitForSeconds(duration);
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        isLocked = false;
    }
}
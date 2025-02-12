using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float builtUp;
    [SerializeField] private float drag;
    [SerializeField] private float groundDrag;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpDegrees;
    [SerializeField] private float quickFallMass;
    private float defaultMass;
    private float currentJumpTime;
    private bool isOnGround;

    [Header("Camera Movement")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Vector2 lookClampY;
    private Transform cameraPlayer;

    [Header("audio")]
    [SerializeField] private AudioSource walkingSound;

    private Rigidbody rb;

    private bool isGameplay = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraPlayer = transform.GetChild(0);
        defaultMass = rb.mass;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == 3)
        {
            currentJumpTime = jumpTime;
            isOnGround = true;
        }
    }

    private void Update()
    {
        Vector3 rotate = cameraPlayer.rotation.eulerAngles + new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * mouseSensitivity * Time.deltaTime * 100.0f;
        //float rotateX = Mathf.Clamp(rotate.x, 0.0f, 180.0f) + (Mathf.Clamp(rotate.x, 180.0f, 360.0f) - 540.0f) * Mathf.Clamp(rotate.x - 180.0f, 0, 1);

        float rotateX = rotate.x;
        if (rotateX > 180)
        {
            rotateX -= 360;
        }

        if(isGameplay)
        {
            cameraPlayer.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(rotateX, lookClampY.x, lookClampY.y), rotate.y, rotate.z));
        }
    }

    public IEnumerator Move(InputAction.CallbackContext context)
    {
        double oldTime = context.time;
        if (rb != null)
        {
            StartCoroutine(WalkingSoundPlayer(context));

            while(context.time == oldTime || new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized.magnitude > 0)
            {
                float extraDrag = 1.0f;
                if (isOnGround)
                {
                    extraDrag = groundDrag;
                    isOnGround = false;
                }
                Vector2 direction = context.ReadValue<Vector2>();
                Vector3 direction3 = direction.y * cameraPlayer.forward + direction.x * cameraPlayer.right;
                rb.velocity += new Vector3(direction3.x, 0, direction3.z) * speed / builtUp / (rb.velocity.magnitude / maxVelocity + 1.0f);
                rb.velocity = new Vector3(rb.velocity.x / (1.0f + drag / 1000.0f) / extraDrag, rb.velocity.y, rb.velocity.z / (1.0f + drag / 1000.0f) / extraDrag);
                yield return null;
            }
        }
    }

    public IEnumerator Jump(InputAction.CallbackContext context)
    {
        double oldTime = context.time;
        if(rb != null)
        {
            while(context.time == oldTime)
            {
                if(currentJumpTime > 0)
                {
                    rb.AddForce(new Vector3(0, jumpHeight * Mathf.Pow(currentJumpTime * jumpDegrees, jumpDegrees), 0), ForceMode.Impulse);
                    currentJumpTime -= Time.deltaTime;
                }
                yield return null;
            }
            currentJumpTime = 0;
        }
    }

    public void ChangeGameplay(bool gameplay)
    {
        isGameplay = gameplay;
    }

    private IEnumerator WalkingSoundPlayer(InputAction.CallbackContext context)
    {
        double oldTime = context.time;
        while (context.time == oldTime || new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized.magnitude > 0)
        {
            if(isOnGround)
            {
                AudioSource audioInstance = walkingSound;
                audioInstance.pitch = UnityEngine.Random.Range(0.0f, 2.0f);
                audioInstance.volume = UnityEngine.Random.Range(0.25f, 0.75f);
                audioInstance.Play();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator QuickFall(InputAction.CallbackContext context)
    {
        double oldTime = context.time;
        while (context.time == oldTime)
        {
            rb.AddForce(new Vector3(0, -quickFallMass, 0));
            rb.mass = quickFallMass;
            yield return null;
        }
        rb.mass = defaultMass;
    }
}

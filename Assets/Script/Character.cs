using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Camera cam;
    CharacterController characterController;
    float maxSpeed = 10, acceleration = 10, jumpForce = 5;
    float speed, verticalMovement;
    Vector3 direction, directionForward, directionRight, nextDir;
    Animator animator;

    [Header("Configuration Audio")]
    public AudioSource footstepSource;

    [Header("Banques de sons (AudioClips)")]
    public AudioClip[] woodClips;
    public AudioClip[] grassClips;
    public AudioClip[] stoneClips; // Extérieur (Terrain)
    public AudioClip[] sandClips;
    public AudioClip[] cliffClips; // Intérieur (Château / Roche 3D)

    [Header("Paramètres du Raycast")]
    public Transform feetPosition;
    public float rayDistance = 1.5f;

    void Awake()
    {
        cam = Camera.main;
        direction = transform.forward;
        nextDir = transform.forward;
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        gravity();
        Move();

        characterController.Move((direction * speed + verticalMovement * Vector3.up) * Time.deltaTime);

        if (animator != null)
            animator.SetFloat("Speed", speed / maxSpeed);
    }

    private void Move()
    {
        if ((Input.GetAxisRaw("Vertical")) != 0 || (Input.GetAxisRaw("Horizontal")) != 0)
        {
            directionForward = cam.transform.forward;
            directionForward.y = 0;
            directionForward *= Input.GetAxisRaw("Vertical");

            directionRight = cam.transform.right;
            directionRight.y = 0;
            directionRight *= Input.GetAxisRaw("Horizontal");

            nextDir = Vector3.Normalize(directionForward + directionRight);
            direction = Vector3.Lerp(direction, nextDir, Time.deltaTime * 10f);

            if (speed < maxSpeed)
                speed += acceleration * Time.deltaTime;
            else
                speed = maxSpeed;
        }
        else
        {
            if (speed != 0)
            {
                if (speed <= 2 * acceleration * Time.deltaTime)
                    speed = 0;
                else
                    speed -= 2 * acceleration * Time.deltaTime;
            }
        }

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }

    private void gravity()
    {
        if (verticalMovement <= 0 && characterController.isGrounded)
            verticalMovement = -5;
        else
            verticalMovement -= jumpForce * 2 * Time.deltaTime;
    }

    private void PlaySound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        footstepSource.pitch = Random.Range(0.9f, 1.1f);
        footstepSource.PlayOneShot(clips[randomIndex]);
    }

    public void PlayFootstepSound()
    {
        Debug.Log("L'ANIMATION APPELLE BIEN LE CODE !");
        RaycastHit hit;
        Vector3 rayOrigin = feetPosition != null ? feetPosition.position : transform.position;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain != null)
            {
                int textureIndex = TerrainSurfaceChecker.GetMainTexture(hit.point, terrain);

                switch (textureIndex)
                {
                    case 0: PlaySound(grassClips); break;
                    case 1: PlaySound(stoneClips); break;
                    case 2: PlaySound(woodClips); break;
                    case 3: PlaySound(sandClips); break;
                }
            }
            else
            {
                string surfaceTag = hit.collider.tag;

                switch (surfaceTag)
                {
                    case "Stone":
                        PlaySound(cliffClips); 
                        break;
                    case "Wood":
                        PlaySound(woodClips);
                        break;
                    default:
                        PlaySound(stoneClips);
                        break;
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dino : MonoBehaviour
{
    bool isGameOver = false;
    public static Action GameEnd;

    public Rigidbody2D rb;
    public float jumpHeight;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;

    //public BoxCollider2D standCollider;
    //public BoxCollider2D crouchCollider;

    public Collider2D crouchCollider;

    public AudioSource jumpSource;
    public AudioSource deadSource;

    public AnimatorOverrideController skin1;
    public AnimatorOverrideController skin2;
    public AnimatorOverrideController skin3;
    public AnimatorOverrideController skin4;

    private bool isGrounded = true;
    private bool crouching = false;

    Collider2D col;
    int i;
    private void Awake()
    {
        ChangeSkin();
        dinoSoundVolumeChange();
    }
    private void ChangeSkin()
    {
        switch(PlayerPrefs.GetInt("DinoSkin", 0))
        {
            case 1:
                transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = skin1 as RuntimeAnimatorController;
                break;
            case 2:
                transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = skin2 as RuntimeAnimatorController;
                break;
            case 3:
                transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = skin3 as RuntimeAnimatorController;
                break;
            case 4:
                transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = skin4 as RuntimeAnimatorController;
                break;
        }
    }
    private void FixedUpdate()
    {
       col =  Physics2D.OverlapCircle(groundCheck.position, .2f, groundLayer);
        if (col != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void Update()
    {
        if (transform.position.y < 3.45)
        {
            transform.position = new Vector2(transform.position.x, 3.45f);
        }
        if (isGameOver == false)
        {
            animator.SetBool("Crouching", crouching);
            animator.SetBool("Jumping", !isGrounded);

            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
            {
                jumpSource.Play();
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                //rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                rb.gravityScale = 6f;
                
                crouching = true;
                crouchCollider.enabled = false;
                //if (!crouchCollider.enabled)
                //{

                //crouchCollider.enabled = true;
                //standCollider.enabled = false;
                //}
                i = 0;
            }
            else
            {
                rb.gravityScale = 3f;
                crouching = false;
                i++;
                if (i == 2)
                {
                    crouchCollider.enabled = true;
                    i = 0;
                }
                //if (crouchCollider.enabled)
                //{

                //crouchCollider.enabled = false;
                //standCollider.enabled = true;
                //}
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Obstacle")|| collision.transform.CompareTag("Laser"))
        {
            
            isGameOver = true;
            GameEnd?.Invoke();
            deadSource.Play();
            if (jumpSource.isPlaying)
            {
                jumpSource.Stop();
                
            }
        }
    }
    public void dinoSoundVolumeChange()
    {
        jumpSource.volume = PlayerPrefs.GetFloat("volume") * 0.05f;
        deadSource.volume = PlayerPrefs.GetFloat("volume") * 0.1f;
    }
}

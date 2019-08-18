﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float laneSpeed;
    [SerializeField] float jumpLength;
    [SerializeField] float jumpHeight;
    [SerializeField] float slideLength;
    [SerializeField] int maxLife = 3;
    [SerializeField] float minSpeed = 10f;
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float invincibleTime;
    [SerializeField] GameObject[] model;

    private Animator anim;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Vector3 verticalTargetPosition;
    private int currentLane = 1;
    private bool jumping = false;
    private bool sliding = false;
    private float jumpStart;
    private float slideStart;
    private Vector3 boxColliderSize;
    private int currentLife;
    private bool invincible = false;
    static int blinkingValue;
    private UIManagement uiManagement;
    private int fishBone;
    private float score;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        anim.Play("runStart");
        currentLife = maxLife;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManagement = FindObjectOfType<UIManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * speed;
        uiManagement.UpdateScore((int)score);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        if (jumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if(ratio >= 1f)
            {
                jumping = false;
                anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }

        if (sliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLength;
            if(ratio >= 1f)
            {
                sliding = false;
                anim.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }

        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }

    void ChangeLane(int direction)
    {
       int changedLand = currentLane + direction;
        if (changedLand < 0 || changedLand > 2)
            return;
        currentLane = changedLand;
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0); 
    }

    void Jump()
    {
        if (!jumping)
        {
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jumping", true);
            jumping = true;
        }
    }

    void Slide()
    {
        if(!jumping && !sliding)
        {
            slideStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / slideLength);
            anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            sliding = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reward"))
        {
            fishBone++;
            uiManagement.UpdateReward(fishBone);
            other.transform.parent.gameObject.SetActive(false);
        }

        if (invincible)
            return; 
        if (other.CompareTag("Obstacle"))
        {
            currentLife--;
            uiManagement.UpdateHealth(currentLife);
            anim.SetTrigger("Hit");
            speed = 0;
            if (currentLife <= 0)
            {
                speed = 0;
                anim.SetBool("Dead", true);
                uiManagement.gameOverPanel.SetActive(true);
            }
            else
            {
                StartCoroutine(InvincibleTimer(invincibleTime)); 
            }
        }
    }

    protected IEnumerator InvincibleTimer(float time)
    {
        invincible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while (timer < time && invincible)
        {
            for (int i =0; i < model.Length; i++)
            {
                model[i].SetActive(enabled);
            }
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        for (int i = 0; i < model.Length; i++)
        {
            model[i].SetActive(true);
        }
        //Shader.SetGlobalFloat(blinkingValue, 0);
        invincible = false;
    }

    public void IncreaseSpeed()
    {
        speed *= 1.5f;
        if (speed >= maxSpeed)
            speed = maxSpeed;
    }
}

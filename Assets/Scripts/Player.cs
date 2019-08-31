using System.Collections;
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
    [HideInInspector]
    public int fishBone;
    [HideInInspector]
    public float score;

    private bool canMove;

    private AudioSource jumpAudio;
    private AudioSource hurtAudio;
    private AudioSource deadAudio;
    private AudioSource slideAudio;
    private AudioSource buttonPressAudio;
    private AudioSource fishBoneAudio;
    private AudioSource powerUpSound;
    AudioSource[] sounds;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        anim.Play("runStart");
        currentLife = maxLife;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManagement = FindObjectOfType<UIManagement>();
        GameManager.gm.StartMissions();
        sounds = GetComponents<AudioSource>();
        jumpAudio = sounds[0];
        hurtAudio = sounds[1];
        deadAudio = sounds[2];
        slideAudio = sounds[3];
        buttonPressAudio = sounds[4];
        fishBoneAudio = sounds[5];
        powerUpSound = sounds[6];
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;

        score += Time.deltaTime * speed;
        uiManagement.UpdateScore((int)score);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            buttonPressAudio.Play();
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            buttonPressAudio.Play();
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
            jumpAudio.Play();
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
            slideAudio.Play();
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
            fishBoneAudio.Play();
            fishBone++;
            uiManagement.UpdateReward(fishBone);
            other.transform.parent.gameObject.SetActive(false);
        }

        if (invincible)
            return; 
        if (other.CompareTag("Obstacle"))
        {
            canMove = false;
            currentLife--;
            uiManagement.UpdateHealth(currentLife);
            anim.SetTrigger("Hit");
            speed = 0;
            if (currentLife <= 0)
            {
                deadAudio.Play();
                speed = 0;
                anim.SetBool("Dead", true);
                uiManagement.gameOverPanel.SetActive(true);
                Invoke("CallMenu", 2f);
            }
            else
            {
                hurtAudio.Play();
                Invoke("CanMove", 0.75f);
                StartCoroutine(InvincibleTimer(invincibleTime)); 
            }
        }
    }

    void CanMove()
    {
        canMove = true;
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

    void CallMenu()
    {
        GameManager.gm.fishbones += fishBone;
        GameManager.gm.Endrun();
    } 

    public void IncreaseSpeed()
    {
        powerUpSound.Play();
        speed *= 1.5f;
        if (speed >= maxSpeed)
            speed = maxSpeed;
    }
}

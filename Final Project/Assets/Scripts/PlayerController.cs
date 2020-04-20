using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    Rigidbody2D rb;
    private float hozMovement;
    private float verMovement;
    private bool facingRight = true;
    private bool isOnGround;
    private int score;
    private int lives;
    [SerializeField]
    private float invincibilityCount;

    public Text scoreText;
    public Text livesText;
    public Text timeText;
    public Text resultText;
    public Text invincibleText;
    public Text missionText;
    public Transform groundCheck;
    public LayerMask allGround;
    public AudioClip winClip;
    public AudioClip coinClip;
    public AudioClip enemyClip;
    public AudioClip teleportClip;
    public AudioClip mushroomClip;
    public AudioClip gameOverClip;
    public float checkRadius;
    public float speed;
    public float timeStart = 60;
    public bool lose = false;
    public bool invincibility = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        score = 0;
        lives = 3;
        invincibilityCount = 5f;
        invincibility = false;
        lose = false;
        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
        invincibleText.text = "";
        resultText.text = "";
        timeText.text = "Time: " + timeStart.ToString();
        Destroy(missionText, 5);
    }

    void Update()
    {
        if (timeStart > 0) 
        {
            timeStart -= Time.deltaTime;
            timeText.text = "Time: " + Mathf.Round(timeStart).ToString();

            if (score == 8)
            {
                timeStart = Time.deltaTime;
                timeText.text = "Time: --";
            }
        }
        else
        {
            audioSource.PlayOneShot(gameOverClip);
            lose = true;
            resultText.text = "Time's up! Game created by Kayla Hoyte.\n\nPress 'R' to restart.";
            Debug.Log("Time's up.");
            Destroy(this);
        }

        if (invincibility == true)
        {
            invincibilityCount -= Time.deltaTime;

            if (invincibilityCount <= 0)
            {
                invincibleText.text = "Invincibility has run out!";
                invincibility = false;
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (isOnGround)
            {
                anim.SetInteger("State", 1);
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (isOnGround)
            {
                anim.SetInteger("State", 1);
            }
        }
        else
        {
            if (isOnGround)
            {
                anim.SetInteger("State", 0);
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))
        {
            anim.SetInteger("State", 2);
        }
    }

    void FixedUpdate()
    {
        hozMovement = Input.GetAxisRaw("Horizontal");
        verMovement = Input.GetAxisRaw("Vertical");

        rb.AddForce(new Vector2(hozMovement * speed, verMovement * speed));

        isOnGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, allGround);

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }
        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(new Vector2(0, 3.5f), ForceMode2D.Impulse);
                anim.SetBool("isInAir", true);
                //anim.SetInteger("State", 2);
            }
            else
            {
                anim.SetBool("isInAir", false);
                //anim.SetInteger("State", 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            audioSource.PlayOneShot(coinClip);
            Debug.Log($"Coin {score+1} was destroyed.");
            Destroy(collision.gameObject);
            score += 1;
            UpdateScore();

            if (score == 4)
            {
                transform.position = new Vector2(39.01f, -4.86f);
                lives = 3;
                audioSource.PlayOneShot(teleportClip);
                UpdateLives();
            }

            if (score == 8)
            {
                lose = false;
                audioSource.PlayOneShot(winClip);
                Debug.Log("All coins have been collected.");
                resultText.text = $"You win! Time left: {Mathf.Round(timeStart)} seconds. Game created by Kayla Hoyte.\n\nPress 'R' to Restart.";
            }
        }

        if (collision.gameObject.CompareTag("Pickup2"))
        {
            audioSource.PlayOneShot(mushroomClip);
            invincibility = true;
            invincibilityCount = 5f;
            invincibleText.text = $"You have gained invincibility for {invincibilityCount.ToString()} seconds!";
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (invincibility == true)
            {
                lives -= 0;
                Debug.Log($"Player has come in contact with the enemy, but is invincible. Current lives is still {lives}");
            }
            else
            {
                audioSource.PlayOneShot(enemyClip);
                Destroy(collision.gameObject);
                lives -= 1;
                UpdateLives();
                Debug.Log($"Player has come in contact with the enemy. Current lives is {lives}");
            }

            if (lives <= 0)
            {
                audioSource.PlayOneShot(gameOverClip);
                lose = true;
                Debug.Log("You lose.");
                resultText.text = "You lose! Game created by Kayla Hoyte.\n\nPress 'R' to restart.";
                Destroy(this);
            }
        }
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    void UpdateLives()
    {
        livesText.text = "Lives: " + lives.ToString();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }
}

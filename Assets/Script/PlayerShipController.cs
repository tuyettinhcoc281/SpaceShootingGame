using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerShipController : MonoBehaviour
{
    // References =========================================
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col2d;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera playerCamera;

    // Settings ===========================================
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float shootSpeed = 7.5f;
    [SerializeField] private int maxHealth = 3;

    // INSTANTIATE OBJECT =================================
    [Header("Instantiate Object")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject hudStarPrefab;
    [SerializeField] private GameObject hudHealthPrefab;
    
    // HUD ================================================
    [Header("HUD")]
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform starBar;
    [SerializeField] private Transform levelComplete;
    [SerializeField] private Transform gameOver;
    
    private List<Image> _healthImage = new();
    private List<Image> _starImage = new();

    // INPUT ACTION =======================================
    private InputAction _moveAction;
    private InputAction _shootAction;

    // MISC Item ==========================================
    private int starCount;
    private int starAmount;
    
    private bool isDead;
    private bool isLevelComplete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.enabled = false;
        
        _moveAction = playerInput.actions["Move"];
        _shootAction = playerInput.actions["Attack"];
        
        starAmount = GameObject.FindGameObjectsWithTag("Star").Length;
        
        for (var i = 0; i < maxHealth; i++)
        {
            var h = Instantiate(hudHealthPrefab, healthBar);
            var component = h.GetComponent<Image>();
            _healthImage.Add(component);
            
            component.color = new Color(1, 1, 1, 1);
        }
        
        for (var i = 0; i < starAmount; i++)
        {
            var s = Instantiate(hudStarPrefab, starBar);
            var component = s.GetComponent<Image>();
            _starImage.Add(component);
            
            component.color = new Color(1, 1, 1, (float) 25 / 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead || isLevelComplete) return;
        
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);

        MoveShip();
        RotateShip();

        if (_shootAction.WasPressedThisFrame()) Shoot();
    }

    Vector2 moveVector;

    private void MoveShip()
    {
        var moveDirection = _moveAction.ReadValue<Vector2>();

        moveVector = Vector2.Lerp(moveVector, moveDirection * moveSpeed, Time.deltaTime * 1.25f);

        rigidBody.linearVelocity = moveVector;
    }

    float angleDeg = 0f;

    private void RotateShip()
    {
        Vector3 mousePos = (Vector2)playerCamera.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        angleDeg = Mathf.LerpAngle(angleDeg, (180 / Mathf.PI) * angleRad - 90, Time.deltaTime * rotateSpeed); // Offset this by 90 Degrees

        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        Debug.DrawLine(transform.position, mousePos, Color.white, Time.deltaTime);
    }

    private void Shoot()
    {
        Debug.Log("Shoot");

        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angleDeg)).GetComponent<Bullet>();
        bullet.rigidBody.linearVelocity = bullet.transform.up * shootSpeed;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            Damage();
        }
        
        if (collision.gameObject.CompareTag("Star"))
        {
            starCount++;
            _starImage[starCount - 1].color = new Color(1, 1, 1, 1);
            
            Destroy(collision.gameObject);
        }
        
        if (collision.gameObject.CompareTag("Finish") && isDead == false  && starCount == starAmount)
        {
            isLevelComplete = true;
            
            TurnOffHud();
            
            levelComplete.gameObject.SetActive(true);
        }
    }
    
    private void Damage()
    {
        if (maxHealth > 0) maxHealth--;
        _healthImage[maxHealth].color = new Color(1, 1, 1, (float) 25 / 255);
        
        // Calculate the opposite direction of the player's movement
        Vector2 oppositeDirection = -moveVector.normalized;

        // Apply a force in the opposite direction
        rigidBody.AddForce(oppositeDirection * moveSpeed, ForceMode2D.Impulse);
        
        if (maxHealth <= 0)
        {
            col2d.enabled = false;
            moveVector = Vector2.zero;
            ShipDestroy();
        }
    }
    
    private void ShipDestroy()
    {
        isDead = true;
        isLevelComplete = false;
        
        animator.enabled = true;
        animator.Play("PlayerExplosion");
        
        TurnOffHud();
        
        gameOver.gameObject.SetActive(true);
    }

    private void TurnOffHud()
    {
        healthBar.gameObject.SetActive(false);
        starBar.gameObject.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
    
    public void NextLevel()
    {
        var nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Application.Quit();
        }
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 input;
    GameManager gameManager;
    private bool hasWon = false;

    [Header("Speed Settings")]
    [SerializeField] float moveSpeed;
    private float originalMoveSpeed;
    private Coroutine speedBoostCoroutine;

    [Header("Animation Settings")]
    [SerializeField] Animator animator;
    private string currentAnimationState;
    public const string Animation_Run = "Player_Run";
    public const string Animation_Idle = "Player_Idle";
    public const string Animation_Push = "Player_Push";
    public const string Animation_Defeat = "Player_Defeat";
    public const string Animation_Victory = "Player_Victory";

    [Header("Push Settings")]
    bool isPushing = false;
    bool hasStartedPush = false;
    [SerializeField] Transform snowball;
    [SerializeField] float minSnowballMass = 1;
    [SerializeField] Transform snowballParent;
    [SerializeField] float maxSnowballSize = 3;
    [SerializeField] float snowballSizeRate = 0.5f;
    float throwForce;
    [SerializeField] float throwMinForce = 50;
    [SerializeField] float throwForceIncreaseRate = 15;
    [SerializeField] GameObject snowballPrefab;

    [Header("Player Data")]
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] PlayerData playerData;
    public float CurrentAttackPower;
    [SerializeField] float attackIncreaseRate = 10;

    private Vector3 pushVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSystem.SetMaxHealth(playerData.MaxHP);
        throwForce = throwMinForce;
        CurrentAttackPower = snowball.gameObject.GetComponent<Snowball>().initialPower;
        gameManager = GameManager.Instance;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameOver || hasWon) return;
        LookAt();
        if (isPushing)
        {
            PushSnowball(); // Only play Push
        }
        else
        {
            Movement(); // Only move when not pushing
        }
    }

    public void LookAt()
    {
        if (input == Vector2.zero) return;

        Vector3 lookDir = new Vector3(input.x, 0, input.y);
        Vector3 targetPos = transform.position + lookDir;
        transform.DOLookAt(targetPos, 0.3f, AxisConstraint.Y);
    }
    
    public void ChangeAnimationState(string newAnimationState)
    {
        if (gameManager.IsGameOver && !hasWon) return;
        if (currentAnimationState == newAnimationState) return;
        animator.CrossFadeInFixedTime(newAnimationState, 0.2f);
        currentAnimationState = newAnimationState;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    void Movement()
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        //Debug.Log("moveDir: " + moveDir);
        Vector3 newPosition = transform.position + moveSpeed * Time.deltaTime * moveDir;
        transform.position = ClampPosition(newPosition);


        if (input != Vector2.zero)
        {
            ChangeAnimationState(Animation_Run);
        }
        else
        {
            ChangeAnimationState(Animation_Idle);
        }
    }
    public void OnPush(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPushing = !isPushing;
            if (!isPushing)
            {
                ThrowSnowball();
                hasStartedPush = false;
                SpawnNewSnowBall();
            }
        }
    }

    void PushSnowball()
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        //Debug.Log("moveDir: " + moveDir);
        Vector3 newPosition = transform.position + (moveSpeed / 2f) * Time.deltaTime * moveDir;
        transform.position = ClampPosition(newPosition);


        if (input != Vector2.zero)
        {
            ChangeAnimationState(Animation_Push);

            if (!hasStartedPush)
            {
                hasStartedPush = true;
                snowball.gameObject.SetActive(true);
                snowball.localScale = Vector3.one * 0.3f;
                Snowball snowballScript = snowball.gameObject.GetComponent<Snowball>();
                snowballScript.SetOwner(this);
            }

            if (snowball.localScale.x < maxSnowballSize)
            {
                float scaleIncrease = snowballSizeRate * Time.deltaTime;
                snowball.localScale += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);
                Rigidbody snowballRb = snowball.GetComponent<Rigidbody>();
                snowballRb.mass += snowballSizeRate * Time.deltaTime;
                throwForce += throwForceIncreaseRate * Time.deltaTime;
                CurrentAttackPower += (attackIncreaseRate * Time.deltaTime);
                Snowball snowballScript = snowball.gameObject.GetComponent<Snowball>();
                snowballScript.CurrentPower += attackIncreaseRate * Time.deltaTime;
                Vector3 newPos = snowball.position;
                newPos.y = snowball.localScale.y / 2;
                snowball.position = newPos;
                

            }
        }
        else
        {
            ChangeAnimationState(Animation_Idle);
        }
    }

    void ThrowSnowball()
    {
        if (snowball == null) return;
        snowball.SetParent(null);

        Rigidbody snowballRb = snowball.GetComponent<Rigidbody>();
        Debug.Log("Thrown");
        Vector3 throwDir = transform.forward;
        snowballRb.AddForce(throwDir * throwForce, ForceMode.Impulse);
        pushVelocity = snowballRb.linearVelocity;

        Destroy(snowball.gameObject, 5);
    }

    public void TakeDamage(float damage)
    {
        if (gameManager.IsGameOver) return;
        healthSystem.TakeDamage(damage);
        Debug.Log("Damage Taken: " +  damage + "\n" + "CurrentHP: " + healthSystem.GetHealth());
        if (healthSystem.GetHealth() <= 0)
        {
            GameEvents.OnDefeat?.Invoke(this);
        }
    }

    public void IncreaseDamage(float value)
    {
        if (gameManager.IsGameOver) return;
        snowball.gameObject.GetComponent<Snowball>().CurrentPower += value;
        Debug.Log("Increased damage: " + value);
    }

    public void HealHP(float value)
    {
        if (gameManager.IsGameOver) return;
        healthSystem.Heal(value);
        Debug.Log("Increased Heal: " + value);
    }

    

    public void IncreaseMoveSpeed(float value, float duration = 20f)
    {
        if (gameManager.IsGameOver) return;
        // Stop any previous boost before applying a new one
        if (speedBoostCoroutine != null)
            StopCoroutine(speedBoostCoroutine);

        speedBoostCoroutine = StartCoroutine(TemporarySpeedBoost(value, duration));
    }

    private IEnumerator TemporarySpeedBoost(float value, float duration)
    {
        originalMoveSpeed = moveSpeed;
        moveSpeed += value;
        Debug.Log("Increased Speed: " + moveSpeed);

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        Debug.Log("Speed reverted to: " + moveSpeed);
    }

    public void SetAsWinner()
    {
        hasWon = true;
        ChangeAnimationState(Animation_Victory);
    }

    void SpawnNewSnowBall()
    {
        GameObject newBall = Instantiate(snowballPrefab, this.transform);
        newBall.transform.localPosition = new Vector3(0, 0.5f, 3);
        newBall.transform.localRotation = Quaternion.identity;
        newBall.SetActive(false);

        snowball = newBall.transform;
        Rigidbody snowballRb = snowball.GetComponent<Rigidbody>();
        snowballRb.mass = minSnowballMass;
        throwForce = throwMinForce;
        Snowball snowballScript = snowball.gameObject.GetComponent<Snowball>();
        snowballScript.CurrentPower = snowball.gameObject.GetComponent<Snowball>().initialPower;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -12f, 12f);
        position.z = Mathf.Clamp(position.z, -4.5f, 4.5f);
        return position;
    }
}

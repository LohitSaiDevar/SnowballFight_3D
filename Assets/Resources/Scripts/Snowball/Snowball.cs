using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] float minForce;
    [SerializeField] float maxForce;
    float impactForce;
    public PlayerController OwnerPlayer {  get; private set; }
    public float initialPower = 10;
    public float CurrentPower { get; set; }

    private void Awake()
    {
        CurrentPower = initialPower;
    }
    public void SetOwner(PlayerController player)
    {
        OwnerPlayer = player;
        Debug.Log("Snowball Owner: " + player.name);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided");
            Rigidbody rb = GetComponent<Rigidbody>();
            impactForce = 0.5f * rb.mass * collision.relativeVelocity.sqrMagnitude;

            if (impactForce >= minForce)
            {
                PlayerController collidedPlayer = collision.gameObject.GetComponent<PlayerController>();
                if (collidedPlayer != null && OwnerPlayer != null)
                {
                    collidedPlayer.TakeDamage(CurrentPower);
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.Snowball_Hit_SFX);
                }
                Destroy(gameObject);
            }
        }
    }
}

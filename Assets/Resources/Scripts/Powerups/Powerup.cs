using UnityEngine;

public enum PowerupType
{
    Increase_Speed,
    Increase_Damage,
    Take_Damage,
    Heal_HP
}

public class Powerup : MonoBehaviour
{
    public PowerupType powerupType;
    [SerializeField] float value;

    public void Apply(PlayerController player)
    {
        switch (powerupType)
        {
            case PowerupType.Increase_Speed:
                player.IncreaseMoveSpeed(value);
                break;

            case PowerupType.Increase_Damage:
                player.IncreaseDamage(value);
                break;
            
            case PowerupType.Take_Damage:
                player.TakeDamage(value);
                break;

            case PowerupType.Heal_HP:
                player.HealHP(value);
                break;
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Snowball"))
        {
            Snowball snowball = collision.gameObject.GetComponent<Snowball>();
            PlayerController player = snowball.OwnerPlayer;
            Apply(player);
        }
    }
}

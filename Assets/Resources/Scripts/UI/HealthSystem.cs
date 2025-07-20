using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Slider easeHealthBar;
    int maxHP;
    float currentHP;
    float lerpSpeed = 0.05f;
    
    public void SetMaxHealth(int _maxHP)
    {
        maxHP = _maxHP;
        currentHP = maxHP;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;
        }

        if (easeHealthBar != null)
        {
            easeHealthBar.maxValue = maxHP;
            easeHealthBar.value = currentHP;
        }
    }

    public float GetHealth()
    {
        return currentHP;
    }

    private void Update()
    {
        if (healthBar != null && healthBar.value != currentHP)
        {
            healthBar.value = currentHP;
        }

        if (easeHealthBar != null && healthBar.value != easeHealthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, currentHP, lerpSpeed);
        }
            
    }
    public void Heal(float healAmount)
    {
        currentHP += healAmount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Max(0, currentHP);
    }
}

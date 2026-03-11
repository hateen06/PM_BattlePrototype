using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHP : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP = 100;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image characterImage;

    private bool isDead;

    private void Start()
    {
        currentHP = maxHP;
        isDead = false;
        UpdateHPText();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }

        UpdateHPText();
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHP += amount;

        if (currentHP > maxHP)
            currentHP = maxHP;

        UpdateHPText();
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;

        if (characterImage != null)
            characterImage.color = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    private void UpdateHPText()
    {
        if (hpText == null)
            return;

        if (isDead)
            hpText.text = "DEAD";
        else
            hpText.text = $"HP {currentHP}/{maxHP}";
    }
}

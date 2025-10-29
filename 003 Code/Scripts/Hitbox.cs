using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damageMultiplier = 1f;

    private IDamageable parentDamageable;

    void Awake()
    {
        parentDamageable = GetComponentInParent<IDamageable>();
    }

    public void ApplyDamage(float baseDamage)
    {
        if (parentDamageable != null)
        {
            float finalDamage = baseDamage * damageMultiplier;
            parentDamageable.TakeDamage(finalDamage);
            Debug.Log(parentDamageable.hp);
        }
    }
}
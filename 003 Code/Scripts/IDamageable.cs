using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float hp { get; }
    public void TakeDamage(float damage);
}

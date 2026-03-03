using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityStatsModifier
{
    #region Methods
    public void DamageReceiver(Weapon weapon,Attack attack);
    public void OnTakeDamage(int damage);
    public IEnumerator Die();
    public void OnHeal(int heal);

    #endregion
}

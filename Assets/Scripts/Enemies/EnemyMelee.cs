using UnityEngine;
using Damnbro.Player;

namespace Damnbro.Enemies
{
    public class EnemyMelee : EnemyBase
    {
        public float meleeDamage = 15f;

        protected override void Attack()
        {
            if (_target == null) return;
            var hp = _target.GetComponentInParent<HealthSystem>();
            hp?.ApplyDamage(meleeDamage, gameObject);
        }
    }
}

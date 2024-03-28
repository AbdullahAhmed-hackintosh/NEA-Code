using UnityEngine;

namespace FistOfTheFree.Guns.Demo.Enemy
{
    [DisallowMultipleComponent]
    // class to control enemy health
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private int _Health; 
        [SerializeField]
        private int _MaxHealth = 100;
        public int CurrentHealth { get => _Health; private set => _Health = value; } // current health initially equals _health
        public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; } // Max health always equals _MaxHealth

        public event IDamageable.TakeDamageEvent OnTakeDamage;
        public event IDamageable.DeathEvent OnDeath; 

        // Enemies start with full health
        private void OnEnable()
        {
            _Health = MaxHealth;
        }

    // When enemy takes damage, current health is reduced, as long as damage taken isnt 0.
        public void TakeDamage(int Damage)
        {
            int damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);

            CurrentHealth -= damageTaken;

            if (damageTaken != 0)
            {
                OnTakeDamage?.Invoke(damageTaken);
            }
        // If enemies current health = 0 and damage taken is > 0, death occurs.
            if (CurrentHealth == 0 && damageTaken != 0)
            {
                OnDeath?.Invoke(transform.position);
            }
        }
    }
}
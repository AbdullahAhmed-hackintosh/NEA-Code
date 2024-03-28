using UnityEngine;

namespace FistOfTheFree.Guns
{
    public interface IDamageable
    {
        public int CurrentHealth { get; } // Current Healh of Enemy
        public int MaxHealth { get; } // Max Health of Enemy

        public delegate void TakeDamageEvent(int Damage); // Event for when damage is taken
        public event TakeDamageEvent OnTakeDamage; // Event is triggered when damage occurs

        public delegate void DeathEvent(Vector3 Position); // Event of when an enemy (object) dies
        public event DeathEvent OnDeath; // Event Triggered when Enemy Dies

        public void TakeDamage(int Damage); // Funtion for how enemy takes damage
    }
}

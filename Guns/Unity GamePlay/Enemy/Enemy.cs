using UnityEngine;

namespace FistOfTheFree.Guns.Demo.Enemy
{
    [DisallowMultipleComponent]
    // class for enemy information
    public class Enemy : MonoBehaviour
    {
        public EnemyHealth Health; // enemy health
        public EnemyMovement Movement; // enemy movement
        public EnemyPainResponse PainResponse; // enemies' response to pain

        private void Start()
        {
            Health.OnTakeDamage += PainResponse.HandlePain; // enemy shall handle the pain it hath receieved 
            Health.OnDeath += Die; //enemy dies when there's too much damage
        }


        // when enemy passes away, movement stops and HandleDeath() is called
        private void Die(Vector3 Position)
        {
            Movement.StopMoving();
            PainResponse.HandleDeath();
        }
    }
}
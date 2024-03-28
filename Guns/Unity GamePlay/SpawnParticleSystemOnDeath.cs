using UnityEngine;

namespace FistOfTheFree.Guns.Demo
// class which spawns a cool effect when something attached to it gets destroyed
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IDamageable))]
    public class SpawnParticleSystemOnDeath : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem DeathSystem; // gets a particle system
        public IDamageable Damageable; 

        private void Awake()
        {
            Damageable = GetComponent<IDamageable>();
        }

        private void OnEnable()
        {
            Damageable.OnDeath += Damageable_OnDeath;
        }
      // when death of the object this is attached to occurs
      // Animation is played
        private void Damageable_OnDeath(Vector3 Position)
        {
            Instantiate(DeathSystem, Position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
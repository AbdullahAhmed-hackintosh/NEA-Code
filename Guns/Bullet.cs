using System.Collections;
using UnityEngine;

namespace FistOfTheFree.Guns
{
    //public class for bullet
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody Rigidbody;
        [field: SerializeField]
        public Vector3 SpawnLocation // gets spawn location of bullet
        {
            get; private set;
        }

        public delegate void CollisionEvent(Bullet Bullet, Collision Collision);
        public event CollisionEvent OnCollsion;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>(); 
        }

// spawn the actual bullet!
        public void Spawn(Vector3 SpawnForce)
        {
            SpawnLocation = transform.position;
            transform.forward = SpawnForce.normalized;
            Rigidbody.AddForce(SpawnForce);
            StartCoroutine(DelayedDisable(2));
        }

        private IEnumerator DelayedDisable(float Time)
        {
            yield return new WaitForSeconds(Time);
            OnCollisionEnter(null);
        }

// when collision occurs invoke it
        private void OnCollisionEnter(Collision collision) 
        {
            OnCollsion?.Invoke(this, collision);
        }

        private void OnDisable() // when bullet hits something, velocity and angular velocity = 0
        {
            StopAllCoroutines();
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            OnCollsion = null;
        }
    }
}
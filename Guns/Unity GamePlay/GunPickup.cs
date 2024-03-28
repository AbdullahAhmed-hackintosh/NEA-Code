using UnityEngine;

namespace FistOfTheFree.Guns.Demo
{
    // Class for Gun Pickup
    [RequireComponent(typeof(Collider))]
    public class GunPickup : MonoBehaviour
    {
        public GunScriptableObject Gun; // Has the gun
        public Vector3 SpinDirection = Vector3.up; // Animation spinning effect

        private void Update()
        {
            transform.Rotate(Vector3.up);
        }
        // when player collides with the gun, they can pick the gun up
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerGunSelector gunSelector))
            {
                gunSelector.PickupGun(Gun);
                Destroy(gameObject);
            }
        }
    }
}

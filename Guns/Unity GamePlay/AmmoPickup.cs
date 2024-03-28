using UnityEngine;

namespace FistOfTheFree.Guns.Demo
// Class for ammo pickup
{
    [RequireComponent(typeof(Collider))]
    public class AmmoPickup : MonoBehaviour
    {
        public int AmmoAmount = 30; // How much per ammo pickup
        public GunType Type; // What gun this is compatible for
        public Vector3 SpinDirection = Vector3.up; // spinning for viusal effect

        private void Update()
        {
            transform.Rotate(SpinDirection);
        }
 // when triggered leads to Gun's ammo being modified and this ammo being added, and the pickup being destroyed.
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerGunSelector gunSelector) && gunSelector.ActiveGun.Type == Type)
            {
                gunSelector.ActiveGun.AmmoConfig.AddAmmo(AmmoAmount);
                Destroy(gameObject);
            }
        }
    }
}

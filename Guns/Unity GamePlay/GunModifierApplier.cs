using FistOfTheFree.Guns.ImpactEffects;
using FistOfTheFree.Guns.Modifiers;
using FistOfTheFree.ImpactSystem;
using UnityEngine;

namespace FistOfTheFree.Guns.Demo
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField]
        private ImpactType ImpactType; // looks at impact type
        [SerializeField]
        private PlayerGunSelector GunSelector; // gun selected

        private void Start()
        {
            new ImpactTypeModifier() // applies new Bullet Type to the gun
            {
                Amount = ImpactType
            }.Apply(GunSelector.ActiveGun);

            GunSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[] //equates it to new Collision Handler
            {
                new Frost(    // for frost rounds, it creates a new Animation Curve
                    1,
                    new AnimationCurve() {
                        keys = new Keyframe[] {
                            new Keyframe(0, 1),
                            new Keyframe(1, 0.25f)
                        }
                    },
                    5,
                    10,
                     new AnimationCurve() {
                        keys = new Keyframe[] {
                            new Keyframe(0, 0.5f),
                            new Keyframe(1.75f, 0.5f),
                            new Keyframe(2, 1),
                        }
                    }
                )
            };
        }
    }
}
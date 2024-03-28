using FistOfTheFree.Guns.Modifiers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FistOfTheFree.Guns.Demo
{
    [DisallowMultipleComponent]
    public class PlayerGunSelector : MonoBehaviour
    {
        public Camera Camera;
        [SerializeField]
        private GunType Gun; 
        [SerializeField]
        private Transform GunParent; 
        [SerializeField]
        private List<GunScriptableObject> Guns; // gets list of guns from GunScriptableObject
        [SerializeField]
        private PlayerIK InverseKinematics; // loads in player IK

        [Space]
        [Header("Runtime Filled")]
        public GunScriptableObject ActiveGun;
        private GunScriptableObject ActiveBaseGun;
        // ensures gun exists in GunScriptableObject and assigns that as the main gun.
        private void Awake()
        {
            GunScriptableObject gun = Guns.Find(gun => gun.Type == Gun); 

            if (gun == null)
            {
                Debug.LogError($"No GunScriptableObject found for GunType: {gun}");
                return;
            }

            ActiveBaseGun = gun;
            SetupGun(ActiveBaseGun);
        }

        // make sthe gun an activegun.
        private void SetupGun(GunScriptableObject Gun)
        {
            ActiveGun = Gun.Clone() as GunScriptableObject;
            ActiveGun.Spawn(GunParent, this, Camera);

            // some magic for IK
            DoIKMagic();
        }


        //IK Magic!

        private void DoIKMagic()
        {
            Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
            InverseKinematics.LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
            InverseKinematics.RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
            InverseKinematics.LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
            InverseKinematics.RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        }


       // function to destroy a gun
        public void DespawnActiveGun()
        {
            ActiveGun.Despawn();
            Destroy(ActiveGun);
        }
        // function to pickup a gun, by first destroying the current gun, then picking up the new gun found.
        public void PickupGun(GunScriptableObject Gun)
        {
            DespawnActiveGun();
            ActiveBaseGun = Gun;
            SetupGun(ActiveBaseGun);
        }

        // applies modifiers where relevent
        public void ApplyModifiers(IModifier[] Modifiers)
        {
            DespawnActiveGun();
            SetupGun(ActiveBaseGun);

            foreach (IModifier modifier in Modifiers)
            {
                modifier.Apply(ActiveGun);    
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FistOfTheFree.Guns.Demo
{
    //Class to control animation based reloading
    [DisallowMultipleComponent]
    public class PlayerAction : MonoBehaviour
    {
        public PlayerGunSelector GunSelector;
        [SerializeField]
        private bool AutoReload = false; // Automatic Reloading boolean
        [SerializeField]
        private PlayerIK InverseKinematics; // Attaches to the Player IK
        [SerializeField]
        private Animator PlayerAnimator; // Attaches the IK's Animator
        [SerializeField]
        private Image Crosshair; // Crosshair for better aim
        private bool IsReloading; // Boolean for whether a gun in reloading

    
        private void Update()
        {
            //checks whether the gun is already reloading, and whether the player is still shooting
            GunSelector.ActiveGun.Tick(
                !IsReloading
                && Application.isFocused && Mouse.current.leftButton.isPressed
                && GunSelector.ActiveGun != null
            );

            // checks whether auto reload is enabled/not enabled, and starts the reload process
            if (ShouldManualReload() || ShouldAutoReload())
            {
                GunSelector.ActiveGun.StartReloading();
                IsReloading = true;
                PlayerAnimator.SetTrigger("Reload");
                InverseKinematics.HandIKAmount = 0.25f; // IK magic using animations
                InverseKinematics.ElbowIKAmount = 0.25f;
            }
        }



// allow/disallow manual reloading
        private bool ShouldManualReload()
        {
            return !IsReloading
                && Keyboard.current.rKey.wasReleasedThisFrame
                && GunSelector.ActiveGun.CanReload();
        }

// allow/disallow automatic reloading
        private bool ShouldAutoReload()
        {
            return !IsReloading
                && AutoReload
                && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
                && GunSelector.ActiveGun.CanReload();
        }

// end reload animation, once the gun has ended reloading
        private void EndReload()
        {
            GunSelector.ActiveGun.EndReload();
            InverseKinematics.HandIKAmount = 1f;
            InverseKinematics.ElbowIKAmount = 1f;
            IsReloading = false;
        }
    }
}
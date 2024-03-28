using UnityEngine;

namespace FistOfTheFree.Guns
{
    // ScriptableObject for defining ammo configuration
    [CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
    public class AmmoConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public int MaxAmmo = 120; // Max amount of bullets available to a gun is 120
        public int ClipSize = 30;  // Number of bullets in a clip

        public int CurrentAmmo = 120;  // Current total ammo count
        public int CurrentClipAmmo = 30; // Current ammo count in the clip

        public void Reload()  // Reloading Algorithm which conserves ammo.
        {
            int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo); 
            int availableBulletsInCurrentClip = ClipSize - CurrentClipAmmo;
            int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);
            CurrentClipAmmo += reloadAmount;
            CurrentAmmo -= reloadAmount;
        }


        public bool CanReload() // Check if reloading is possible
        {
            return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
        }

        public void AddAmmo(int Amount) // Add ammo to the current ammo count
        {
            if (CurrentAmmo + Amount > MaxAmmo)
            {
                CurrentAmmo = MaxAmmo;
            }
            else
            {
                CurrentAmmo += Amount;
            }
        }

        public object Clone() // Clones the current instance of the Ammo Scriptable Object
        {
            AmmoConfigScriptableObject config = CreateInstance<AmmoConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}
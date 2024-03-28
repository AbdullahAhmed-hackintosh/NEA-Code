using UnityEngine;
using System.Linq;

namespace FistOfTheFree.Guns
{
    // Scriptable Object for Shoot Configuration
    [CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
    public class ShootConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public bool IsHitscan = true; // Boolean to show if it is a hitscan gun or not
        public Bullet BulletPrefab; // Prefab Asset to simulate the bullet being shot from the gun
        public float BulletSpawnForce = 100; // The force that is initially comes out of the weapon
        public LayerMask HitMask; // Tells Unity what layers it can and cannot effect
        public float FireRate = 0.25f; // Rate at which the gun can fire, per second
        public BulletSpreadType SpreadType = BulletSpreadType.Simple; // Bullet Spread
        public float RecoilRecoverySpeed = 1f; // Recoil effect and recovery
        public float MaxSpreadTime = 1f; // Spread time
        public float BulletWeight = 0.1f; // Bullet weight, effects other 

        public ShootType ShootType = ShootType.FromGun;

        // Defines Simple Bullet Spread
        [Header("Simple Spread")]
        public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
        [Header("Texture-Based Spread")]

        // Multiplier applied to the vector from the center of a gun and a chosen pixel. Smaller values mean less spread is applied.
        [Range(0.001f, 5f)]
        public float SpreadMultiplier = 0.1f;
        public Texture2D SpreadTexture;
            // Get the spread based on the Shoot Time
        public Vector3 GetSpread(float ShootTime = 0)
        {
            Vector3 spread = Vector3.zero;

            if (SpreadType == BulletSpreadType.Simple)
            {
                spread = Vector3.Lerp(
                    Vector3.zero,
                    new Vector3(
                        Random.Range(-Spread.x, Spread.x),
                        Random.Range(-Spread.y, Spread.y),
                        Random.Range(-Spread.z, Spread.z)
                    ),
                    Mathf.Clamp01(ShootTime / MaxSpreadTime)
                );
            }
            else if (SpreadType == BulletSpreadType.TextureBased)
            {
                spread = GetTextureDirection(ShootTime);
                spread *= SpreadMultiplier;
            }

            return spread;
        }
        // Reads the SpreadTexture and determines the spread direction
        private Vector2 GetTextureDirection(float ShootTime)
        {
            Vector2 halfSize = new Vector2(SpreadTexture.width / 2f, SpreadTexture.height / 2f); 

            int halfSquareExtents = Mathf.CeilToInt(Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(ShootTime / MaxSpreadTime)));

            int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
            int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

            Color[] sampleColors = SpreadTexture.GetPixels(
                minX,
                minY,
                halfSquareExtents * 2,
                halfSquareExtents * 2
            );
            // Converts all colours to greyscale
            float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
            float totalGreyValue = colorsAsGrey.Sum();
            // chooses random number, within the calculated total range
            float grey = Random.Range(0, totalGreyValue);
            int i = 0;
            for (; i < colorsAsGrey.Length; i++)
            {
                grey -= colorsAsGrey[i];
                if (grey <= 0)
                {
                    break;
                }
            }
            // calculates target position based on Index
            int x = minX + i % (halfSquareExtents * 2);
            int y = minY + i / (halfSquareExtents * 2);

            Vector2 targetPosition = new Vector2(x, y);
            // Calculate the direction vector from the center to the target position
            Vector2 direction = (targetPosition - new Vector2(halfSize.x, halfSize.y)) / halfSize.x;

            return direction; // returns direction
        }
        // Clone the current instance of the Shoot Configuration Scriptable Object
        public object Clone()
        {
            ShootConfigScriptableObject config = CreateInstance<ShootConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}
using FistOfTheFree.Guns.ImpactEffects;
using FistOfTheFree.ImpactSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace FistOfTheFree.Guns
// Scriptable objects for guns.
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject, ICloneable
    {
        public ImpactType ImpactType;
        public GunType Type; 
        public string Name;
        public GameObject ModelPrefab; 
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;


    // brings in relevent information, from other scriptable objects
        public DamageConfigScriptableObject DamageConfig; 
        public ShootConfigScriptableObject ShootConfig;
        public AmmoConfigScriptableObject AmmoConfig;
        public TrailConfigScriptableObject TrailConfig;
        public AudioConfigScriptableObject AudioConfig;

        public ICollisionHandler[] BulletImpactEffects = new ICollisionHandler[0];

        private MonoBehaviour ActiveMonoBehaviour;
        private AudioSource ShootingAudioSource; // Audio source played with gun is being shot
        private GameObject Model; // Model of the Gun
        private Camera ActiveCamera; // Which player camera is active
        private float LastShootTime; 
        private float InitialClickTime;
        private float StopShootingTime; 

        private ParticleSystem ShootSystem;
        private ObjectPool<TrailRenderer> TrailPool; // Trail Pool
        private ObjectPool<Bullet> BulletPool; // Bullet Pool
        private bool LastFrameWantedToShoot;

        public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour, Camera Camera = null) // Uses reaycasting method in "ShootConfigScriptableObject.ShootType"
        {
            this.ActiveMonoBehaviour = ActiveMonoBehaviour;

            TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
            if (!ShootConfig.IsHitscan)
            {
                BulletPool = new ObjectPool<Bullet>(CreateBullet);
            }

            Model = Instantiate(ModelPrefab); // Spawns the Gun Model into the scene
            Model.transform.SetParent(Parent, false);
            Model.transform.localPosition = SpawnPoint;
            Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

            ActiveCamera = Camera;

            ShootingAudioSource = Model.GetComponent<AudioSource>();
            ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        }

    
        /// Despawns the active bullet game object, when a bullet is shot, and cleans up pools.
        public void Despawn()
        {
            Model.gameObject.SetActive(false);
            Destroy(Model);
            TrailPool.Clear();
            if (BulletPool != null)
            {
                BulletPool.Clear();
            }

            ShootingAudioSource = null;
            ShootSystem = null;
        }


        public void UpdateCamera(Camera Camera)
        {
            ActiveCamera = Camera;
        }

        public void Tick(bool WantsToShoot)
        {
            Model.transform.localRotation = Quaternion.Lerp(
                Model.transform.localRotation,
                Quaternion.Euler(SpawnRotation),
                Time.deltaTime * ShootConfig.RecoilRecoverySpeed
            );

            if (WantsToShoot) // Whether or not the player is trying to shoot
            {
                LastFrameWantedToShoot = true;
                TryToShoot();
            }

            if (!WantsToShoot && LastFrameWantedToShoot)
            {
                StopShootingTime = Time.time;
                LastFrameWantedToShoot = false;
            }
        }

        /// Plays the reloading audio clip if assigned.
        public void StartReloading()
        {
            AudioConfig.PlayReloadClip(ShootingAudioSource);
        }

        // Handle ammo mathamtics after the IK does the reload animation.
        public void EndReload()
        {
            AmmoConfig.Reload();
        }

  
        // Whether or not this gun can be reloaded
        public bool CanReload()
        {
            return AmmoConfig.CanReload();
        }

 
        // Performs the shooting raycast  based on gun rate of fire. Also applies bullet spread and plays sound effects based on the "AudioConfigScriptableObject"
        private void TryToShoot()
        {
            if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
            {
                float lastDuration = Mathf.Clamp(
                    0,
                    (StopShootingTime - InitialClickTime),
                    ShootConfig.MaxSpreadTime
                );
                float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTime))
                    / ShootConfig.RecoilRecoverySpeed;

                InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            }

            if (Time.time > ShootConfig.FireRate + LastShootTime)
            {
                LastShootTime = Time.time;
                if (AmmoConfig.CurrentClipAmmo == 0)
                {
                    AudioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
                    return;
                }

                ShootSystem.Play();
                AudioConfig.PlayShootingClip(ShootingAudioSource, AmmoConfig.CurrentClipAmmo == 1);

                Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);

                Vector3 shootDirection = Vector3.zero;
                Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
                if (ShootConfig.ShootType == ShootType.FromGun)
                {
                    shootDirection = ShootSystem.transform.forward;
                }
                else
                {
                    shootDirection = ActiveCamera.transform.forward + ActiveCamera.transform.TransformDirection(spreadAmount);
                }

                AmmoConfig.CurrentClipAmmo--;

                if (ShootConfig.IsHitscan)
                {
                    DoHitscanShoot(shootDirection);
                }
                else
                {
                    DoProjectileShoot(shootDirection);
                }
            }
        }

        // Generates a live Bullet instance that is launched in the ShootDirection
        // with velocity, calclated in "ShootConfigScriptableObject.BulletSpawnForce"

        private void DoProjectileShoot(Vector3 ShootDirection)
        {
            Bullet bullet = BulletPool.Get();
            bullet.gameObject.SetActive(true);
            bullet.OnCollsion += HandleBulletCollision;

            // We have to ensure if shooting from the camera, but shooting real projectiles, that we aim the gun at the hit point
            // of the raycast from the camera.
            if (ShootConfig.ShootType == ShootType.FromCamera
                && Physics.Raycast(
                    GetRaycastOrigin(),
                    ShootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
            {
                Vector3 directionToHit = (hit.point - ShootSystem.transform.position).normalized;
                Model.transform.forward = directionToHit;
                ShootDirection = directionToHit;
            }

            bullet.transform.position = ShootSystem.transform.position;
            bullet.Spawn(ShootDirection * ShootConfig.BulletSpawnForce);

            TrailRenderer trail = TrailPool.Get();
            if (trail != null)
            {
                trail.transform.SetParent(bullet.transform, false);
                trail.transform.localPosition = Vector3.zero;
                trail.emitting = true;
                trail.gameObject.SetActive(true);
            }
        }

        // Calculates a Raycast to determine if a shot hits something by Spawning a TrailRenderer
        // and will apply impact effects and damage after the TrailRenderer simulates moving to the hit point. 
        private void DoHitscanShoot(Vector3 ShootDirection)
        {
            if (Physics.Raycast(
                    GetRaycastOrigin(),
                    ShootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                ))
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        hit.point,
                        hit
                    )
                );
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        ShootSystem.transform.position,
                        ShootSystem.transform.position + (ShootDirection * TrailConfig.MissDistance),
                        new RaycastHit()
                    )
                );
            }
        }

        // Returns the Origin point for raycasting based on "ShootConfigScriptableObject.ShootType

        public Vector3 GetRaycastOrigin()
        {
            Vector3 origin = ShootSystem.transform.position;

            if (ShootConfig.ShootType == ShootType.FromCamera)
            {
                origin = ActiveCamera.transform.position
                    + ActiveCamera.transform.forward * Vector3.Distance(
                            ActiveCamera.transform.position,
                            ShootSystem.transform.position
                        );
            }

            return origin;
        }

        // Returns the spawned gun model
        public Vector3 GetGunForward()
        {
            return Model.transform.forward;
        }

        // Plays a bullet trail from start to end point.  
        private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
        {
            TrailRenderer instance = TrailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = StartPoint;
            yield return null;

            instance.emitting = true;

            float distance = Vector3.Distance(StartPoint, EndPoint); // StartPoint: Starting point for the trail // EndPoint: Ending point for the trail
            float remainingDistance = distance;
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(
                    StartPoint,
                    EndPoint,
                    Mathf.Clamp01(1 - (remainingDistance / distance))
                );
                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = EndPoint;

            if (Hit.collider != null) // The hit object: If nothing is hit, simply pass new RaycastHit
            {
                HandleBulletImpact(distance, EndPoint, Hit.normal, Hit.collider); // Will also play an Impact, handled by surface manager, if a compatible surface is hit.
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            TrailPool.Release(instance);
        }

        //Disables TrailRenderer, releases the  Bullet from the BulletPool, and applies impact effects 
        //Collision is not null. (i.e the bullet has hit something)
        private void HandleBulletCollision(Bullet Bullet, Collision Collision)
        {
            TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
            if (trail != null)
            {
                trail.transform.SetParent(null, true);
                ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
            }

            Bullet.gameObject.SetActive(false);
            BulletPool.Release(Bullet);

            if (Collision != null)
            {
                ContactPoint contactPoint = Collision.GetContact(0);

                HandleBulletImpact(
                    Vector3.Distance(contactPoint.point, Bullet.SpawnLocation),
                    contactPoint.point,
                    contactPoint.normal,
                    contactPoint.otherCollider
                );
            }
        }


        // Disables the trail renderer based on the time provided by TrailConfigScriptableObject.Duration 
        private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
        {
            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            Trail.emitting = false;
            Trail.gameObject.SetActive(false);
            TrailPool.Release(Trail); // Releases from the Trail Pool as an alternative
        }

        // Uses code made initially in surface manager, in order to create damage on areas of teh map where this is possible.
        private void HandleBulletImpact(
            float DistanceTraveled,
            Vector3 HitLocation,
            Vector3 HitNormal,
            Collider HitCollider)
        {
            SurfaceManager.Instance.HandleImpact(
                    HitCollider.gameObject,
                    HitLocation,
                    HitNormal,
                    ImpactType,
                    0
                );

            if (HitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(DistanceTraveled));
            }

            foreach (ICollisionHandler collisionHandler in BulletImpactEffects)
            {
                collisionHandler.HandleImpact(HitCollider, HitLocation, HitNormal, this);
            }
        }


        // Creates a live trail renderer for the active GameObject, in the object pool.
        private TrailRenderer CreateTrail()
        {
            GameObject instance = new GameObject("Bullet Trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = TrailConfig.Color;
            trail.material = TrailConfig.Material;
            trail.widthCurve = TrailConfig.WidthCurve;
            trail.time = TrailConfig.Duration;
            trail.minVertexDistance = TrailConfig.MinVertexDistance;

            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }


        // Creates a live bullet in the Object Pool
        private Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(ShootConfig.BulletPrefab);
            Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
            rigidbody.mass = ShootConfig.BulletWeight;

            return bullet;
        }

        // Only clones certain variables from base script, as different guns may/may not require a certain value
        // while other guns may not use the base variable value
        public object Clone()
        {
            GunScriptableObject config = CreateInstance<GunScriptableObject>();

            config.ImpactType = ImpactType;
            config.Type = Type;
            config.Name = Name;
            config.name = name;
            config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
            config.ShootConfig = ShootConfig.Clone() as ShootConfigScriptableObject;
            config.AmmoConfig = AmmoConfig.Clone() as AmmoConfigScriptableObject;
            config.TrailConfig = TrailConfig.Clone() as TrailConfigScriptableObject;
            config.AudioConfig = AudioConfig.Clone() as AudioConfigScriptableObject;

            config.ModelPrefab = ModelPrefab;
            config.SpawnPoint = SpawnPoint;
            config.SpawnRotation = SpawnRotation;

            return config;
        }
    }
}
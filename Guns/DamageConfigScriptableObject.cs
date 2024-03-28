using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace FistOfTheFree.Guns
// Scriptable Objects for Damage Curve
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
    public class DamageConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public MinMaxCurve DamageCurve; // Creates a Unity MinMax Curve

        private void Reset()
        {
            DamageCurve.mode = ParticleSystemCurveMode.Curve; 
        }

        // calculates the damage based on a given distance, calculatd to the nearest integer
        public int GetDamage(float Distance = 0)
        {
            return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));
        }

        public object Clone()
        {
            DamageConfigScriptableObject config = CreateInstance<DamageConfigScriptableObject>();

            config.DamageCurve = DamageCurve;
            return config;
        }
    }
}
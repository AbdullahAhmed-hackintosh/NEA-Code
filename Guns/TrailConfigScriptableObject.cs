using System;
using UnityEngine;

namespace FistOfTheFree.Guns
{
// ScriptableObject for defining trail configuration
    [CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Trail Config", order = 4)]
    public class TrailConfigScriptableObject : ScriptableObject, ICloneable
    {
        public Material Material; // Material aeet used for the trail
        public AnimationCurve WidthCurve; // Damage Curve defining the width of the trail
        public float Duration = 0.5f;  // Duration of the trail effect
        public float MinVertexDistance = 0.1f; // Minimum distance between vertices on the graph
        public Gradient Color; // Color gradient of the trail

        public float MissDistance = 100f; // Maximum distance for a shot to miss
        public float SimulationSpeed = 100f; // Speed of the gun shot

        public object Clone() // Clones the current instance of the Trail Configuration Scriptable Object
        {
            TrailConfigScriptableObject config = CreateInstance<TrailConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}
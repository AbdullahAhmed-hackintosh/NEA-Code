using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Llamacademy
{
    [DisallowMultipleComponent]
    public class EnemyReferences : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgent NavMeshagent;
        [HideInInspector] public Animator animator;

        [Header("Stats")]

        public float pathUpdateDelay = 0.2f;

        private void Awake() {
            NavMeshagent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

    }
}
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

        public float pathUpdateDelay = 0.2f; //sets the delay of after how many seconds the path should update.

        private void Awake() {
            NavMeshagent = GetComponent<NavMeshAgent>(); // Wnemy NavMesh Insterted here
            animator = GetComponent<Animator>(); // Enemy Navmesh's Animator inserted
        }

    }
}
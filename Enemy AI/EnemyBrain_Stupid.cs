using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Llamacademy
{
    public class EnemyBrain_Stupid : MonoBehaviour
    {
        public Transform target;

        private EnemyReferences enemyReferences;

        private float shootingDistance;

        private float pathUpdateDeadline;

        private void Awake() {
            enemyReferences = GetComponent<EnemyReferences>();
        }

        void Start() {
            shootingDistance = enemyReferences.NavMeshagent.stoppingDistance;
        }

        void Update() {
            if(target !=null) {

                bool inRange = Vector3.Distance(transform.position, target.position) <= shootingDistance;

                if (inRange) {
                    LookAtTarget();
                } else {
                    UpdatePath();
                }

                enemyReferences.animator.SetBool("shooting", inRange);
            }
            enemyReferences.animator.SetFloat("speed", enemyReferences.NavMeshagent.desiredVelocity.sqrMagnitude);
            
        }

        private void LookAtTarget() {
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
        }

        private void UpdatePath() {

            if ( Time.time >= pathUpdateDeadline ) {
            Debug.Log("Updating Path");
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            enemyReferences.NavMeshagent.SetDestination(target.position);
            }
        }
        
    }
}

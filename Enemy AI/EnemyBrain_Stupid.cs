using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistOfTheFree
{
    public class EnemyBrain_Stupid : MonoBehaviour
    {
        public Transform target; // a transform, that follows the target (player)

        private EnemyReferences enemyReferences; // gets references from enemy references

        private float shootingDistance; // maximum distance from which the AI can shoot

        private float pathUpdateDeadline;

    }

// Awake function: Initializes the enemyReferences variable by getting the EnemyReferences component attached to the GameObject.
private void Awake() {
    enemyReferences = GetComponent<EnemyReferences>();
}

// Start function: Sets the shootingDistance  to the stopping distance of the NavMesh Agent in enemyReferences.
void Start() {
    shootingDistance = enemyReferences.NavMeshagent.stoppingDistance;
}

// Update function: Checks whether the target is null, calculates if the target is within shooting distance, updates the enemy's behavior accordingly, and sets the animator.
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

// LookAtTarget function: Rotates the enemy to look at the target player.
private void LookAtTarget() {
    Vector3 lookPos = target.position - transform.position;
    lookPos.y = 0;
    Quaternion rotation = Quaternion.LookRotation(lookPos);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
}

// UpdatePath function: Updates the enemy's path by setting a new destination if the path update time has been reached.
private void UpdatePath() {
    if (Time.time >= pathUpdateDeadline) {
        Debug.Log("Updating Path");
        pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
        enemyReferences.NavMeshagent.SetDestination(target.position);
    }
}
}
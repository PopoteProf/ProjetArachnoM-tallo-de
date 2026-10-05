using System;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyRedCube : MonoBehaviour {
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _fireRange;
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private LayerMask _fireLayer;
    [SerializeField] private TagHandle _playerTag;
    [SerializeField] private float _firerate = 2;
    [SerializeField] private PopoteTimer _fireTimer;

    public void SetUpPlayerTarget(GameObject target) {
        _playerTarget = target.transform;
    }
    private void Start() {
        _fireTimer = new PopoteTimer(1/_firerate);
        _agent.SetDestination(_playerTarget.position);
    }

    private bool CanFire() {
        if (Vector3.Distance(transform.position, _playerTarget.position) > _fireRange) return false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _playerTarget.position - transform.position, out hit, _fireRange,
                _fireLayer)) {
            if (hit.collider.gameObject.CompareTag(_playerTag)) {
                return true;
            }
        }

        return false;
    }

    private void Update() {
        _fireTimer.UpdateTimer();
        if (CanFire()) {
            _agent.isStopped = true;
            ManageFire();
        }
        else {
            _agent.SetDestination(_playerTarget.position);
            _agent.isStopped = false;
        }
    }

    private void ManageFire()
    {
        
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position ,_fireRange);
    }
}

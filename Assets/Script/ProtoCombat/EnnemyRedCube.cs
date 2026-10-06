using UnityEngine;
using UnityEngine.AI;

public class EnnemyRedCube : MonoBehaviour, IDamable {
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _fireRange;
    [SerializeField] private Transform _playerTarget;
    //[SerializeField] private LayerMask _fireLayer;
    [SerializeField] private string _playerTag;
    [SerializeField] private int _health =4;
    [Header("Attack")] 
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _prfProjectile;
    [SerializeField] private float _firerate = 2;
    [SerializeField] private SoProjectileData _projectileData;
    [SerializeField] private float _bulletSpeed =25f;
    
    private PopoteTimer _fireTimer;
    

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
                _projectileData.LayerMask)) {
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
            transform.forward = _playerTarget.position - transform.position;
            ManageFire();
        }
        else {
            _agent.SetDestination(_playerTarget.position);
            _agent.isStopped = false;
        }
    }

    private void ManageFire()
    {
        if (_fireTimer.IsPlaying) return;
        Projectile bullet =Instantiate(_prfProjectile,_firePoint.position,_firePoint.rotation);
        bullet.Setup(_projectileData);
        bullet.Rigidbody.AddForce(transform.forward * _bulletSpeed,ForceMode.Impulse);
        _fireTimer.Play();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position ,_fireRange);
    }

    public void TakeDamage(int damage) {
        _health -= damage;
        if (_health <= 0) {
            Destroy(gameObject);
        }
    }
}
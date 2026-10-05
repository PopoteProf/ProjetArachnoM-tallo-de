using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    public Rigidbody Rigidbody;
    private Vector3 _lastpos;
    private SoProjectileData _projectileData;

    public void Setup(SoProjectileData data) {
        _lastpos = transform.position;
        _projectileData = data;
    }

    private void Update() {
        RaycastHit _rayHit;
        Vector3 rayDir = transform.position-_lastpos;
        Ray ray = new Ray(_lastpos, rayDir);
        if (Physics.Raycast(ray, out _rayHit, rayDir.magnitude, _projectileData.LayerMask)) {
            if (_rayHit.collider.GetComponent<IDamable>() != null) {
                _rayHit.collider.GetComponent<IDamable>().TakeDamage(_projectileData.Damage);
            }
            Destroy(gameObject);
        }

        transform.forward = rayDir;
        _lastpos = transform.position;
    }
}
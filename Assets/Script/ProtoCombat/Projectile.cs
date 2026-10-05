using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    public Rigidbody Rigidbody;
    private Vector3 _lastpos;

    public void Setup() {
        _lastpos = transform.position;
    }

    private void Update() {
        RaycastHit _rayHit;
        Vector3 rayDir = transform.position-_lastpos;
        Ray ray = new Ray(_lastpos, rayDir);
        if (Physics.Raycast(ray, out _rayHit, rayDir.magnitude)) {
            Destroy(gameObject);
        }

        transform.forward = rayDir;
        _lastpos = transform.position;
    }
}
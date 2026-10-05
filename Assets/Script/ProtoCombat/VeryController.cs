using System;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class VeryController : MonoBehaviour, IDamable
{
    
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDidstance = 1.5f;
    [SerializeField] private float _rideHeight;
    [SerializeField] private float _rideSpringStrength = 1;
    [SerializeField] private float _rideSpringDamper = 1;
    [SerializeField] private float _upRightJointSpringStrenght = 10;
    [SerializeField] private float _upRightJointSprintDamper = 1;

    [Space(10), Header("Locomotion")]
    [SerializeField] private float MaxSpeed= 8;
    [SerializeField] private float Acceleration= 200;
    [SerializeField] private AnimationCurve AccelerationFactorFromDot =  AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private float MaxAccelForce = 150;

    [SerializeField] private float SlowDownFactor = 1.5f;
    [SerializeField] private AnimationCurve MaxAccelerationForceFactorFromDot =  AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private Vector3 ForceScale = new Vector3(1, 0, 1);
    [SerializeField] private float GravityScaleDrop = 10f;

    [SerializeField] private SoProjectileData _projectileData;
    [SerializeField] private Transform _pos1;
    [SerializeField] private Transform _pos2;
    [SerializeField] private float _bulletForce=50;
    [SerializeField] private Projectile _prfProjectile;
    [SerializeField] private float _fireRate=0.5f;
    [SerializeField] private float _recoilPower = 10;
    [SerializeField] private Transform _transformGuide;
    
    private PopoteTimer _fireTimer;
    private bool _fireCanon1;
    private bool _isFireInputDown;

    private InputAction _inputActionMove;
    private InputAction _inputActionAttack;
    private InputAction _inputActionLook;

    private Vector3 m_UnitGoal;
    private Vector3 m_goalVel;
    private Vector3 _targetLookDirection;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _inputActionMove =InputSystem.actions.FindAction("Move");
        _inputActionAttack = InputSystem.actions.FindAction("Attack");
        
        _inputActionLook = InputSystem.actions.FindAction("LookPad");
        _inputActionLook.performed+= InputActionLookOnperformed;
        _inputActionLook.started+= InputActionLookOnstarted;
        _inputActionLook.canceled+= InputActionLookOncanceled;
        _fireTimer= new PopoteTimer(_fireRate);
    }

    private void InputActionLookOncanceled(InputAction.CallbackContext obj) =>_isFireInputDown = false;

    private void InputActionLookOnstarted(InputAction.CallbackContext obj) => _isFireInputDown = true;
    

    private void InputActionLookOnperformed(InputAction.CallbackContext obj)
    {
        Vector2 rawIput = _inputActionLook.ReadValue<Vector2>();
        if (rawIput.magnitude < 0.2f) return;
        Vector3 lookdir = new Vector3(rawIput.x,0,rawIput.y);
        _targetLookDirection = lookdir;
        Debug.DrawRay(transform.position, lookdir, Color.red);
        
    }

    private void ManageFire()
    {
        if( _fireTimer.IsPlaying)return;
        if (_fireCanon1) {
            Projectile bullet =Instantiate(_prfProjectile,_pos1.position,_pos1.rotation);
            bullet.Setup(_projectileData);
            bullet.Rigidbody.AddForce(transform.forward * _bulletForce,ForceMode.Impulse);
            _rigidbody.AddForceAtPosition( -transform.forward*_recoilPower,_pos1.position,ForceMode.Impulse);
            _fireCanon1 = false;
            _fireTimer.Play();
            return;
        }
        else
        {
            Projectile bullet =Instantiate(_prfProjectile,_pos2.position,_pos2.rotation);
            bullet.Setup(_projectileData);
            bullet.Rigidbody.AddForce(transform.forward * _bulletForce,ForceMode.Impulse );
            _rigidbody.AddForceAtPosition( -transform.forward*_recoilPower,_pos2.position,ForceMode.Impulse);
            _fireTimer.Play();
            _fireCanon1 = true;
            
        }
        //if(Random.Range(0f,1f)<0.5f)
        //{
        //    Vector3 dir = transform.position - _pos1.position;
        //    dir.y=0;
        //    _rigidbody.AddForceAtPosition(dir*_pos1Force, _pos1.position);
        //}
        //else
        //{
        //    Vector3 dir = transform.position - _pos2.position;
        //    dir.y=0;
        //    _rigidbody.AddForceAtPosition(dir*_pos2Force, _pos2.position);
        //}
    }

    private void Update() {
        ManagerMouvement();
        UpdateGuideDirection();
        _fireTimer.UpdateTimer();
        if( _isFireInputDown) ManageFire();
    }

    private void FixedUpdate() {
        ManageUpPosition();
        UpdateUprightForce();
    }

    private void ManageUpPosition() {
        RaycastHit _rayHit;
        if (Physics.Raycast(transform.position, Vector3.down, out _rayHit, _groundCheckDidstance, _groundLayer))
        {
            Vector3 vel =  _rigidbody.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(-transform.up);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitbody = _rayHit.rigidbody;
            if (hitbody != null) {
                otherVel = hitbody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVer = Vector3.Dot(rayDir, otherVel);
            float relVel = rayDirVel - otherDirVer;

            float x = _rayHit.distance - _rideHeight;

            float springForce = (x * _rideSpringStrength) - (relVel * _rideSpringDamper);
            
            _rigidbody.AddForce (rayDir * springForce);
        }
    }

    private void UpdateUprightForce() {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = FromToQuaternion(Quaternion.LookRotation(_targetLookDirection, Vector3.up), characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;
        
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();
        rotDegrees = 360-rotDegrees;
        //Debug.Log( "Degree a compencer ="+ rotDegrees);
        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        //Debug.Log( rotRadians);
        _rigidbody.AddTorque(rotAxis* (rotRadians * _upRightJointSpringStrenght)-(_rigidbody.angularVelocity*_upRightJointSprintDamper));
    }

    private Quaternion FromToQuaternion(Quaternion from, Quaternion to) {
        return to *Quaternion.Inverse(from);
    }

    private void ManagerMouvement()
    {
        Vector2 Input =_inputActionMove.ReadValue<Vector2>();
        Vector3 move= new Vector3(Input.x, 0, Input.y); 
        
        if( move.magnitude>1) move.Normalize();
        
        //
        // Calculate new goal Vel...
        m_UnitGoal = move;
        Vector3 unitVel = m_UnitGoal.normalized;
        
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);
        
        
        //Debug.Log("Accel ="+  accel+ "        Vel Dot = "+ velDot);
        //_rigidbody.AddForce(unitVel * accel*speedFactor);
        //if (m_UnitGoal.magnitude < 0.1f)
        //{
        //    _rigidbody.linearVelocity = Vector3.Lerp( Vector3.zero,_rigidbody.linearVelocity, SlowDownFactor*Time.deltaTime);
        //}
        
        Vector3 goalVel = m_UnitGoal * MaxSpeed;
        
        m_goalVel = Vector3.MoveTowards(m_goalVel, goalVel, accel * Time.deltaTime);
        
        
        Vector3 neededAccel = (m_goalVel - _rigidbody.linearVelocity);
        float maxAccel = MaxAccelForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot); 
        Debug.Log("m_goalVel =    "+m_goalVel+"      _rigidbody vel"+ _rigidbody.linearVelocity.magnitude);
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        _rigidbody.AddForce(Vector3.Scale(neededAccel*_rigidbody.mass, ForceScale));
    }

    public void TakeDamage(int damage) {
        StaticData.ChangeHealth(-damage);
    }

    private void UpdateGuideDirection() {
        _transformGuide.up = StaticData.EndLevelPosition -transform.position;
    }
}
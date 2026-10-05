using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnnemySpawner : MonoBehaviour {
    [SerializeField] private float _spawningRange = 40;
    [SerializeField] private Vector3 _spawOffSet;
    [SerializeField] private GameObject _playerTarget;
    [SerializeField] private EnnemyRedCube _prfEnnemyRedCube;
    [SerializeField] private int _minInicialSpawn =3;
    [SerializeField] private int _maxInicialSpawn =6;
    [SerializeField] private float _inRangeSpawnRate = 0.5f;

    private PopoteTimer _spawnTimer;
    private bool _isPlayerInRange;

    public void SetUpPlayerTarget(GameObject target) {
        _playerTarget = target;
    }

    private void Start()
    {
        _spawnTimer = new PopoteTimer(1 / _inRangeSpawnRate);
    }

    private void SpwanEnnemyCube() {
        EnnemyRedCube ennemyRedCube = Instantiate( _prfEnnemyRedCube, transform.position+_spawOffSet, transform.rotation);
        ennemyRedCube.SetUpPlayerTarget(_playerTarget);
    }

    private void Update() {
        _spawnTimer.UpdateTimer();
        if (Vector3.Distance(transform.position, _playerTarget.transform.position) > _spawningRange) {
            _isPlayerInRange = false;
        }
        else {
            if (!_isPlayerInRange) SpawnInitialEnnemies();

            if (!_spawnTimer.IsPlaying) {
                _spawnTimer.Play();
                SpwanEnnemyCube();
            }
            _isPlayerInRange = true;
        }
    }

    private void SpawnInitialEnnemies() {
        int count = Random.Range(_maxInicialSpawn, _maxInicialSpawn);
        for (int i = 0; i < count; i++) {
            SpwanEnnemyCube();
        }
    }
    

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawningRange);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "newSoProjectileData", menuName = "SO/SoProjectileData")]
public class SoProjectileData : ScriptableObject {
    public LayerMask LayerMask;
    public int Damage;
    public int Force;
}
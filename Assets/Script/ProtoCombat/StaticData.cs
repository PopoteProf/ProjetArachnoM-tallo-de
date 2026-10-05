using System;
using UnityEngine;

public static class StaticData {
    public static event EventHandler OnHealthChange;
    public static event EventHandler OnDeath;
    public static event EventHandler OnWin;
    
    public static int Health = 20;
    public static int MaxHealth = 20;
    public static Vector3 EndLevelPosition;

    public static void ChangeHealth(int dif) {
        Health =Mathf.Clamp(Health+dif,0,MaxHealth);
        OnHealthChange?.Invoke(null, null);

        if (Health == 0) {
            OnDeath?.Invoke(null, null);
        }
    }

    public static float GetNormalizeHeath() {
        return (float)Health / MaxHealth;
    }

    public static void DoWin() {
        OnWin?.Invoke(null, null);
    }
}
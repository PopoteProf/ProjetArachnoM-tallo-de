using System;
using UnityEngine;

public class UIGameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panelGameOver;
    private void Start() {
        StaticData.OnDeath+= StaticDataOnOnDeath;
        _panelGameOver.SetActive(false);
    }

    private void StaticDataOnOnDeath(object sender, EventArgs e)
    {
        _panelGameOver.SetActive(true);
    }

    private void OnDestroy() {
        StaticData.OnHealthChange-= StaticDataOnOnDeath;
    }
}
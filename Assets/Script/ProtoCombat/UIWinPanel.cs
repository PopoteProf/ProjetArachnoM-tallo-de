using System;
using UnityEngine;

public class UIWinPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panelGameOver;
    private void Start() {
        StaticData.OnWin+= StaticDataOnWin;
        _panelGameOver.SetActive(false);
    }

    private void StaticDataOnWin(object sender, EventArgs e)
    {
        _panelGameOver.SetActive(true);
    }

    private void OnDestroy() {
        StaticData.OnWin-= StaticDataOnWin;
    }
}
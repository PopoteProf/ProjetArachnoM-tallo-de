using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image _imgBar;

    private void Start() {
        StaticData.OnHealthChange+= StaticDataOnOnHealthChange;
    }

    private void OnDestroy() {
        StaticData.OnHealthChange-= StaticDataOnOnHealthChange;
    }

    private void StaticDataOnOnHealthChange(object sender, EventArgs e) {
        _imgBar.fillAmount = StaticData.GetNormalizeHeath();
    }
}
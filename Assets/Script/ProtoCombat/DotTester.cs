using System;
using TMPro;
using UnityEngine;

public class DotTester : MonoBehaviour
{
    [SerializeField] private Transform _transform1;
    [SerializeField] private Transform _transform2;
    [SerializeField] private TMP_Text _text;

    private void Update() {
        Debug.DrawLine(transform.position, _transform1.position, Color.blue);
        Debug.DrawLine(transform.position, _transform2.position, Color.red);
        _text.text = Vector3.Dot(_transform1.position - transform.position, _transform2.position - transform.position)
            .ToString();
    }

    private void OnDrawGizmos()
    {
        if( _transform1 == null )return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Vector3.Distance(transform.position, _transform1.position));
    }
}
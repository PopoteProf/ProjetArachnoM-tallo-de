using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileTesterButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;


    public TileConnectionData.ConnectionType ConnectionType {
        get => _connectionType;
        set {
            _connectionType = value;
            _text.text = _connectionType.ToString();
            switch (_connectionType) {
                case TileConnectionData.ConnectionType.None:_button.image.color = new Color(0.5f, 0.5f, 0.5f, 1);
                    break;
                case TileConnectionData.ConnectionType.NoConnection:_button.image.color = Color.brown;
                    break;
                case TileConnectionData.ConnectionType.StandardConnection:_button.image.color = Color.darkGreen;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private TileConnectionData.ConnectionType _connectionType;

    private void Awake()
    {
        _button.onClick.AddListener(ButtonOnclicked);
        ConnectionType = _connectionType;
    }

    private void ButtonOnclicked() {
        ConnectionType = (TileConnectionData.ConnectionType)(((int)_connectionType + 1) % 3);
        
        
    }
}
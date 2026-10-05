using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TileTesterManager: MonoBehaviour {
    [SerializeField] private TileTesterButton _buttonBottom;
    [SerializeField] private TileTesterButton _buttonLeft;
    [SerializeField] private TileTesterButton _buttonTop;
    [SerializeField] private TileTesterButton _buttonRight;
    [SerializeField] private TMP_Text _txtBot;
    [SerializeField] private TMP_Text _txtLeft;
    [SerializeField] private TMP_Text _txtTop;
    [SerializeField] private TMP_Text _txtRight;
    [SerializeField] private Button _buttonRotateLeft;
    [SerializeField] private Button _buttonRotateRight;
    [SerializeField] private Button _pickTile;
    [SerializeField] private SoTilesCollection _soTilesCollection;
    
    
    
    [SerializeField] private TMP_Text _txtDirection;
   
    
    [SerializeField] private DirectionType _directionType;

    private TerrainTile _tile;
    private void Awake() {
        _buttonRotateLeft.onClick.AddListener( RotateLeft);
        _buttonRotateRight.onClick.AddListener(RotateRight);
        _pickTile.onClick.AddListener( PickTile );
        _txtDirection.text = _directionType.ToString();
    }

    private void ClickableOnclicked()
    {
        throw new NotImplementedException();
    }

    private void RotateLeft() {
         _directionType--;
        if((int)_directionType < 0) _directionType = DirectionType.Right;
        if((int)_directionType > 3) _directionType = DirectionType.Bottom;
        _txtDirection.text = _directionType.ToString();
        
        TileConnectionData data = new TileConnectionData(_buttonBottom.ConnectionType, _buttonLeft.ConnectionType,_buttonTop.ConnectionType, _buttonRight.ConnectionType);
        data =data.Rotate( DirectionType.Left);
        _buttonBottom.ConnectionType = data.Bottom;
        _buttonLeft.ConnectionType = data.Left;
        _buttonTop.ConnectionType = data.Top;
        _buttonRight.ConnectionType = data.Right;
        //TileConnectionData data = new TileConnectionData(_buttonBottom.ConnectionType, _buttonLeft.ConnectionType, _buttonTop.ConnectionType, _buttonRight.ConnectionType);
        //data.Rotate()
    }

    private void RotateRight() {
         _directionType++;
        if( (int)_directionType < 0) _directionType = DirectionType.Right;
        if( (int)_directionType > 3) _directionType = DirectionType.Bottom;
        _txtDirection.text = _directionType.ToString();
        
        TileConnectionData data = new TileConnectionData(_buttonBottom.ConnectionType, _buttonLeft.ConnectionType,_buttonTop.ConnectionType, _buttonRight.ConnectionType);
        data =data.Rotate( DirectionType.Right);
        _buttonBottom.ConnectionType = data.Bottom;
        _buttonLeft.ConnectionType = data.Left;
        _buttonTop.ConnectionType = data.Top;
        _buttonRight.ConnectionType = data.Right;
    }

    private void PickTile() {
        if (_tile!=null) Destroy( _tile.gameObject );
        TileConnectionData data = new TileConnectionData(_buttonBottom.ConnectionType, _buttonLeft.ConnectionType,_buttonTop.ConnectionType, _buttonRight.ConnectionType);
        List<ReturnTile> tiles =_soTilesCollection.GetPotencialTiles(data);
        Debug.Log(tiles.Count + " Possible Tiles found");
        ReturnTile selectedTile = tiles[Random.Range(0, tiles.Count)];
        _tile =Instantiate(selectedTile.Tile,Vector3.zero,Quaternion.identity);
        _tile.transform.forward = selectedTile.Direction.GetWorldDirection();

        TileConnectionData returnConnection = selectedTile.GetConnectionDataWithRoration();
        
        _txtBot.text = returnConnection.Bottom.ToString();
        _txtLeft.text = returnConnection.Left.ToString();
        _txtTop.text = returnConnection.Top.ToString();
        _txtRight.text = returnConnection.Right.ToString();
        _txtDirection.text = selectedTile.Direction.ToString();
        

    }

    
}
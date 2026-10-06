using System;
using UnityEngine;

public class TerrainTile : MonoBehaviour
{
    [SerializeField] private TileConnectionData _connectionData;
    //[SerializeField]private TileConnectionData.ConnectionType _connexionTop;
    //[SerializeField]private TileConnectionData.ConnectionType _connexionRight;
    //[SerializeField]private TileConnectionData.ConnectionType _connexionBot;
    //[SerializeField]private TileConnectionData.ConnectionType _connexionLeft;

    public TileConnectionData ConnectionData => _connectionData;
    public enum TileForme {
        none, left, right, opposite
    }
    

    public bool DidFeetRequirement(TileConnectionData connectionData)
    {
        //if (connectionData.Bottom == TileConnectionData.ConnectionType.StandardConnection &&
        //    _connectionData.Bottom != TileConnectionData.ConnectionType.StandardConnection) return false;
        //if (connectionData.Left == TileConnectionData.ConnectionType.StandardConnection &&
        //    _connectionData.Left != TileConnectionData.ConnectionType.StandardConnection) return false;
        //if (connectionData.Top == TileConnectionData.ConnectionType.StandardConnection &&
        //    _connectionData.Top != TileConnectionData.ConnectionType.StandardConnection) return false;
        //if (connectionData.Right == TileConnectionData.ConnectionType.StandardConnection &&
        //    _connectionData.Right != TileConnectionData.ConnectionType.StandardConnection) return false;
        
        if(!connectionData.Top.DidConnectionMatch(_connectionData.Top))return false;
        if(!connectionData.Right.DidConnectionMatch(_connectionData.Right))return false;
        if(!connectionData.Bottom.DidConnectionMatch(_connectionData.Bottom))return false;
        if(!connectionData.Left.DidConnectionMatch(_connectionData.Left))return false;
        return true;
    } 
    
}

public enum DirectionType {
    Bottom,Left, Top, Right
}
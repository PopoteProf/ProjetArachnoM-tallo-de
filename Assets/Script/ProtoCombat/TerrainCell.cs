using System.Collections.Generic;
using UnityEngine;

public class TerrainCell {
    public Vector2Int Coordinates;
    public DebugCell DebugCell;
    public float HeightValue;
    
    public float GCostValue;
    public float HCostValue;
    
    public ReturnTile ReturnTile;
    public DirectionType Direction;
    public TerrainTile.TileForme TileForme;
    public TerrainTile TerrainTile;

    public float FCostValue { get => GCostValue + HCostValue; }
    public TerrainCell CellFrom;

    public TerrainCell(Vector2Int coordinates) {
        Coordinates = coordinates;
    }
    public TileConnectionData GetConnectionDataWithRoration() {
        return TerrainTile.ConnectionData.AntiRotate(Direction);
    }

    public List<Vector2Int> GetCellConnection() {
        TileConnectionData data = ReturnTile.GetConnectionDataWithRoration();
        List<Vector2Int> Directions = new List<Vector2Int>();
        if(data.Bottom == TileConnectionData.ConnectionType.StandardConnection) Directions.Add(Vector2Int.down);
        if(data.Left == TileConnectionData.ConnectionType.StandardConnection) Directions.Add(Vector2Int.left);
        if(data.Top == TileConnectionData.ConnectionType.StandardConnection) Directions.Add(Vector2Int.up);
        if(data.Right == TileConnectionData.ConnectionType.StandardConnection) Directions.Add(Vector2Int.right);
        return Directions;
    }

    public TileConnectionData.ConnectionType GetConnectionTypeFor(DirectionType directionType) {
        if (!ReturnTile.IsSeted) return TileConnectionData.ConnectionType.None;
        return ReturnTile.GetConnectionDataWithRoration().GetConnections[(int)directionType];
    }
}
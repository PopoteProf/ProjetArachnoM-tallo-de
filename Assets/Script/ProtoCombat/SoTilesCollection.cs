using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoTilesCollection", menuName = "SO/SoTileCollection")]
public class SoTilesCollection : ScriptableObject
{
    [SerializeField]private TerrainTile[] _tiles;

    public List<ReturnTile> GetPotencialTiles(TileConnectionData connectionData) {
        List<ReturnTile> returnTile = new List<ReturnTile>();
        foreach (var tile in _tiles) {
            if (tile == null) continue;
            for (int i = 0; i < 4; i++) {
                TileConnectionData rotatedData = connectionData.Rotate((DirectionType)i);
                if (tile.DidFeetRequirement(rotatedData))returnTile.Add(new ReturnTile((DirectionType)i, tile));
            }
            
        }
        return returnTile;
    }
}
public struct ReturnTile {
    public DirectionType Direction;
    public TerrainTile Tile;
    public float ChanceToSpawn;

    public bool IsSeted {
        get => Tile != null;
        
    } 
    public ReturnTile(DirectionType direction, TerrainTile tile, float chanceToSpawn=1) {
        Direction = direction;
        Tile = tile;
        ChanceToSpawn = chanceToSpawn;
    }

    public TileConnectionData GetConnectionDataWithRoration() {
        return Tile.ConnectionData.AntiRotate(Direction);
    }
}
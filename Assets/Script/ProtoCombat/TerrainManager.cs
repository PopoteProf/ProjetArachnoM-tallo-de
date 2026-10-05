using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] private bool _generateTerrainAtSart = true;
    [SerializeField] private NavMeshSurface _navMesh;
    [SerializeField] private Vector2Int _terrainSize = new Vector2Int(5, 5);
    [SerializeField] private DebugCell _prfDebugCell;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _scale =0.5f;
    [SerializeField] private bool _perlinInUpdate;
    [Header("A Star")]
    [SerializeField] private Vector2Int _startPos = new Vector2Int(0, 0);
    [SerializeField] private  Vector2Int _endPos = new Vector2Int(4,4);
    [SerializeField] private float _aStartMoveCost = 10;
    [SerializeField] private float _aStartHeightMuiltiplier = 1;
    [Header("Tiles")] 
    [SerializeField] private Vector3 TileSize = new Vector3(5, 0, 5);
    [SerializeField] private TerrainTile _prfNoneTile;
    [SerializeField] private TerrainTile _prfLeftTile;
    [SerializeField] private TerrainTile _prfRightTile;
    [SerializeField] private TerrainTile _prfOpositeTile;
    [SerializeField] private SoTilesCollection _soTilesCollection;

    [Header("EnnemiSpawner")] 
    [SerializeField] private int _spawnerCounts = 5;
    [SerializeField] private Vector3 _spawnerOffset = new Vector3(0,1,0);
    [SerializeField] private EnnemySpawner _prfspawner;
    [SerializeField] private GameObject _playerController;
    private TerrainCell[,] _cells;
    
    private List<TerrainCell> _mainPass;
    private List<DirectionType> _directions;
    private List<ReturnTile> _returnTiles;
    private List<TerrainCell> _cellConnected = new List<TerrainCell>();

    private TerrainCell GetCell(Vector2Int pos) {
        if (pos.x < 0 || pos.x >= _terrainSize.x || pos.y < 0 || pos.y >= _terrainSize.y) return null;
        return _cells[pos.x, pos.y];
    }

    private List<TerrainCell> GetNeighboursCell(TerrainCell cell) {
        List<TerrainCell> neighbours = new List<TerrainCell>();
        if( GetCell(cell.Coordinates+new Vector2Int(-1,0)) != null)neighbours.Add(GetCell(cell.Coordinates+new Vector2Int(-1,0)));
        if( GetCell(cell.Coordinates+new Vector2Int(1,0)) != null)neighbours.Add(GetCell(cell.Coordinates+new Vector2Int(1,0)));
        if( GetCell(cell.Coordinates+new Vector2Int(0,-1)) != null)neighbours.Add(GetCell(cell.Coordinates+new Vector2Int(0,-1)));
        if( GetCell(cell.Coordinates+new Vector2Int(0,1)) != null)neighbours.Add(GetCell(cell.Coordinates+new Vector2Int(0,1)));
        return neighbours;
    }

    private Vector3 GetCellWorldPosition(TerrainCell cell)=> new Vector3(cell.Coordinates.x*TileSize.x,0,cell.Coordinates.y*TileSize.z); 
    
    

    private void Start() {
        GenerateCells();
        UpdatePerlinNose();
        if (_generateTerrainAtSart) {
            CalculatePath();
            SpawnPassTiles();
        }
    }
    private void GenerateCells() {
        _cells = new TerrainCell[_terrainSize.x, _terrainSize.y];
        for (int x = 0; x < _terrainSize.x; x++) {
            for (int y = 0; y < _terrainSize.y; y++) {
                _cells[x, y] = new TerrainCell(new Vector2Int(x, y));
                _cells[x, y].DebugCell = Instantiate(_prfDebugCell,gameObject.transform);
                _cells[x,y].DebugCell.transform.localPosition =  new Vector3 (x, 0, y);
            }
        }
    }

    private void UpdatePerlinNose() {
        for (int x = 0; x < _terrainSize.x; x++) {
            for (int y = 0; y < _terrainSize.y; y++)
            {
                _cells[x, y].HeightValue = Mathf.PerlinNoise(x * _scale + _offset.x, y * _scale + _offset.y);
                _cells[x, y].DebugCell.ChangeColor(new Color(_cells[x, y].HeightValue,_cells[x, y].HeightValue,_cells[x, y].HeightValue,1));
            }
        }
    }

    private void GenerateTheMainPass()
    {
        UpdatePerlinNose();
        
    }

    [ContextMenu("GenerateMainPass")]
    private List<TerrainCell> CalculatePath() {
        TerrainCell startCell = GetCell(_startPos);
        TerrainCell endCell = GetCell(_endPos);

        if (startCell == null || endCell == null) {
            Debug.LogWarning("Start or end cell Not Found");
            return null;
        }
        
        List<TerrainCell> cells = CalculateAStartPass(startCell, endCell);
        foreach (var cell in cells)
        {
            cell.DebugCell.ChangeColor(Color.blue);
        }
        startCell.DebugCell.ChangeColor(Color.green);
        endCell.DebugCell.ChangeColor(Color.green);
        _mainPass = cells;
        return cells;
    }

    private  List<TerrainCell> CalculateAStartPass(TerrainCell startCell, TerrainCell endCell ) {
       

        foreach (var cell in _cells) {
            cell.HCostValue = Vector2Int.Distance(cell.Coordinates, endCell.Coordinates);
            cell.GCostValue = Mathf.Infinity;
        }

        List<TerrainCell> openList = new List<TerrainCell>();
        List<TerrainCell> closeList = new List<TerrainCell>();
        startCell.GCostValue = 0;
        openList.Add(startCell);
        
        while (openList.Count != 0) {
            TerrainCell currentCell = openList[0];
            if (currentCell == endCell)
            {
                List<TerrainCell> returnList = new List<TerrainCell>();
                returnList.Add(currentCell);
                while (currentCell.CellFrom!=null) {
                    returnList.Add(currentCell.CellFrom);
                    currentCell = currentCell.CellFrom;
                }

                return returnList;
            }

            foreach (var neighbour in GetNeighboursCell(currentCell)) {
                if (neighbour == null) continue;
                if (closeList.Contains(neighbour)) continue;
                float newGCost = currentCell.GCostValue + currentCell.HeightValue * _aStartHeightMuiltiplier +
                                 _aStartMoveCost;
                if (newGCost < neighbour.GCostValue) {
                    neighbour.GCostValue = newGCost;
                    neighbour.CellFrom = currentCell;
                    if (!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
                    }
                }
            }
            closeList.Add(currentCell);
            openList.Remove(currentCell);
            openList.Sort((x, y) => x.FCostValue.CompareTo(y.FCostValue));
        }
        Debug.LogWarning("A Start Don't found path!");
        return null;
    }

    [ContextMenu("SpawnMainPassTiles")]
    private void SpawnPassTiles() {
        Debug.Log("SpawnMainPassTiles with "+ _mainPass.Count+ " tiles to spawn");
        CalculateTileTypes(_mainPass);
        CheckAllConnectedCellsToMainPass();
        SetAllTheNeighborsData();
        foreach (var cell in _cellConnected)
        {
            cell.DebugCell.ChangeColor(Color.yellow);
        }
        SpawnTileV3();
        SpawnSpawners();
        _navMesh.BuildNavMesh();
    }
    private void CalculateTileTypes(List<TerrainCell> cells) {
        List<DirectionType> directions = new List<DirectionType>();
        List<TileConnectionData> connectionDatas = new List<TileConnectionData>();
        
        DirectionType startDir = ExtensionsMethodes.GetDirectionType(cells[0].Coordinates,cells[1].Coordinates);
        directions.Add(ExtensionsMethodes.GetDirectionType(cells[0].Coordinates,cells[1].Coordinates));
        connectionDatas.Add(TileConnectionData.StandardDeadEnd().Rotate(startDir));
        
        for (int i = 1; i < cells.Count; i++) {
            Vector2Int currentPos = cells[i].Coordinates;
            Vector2Int prewPos = cells[i - 1].Coordinates;
            DirectionType tileDir = ExtensionsMethodes.GetDirectionType(prewPos,currentPos);
            TileConnectionData connectionData;
            if (i == cells.Count - 1) {
                connectionData = TileConnectionData.StandardDeadEnd();
                connectionData = connectionData.Rotate(tileDir.GetOppositeDirectionType());
            }
            else {
                Vector2Int nextPos = cells[i + 1].Coordinates; 
                DirectionType nextTileDir = ExtensionsMethodes.GetDirectionType(currentPos, nextPos);
                connectionData =GetTileDataNeeded(cells[i],  tileDir.GetOppositeDirectionType(), nextTileDir);
            }
            directions.Add(tileDir);
            connectionDatas.Add(connectionData);
            cells[i].Direction =  tileDir;
        }

        List<ReturnTile>   returnTiles = new List<ReturnTile>();

        for (int i = 0; i < connectionDatas.Count; i++) {
            List<ReturnTile> returnTile = new List<ReturnTile>();
            returnTile=_soTilesCollection.GetPotencialTiles(connectionDatas[i]);
            cells[i].ReturnTile = returnTile.GetRandomTile();
            returnTiles.Add(cells[i].ReturnTile);
        }
        _directions = directions;
        _returnTiles = returnTiles;
    }

    private void CheckAllConnectedCellsToMainPass() {
        foreach (var cell in _mainPass) {
            foreach (var dir in cell.GetCellConnection()) {
                TerrainCell neighbours = GetCell(dir+cell.Coordinates);
                if (neighbours == null) continue;
                if( _mainPass.Contains(neighbours)) continue;
                if( _cellConnected.Contains(neighbours)) continue;
                _cellConnected.Add(neighbours);
            }
        }
    }

    private void SetAllTheNeighborsData() {
        while (_cellConnected.Count > 0) {
            TileConnectionData data =GetTileDataNeeded(_cellConnected[0]);
            _cellConnected[0].ReturnTile =_soTilesCollection.GetPotencialTiles(data).GetRandomTile();
            
            foreach (var dir in _cellConnected[0].GetCellConnection()) {
                TerrainCell neighbours = GetCell(dir+_cellConnected[0].Coordinates);
                if (neighbours == null) continue;
                if( neighbours.ReturnTile.IsSeted) continue;
                _cellConnected.Add(neighbours);
            }
            _cellConnected.RemoveAt(0);
        }
    }

    private TileConnectionData GetTileRequirement(TerrainCell cell, DirectionType nexDirections) {
        TileConnectionData.ConnectionType topConnection;
        TileConnectionData.ConnectionType bopConnection;
        TileConnectionData.ConnectionType LeftConnection;
        TileConnectionData.ConnectionType RightConnection;

        return TileConnectionData.StandardDeadEnd();
    }

    private TileConnectionData.ConnectionType DetermineConnectionType(Vector2Int coords, Vector2Int direction) {
        TerrainCell cell = GetCell(coords +direction);
        if (cell == null) return TileConnectionData.ConnectionType.NoConnection;
        return TileConnectionData.ConnectionType.None;
    }

    private void SpawnTiles(List<TerrainCell> cells) {
        foreach (var cell in cells) {
            Debug.Log(" Spaw Tile At Coordinate"+ cell.Coordinates +" with a direction of "+ cell.Direction+ " ans a Type of "+ cell.TileForme);
            TerrainTile tileToSpaw = cell.TileForme switch {
                TerrainTile.TileForme.none => _prfNoneTile,
                TerrainTile.TileForme.left => _prfLeftTile,
                TerrainTile.TileForme.right => _prfRightTile,
                TerrainTile.TileForme.opposite => _prfOpositeTile,
                _ => throw new ArgumentOutOfRangeException()
            };
            Vector3 pos =new Vector3(cell.Coordinates.x*TileSize.x,0,cell.Coordinates.y*TileSize.z); 
            TerrainTile tile = Instantiate( tileToSpaw, pos, Quaternion.identity);
            tile.transform.forward = cell.Direction.GetWorldDirection();
        }
    }

    private void SpawnTileV2() {
        for (int i = 0; i < _mainPass.Count; i++) {
            TerrainTile tile= Instantiate(_returnTiles[i].Tile, GetCellWorldPosition(_mainPass[i]), Quaternion.identity);
            //DirectionType tileDir = _directions[i].RotateBy(_returnTiles[i].Direction);
            tile.transform.forward = _returnTiles[i].Direction.GetWorldDirection();
            _mainPass[i].TerrainTile = tile;
            _mainPass[i].Direction = _returnTiles[i].Direction;
        }
    }
    private void SpawnTileV3() {

        foreach (var cell in _cells) {
            if(!cell.ReturnTile.IsSeted)continue;
            TerrainTile tile= Instantiate(cell.ReturnTile.Tile, GetCellWorldPosition(cell), Quaternion.identity);
            tile.transform.forward = cell.ReturnTile.Direction.GetWorldDirection();
            tile.transform.SetParent(transform);
            cell.TerrainTile = tile;
            cell.Direction = cell.ReturnTile.Direction;
        }
        //for (int i = 0; i < _cells.Length; i++) {
        //    TerrainTile tile= Instantiate(_returnTiles[i].Tile, GetCellWorldPosition(_mainPass[i]), Quaternion.identity);
        //    //DirectionType tileDir = _directions[i].RotateBy(_returnTiles[i].Direction);
        //    tile.transform.forward = _returnTiles[i].Direction.GetWorldDirection();
        //    _mainPass[i].TerrainTile = tile;
        //    _mainPass[i].Direction = _returnTiles[i].Direction;
        //}
    }

    private void SpawnSpawners() {
        for (int i = 0; i < _spawnerCounts; i++) {
            TerrainCell cell =_cells[Random.Range(0, _terrainSize.x), Random.Range(0, _terrainSize.y)];
            EnnemySpawner spawner =Instantiate(_prfspawner, GetCellWorldPosition(cell)+_spawnerOffset, Quaternion.identity);
            spawner.SetUpPlayerTarget(_playerController);
        }
    }

    private TileConnectionData GetTileDataNeeded(TerrainCell cell, DirectionType from, DirectionType to) {
        TileConnectionData.ConnectionType bottonConnection;
        TileConnectionData.ConnectionType leftConnection;
        TileConnectionData.ConnectionType topConnection;
        TileConnectionData.ConnectionType rightConnection;
        if (from == DirectionType.Bottom || to == DirectionType.Bottom) {
            bottonConnection = TileConnectionData.ConnectionType.StandardConnection;
        }
        else {
            if (GetCell(cell.Coordinates + Vector2Int.down) == null) bottonConnection = TileConnectionData.ConnectionType.NoConnection;
            else bottonConnection = TileConnectionData.ConnectionType.None;
        }
        
        if (from == DirectionType.Left || to == DirectionType.Left) {
            leftConnection = TileConnectionData.ConnectionType.StandardConnection;
        }
        else {
            if (GetCell(cell.Coordinates + Vector2Int.left) == null) leftConnection = TileConnectionData.ConnectionType.NoConnection;
            else leftConnection = TileConnectionData.ConnectionType.None;
        }
        
        if (from == DirectionType.Top || to == DirectionType.Top) {
            topConnection = TileConnectionData.ConnectionType.StandardConnection;
        }
        else {
            if (GetCell(cell.Coordinates + Vector2Int.up) == null) topConnection = TileConnectionData.ConnectionType.NoConnection;
            else topConnection = TileConnectionData.ConnectionType.None;
        }
        
        if (from == DirectionType.Right || to == DirectionType.Right) {
            rightConnection = TileConnectionData.ConnectionType.StandardConnection;
        }
        else {
            if (GetCell(cell.Coordinates + Vector2Int.right) == null) rightConnection = TileConnectionData.ConnectionType.NoConnection;
            else rightConnection = TileConnectionData.ConnectionType.None;
        }
        return new TileConnectionData(bottonConnection, leftConnection, topConnection, rightConnection);
    }

    private TileConnectionData GetTileDataNeeded(TerrainCell cell) {
        TileConnectionData.ConnectionType bottonConnection;
        TileConnectionData.ConnectionType leftConnection;
        TileConnectionData.ConnectionType topConnection;
        TileConnectionData.ConnectionType rightConnection;
        TerrainCell neigthbour =GetCell(cell.Coordinates + Vector2Int.down) ;
        if (neigthbour == null) bottonConnection = TileConnectionData.ConnectionType.NoConnection;
        else bottonConnection = neigthbour.GetConnectionTypeFor(DirectionType.Top);
        
        neigthbour =GetCell(cell.Coordinates + Vector2Int.left) ;
        if (neigthbour == null) leftConnection = TileConnectionData.ConnectionType.NoConnection;
        else leftConnection = neigthbour.GetConnectionTypeFor(DirectionType.Right);
        
        neigthbour =GetCell(cell.Coordinates + Vector2Int.up) ;
        if (neigthbour == null) topConnection = TileConnectionData.ConnectionType.NoConnection;
        else topConnection = neigthbour.GetConnectionTypeFor(DirectionType.Bottom);
        
        neigthbour =GetCell(cell.Coordinates + Vector2Int.right) ;
        if (neigthbour == null) rightConnection = TileConnectionData.ConnectionType.NoConnection;
        else rightConnection = neigthbour.GetConnectionTypeFor(DirectionType.Left);
        
        return new TileConnectionData(bottonConnection, leftConnection, topConnection, rightConnection);
    }

    private void Update() {
        if (_perlinInUpdate)UpdatePerlinNose();
    }
}
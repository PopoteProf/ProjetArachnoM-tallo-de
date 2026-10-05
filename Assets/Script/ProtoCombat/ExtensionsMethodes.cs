using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

public static class ExtensionsMethodes
{
    public static DirectionType GetOppositeDirectionType(this DirectionType dir) {
        if (dir == DirectionType.Left) return DirectionType.Right;
        if (dir == DirectionType.Right) return DirectionType.Left;
        if (dir == DirectionType.Top) return DirectionType.Bottom;
        if (dir == DirectionType.Bottom) return DirectionType.Top;
        return DirectionType.Bottom;
    } 
    public static DirectionType GetDirectionType(Vector2Int from, Vector2Int to) {
        Vector2Int dir = to - from;
        if (dir == Vector2Int.left) return DirectionType.Left;
        if (dir == Vector2Int.right) return DirectionType.Right;
        if (dir == Vector2Int.up) return DirectionType.Top;
        if (dir == Vector2Int.down) return DirectionType.Bottom;
        return DirectionType.Bottom;
    }

    public static Vector3 GetWorldDirection(this DirectionType dir) {
        return dir switch {
            DirectionType.Top => new Vector3(0, 0,-1),
            DirectionType.Bottom => new Vector3(0, 0, 1),
            DirectionType.Right => new Vector3(1, 0, 0),
            DirectionType.Left => new Vector3(-1, 0, 0),
            _ => Vector3.zero
        };
    }

    public static TerrainTile.TileForme GetTileForm(this DirectionType dir, DirectionType nextDir) {
        switch (dir) {
            case DirectionType.Top:
                return nextDir switch {
                    DirectionType.Top => TerrainTile.TileForme.opposite,
                    DirectionType.Right => TerrainTile.TileForme.right,
                    DirectionType.Bottom => TerrainTile.TileForme.none,
                    DirectionType.Left => TerrainTile.TileForme.left,
                    _ => throw new ArgumentOutOfRangeException(nameof(nextDir), nextDir, null)
                };
                break;
            case DirectionType.Right:return nextDir switch {
                DirectionType.Top => TerrainTile.TileForme.left,
                DirectionType.Right => TerrainTile.TileForme.opposite,
                DirectionType.Bottom => TerrainTile.TileForme.right,
                DirectionType.Left => TerrainTile.TileForme.none,
                _ => throw new ArgumentOutOfRangeException(nameof(nextDir), nextDir, null)
            };
            case DirectionType.Bottom:return nextDir switch
            {
                DirectionType.Top => TerrainTile.TileForme.none,
                DirectionType.Right => TerrainTile.TileForme.left,
                DirectionType.Bottom => TerrainTile.TileForme.opposite,
                DirectionType.Left => TerrainTile.TileForme.right,
                _ => throw new ArgumentOutOfRangeException(nameof(nextDir), nextDir, null)
            };
            case DirectionType.Left:return nextDir switch {
                DirectionType.Top => TerrainTile.TileForme.right,
                DirectionType.Right => TerrainTile.TileForme.none,
                DirectionType.Bottom => TerrainTile.TileForme.left,
                DirectionType.Left => TerrainTile.TileForme.opposite,
                _ => throw new ArgumentOutOfRangeException(nameof(nextDir), nextDir, null)
            };
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    public static bool DidConnectionMatch(this TileConnectionData.ConnectionType thisConnection,
        TileConnectionData.ConnectionType otherConnection) {
        if (thisConnection == TileConnectionData.ConnectionType.None ||
            otherConnection == TileConnectionData.ConnectionType.None) {
            return true;
        }
        return thisConnection == otherConnection;
    }

    public static DirectionType RotateBy(this DirectionType baseRot, DirectionType secondRot) {
        return (DirectionType)(Math.Abs(baseRot - secondRot));
    }
    
    /// <summary>
    /// Create a new TileConnectionData base on this connection Data and rotated base on direction
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static TileConnectionData Rotate(this TileConnectionData conn, DirectionType dir)
    {
        TileConnectionData.ConnectionType[] oldCons = conn.GetConnections;
        TileConnectionData.ConnectionType[] newCons = new TileConnectionData.ConnectionType[4];
        for (int i = 0; i < oldCons.Length; i++) {
            newCons[(i+(int)dir)%4] = oldCons[i];
        }
        return new TileConnectionData(newCons);
    }
    public static TileConnectionData AntiRotate(this TileConnectionData conn, DirectionType dir)
    {
        TileConnectionData.ConnectionType[] oldCons = conn.GetConnections;
        TileConnectionData.ConnectionType[] newCons = new TileConnectionData.ConnectionType[4];
        for (int i = 0; i < oldCons.Length; i++) {
            int index = i - (int)dir;
            if (index < 0) {
                index = 4 + index;
            }
            newCons[index] = oldCons[i];
        }
        return new TileConnectionData(newCons);
    }

    public static ReturnTile GetRandomTile(this List<ReturnTile> returnTiles) {
        return returnTiles[Random.Range(0, returnTiles.Count)];
    }
}
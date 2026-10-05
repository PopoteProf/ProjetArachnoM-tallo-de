
using System;

[Serializable]
public struct TileConnectionData {
    public enum ConnectionType {
        None, NoConnection, StandardConnection 
    }

    public ConnectionType Bottom;
    public ConnectionType Left;
    public ConnectionType Top;
    public ConnectionType Right;

    public ConnectionType[] GetConnections {
        get => new []{Bottom, Left, Top, Right};
    }

    public TileConnectionData( ConnectionType bottom, ConnectionType left,ConnectionType top, ConnectionType right) {
        Bottom = bottom;
        Left = left;
        Top = top;
        Right = right;
    }
    public TileConnectionData( ConnectionType[] connections) {
        Bottom = connections[0];
        Left = connections[1];
        Top = connections[2];
        Right = connections[3];
    }

    public static TileConnectionData StandardDeadEnd()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.NoConnection,
            ConnectionType.NoConnection, ConnectionType.NoConnection);
    }
    public static TileConnectionData StandardTunnel()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.NoConnection,
            ConnectionType.StandardConnection, ConnectionType.NoConnection);
    }
    public static TileConnectionData StandardLeftTurn()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.StandardConnection,
            ConnectionType.NoConnection, ConnectionType.NoConnection);
    }
    public static TileConnectionData StandardRightTurn()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.NoConnection,
            ConnectionType.NoConnection, ConnectionType.StandardConnection);
    }
    public static TileConnectionData StandardTCrossing()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.StandardConnection,
            ConnectionType.NoConnection, ConnectionType.StandardConnection);
    }
    public static TileConnectionData StandardXCrossing()
    {
        return new TileConnectionData(ConnectionType.StandardConnection, ConnectionType.StandardConnection,
            ConnectionType.StandardConnection, ConnectionType.StandardConnection);
    }

    
}


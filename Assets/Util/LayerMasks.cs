﻿public class LayerMasks {
	public static int WallLayerMask = 1 << 8;
    public static int EnemyLayerMask = 1 << 9;
    public static int MissileLayerMask = 1 << 10;
    public static int ObstacleLayerMask = 1 << 12;
    public static int WallAndObstacleLayerMask = 1 << 8 | 1 << 12;
    public static int MovementObstructedLayerMask = 1 << 4 | 1 << 8 | 1 << 12;
    public static int EnemySniperLayerMask = 1 << 4 | 1 << 8 | 1 << 12 | 1 << 13;
    public static int PlayerSniperLayerMask = 1 << 8 | 1 << 12 | 1 << 9;
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FullSerializer;

[Serializable]
public class Level
{
	public int base_x;
	public int base_y;
	public LevelLayer[] layers;
}

[Serializable]
public class LevelLayer
{
	public int order;
	public TextureToCell[] cells;
	public bool? walkable;
}

[Serializable]
public class TextureToCell
{
    public string texture;
    public CellInfo[] cells;
}

[Serializable]
public class CellInfo
{
	public int x_location;
	public int y_location;
	public bool? walkable;
}
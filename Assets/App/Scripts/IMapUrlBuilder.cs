using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapUrlBuilder
{
    string GetTileUrl(TileInfo tileInfo);
}

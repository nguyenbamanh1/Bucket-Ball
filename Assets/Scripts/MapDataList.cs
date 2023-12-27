using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Map_X", menuName = "Map/New Map Data L")]
public class MapDataList : ScriptableObject
{
    public List<GameObject> mapDatas;

    internal GameObject GetMapData(int level)
    {
        if (level < 0 || level > mapDatas.Count)
            throw new Exception("Map not found");
        return mapDatas[level - 1];
    }
}
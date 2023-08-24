using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
 
public class AstarManager : MonoBehaviour
{
    public Vector2Int mapSize;//地图尺寸
 
    public Tilemap tilemap;//获取地图

    private Dictionary<Vector3Int, int> search = new Dictionary<Vector3Int, int>();//要进行的查找任务
    private Dictionary<Vector3Int, int> cost = new Dictionary<Vector3Int, int>();//起点到当前点的消耗
    private Dictionary<Vector3Int, Vector3Int> pathSave = new Dictionary<Vector3Int, Vector3Int>();//保存回溯路径
 
    private List<Vector3Int> obstacle = new List<Vector3Int>();//障碍物坐标

    private void Awake()
    {
        mapSize.x = tilemap.cellBounds.size.x;
        mapSize.y = tilemap.cellBounds.size.y; //获取尺寸

        for(int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector3Int position = new Vector3Int(i, j, 0); //获取坐标
                TileBase tile = tilemap.GetTile(position); //找到瓦片
                if(tile == null) //如果瓦片在路径图层不存在，则将其添加为障碍物
                {
                    obstacle.Add(position);
                }
            }
        }
    }
 
    
    //AStar算法查找
    public List<Vector3Int> AStarSearchPath(Vector3Int startPos, Vector3Int endPos)
    {
        //初始化
        search.Clear();
        cost.Clear();
        pathSave.Clear();
        search.Add(startPos, 0);
        cost.Add(startPos, 0);
        pathSave.Add(startPos, startPos);
 
        while (search.Count > 0)
        {
            Vector3Int current = GetShortestPos();//获取任务列表里的最少消耗的那个坐标
 
            if (current.Equals(endPos))
                break;
 
            List<Vector3Int> neighbors = GetNeighbors(current);//获取当前坐标的邻居
 
            foreach (var next in neighbors)
            {
                int newCost = cost[current] + 1; //计算当前格子的消耗，其实就是上一个格子加1步

                if (!cost.ContainsKey(next) || newCost < cost[next]) //如果没被搜索过或者当前的代价小于之前搜索到的代价
                {
                    cost.Add(next, newCost); //更新代价字典
                    search.Add(next, cost[next] + GetHeuristic(next, endPos));//添加要查找的任务，消耗值为当前消耗加上当前点到终点的距离
                    pathSave.Add(next, current);//保存路径
                }
            }
        }
         
        if (pathSave.ContainsKey(endPos))
            return ShowPath(startPos, endPos);
        else
            return null;
    }
    //获取周围可用的邻居
    private List<Vector3Int> GetNeighbors(Vector3Int target)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
 
        Vector3Int up = target + Vector3Int.up;
        Vector3Int right = target + Vector3Int.right;
        Vector3Int left = target - Vector3Int.right;
        Vector3Int down = target - Vector3Int.up;
 
        //Up
        if (up.y < mapSize.y && !obstacle.Contains(up))
        {
            neighbors.Add(up);
        }
        //Right
        if (right.x < mapSize.x && !obstacle.Contains(right))
        {
            neighbors.Add(right);
        }
        //Left
        if (left.x >= 0 && !obstacle.Contains(left))
        {
            neighbors.Add(left);
        }
        //Down
        if (down.y >= 0 && !obstacle.Contains(down))
        {
            neighbors.Add(down);
        }
 
        return neighbors;
    }
    //获取当前位置到终点的消耗
    private int GetHeuristic(Vector3Int posA, Vector3Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
    //获取任务字典里面最少消耗的坐标
    private Vector3Int GetShortestPos()
    {
        KeyValuePair<Vector3Int, int> shortest = new KeyValuePair<Vector3Int, int>(Vector3Int.zero, int.MaxValue);
 
        foreach (var item in search)
        {
            if (item.Value < shortest.Value)
            {
                shortest = item;
            }
        }
 
        search.Remove(shortest.Key);
 
        return shortest.Key;
    }
    //显示查找完成的路径
    private List<Vector3Int> ShowPath(Vector3Int startPos, Vector3Int endPos)
    {
        Vector3Int current = endPos;
        List<Vector3Int> path = new List<Vector3Int>();
 
        while (current != startPos)
        {
            Vector3Int next = pathSave[current]; //找到父节点
 
            path.Add(current); //添加当前节点至路径列表
 
            current = next; //更新当前节点
        }

        path.Reverse(); //反转列表

        return path;
    }
}

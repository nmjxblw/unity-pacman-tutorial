using UnityEngine;
using MyNamespace;
using System.Collections.Generic;

public class GhostChase : GhostBehavior
{
    public AstarManager astarManager;

    // private void OnDisable()
    // {
    //     ghost.scatter.Enable();
    // }
    
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Node node = other.GetComponent<Node>();

    //     astarManager = GetComponent<AstarManager>();


    //     if (node != null && enabled && !ghost.frightened.enabled)
    //     {
    //         Vector3Int ghostPos = Vector3Int.RoundToInt(transform.position);
    //         Vector3Int targetPos = Vector3Int.RoundToInt(ghost.target.position);

    //         List<Vector3Int> path = astarManager.AStarSearchPath(ghostPos, targetPos);
    //         Debug.LogWarning(path);
    //         if (path == null)
    //         {
    //             Debug.LogWarning("Invalid");
    //             return;
    //         }
    //         else if (path != null && path.Count > 1)
    //         {
    //             Vector3Int nextStep = path[0];
    //             Vector2 direction = new Vector2(nextStep.x - ghostPos.x, nextStep.y - ghostPos.y);
    //             ghost.movement.SetDirection(direction.normalized);
    //         }
            
    //     }
    // }










    private List<Vector3Int> currentPath = new List<Vector3Int>();

    private void Awake()
    {
        InvokeRepeating("CalculatePath", 0f, 0.5f);
    }

    private void CalculatePath()
    {
        // Clear the old path
        currentPath.Clear();

        if (astarManager == null || ghost == null || ghost.target == null)
        {
            Debug.LogWarning("One of the critical references is null!");
            return;
        }

        Vector3Int ghostPos = Vector3Int.RoundToInt(transform.position);
        Vector3Int targetPos = Vector3Int.RoundToInt(ghost.target.position);

        currentPath = astarManager.AStarSearchPath(ghostPos, targetPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        astarManager = other.GetComponent<AstarManager>();

        if (node == null || astarManager==null || !enabled || ghost.frightened.enabled)
            return;

        if (currentPath == null || currentPath.Count < 1)
        {
            Debug.LogWarning("A* path is invalid or too short!");
            return;
        }
        Debug.Log("hello");

        Debug.Log("Current Path: " + string.Join(" -> ", currentPath));

        Vector3Int nextStep = currentPath[0]; 
        Vector2 direction = new Vector2(nextStep.x - node.transform.position.x, nextStep.y - node.transform.position.y);
        ghost.movement.SetDirection(direction.normalized);
    }

    private void OnDisable()
    {
        CancelInvoke("CalculatePath");  // Stop invoking the method when the object is disabled
    }


    


    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Node node = other.GetComponent<Node>();

    //     // Do nothing while the ghost is frightened
    //     if (node != null && enabled && !ghost.frightened.enabled)
    //     {
    //         Vector2 direction = Vector2.zero;
    //         float minDistance = float.MaxValue;

    //         // Find the available direction that moves closet to pacman
    //         foreach (Vector2 availableDirection in node.availableDirections)
    //         {
    //             // If the distance in this direction is less than the current
    //             // min distance then this direction becomes the new closest
    //             Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
    //             float distance = (ghost.target.position - newPosition).sqrMagnitude;

    //             if (distance < minDistance)
    //             {
    //                 direction = availableDirection;
    //                 minDistance = distance;
    //             }
    //         }

    //         ghost.movement.SetDirection(direction);
    //     }
    // }

}

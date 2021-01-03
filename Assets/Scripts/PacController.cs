using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DataStructures.PriorityQueue;

public class PacController : MonoBehaviour
{

    [SerializeField]
    public Tilemap wallTilemap = null;
    [SerializeField]
    private Tilemap pelletTilemap = null;

    

    private float moveTime;
    public bool isMoving = false;
    private bool isCalulatingPath = false;
    private bool[,] validPathTiles;
    private Dictionary<Vector3Int, bool> pelletTiles = new Dictionary<Vector3Int, bool>();
    private int movementCost = 1;
    private Dictionary<Vector3Int, int> initialGScore = new Dictionary<Vector3Int, int>();
    LinkedList<Vector3Int> totalPath = new LinkedList<Vector3Int>();
    GameController gameController;
    public Coroutine moving;

    // Start is called before the first frame update
    void Start()
    {
        moveTime = StaticValues.pacMoveTime;
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        validPathTiles = gameController.GetWallMap();
        pelletTilemap = (Tilemap) GameObject.Find("Pellet Tilemap").GetComponent<Tilemap>();
        pelletTiles = gameController.GetPelletTiles();
        
        // Center in the starting cell
        Vector3Int startingCell = wallTilemap.WorldToCell(transform.position);
        transform.position = wallTilemap.GetCellCenterWorld(startingCell);
        /*
        for (int y = 0; y < tilemap.cellBounds.yMax; y++)
        {
            for (int x = 0; x < tilemap.cellBounds.xMax; x++)
            {
                initialGScore[new Vector3Int(x, y,0)] = int.MaxValue;
            }
        }
        */
        //Vector3Int nearestPellet = FindNearestPellet();
        Vector3Int randomPellet = FindRandomPellet();
        StartCoroutine(PathThread(randomPellet));
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        if (isMoving)
        {
            return;
        }
        
        // Get the inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Only allow movement in one direction at once
        if (horizontal != 0f)
        {
            vertical = 0f;
        }

        if (horizontal != 0f || vertical != 0f)
        {
            Move(new Vector2(horizontal, vertical));
        }
        */
        //Vector3Int currentCell = wallTilemap.WorldToCell(transform.position);


        if (!isMoving)
        {
            
            moving = StartCoroutine(MoveForward());
        }
        //StartCoroutine(Move(totalPath));

        /*
        if (!isMoving)
        {
            
            if (nearestPellet != Vector3Int.zero)
            {
                
            }
            
        }
        */
    }

    private IEnumerator PathThread(Vector3Int target)
    {
        yield return null;
        isCalulatingPath = true;
        totalPath = AStarPath(target);
        isCalulatingPath = false;
        
    }

    private IEnumerator MoveForward()
    {

        yield return new WaitWhile(() => isCalulatingPath);
        isMoving = true;
        Vector3Int currentCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int nextCell = currentCell;
        try
        {
            nextCell = totalPath.First.Value;
            totalPath.RemoveFirst();
            if (currentCell == nextCell)
            {
                nextCell = totalPath.First.Value;
                totalPath.RemoveFirst();
            }
        } catch (System.Exception)
        {
            Vector3Int nearestPellet = FindNearestPellet();
            StartCoroutine(PathThread(nearestPellet));
        }
        yield return new WaitWhile(() => isCalulatingPath);

        Vector3 startPosition = transform.position;
        Vector3 destinationPosition = wallTilemap.GetCellCenterWorld(nextCell);
        transform.right = destinationPosition - startPosition;
        float timer = 0f;

        
        while (timer < moveTime)
        {
            yield return null;
            

            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, destinationPosition, timer / moveTime);

        }
        // Snap to the grid
        transform.position = destinationPosition;






        //float angle = Mathf.Floor(Mathf.Atan2(destinationPosition.x, destinationPosition.y) * Mathf.Rad2Deg - 45);
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.LookAt(destinationPosition);

        //Vector3 right = transform.position - wallTilemap.CellToWorld(nextCell);
        //transform.right = right;
        //Vector3 destinationPosition = wallTilemap.CellToWorld(nextCell);


        //Vector3Int cellPosition = wallTilemap.WorldToCell(transform.position);


        
        if (totalPath.Count == 0)
        {
            Vector3Int nearestPellet = FindNearestPellet();
            //totalPath = AStarPath(nearestPellet);
            //Vector3Int randomPellet = FindRandomPellet();
            StartCoroutine(PathThread(nearestPellet));
            
        }
        isMoving = false;
    }
    /*
    IEnumerator Move(LinkedList<Vector3Int> totalPath)
    {
        Vector3Int startCell = wallTilemap.WorldToCell(transform.position);
       
        foreach (Vector3Int destinationCell in totalPath)
        {
            
            
            while (isMoving)
            {
                yield return new WaitWhile(isActiveAndEnabled;
            }
            
            transform.right = destinationCell - startCell;
            Vector3 destinationPosition = wallTilemap.GetCellCenterWorld(destinationCell);
            StartCoroutine(MoveToNextCell(transform.position, destinationPosition, moveTime));
            yield return new WaitWhile(() => isMoving);
        }
            
        

        
        //transform.position = Vector3.Lerp(transform.position, destinationPosition, 1);
    }
*/

    /*
    private IEnumerator MoveToNextCell(Vector3 startPosition, Vector3 destinationPosition, float moveTime)
    {
        isMoving = true;

        float timer = 0f;

        while (timer < moveTime)
        {
            yield return null;

            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, destinationPosition, timer / moveTime);
        }
        // Snap to the grid
        transform.position = destinationPosition;

        isMoving = false;

    }
    */
    /*
    void Move(Vector2 direction)
    {
        Debug.Log(AStarPath());
        Vector2 startingPosition = transform.position;
        Vector2 endPosition = startingPosition + direction.normalized;

        

        Vector3Int endPositionCell = tilemap.WorldToCell(endPosition);
        if (!tilemap.HasTile(endPositionCell))
        {
            transform.right = endPosition - startingPosition;
            endPosition = tilemap.GetCellCenterWorld(endPositionCell);
            //endPosition = tilemap.CellToWorld(endPositionCell);
            //transform.position = endPosition;
            StartCoroutine(MoveToNextCell(startingPosition, endPosition, moveTime));
        }
        //AStarPath();
        
                
    }
    */


    /*
    private IEnumerator MoveToNextCell(Vector2 startingPosition, Vector2 endPosition, float moveTime)
    {
        isMoving = true;

        float timer = 0f;

        while (timer < moveTime)
        {
            yield return null;

            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(startingPosition, endPosition, timer / moveTime);
        }

        // Snap to the grid
        transform.position = endPosition;

        isMoving = false;
    }
    */
    private LinkedList<Vector3Int> AStarPath(Vector3Int endPosition)
    {
        Vector3Int startPosition = wallTilemap.WorldToCell(transform.position);
        //Vector3Int endPosition = new Vector3Int(2, 1, 0);
        
        List < Vector3Int > openSet = new List<Vector3Int>();
        openSet.Add(startPosition);
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();


        

        //PriorityQueue<Vector3Int, int> gScore = new PriorityQueue<Vector3Int, int>(0);
        //Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int>();
        Dictionary<Vector3Int, int> gScore = new Dictionary<Vector3Int, int>();
        for (int y = 0; y < wallTilemap.cellBounds.yMax; y++)
        {
            for (int x = 0; x < wallTilemap.cellBounds.xMax; x++)
            {
                gScore[new Vector3Int(x, y, 0)] = int.MaxValue;
            }
        }

        gScore[startPosition] = 0;
        PriorityQueue<Vector3Int, int> fScore = new PriorityQueue<Vector3Int, int>(0);
        fScore.Insert(startPosition, CalculateH(startPosition, endPosition));
        while (openSet.Count > 0)
        {
            Vector3Int current = fScore.Pop();
            if (current == endPosition)
            {
                return ReconstructPath(cameFrom, current);
            }
            openSet.Remove(current);

            // Foreach of the neighbours of the current tile
            Vector3Int[] neighbours = 
                {
                    new Vector3Int (current.x, current.y - 1, current.z),
                    new Vector3Int (current.x, current.y + 1, current.z),
                    new Vector3Int (current.x - 1, current.y, current.z),
                    new Vector3Int (current.x + 1, current.y, current.z),
            };
            foreach (Vector3Int neighbour in neighbours)
            {
                // Try-Catch this so we don't have to check if we're out of range
                try
                {
                    // If the neighbour is a valid path tile
                    if (validPathTiles[neighbour.x, neighbour.y])
                    {
                        int tentativeGScore = gScore[current] + movementCost;
                        if (tentativeGScore < gScore[neighbour])
                        {

                            cameFrom.Add(neighbour, current);
                            gScore[neighbour] = tentativeGScore;
                            fScore.Insert(neighbour, gScore[neighbour] + CalculateH(neighbour, endPosition));
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                } catch (System.IndexOutOfRangeException)
                {
                    // Do nothing
                    //Debug.Log("index out of bounds");
                }
    
            }
        }
        //Debug.Log("Null");
        return null;
    }

    private LinkedList<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        
        //totalPath = new LinkedList<Vector3Int>();
        totalPath.AddFirst(current);
        bool containsKey = cameFrom.ContainsKey(current);
        while (containsKey)
        {
            current = cameFrom[current];
            totalPath.AddFirst(current);
            containsKey = cameFrom.ContainsKey(current);
        }
        /*
        while (cameFrom.ContainsKey(current));
        {
            
            
        }
        */
        return totalPath;
    }

    private int CalculateH(Vector3Int position, Vector3Int target)
    {
        return Mathf.Abs(position.x - position.x) + Mathf.Abs(target.y - target.y);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
     //   pelletTilemap.SetTile(pelletTilemap.WorldToCell(transform.position), null);
        //if (other.GetType() == )
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector3Int cellPosition = pelletTilemap.WorldToCell(transform.position);
        pelletTilemap.SetTile(cellPosition, null);

        pelletTiles.Remove(cellPosition);

    }

    

    private Vector3Int FindNearestPellet()
    {
        int minDistance = int.MaxValue;
        
        Vector3Int cellPosition = pelletTilemap.WorldToCell(transform.position);
        Vector3Int nearestPellet = cellPosition;
        foreach (KeyValuePair<Vector3Int, bool> pair in pelletTiles)
        {
//            Debug.Log(Mathf.Abs(pair.Key.x - cellPosition.x) + Mathf.Abs(pair.Key.y - cellPosition.y));
            int distance = Mathf.Abs(pair.Key.x - cellPosition.x) + Mathf.Abs(pair.Key.y - cellPosition.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPellet = pair.Key;
            }
        }
        return nearestPellet;
    }

    private Vector3Int FindRandomPellet()
    {
        List<Vector3Int> keys = new List<Vector3Int>(pelletTiles.Keys);
        int randomKeyPosition = Random.Range(0, keys.Count);
        
        return keys[randomKeyPosition];

    }

}

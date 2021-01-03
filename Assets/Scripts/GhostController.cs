using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GhostController : MonoBehaviour
{

    [SerializeField]
    public Tilemap wallTilemap = null;
    [SerializeField]
    private float moveTime = 1f;
    public bool isMoving = false;
    private bool[,] validPathTiles;
    public Coroutine moving;
    GameController gameController;
    Vector2 direction = Vector2.zero;
    Vector2 newDirection = Vector2.zero;

    [SerializeField]
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        animator = gameObject.GetComponent<Animator>();
        //BuildWallMap();
        validPathTiles = gameController.GetWallMap();
        // Center in the starting cell
        Vector3Int startingCell = wallTilemap.WorldToCell(transform.position);
        transform.position = wallTilemap.GetCellCenterWorld(startingCell);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
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
            newDirection = new Vector2(horizontal, vertical);
        }

        if (!isMoving)
        {
            Move(newDirection);
        }


    }


    void Move(Vector2 newDirection)
    {
        Vector2 startingPosition = transform.position;
        Vector2 endPosition = startingPosition + direction.normalized;
        Vector2 potentialEndPosition = startingPosition + newDirection.normalized;
        Vector3Int potentialEndPositionCell = wallTilemap.WorldToCell(potentialEndPosition);
        // Check if the desired direction is a valid path and that it is not the reverse of the direction we are going
        if (!wallTilemap.HasTile(potentialEndPositionCell) && newDirection != -direction)
        {
            endPosition = potentialEndPosition;
            direction = newDirection;
        }


            
        if (direction.x == -1)
        {
            animator.SetTrigger("Left");
        } else if (direction.x == 1)
        {
            animator.SetTrigger("Right");
        } else if (direction.y == -1)
        {
            animator.SetTrigger("Down");
        } else if (direction.y == 1)
        {
            animator.SetTrigger("Up");
        }

        Vector3Int endPositionCell = wallTilemap.WorldToCell(endPosition);
        
        if (!wallTilemap.HasTile(endPositionCell))
        {
            //transform.right = endPosition - startingPosition;
            endPosition = wallTilemap.GetCellCenterWorld(endPositionCell);
            //endPosition = tilemap.CellToWorld(endPositionCell);
            //transform.position = endPosition;
            moving = StartCoroutine(MoveToNextCell(startingPosition, endPosition, moveTime));
        }
        


    }

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

        // Teleport on sides
        Vector3Int endPositionCell = wallTilemap.WorldToCell(endPosition);
        BoundsInt bounds = wallTilemap.cellBounds;
        if (endPositionCell.x < bounds.xMin)
        {
            endPositionCell.x = bounds.xMax;
            endPosition = wallTilemap.CellToWorld(endPositionCell);
        } else if (endPositionCell.x > bounds.xMax)
        {
            endPositionCell.x = bounds.xMin;
            endPosition = wallTilemap.GetCellCenterWorld(endPositionCell);
        }
        // Snap to the grid
        transform.position = endPosition;

        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pac")
        {

            //Destroy(collision.gameObject);
            gameController.PacCaught();
        }
        
    }
}

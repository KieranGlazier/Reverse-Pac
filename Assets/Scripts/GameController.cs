using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Tilemap wallTilemap = null;
    private bool[,] validPathTiles;
    [SerializeField]
    private Tilemap pelletTilemap = null;
    private Dictionary<Vector3Int, bool> pelletTiles = new Dictionary<Vector3Int, bool>();

    [SerializeField]
    private Vector3Int pacGridStartPosition = new Vector3Int(13, 7, 0);

    [SerializeField]
    private BoundsInt ghostSpawnBounds = new BoundsInt(11, 15, 0, 6, 3, 0);
    


    [SerializeField]
    private int pacLives = 3;

    private GameObject pac;
    private PacController pacController;
    private GameObject pacPrefab;
    Vector3Int ghostSpawnCell;
    private GameObject ghostPrefab;
    private GameObject ghost;
    GhostController ghostController;

    Vector3 ghostSpawnPosition;
    private GameObject lives;

    private bool isDestroying = false;


    private AudioSource audioSource;
 
    private AudioClip deathSound;

    // Start is called before the first frame update
    void Start()
    {
        pacPrefab = (GameObject)Resources.Load("Prefabs/Pac");
        ghostPrefab = (GameObject)Resources.Load("Prefabs/Ghost " + Random.Range(1, 4));
        lives = GameObject.Find("Lives");
        
        ghostSpawnCell = new Vector3Int(Random.Range(ghostSpawnBounds.xMin, ghostSpawnBounds.xMax),
            Random.Range(ghostSpawnBounds.yMin, ghostSpawnBounds.yMax), 0);
        ghostSpawnPosition = wallTilemap.GetCellCenterWorld(ghostSpawnCell);
        BuildWallMap();
        BuildPelletMap();
        StartCoroutine(StartLevel());

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        deathSound = (AudioClip)Resources.Load("Audio/Death Sound");

    }

    // Update is called once per frame
    void Update()
    {
        if (pelletTiles.Count == 0)
        {
            SceneManager.LoadScene("Game Over Scene");
        }
    }

    private void BuildWallMap()
    {
        BoundsInt tilemapBounds = wallTilemap.cellBounds;
        TileBase[] allTiles = wallTilemap.GetTilesBlock(tilemapBounds);

        // Note: validPathTiles ends up upsidedown compared to scene
        validPathTiles = new bool[tilemapBounds.size.x, tilemapBounds.size.y];
        for (int y = 0; y < tilemapBounds.size.y; y++)
        {
            int cellsInRow = tilemapBounds.size.x;
            for (int x = 0; x < tilemapBounds.size.x; x++)
            {

                TileBase tile = allTiles[x + y * cellsInRow];
                if (tile != null)
                {
                    validPathTiles[x, y] = false;
                }
                else
                {
                    validPathTiles[x, y] = true;
                }
            }
        }
    }

    private void BuildPelletMap()
    {
        BoundsInt tilemapBounds = pelletTilemap.cellBounds;
        TileBase[] allTiles = pelletTilemap.GetTilesBlock(tilemapBounds);
        for (int y = 0; y < tilemapBounds.size.y; y++)
        {
            int cellsInRow = tilemapBounds.size.x;
            for (int x = 0; x < tilemapBounds.size.x; x++)
            {

                TileBase tile = allTiles[x + y * cellsInRow];
                if (tile != null)
                {
                    if (tile.name == "Small Pellet")
                    {
                        pelletTiles.Add(new Vector3Int(x, y, 0), false);
                    }
                    else if (tile.name == "Large Pellet")
                    {
                        pelletTiles.Add(new Vector3Int(x, y, 0), true);

                    }
                }

                /*
                
                */
            }
        }
    }

    public bool[,] GetWallMap()
    {
        return validPathTiles;
    }

    public Dictionary<Vector3Int, bool> GetPelletTiles()
    {
        return pelletTiles;
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1f);
        SpawnPac();
        SpawnGhost();
    }

    private void SpawnPac()
    {
        audioSource.Play();
        Vector3 pacStartPosition = wallTilemap.CellToWorld(pacGridStartPosition);
        pac = Instantiate(pacPrefab, pacStartPosition, Quaternion.identity);
        pacController = pac.GetComponent<PacController>();
        pacController.wallTilemap = wallTilemap;
        //pacController.moveTime -= speedIncrease;
    }

    private void SpawnGhost()
    {
        
        
        ghost = Instantiate(ghostPrefab, ghostSpawnPosition, Quaternion.identity);
        ghostController = ghost.GetComponent<GhostController>();
        ghostController.wallTilemap = wallTilemap;
    }

    public void PacCaught()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        StopCoroutine(pacController.moving);
        pacController.isMoving = true;
        ghost.transform.position = ghostSpawnPosition;
        StopCoroutine(ghostController.moving);
        ghostController.isMoving = false;
        StartCoroutine(ShrinkObjectBeforeDestroy(pac));
        StartCoroutine(RespawnPac());
        
    }

    private IEnumerator RespawnPac()
    {
        Destroy(lives.transform.GetChild(lives.transform.childCount - 1).gameObject);
        yield return new WaitWhile(() => isDestroying);
        pacLives--;
        if (pacLives > 0)
        {
            
            SpawnPac();
        } else
        {
            SceneManager.LoadScene(StaticValues.GetNextLevel());
        }
    }

    private IEnumerator ShrinkObjectBeforeDestroy (GameObject gameObject)
    {
        isDestroying = true;
        Vector3 initialSize = gameObject.transform.localScale;

        float timer = 0f;
        float timeToDestroy = 1f;

        while (timer < timeToDestroy)
        {
            yield return null;

            timer += Time.deltaTime;
            gameObject.transform.localScale = Vector2.Lerp(initialSize, Vector3.zero, timer / timeToDestroy);
        }
        Destroy(pac);
        yield return new WaitForSeconds(2f);
        isDestroying = false;
    }
}

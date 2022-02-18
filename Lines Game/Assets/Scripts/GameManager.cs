using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	public static GameManager manager;

	[SerializeField]
	private Camera mainCamera;
	[SerializeField]
	private GameObject selection;

	[SerializeField]
	public int[,] grid;
	[SerializeField]
	private Color[] colors = new Color[6];

	private bool isMoving = false;
	private GameObject objectToMove;
	private PathFinding pathFinder;

	private void Awake()
	{
		manager = this;
		GenerateGrid();
	}
	void Start()
	{
		pathFinder = new PathFinding();
		SpawnBall();
	}

	private void Update()
	{
		BallControl();
	}

	void SpawnBall()
	{
		for(int i = 0; i < 3; i++)
		{
			int x, y;
			do
			{
				x = Random.Range(0, 9);
				y = Random.Range(0, 9);
			}
			while (grid[x, y] != -1);

			int c = Random.Range(0, 6);
			GameObject ball = ObjectPool.instance.GetPooledObject();

			if (ball != null)
			{
				ball.transform.position = new Vector2(x + 0.5f, y + 0.5f);
				ball.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colors[c];
				ball.SetActive(true);
				grid[x, y] = c;
			}
		}
	}

	void BallControl()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0f;
			/*Debug.Log(mouseWorldPos);*/
			if (mouseWorldPos.x > 0 && mouseWorldPos.x < 9 && mouseWorldPos.y > 0 && mouseWorldPos.y < 9)
			{
				int x = Mathf.FloorToInt(mouseWorldPos.x);
				int y = Mathf.FloorToInt(mouseWorldPos.y);

				Vector2 position = new Vector2(x + 0.5f, y + 0.5f);
				RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0f);

				if (objectToMove == null && isMoving == false)
				{
					if (hit.collider != null && hit.transform.name == "Ball child")
					{
						Debug.Log("hit bald child");
						objectToMove = hit.transform.parent.gameObject;
						GameObject tmp = Instantiate(selection, position, Quaternion.identity);
						tmp.transform.parent = objectToMove.transform;
					}
				}
				else if(hit.collider == null)
				{
					isMoving = true;
					int[] startPos = { Mathf.FloorToInt(objectToMove.transform.position.x), Mathf.FloorToInt(objectToMove.transform.position.y) };
					int[] endPos = { Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y) };
					List<Tile> path = pathFinder.FindPath(startPos[0], startPos[1], endPos[0], endPos[1]);

					if(path == null)
					{
						Debug.Log("Cant find path");
						return;
					}
					Destroy(objectToMove.transform.GetChild(1).gameObject);
					grid[endPos[0], endPos[1]] = grid[startPos[0], startPos[1]];
					grid[startPos[0], startPos[1]] = -1;
					StartCoroutine(Move(path));
				}
				

			}
		}
	}

	private void BallMove(Vector2 position)
	{
		
		if (isMoving && (Vector2)objectToMove.transform.position != position)
		{
			objectToMove.transform.position = Vector2.Lerp(position, objectToMove.transform.position, 0.001f * Time.deltaTime);
		}
	}

	private void GenerateGrid()
	{
		grid = new int[9, 9];
		for (int x = 0; x < 9; x++)
		{
			for (int y = 0; y < 9; y++)
			{
				grid[x, y] = -1;
			}
		}
	}

	IEnumerator Move(List<Tile> path)
	{
		int i = 0;
		while (i < path.Count)
		{
			BallMove(new Vector2(path[i].x + 0.5f, path[i].y + 0.5f));
			i++;
			yield return new WaitForSeconds(0.1f);
		}
		objectToMove = null;
		isMoving = false;
	}
	void UpdatePathFindingGrid()
	{
		for( int i = 0; i < 9; i ++)
		{
	
		}
	}
}

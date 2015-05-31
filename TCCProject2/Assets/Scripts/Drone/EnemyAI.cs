using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
	//public Transform target;
	public int moveSpeed;
	public int rotationSpeed;
	public PlayerController playerVoronoi;

	private Transform myTransform;

	public float floatHeight;
	public float liftForce;
	public float damping;
	public Rigidbody2D rb2D;
	LineRenderer linha;
	public Vector2 voronoi2Dpos;
	public Vector2 lua2Dpos;
	int municao;
	float reloadTime;
	public GameObject tiro;
	public Transform shooterPos;
	Vector2 direcao;
	enum EnemyPathStates{SEARCHING, PLAYERONSIGHT};
	enum EnemyActionStates{ATTACKING, RELOADING, DEFENDING};

	EnemyPathStates enemyPath;
	EnemyActionStates enemyAction;

	void Awake(){
		myTransform = transform;
	}
	
	// Use this for initialization
	void Start () {
		//GameObject go = GameObject.FindGameObjectWithTag("Player");
		//target = go.transform;
		municao = 500;
		rb2D = GetComponent<Rigidbody2D>();
		linha = this.GetComponent<LineRenderer>();
	}

	void Shoot(){
		var tiroS = (GameObject)Instantiate(tiro, shooterPos.position, Quaternion.identity);
		tiroS.rigidbody2D.velocity = new Vector2 (-100*direcao.x, -100*direcao.y);
		municao--;
	}
	
	// Update is called once per frame
	void Update () {
		if (enemyPath == EnemyPathStates.PLAYERONSIGHT){
			enemyAction = EnemyActionStates.ATTACKING;
		} else if (enemyPath == EnemyPathStates.SEARCHING){
			enemyAction = EnemyActionStates.DEFENDING;
		}

		if (municao <= 0){
			enemyAction = EnemyActionStates.RELOADING;
		}

		if (enemyAction == EnemyActionStates.ATTACKING){
			Shoot();
		}

		Debug.DrawLine(playerVoronoi.posicao, myTransform.position, Color.cyan);
		
		//look at target
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(playerVoronoi.posicao + myTransform.position), rotationSpeed * Time.deltaTime);
		
		//move towards target
		myTransform.position -= myTransform.forward * moveSpeed * Time.deltaTime;

		voronoi2Dpos = new Vector2 (playerVoronoi.posicao.x, playerVoronoi.posicao.y);
		lua2Dpos = new Vector2 (myTransform.position.x, myTransform.position.y);
		/*
		voronoi2Dpos.x = voronoi2Dpos.x - myTransform.position.x;
		voronoi2Dpos.y = voronoi2Dpos.y - myTransform.position.y;
		voronoi2Dpos.Normalize();*/
		direcao = (lua2Dpos - voronoi2Dpos);
		direcao.Normalize();

		RaycastHit2D hit = Physics2D.Raycast(lua2Dpos, direcao, 1000f, 1 << 10);
		if (hit) {
			linha.SetPosition(1, playerVoronoi.posicao);
			enemyPath = EnemyPathStates.PLAYERONSIGHT;
		}
		else 
		{
			linha.SetPosition(1, myTransform.position);
			enemyPath = EnemyPathStates.SEARCHING;
		}
		linha.SetPosition(0, myTransform.position);
	}
}
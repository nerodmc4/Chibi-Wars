using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public float OneDirection;
	public float MenorDirection;
	public float Speed;
	public Vector3 PosDirection;
	public List <Collider2D> visitados = new List<Collider2D>();

	public Vector2 vectorDir;
	int municao;
	float reloadTime;
	public GameObject tiro;
	public Transform shooterPos;
	public float raio;
	Vector2 direcao;
	public enum EnemyPathStates{SEARCHING, PLAYERONSIGHT, MOVING};
	public enum EnemyActionStates{ATTACKING, RELOADING, DEFENDING};

	public EnemyPathStates enemyPath;
	public EnemyActionStates enemyAction;

	void Awake(){
		myTransform = transform;
	}
	
	// Use this for initialization
	void Start () {
		//GameObject go = GameObject.FindGameObjectWithTag("Player");
		//target = go.transform;
		enemyPath = EnemyPathStates.SEARCHING;
		municao = 15;
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

		float Step = Speed * Time.deltaTime;
		switch(enemyPath){
			case EnemyPathStates.SEARCHING:
				Collider2D[] pointsVertex = Physics2D.OverlapCircleAll((Vector2) transform.position, raio, 1<<11);
				MenorDirection = Mathf.Infinity;
				foreach (Collider2D circulinho in pointsVertex){
					if(visitados.Contains(circulinho)){
						continue;
					}
					vectorDir = circulinho.transform.position - playerVoronoi.posicao;
					OneDirection = vectorDir.magnitude;
					if (OneDirection < MenorDirection){
						MenorDirection = OneDirection;
						PosDirection = circulinho.transform.position;
						enemyPath = EnemyPathStates.MOVING;
					}
				}
				
			break;

			case EnemyPathStates.MOVING:
				transform.position = Vector3.MoveTowards(transform.position, PosDirection, Step);
				if(transform.position == PosDirection){
					enemyPath = EnemyPathStates.SEARCHING;
					visitados.Add(Physics2D.OverlapPoint(transform.position, 1<<11));
				}
				

			break;

		}

		Debug.DrawLine(playerVoronoi.posicao, myTransform.position, Color.cyan);
		
		//look at target
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(playerVoronoi.posicao - myTransform.position), rotationSpeed * Time.deltaTime);
		
		//move towards target
		//myTransform.position -= myTransform.forward * moveSpeed * Time.deltaTime;

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
			if(enemyPath != EnemyPathStates.MOVING && enemyPath != EnemyPathStates.SEARCHING){
				enemyPath = EnemyPathStates.SEARCHING;
				visitados.Clear();
			}
		}
		linha.SetPosition(0, myTransform.position);
	}
}
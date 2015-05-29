using UnityEngine;
using System.Collections;

public class Drone01 : MonoBehaviour {
	PlayerController jogador;
	public Vector3 playerPos;

	void Start () {
		playerPos = jogador.posicao;

	}

	void Update () {
		playerPos = jogador.posicao;
	}
}

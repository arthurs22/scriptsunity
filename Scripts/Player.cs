using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	// primeiro as VAR "globais"
	//references
	[Header("References")]
	public Transform trans;
	public Transform modelTrans;
	public CharacterController characterController;

	//movement
	[Header("Movement")]
	[Tooltip("Units moved per second at maximum speed.")]
	public float movespeed = 24f; // uso pra pegar o valor min/max na Mathf.max/min

	[Tooltip("Time, in seconds, to reach maximum speed.")]
	public float timetoMaxSpeed = 0.26f; // tempo clicando na tecla para chegar na velocidade maxima.

	private float VelocityGainPerSecond { get {return movespeed / timetoMaxSpeed; }}  // calculo de V que tu ganha por segundo, no caso por segundo tu determina com timetoMaxSpeed

	[Tooltip("Time, in seconds, to go from maximum speed to stationary.")]
	public float timeToLoseMaxSpeed = 0.2f; // Tempo que leva pra parar 

	private float VelocityLossPerSecond {get {return movespeed / timeToLoseMaxSpeed;}} // calculo de quanto tempo tu leva para parar

	[Tooltip("Multiplier for momentum when attempting to move in a direction opposite the current traveling direction")]
	public float reverseMomentumMultiplier = 2.2f; // Momento ( "resistencia" que tu tem ao virar de direção)

	private Vector3 movementVelocity = Vector3.zero; //Vector3 é um objeto com tres valores, e .zero define x y z como 0.

	// Death and respawning

	[Header("Death and Respawning")]
	[Tooltip("How long after the players death, in seconds, before they are respawned?")]
	public float respawnWaitTime = 2f;
	private bool dead = false;
	private Vector3 spawnPoint;
	private Quaternion spawnRotation;

	void Start()
    {
    	spawnPoint = trans.position;
    	spawnRotation = modelTrans.rotation;

    }
	
	public void Die()
	{
		if (!dead)
		{
			dead = true;
			Invoke("Respawn",respawnWaitTime);
			movementVelocity = Vector3.zero;
			enabled = false;
			characterController.enabled = false;
			modelTrans.gameObject.SetActive(false);
		}
	}    

	public void Respawn()
	{
		dead = false;
		trans.position = spawnPoint;
		enabled = true;
		characterController.enabled = true;
		modelTrans.gameObject.SetActive(true);
		modelTrans.rotation = spawnRotation;
	}
    private void Movement() // Teclas, calculo para ganhar velocidade e aceleração.
    {
    	if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // W 
    	{
    	// se movementVelocity na Z for maior OU igual a zero, movementVelocity vira o menor valor entre movespeed OU aquele calculo. Para casos que não esteja se movento ou ja esteja no eixo positivo
    		if (movementVelocity.z >= 0 )
    			movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime); 
    	// Se for menor que zero, movementVelocity vira o menor valor entre 0 OU aquele calculo. Para casos que você esteja andando no eixo negativo
    		else
    			movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
    	}
   		else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) //  S
   		{
   			if (movementVelocity.z > 0) 
   				movementVelocity.z = Mathf.Max(0 , movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
   			else
   				movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);
   		}
   		else	// Se tu tiver velocidade no eixo Z mas não estiver apertando nenhuma tecla 
   		{
   			if(movementVelocity.z > 0)
   			movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);

   			else
   				movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond *Time.deltaTime);
   		}
   		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) // D
    	{
    		if (movementVelocity.x >= 0 )
    			movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);
    		else
    			movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
    	}
   		else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // A
   		{
   			if (movementVelocity.x > 0)
   				movementVelocity.x = Mathf.Max(0 , movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
   			else
   				movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);
   		}
   		else	
   		{
   			if(movementVelocity.x > 0)
   			movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);

   			else
   				movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond *Time.deltaTime);
   		}
   		if (movementVelocity.x != 0 || movementVelocity.z != 0)
   		{
   			characterController.Move(movementVelocity * Time.deltaTime);

   			modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity),.18F);
   		}
    }

    private void Update()
    {
    	Movement();
    	if (Input.GetKeyDown(KeyCode.T))
    		Die();
    }
}

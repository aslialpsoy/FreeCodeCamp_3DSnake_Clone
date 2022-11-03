using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerDirection direction;
 
    [HideInInspector]
    public float step_Length = 0.2f; 

    [HideInInspector]
    public float movement_Frequency = 0.1f; // how many times we can move in a sec

    private float counter; // counts and adds up the time btw each frame
    private bool move; 
 
    [SerializeField]
    private GameObject tailPrefab; 

    private List<Vector3> delta_Position; 

    private List<Rigidbody> nodes; // snake's body parts

    private Rigidbody main_Body; // snake
    private Rigidbody head_Body;
    private Transform tr; 

    private bool create_Node_At_Tail;

    void Awake()
    {
        tr = transform;
        main_Body = GetComponent<Rigidbody>(); // Snake

        InitSnakeNodes();
        InitPlayer();

        delta_Position = new List<Vector3>(){

            new Vector3(-step_Length, 0f), // -dx, LEFT
            new Vector3(0f, step_Length),  // dy, UP
            new Vector3(step_Length, 0f),  // dx, RIGHT
            new Vector3(0f, -step_Length), // -dy, DOWN 
        };
    }

    // Update is called once per frame
    void Update(){
        CheckMovementFrequency();
        
    }
    void FixedUpdate(){
        if(move){ // move is true
            move = false;
            Move(); // move the snake by one increment
        }

    }

    void InitSnakeNodes(){ // initialize the nodes

        nodes = new List<Rigidbody>(); 
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>()); //head
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>()); //node
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>()); //tail

        head_Body = nodes[0];
    }
    
    void SetDirectionRandom(){
        int dirRandom = Random.Range(0,(int)PlayerDirection.COUNT);
        direction = (PlayerDirection)dirRandom;
    }

    void InitPlayer(){ // initialize the player
        
        SetDirectionRandom();

        switch(direction){
            case PlayerDirection.RIGHT:

                nodes[1].position = nodes[0].position - new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position - new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;

            case PlayerDirection.LEFT:
                nodes[1].position = nodes[0].position + new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position + new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;

            case PlayerDirection.UP:
                nodes[1].position = nodes[0].position - new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position - new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;

            case PlayerDirection.DOWN:
                nodes[1].position = nodes[0].position + new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position + new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
        }       
    }

    void Move(){

        Vector3 dPosition = delta_Position[(int)direction];
        Vector3 parentPos = head_Body.position;
        Vector3 prevPosition;

        main_Body.position = main_Body.position + dPosition;
        head_Body.position = head_Body.position + dPosition;

        for (int i = 1; i < nodes.Count; i++) { //0 is head, 1 is node.starting from the nodes
            
            prevPosition = nodes[i].position;
             
            nodes[i].position = parentPos;
            parentPos = prevPosition;
        } 
        // check if we need to create a new node couse we ate a fruit
        if (create_Node_At_Tail){

            create_Node_At_Tail = false; // not to create multiple nodes

            GameObject newNode = Instantiate(tailPrefab, nodes[nodes.Count-1].position,
                                            Quaternion.identity);
            newNode.transform.SetParent(transform, true);
            nodes.Add(newNode.GetComponent<Rigidbody>());                                
        }
    }


    void CheckMovementFrequency (){

        counter += Time.deltaTime;
        if(counter >= movement_Frequency){
            counter = 0f;
            move = true; 
        }
    }

    public void SetInputDirection(PlayerDirection dir){
        if(dir == PlayerDirection.UP && direction == PlayerDirection.DOWN ||
           dir == PlayerDirection.DOWN && direction == PlayerDirection.UP ||
           dir == PlayerDirection.RIGHT && direction == PlayerDirection.LEFT ||
           dir == PlayerDirection.LEFT && direction == PlayerDirection.RIGHT){
            
             return;
        }
        direction = dir;

        ForceMove();
    }
    void ForceMove(){ // to move without waiting for the movement frequency. right away pressing the button
        counter = 0;
        move = false;
        Move();
    }
    void OnTriggerEnter(Collider target){

        if(target.tag == Tags.FRUIT){
            target.gameObject.SetActive(false); // the fruit dissapears after collision
            create_Node_At_Tail = true;
            GameplayController.instance.IncreaseScore();
            AudioManager.instance.Play_PickUpSound();
        }
        if(target.tag == Tags.WALL || target.tag == Tags.BOMB || target.tag == Tags.TAIL){
           Time.timeScale = 0f; //stop the game
           AudioManager.instance.Play_DeadSound();

        }
    }

     
} // class

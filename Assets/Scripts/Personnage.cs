using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour

{
    PlayerInputReader inputReader;
    private Vector2 direction;

    // Start is called beore the first frame update
    void Start()
    {
        inputReader = GetComponent<PlayerInputReader>();

        inputReader.BS.callback += Sauter;
        inputReader.LS_m.callback += Deplacer;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction);
    }
    void Sauter(){
        Debug.Log("Saut");
    }
    void Deplacer(Vector2 direction){

        Debug.Log("Deplacer");

    }
}

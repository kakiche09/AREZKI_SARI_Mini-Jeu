using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public PlayerInputReader inputReader        { get; protected set; }

    [SerializeField] private GameObject canvasInfo; 
    [SerializeField] private GameObject menuPause;  

    // Start is called before the first frame update
    void Start()
    {
        inputReader = GetComponent<PlayerInputReader>();

        inputReader.Menu.callback += ChangerCanvas;

        Time.timeScale = 0f;        
        
        menuPause.SetActive(true); 
        canvasInfo.SetActive(false);
        
    }

    // Changer de Canvas entre le CanvasInfo et le menuPause
    void ChangerCanvas()
    {
        // Si le canvasInfo est actif, on le cache et on affiche le menuPause
        // Sinon, on affiche le canvasInfo et on cache le menuPause
        if (canvasInfo.activeSelf)
        {
            canvasInfo.SetActive(false);
            menuPause.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            canvasInfo.SetActive(true);
            menuPause.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}

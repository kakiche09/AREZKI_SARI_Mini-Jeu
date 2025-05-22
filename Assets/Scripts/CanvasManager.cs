using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private PlayerInputReader inputReader;
    [SerializeField] private GameObject canvasInfo; // Référence au Canvas d'information
    [SerializeField] private GameObject menuPause;  // Référence au menu de pause

    // Start is called before the first frame update
    void Start()
    {
        inputReader = GetComponent<PlayerInputReader>();
        inputReader.Menu.callback += ChangerCanvas; // Associer la touche Menu à la méthode ChangerCanvas
        Time.timeScale = 0f;         // Mettre le temps en pause
        menuPause.SetActive(true); // Activer le menu de pause
        canvasInfo.SetActive(false); // Désactiver le Canvas d'information
        
    }

    // Changer de Canvas entre le CanvasInfo et le menuPause
    void ChangerCanvas()
    {
        if (canvasInfo.activeSelf)
        {
            canvasInfo.SetActive(false);
            menuPause.SetActive(true);
            Time.timeScale = 0f; // Mettre le temps en pause
        }
        else
        {
            canvasInfo.SetActive(true);
            menuPause.SetActive(false);
            Time.timeScale = 1f; // Reprendre le temps
        }
    }
}

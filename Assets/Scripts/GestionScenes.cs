using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTouches : MonoBehaviour
{
    private PlayerInputReader inputReader;

    // Variable pour savoir si le jeu est actuellement en pause ou non
    private bool isPaused = false;

    void Start()
    {
        // Récupérer l'objet PlayerInputReader
        inputReader = GetComponent<PlayerInputReader>();
        inputReader.Menu.callback += ChangerScene;

    }


    void ChangerScene()
    {
        // Si on est dans le menu, basculer vers la scène principale
        if (SceneManager.GetActiveScene().name == "menu")
        {
            SceneManager.LoadScene("scenePrincipale");
            // Reprendre le temps quand on sort du menu
            Time.timeScale = 1f;
            isPaused = false;
        }
        // Si on est dans la scène principale, revenir au menu
        else if (SceneManager.GetActiveScene().name == "scenePrincipale")
        {
            SceneManager.LoadScene("menu");
            // Mettre le temps en pause quand on entre dans le menu
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
}

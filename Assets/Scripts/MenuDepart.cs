using UnityEngine;

public class MenuTouches : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;       // Le menu d'accueil (Canvas ou Panel)
    [SerializeField] private GameObject joueur;       // Ton personnage ou la logique du jeu

    private bool jeuDemarre = false;

    void Start()
    {
        Time.timeScale = 0f; // Gèle le jeu

        // Force tous les animators à utiliser le temps non-scalé
        Animator[] animators = menuUI.GetComponentsInChildren<Animator>();
        foreach (var animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        joueur.SetActive(false); // désactive le joueur au démarrage (optionnel)
    }

    void Update()
    {
        if (!jeuDemarre && Input.anyKeyDown)
        {
            CommencerJeu();
        }
    }

    void CommencerJeu()
    {
        jeuDemarre = true;

        // Désactiver le menu après une petite animation ? (optionnel)
        menuUI.SetActive(false);

        joueur.SetActive(true);      // Active le joueur
        Time.timeScale = 1f;         // Reprend le temps
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestionnaireUI : MonoBehaviour
{
    [SerializeField] private Slider sliderVie;
    [SerializeField] private Slider sliderArme;
    [SerializeField] private GameObject coeur2;
    [SerializeField] private GameObject coeur3;
    [SerializeField] private TMP_Text texteScore;
    [SerializeField] private TMP_Text chronoTexte;
    [SerializeField] private Personnage personnage;
    [SerializeField] private GameObject arme1;
    [SerializeField] private GameObject arme2;
    [SerializeField] private GameObject arme3;
    [SerializeField] private AudioClip sonAmbiance;
    [SerializeField] AudioSource audioSource2;

    public float TempsPasse             { get; protected set; }



    void Start()
    {
        TempsPasse = 0f;
        audioSource2 = GetComponent<AudioSource>();
        audioSource2.clip = sonAmbiance;
        audioSource2.loop = true;
        audioSource2.Play();
    }

    void Update()
    {
        if (personnage == null) return;

        // Mettre à jour les éléments de l'interface utilisateur
        MettreAJourCoeurs();
        MettreAJourScore();
        MettreAJourArmes();
        MettreAJourVie();
        MettreAJourChrono();
    }

    // Mettre à jour la barre de vie
    private void MettreAJourVie()
    {
        sliderVie.value = personnage.viesRestantes;
    }

    // Mettre à jour le score
    private void MettreAJourScore()
    {
        texteScore.text = "Score : " + personnage.score;
    }

    // Mettre à jour les coeurs
    private void MettreAJourCoeurs()
    {
        int v = personnage.coeursRestants;
        if (v == 2) coeur3.SetActive(false);
        if (v == 1) coeur2.SetActive(false);
    }

    // Mettre à jour les armes
    private void MettreAJourArmes()
    {

        arme1.SetActive(false);
        arme2.SetActive(false);
        arme3.SetActive(false);

        // Activer seulement l'image correspondante à l'arme sélectionnée
        switch (personnage.typeAttaque)
        {
            case Personnage.TypeAttaque.Directe:
                arme1.SetActive(true);
                break;
            case Personnage.TypeAttaque.Spirale:
                arme2.SetActive(true);
                break;
            case Personnage.TypeAttaque.Poursuite:
                arme3.SetActive(true);
                break;
        }
        // Mettre à jour la barre d'arme
        sliderArme.value = personnage.progressionArme;
    }
    
    // Mettre à jour le chronomètre
    private void MettreAJourChrono()
    {
        TempsPasse += Time.deltaTime;
        int minutes = Mathf.FloorToInt(TempsPasse / 60F);
        int secondes = Mathf.FloorToInt(TempsPasse - minutes * 60);
        chronoTexte.text = string.Format("{0:0}:{1:00}", minutes, secondes);
    }
}
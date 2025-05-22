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
    [SerializeField] private Personnage personnage;

    [SerializeField] private GameObject arme1;
    [SerializeField] private GameObject arme2;
    [SerializeField] private GameObject arme3;

    // Texte et chronomètre
    public TMP_Text chronoTexte;
    public float TempsPasse;



    void Update()
    {
        if (personnage == null) return;

        MettreAJourCoeurs();
        MettreAJourScore();
        MettreAJourArmes();
        MettreAJourVie();
        MettreAJourChrono();
    }

    private void MettreAJourVie()
    {
        sliderVie.value = personnage.ViesRestantes;
    }

    private void MettreAJourScore()
    {
        texteScore.text = "Score : " + personnage.score;
    }

    private void MettreAJourCoeurs()
    {
        int v = personnage.CoeursRestants;
        if (v == 2) coeur3.SetActive(false);
        if (v == 1) coeur2.SetActive(false);
    }

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
    
    private void MettreAJourChrono()
    {
        TempsPasse += Time.deltaTime;
        int minutes = Mathf.FloorToInt(TempsPasse / 60F);
        int secondes = Mathf.FloorToInt(TempsPasse - minutes * 60);
        chronoTexte.text = string.Format("{0:0}:{1:00}", minutes, secondes);
    }
}
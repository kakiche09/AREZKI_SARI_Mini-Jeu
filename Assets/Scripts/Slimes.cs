using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Slimes : MonoBehaviour
{
    enum Etats { Recherche, Combat, Mort };
    [SerializeField] private Etats etatActuel;

    void Start()
    {
        etatActuel = Etats.Recherche;

    }

    void Update()
    {
        switch (etatActuel)
        {
            case Etats.Recherche: Update_EtatRecherche(); break;
            case Etats.Combat: Update_EtatCombat(); break;
            case Etats.Mort: Update_EtatMort(); break;
        }
    }

    void Update_EtatRecherche()
    {
        if (Chercherjoueur())
        {
            etatActuel = Etats.Combat;
        }
    }
    void Update_EtatCombat()
    {
        if (!Chercherjoueur())
        {
            etatActuel = Etats.Recherche;
        }
    }
    void Update_EtatMort()
    {
        Destroy(gameObject);
    }

    Transform Chercherjoueur()
    {
        float rayonDetection = 20f;    
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rayonDetection);
        float distanceMin = Mathf.Infinity;
        Transform ennemiLePlusProche = null;
        foreach (var collider  in colliders)
        {
            Personnage potentielEnnemi = collider.GetComponent<Personnage>();
            if (potentielEnnemi != null)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);

                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    ennemiLePlusProche = collider.transform;
                }
            }
        }
        return ennemiLePlusProche;
    }
}

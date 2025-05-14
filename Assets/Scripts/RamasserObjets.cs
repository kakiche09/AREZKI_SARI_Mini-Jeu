using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RamasserObjets : MonoBehaviour
{
    public enum TypeObjet { Score, Vie, Coeur, Arme }
    public TypeObjet typeObjet;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Personnage joueur = other.GetComponent<Personnage>();
        if (joueur != null)
        {
            switch (typeObjet)
            {
                case TypeObjet.Score:
                
                    joueur.score += Random.Range(10, 40);
                    break;
                case TypeObjet.Vie:

                    joueur.AjouterVie(Random.Range(10,30));
                    break;
                case TypeObjet.Coeur:
                    joueur.AjouterCoeur(1);
                    break;
                case TypeObjet.Arme:
                    joueur.AugmenterBarreArme(Random.Range(2, 4));
                    break;
            }
            Destroy(gameObject);
        }
    }
}

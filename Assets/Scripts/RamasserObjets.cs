using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RamasserObjets : MonoBehaviour
{
    public enum TypeObjet { Score, Vie, Coeur, Arme } 
    public TypeObjet typeObjet;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Vérifie si le joueur entre en collision avec l'objet
        Personnage joueur = collider.GetComponent<Personnage>();
        Debug.Log("Collision avec : " + collider.name);
        // Si le joueur entre en collision avec l'objet, on lui donne un bonus
        if (joueur != null)
        {   
            Debug.Log("Objet ramassé : " + typeObjet);
            // On vérifie le type d'objet et on applique le bonus correspondant
            switch (typeObjet)
            {
                case TypeObjet.Score:

                    joueur.score += Random.Range(10, 40);
                    break;
                case TypeObjet.Vie:

                    joueur.AjouterVie(Random.Range(10, 30));
                    break;
                case TypeObjet.Coeur:
                    joueur.AjouterCoeur(1);
                    break;
                case TypeObjet.Arme:
                    joueur.AugmenterBarreArme(Random.Range(4, 6));
                    break;
            }
 
            Destroy(gameObject);
        }
    }
}

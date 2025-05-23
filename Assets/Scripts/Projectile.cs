using System;
using UnityEngine;




public class Projectile : MonoBehaviour
{
    public enum TypeProjectile { Directe, Spirale, Poursuite }

    public float vitesse;
    public float tempsDeVie;
    public float rayonSpirale;
    public float frequenceSpirale;
    public float tempsDeVieSpirale;
    public float acceleration;

    public GameObject lanceur               { get; protected set; }
    public float vitesseActuelle            { get; protected set; }
    public Vector2 direction                { get; protected set; }
    public Transform cible                  { get; protected set; }
    public float angleSpirale               { get; protected set; }
    public TypeProjectile typeProjectile    { get; protected set; }


    
    private void Start()
    {
        // Initialiser les attributs
        SetAttributes();
    }


    void Update()
    {
        switch (typeProjectile)
        {
            // Appeler la méthode de déplacement appropriée en fonction du type de projectile
            case TypeProjectile.Directe:
                DeplacementLigne();
                break;
            case TypeProjectile.Spirale:
                DeplacementSpirale();
                break;
            case TypeProjectile.Poursuite:
                DeplacementPoursuite();
                break;
        }
    }

    // Initialiser le projectile avec la direction, la cible, le type et le lanceur
    // directionInitiale : direction de départ du projectile    
    // cibleSuivie : cible que le projectile doit suivre
    // type : type de projectile (Directe, Spirale, Poursuite)
    // lancePar : le lanceur du projectile
    // On détruit le projectile après un certain temps
    // tempsDeVie : durée de vie du projectile
    public void Initialiser(Vector2 directionInitiale, Transform cibleSuivie, TypeProjectile type, GameObject lancePar)
    {
        // Initialiser les attributs du projectile
        direction = directionInitiale.normalized;
        cible = cibleSuivie;
        typeProjectile = type;
        vitesseActuelle = 0f;
        lanceur = lancePar;
        Destroy(gameObject, tempsDeVie);
    }

    // Déplacement en ligne droite
    public void DeplacementLigne()
    {
        GetComponent<Rigidbody2D>().velocity = direction * vitesse;
    }

    // Déplacement en spirale
    private void DeplacementSpirale()
    {
        angleSpirale += 360f * frequenceSpirale * Time.deltaTime;
        Vector2 perpendiculaire = new Vector2(-direction.y, direction.x);
        float offset = Mathf.Sin(angleSpirale * Mathf.Deg2Rad) * rayonSpirale;
        Vector2 spiraleOffset = perpendiculaire * offset;
        Vector2 mouvement = (direction * vitesse * Time.deltaTime) + (spiraleOffset * Time.deltaTime);
        transform.Translate(mouvement, Space.World);
    }

    // Déplacement en suivant la cible
    private void DeplacementPoursuite()
    {
        // Déplacement vers la cible en la suivant
        if (cible == null) return;

        Vector2 directionVoulue = ((Vector2)cible.position - (Vector2)transform.position).normalized;
        direction = Vector2.Lerp(direction, directionVoulue, Time.deltaTime).normalized;
        vitesseActuelle = Mathf.Min(vitesseActuelle + acceleration * Time.deltaTime, vitesse);
        transform.Translate(direction * vitesseActuelle * Time.deltaTime, Space.World);
    }

    // Fonction pour gérer les collisions du projectile
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == lanceur) return;

        // Vérifier si le projectile touche un ennemi ou un joueur
        Slimes cibleSlime = collision.gameObject.GetComponent<Slimes>();
        if (cibleSlime != null)
        {
            int degats = UnityEngine.Random.Range(10, 20);
            cibleSlime.SubirDegats(degats);
            Destroy(gameObject);
            return;
        }
        // Vérifier si le projectile touche un joueur
        Personnage ciblePerso = collision.gameObject.GetComponent<Personnage>();
        if (ciblePerso != null)
        {
            int degats = UnityEngine.Random.Range(5, 10);
            ciblePerso.SubirDegats(degats);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    // fonction SetAttributes qui permet de définir les attributs du projectile
    private void SetAttributes()
    {
        vitesse = 8f;
        tempsDeVie = 4f;
        rayonSpirale = 10f;
        frequenceSpirale = 4f;
        tempsDeVieSpirale = 2f;
        acceleration = 50f;
    }
}

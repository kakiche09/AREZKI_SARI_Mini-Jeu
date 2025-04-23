using System;
using UnityEngine;



public class Projectile : MonoBehaviour
{
    public enum TypeProjectile { Directe, Spirale, Poursuite }
    [SerializeField] private float vitesse = 8f;
    [SerializeField] private float tempsDeVie = 4f;
    [SerializeField] private float rayonSpirale = 10f;
    [SerializeField] private float frequenceSpirale = 4f; 

    private float vitesseActuelle;
    [SerializeField] private float acceleration = 50f; // Accélération en unités/sec²
    private Vector2 direction;
    private Transform cible;
    private float angleSpirale;
    private TypeProjectile typeProjectile;

    void Update()
    {
        switch (typeProjectile)
        {
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

    public void Initialiser(Vector2 directionInitiale, Transform cibleSuivie, TypeProjectile type)
    {
        direction = directionInitiale.normalized;
        cible = cibleSuivie;
        typeProjectile = type;
        vitesseActuelle = 0f;
        Destroy(gameObject, tempsDeVie);
    }

    public void DeplacementLigne()
    {
        transform.Translate(direction * vitesse * Time.deltaTime, Space.World);
    }

    private void DeplacementSpirale()
    {
        angleSpirale += 360f * frequenceSpirale * Time.deltaTime;        
        Vector2 perpendiculaire = new Vector2(-direction.y, direction.x);
        float offset = Mathf.Sin(angleSpirale * Mathf.Deg2Rad) * rayonSpirale;
        Vector2 spiraleOffset = perpendiculaire * offset;
        Vector2 mouvement = (direction * vitesse * Time.deltaTime) + (spiraleOffset * Time.deltaTime);
        transform.Translate(mouvement, Space.World);
    }

    private void DeplacementPoursuite()
    {
        if (cible == null) return;

        Vector2 directionVoulue = ((Vector2)cible.position - (Vector2)transform.position).normalized;
        direction = Vector2.Lerp(direction, directionVoulue, Time.deltaTime).normalized;
        vitesseActuelle = Mathf.Min(vitesseActuelle + acceleration * Time.deltaTime, vitesse);
        transform.Translate(direction * vitesseActuelle * Time.deltaTime, Space.World);
    }
   
}

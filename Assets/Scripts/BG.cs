using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BG : MonoBehaviour
{
    public Color aquaColor = new Color(0f, 1f, 1f, 0.5f);

    // Dictionnaire pour sauvegarder les couleurs originales
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    void Start()
    {
        ApplyAquaTint();
    }

#if UNITY_EDITOR
    void Update()
    {
        ApplyAquaTint();
    }
#endif

    void ApplyAquaTint()
    {
        foreach (Transform child in transform)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = Application.isPlaying ? rend.material : rend.sharedMaterial;

                if (mat != null && mat.HasProperty("_Color"))
                {
                    // Sauvegarder la couleur si ce n’est pas déjà fait
                    if (!originalColors.ContainsKey(rend))
                    {
                        originalColors[rend] = mat.color;
                    }

                    mat.color = aquaColor;
                }
            }
        }
    }

    public void ResetColors()
    {
        foreach (var pair in originalColors)
        {
            Renderer rend = pair.Key;
            Color originalColor = pair.Value;

            if (rend != null)
            {
                Material mat = Application.isPlaying ? rend.material : rend.sharedMaterial;
                if (mat != null && mat.HasProperty("_Color"))
                {
                    mat.color = originalColor;
                }
            }
        }

        // Vider le dictionnaire si tu veux éviter des réinitialisations multiples
        originalColors.Clear();
    }
}

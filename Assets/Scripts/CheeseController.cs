using UnityEngine;
using System.Collections;

public class CheeseController : MonoBehaviour
{
    private int cheeseLife;
    public int clickDamage = 100;
    [SerializeField, Range(0f, 0.9f)]
    private float bottomColliderInset = 0.25f;

    private Vector3 initialScale;
    private CheeseSpawner cheeseSpawner;
    private Coroutine shrinkCoroutine;
    private SpriteRenderer cheeseSpriteRenderer;
    

    private void Start()
    {
        cheeseLife = 1000;
        initialScale = transform.localScale;
        cheeseSpawner = FindAnyObjectByType<CheeseSpawner>();
        FitColliderToSprite();
    }

    private void FitColliderToSprite()
    {
        cheeseSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (cheeseSpriteRenderer == null || cheeseSpriteRenderer.sprite == null || boxCollider == null)
            return;

        Vector2[] vertices = cheeseSpriteRenderer.sprite.vertices;
        if (vertices.Length == 0)
            return;

        Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        foreach (Vector2 vertex in vertices)
        {
            Vector2 point = transform.InverseTransformPoint(
                cheeseSpriteRenderer.transform.TransformPoint(vertex));
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        min.y += (max.y - min.y) * Mathf.Clamp(bottomColliderInset, 0f, 0.9f);
        boxCollider.offset = (min + max) * 0.5f;
        boxCollider.size = max - min;
    }

    private void OnMouseDown()
    {
        if (cheeseLife <= 0)
            return;

        cheeseLife -= clickDamage;

        cheeseSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Vector3 popupPosition = cheeseSpriteRenderer.bounds.center;
        popupPosition.y = cheeseSpriteRenderer.bounds.max.y;

        DamagePopup.Create(popupPosition, clickDamage);

        if (cheeseLife <= 0)
        {
            gameObject.SetActive(false);
            if (cheeseSpawner != null)
                cheeseSpawner.SpawnCheese();
            Destroy(gameObject);
            return;
        }

        if (shrinkCoroutine != null)
            StopCoroutine(shrinkCoroutine);
        shrinkCoroutine = StartCoroutine(ShrinkCheese());
    }

    private IEnumerator ShrinkCheese()
    {
        Vector3 targetScale = initialScale * ((float) cheeseLife / 1000);
        Vector3 currentScale = transform.localScale;

        float duration = 0.15f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
    

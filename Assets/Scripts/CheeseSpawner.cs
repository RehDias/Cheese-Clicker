using UnityEngine;

public class CheeseSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject cheesePrefab;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Sprite[] cheeseSprites;

    private int spriteIndex = 0;

    public void SpawnCheese()
    {
        if (cheesePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Assign Cheese Prefab and Spawn Point on CheeseSpawner.", this);
            return;
        }

        GameObject newCheese = Instantiate(cheesePrefab, spawnPoint.position, Quaternion.identity);
                
        SpriteRenderer spriteRenderer =
            newCheese.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null && cheeseSprites != null && cheeseSprites.Length > 0)
        {
            Sprite newSprite = cheeseSprites[spriteIndex];
            if (newSprite != null)
                spriteRenderer.sprite = newSprite;
            spriteIndex = (spriteIndex + 1) % cheeseSprites.Length;
        }
    }
}

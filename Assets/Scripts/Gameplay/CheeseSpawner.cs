using UnityEngine;

public class CheeseSpawner : MonoBehaviour
{
    [Header("Spawn Location")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform cheesesParent;

    private int spriteIndex;

    public void SpawnCheese(int maxHealth)
    {
        GameAssets assets = GameAssets.Instance;
        if (assets == null || assets.CheesePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Assign GameAssets' Cheese Prefab and the spawner's Spawn Point.", this);
            return;
        }

        GameObject cheese = Instantiate(
            assets.CheesePrefab, spawnPoint.position, Quaternion.identity, cheesesParent);
        CheeseController cheeseController = cheese.GetComponent<CheeseController>();
        
        if (cheeseController != null)
            cheeseController.InitializeHealth(maxHealth);
        else 
            Debug.Log("The Cheese prefab needs a CheeseController component.", cheese);

        ApplyNextSprite(cheese, assets.CheeseSprites);
    }

    private void ApplyNextSprite(GameObject cheese, Sprite[] sprites)
    {
        SpriteRenderer spriteRenderer = cheese.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null || sprites == null || sprites.Length == 0)
            return;

        spriteIndex %= sprites.Length;
        if (sprites[spriteIndex] != null)
            spriteRenderer.sprite = sprites[spriteIndex];

        spriteIndex = (spriteIndex + 1) % sprites.Length;
    }
}

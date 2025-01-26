using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlatformController : MonoBehaviour
{
    public List<Sprite> platformTextures = new List<Sprite>();

    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = platformTextures[Random.Range(0, platformTextures.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

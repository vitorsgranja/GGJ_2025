using System.Collections.Generic;
using UnityEngine;

public class ForegroundPlantController : MonoBehaviour
{
    public List<Sprite> images;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = images[Random.Range(0, images.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

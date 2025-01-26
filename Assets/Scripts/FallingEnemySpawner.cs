using UnityEngine;

public class FallingEnemySpawner : MonoBehaviour {
  [Header("Spawn Settings")]
  public GameObject[] enemyPrefabs; // Array de prefabs de inimigos para spawnar aleatoriamente
  public float leftSpawnLimit = -5f; // Limite esquerdo para spawn
  public float rightSpawnLimit = 5f; // Limite direito para spawn
  public float spawnHeight = 10f; // Altura do spawn (de onde os inimigos vão cair)

  private void Start() {
    InvokeRepeating(nameof(SpawnEnemy), 0f, 1f);
  }

  /// <summary>
  /// Spawna um inimigo aleatório em uma posição dentro dos limites especificados.
  /// </summary>
  public void SpawnEnemy() {
    if(enemyPrefabs.Length == 0) {
      Debug.LogWarning("Nenhum prefab de inimigo configurado no spawner.");
      return;
    }

    // Escolhe um inimigo aleatório do array de prefabs
    GameObject selectedEnemy = enemyPrefabs[Random.Range(0,enemyPrefabs.Length)];

    // Calcula uma posição X aleatória dentro dos limites
    float randomX = Random.Range(leftSpawnLimit,rightSpawnLimit);

    // Define a posição do spawn usando o X aleatório e a altura definida
    Vector3 spawnPosition = new Vector3(randomX,spawnHeight,0f);

    // Instancia o inimigo selecionado na posição calculada
    Instantiate(selectedEnemy,spawnPosition,Quaternion.identity);
  }

  private void OnDrawGizmos() {
    // Desenha os limites no editor para referência
    Gizmos.color = Color.green;
    Gizmos.DrawLine(new Vector3(leftSpawnLimit,spawnHeight,0),new Vector3(rightSpawnLimit,spawnHeight,0));
  }
}
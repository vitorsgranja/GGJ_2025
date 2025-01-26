using UnityEngine;
using TMPro;

public class BubbleTextEffect : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    [SerializeField] private float waveSpeed = 2f;
    [SerializeField] private float waveHeight = 5f;
    [SerializeField] private float characterSpacing = 1f;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component not found!");
            return;
        }
        textMesh.enableVertexGradient = true;
    }

    private void OnDisable()
    {
        ResetTextPosition();
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            var charInfo = textInfo.characterInfo[i];
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            
            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                var offset = Mathf.Sin(Time.time * waveSpeed + i * characterSpacing) * waveHeight;
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, offset, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void ResetTextPosition()
    {
        if (textMesh == null) return;

        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            var charInfo = textInfo.characterInfo[i];
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            
            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}

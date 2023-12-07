using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;

public class RoadPainter : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Ball ball;
    [SerializeField] private MeshRenderer mesh;

    public int paintedRoadCount = 0;

    private void Start() {
        mesh.material.color = levelManager.paintColor;
        Paint(levelManager.defaultBallRoadTile,.5f,0f);
        ball.OnMoveStart += OnBallMoveHandler;
    }
    private void OnBallMoveHandler(List<RoadTile> roadTiles, float totalDuration)
    {
        float stepDuration = totalDuration / roadTiles.Count;
        for (int i = 0; i < roadTiles.Count; i++)
        {
            RoadTile roadTile = roadTiles[i];
            if (!roadTile.isPainted)
            {
                float duration = totalDuration / 2f;
                float delay = i * (stepDuration / 2f);
                Paint(roadTile, duration, delay);

                //Check if Level Completed:
                if (paintedRoadCount == levelManager.roadTiles.Count)
                {
                    Debug.Log("Level Completed");
                    // Load new level..
                }
            }
        }
    }
    private void Paint(RoadTile roadTile, float duration,float delay)
     {
        StartCoroutine(LerpColor(roadTile.meshRenderer.material, levelManager.paintColor, duration, delay));
        roadTile.isPainted = true;
        paintedRoadCount++;
     }
    private IEnumerator LerpColor(Material material, Color targetColor, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Color initialColor = material.color;

        while (elapsed < duration)
        {
            material.color = Color.Lerp(initialColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        material.color = targetColor; 
    }
}

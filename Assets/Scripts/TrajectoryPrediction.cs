using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryPrediction : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxPhysicsIteration = 10;
    
    private Scene simulationScene;
    private PhysicsScene physicsScene;
    private Dictionary<Transform, Transform> spawnedObstacles = new Dictionary<Transform, Transform>();
    private Puck ghostPuck;
    
    public void CreatePhysicsScene(Level level)
    {
        if (!SceneManager.GetSceneByName("Simulation").IsValid())
        {
            simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            physicsScene = simulationScene.GetPhysicsScene();
        }

        CreateGhostObstacles(level.levelObstacles);
        CreateGhostWorldElement(level.border);
        CreateGhostWorldElement(level.ground);

    }

    private void CreateGhostObstacles(List<Obstacle> obstacles)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            GameObject ghostObstacle = Instantiate(obstacle.gameObject, obstacle.transform.position, obstacle.transform.rotation);
            Renderer[] ghostRenderers = ghostObstacle.GetComponentsInChildren<Renderer>();
            foreach (Renderer ghostRenderer in ghostRenderers)
            {
                if (ghostRenderer)
                {
                    ghostRenderer.enabled = false;
                }
            }

            SceneManager.MoveGameObjectToScene(ghostObstacle, simulationScene);
            spawnedObstacles.Add(obstacle.transform, ghostObstacle.transform);
        }
    }

    private void CreateGhostWorldElement(WorldElement worldElement)
    {
        GameObject ghostElement = Instantiate(worldElement.gameObject, worldElement.transform.position, worldElement.transform.rotation);
        Renderer ghostElementRenderer = ghostElement.GetComponent<Renderer>();
        
        if (ghostElementRenderer)
        {
            ghostElementRenderer.enabled = false;
        }
        SceneManager.MoveGameObjectToScene(ghostElement, simulationScene);
        spawnedObstacles.Add(worldElement.transform, ghostElement.transform);
    }

    public void PredictTrajectory(Puck puckPrefab, Vector3 startingPos, Vector3 force)
    {
        if (physicsScene.IsValid())
        {
            if (ghostPuck == null)
            {
                ghostPuck = Instantiate(puckPrefab);
                ghostPuck.isGhost = true;
                SceneManager.MoveGameObjectToScene(ghostPuck.gameObject, simulationScene);
            }

            ghostPuck.transform.position = startingPos;
            ghostPuck.transform.rotation = Quaternion.identity;
            ghostPuck.AddImpulse(force);

            lineRenderer.positionCount = maxPhysicsIteration;

            for (int i = 0; i < maxPhysicsIteration; i++)
            {
                physicsScene.Simulate(Time.fixedDeltaTime);
                lineRenderer.SetPosition(i, ghostPuck.transform.position);
            }
            Destroy(ghostPuck.gameObject);
        }
    }

    public void StopPrediction()
    {
        lineRenderer.positionCount = 0;
    }

    public void RemoveObstacle(Obstacle obstacle)
    {
        Destroy(spawnedObstacles[obstacle.transform].gameObject);
        spawnedObstacles.Remove(obstacle.transform);
    }

    public void CleanLevel()
    {
        foreach (Transform ghostObstacle in spawnedObstacles.Values)
        {
            Destroy(ghostObstacle.gameObject);
        }
        spawnedObstacles.Clear();
    }
}

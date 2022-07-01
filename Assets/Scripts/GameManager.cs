using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Puck puckPrefab;
    public int restingTries{ get; private set; }
    
    [SerializeField] private Transform puckStartingPos;
    [SerializeField] private TrajectoryPrediction prediction;
    [SerializeField] private Button puckButton;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject winImage;
    [SerializeField] private GameObject looseImage;
    [SerializeField] private Text triesText;
    [SerializeField] private int forcePower;

    private Level currentLevel;
    private GameObject puckGameObject;
    private Rigidbody puckRigidbody;
    private string discText = " discs left";
    
    [HideInInspector] public bool shouldPredict = false;
    [HideInInspector] public Puck currentPuck;
    [HideInInspector] public Vector3 force = new Vector3();
    [HideInInspector] public bool isLastPuck = false;
    
    public bool isPuckReady { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void StartLevel(Level level)
    {
        endScreen.SetActive(false);
        winImage.SetActive(false);
        looseImage.SetActive(false);
        currentLevel = Instantiate(level.gameObject).GetComponent<Level>();
        InstantiatePuck();
        prediction.CreatePhysicsScene(currentLevel);
        restingTries = 4;
        triesText.text = restingTries + discText;
        puckButton.interactable = false;
    }
    
    private void FixedUpdate()
    {
        if (shouldPredict)
        {
            prediction.PredictTrajectory(puckPrefab, puckStartingPos.position, force*forcePower);
        }
    }

    public void StopPrediction()
    {
        prediction.StopPrediction();
    }

    public void AddForce(Vector3 forceDirection)
    {
        if (puckRigidbody == null)
        {
            return;
        }
        restingTries--;
        triesText.text = restingTries + discText;
        isPuckReady = false;
        puckButton.interactable = true;
        if (restingTries == 0)
        {
            isLastPuck = true;
            puckButton.interactable = false;
        }
        currentPuck.AddImpulse(forceDirection*forcePower, isLastPuck);
    }

    private void InstantiatePuck()
    {
        puckGameObject = Instantiate(puckPrefab.gameObject, puckStartingPos.transform.position, puckStartingPos.transform.rotation, puckStartingPos);
        puckRigidbody = puckGameObject.GetComponent<Rigidbody>();
        currentPuck = puckGameObject.GetComponent<Puck>();
        isPuckReady = true;
        puckGameObject.SetActive(true);
        isLastPuck = false;
    }

    public void ReplacePuck()
    {
        if (isPuckReady)
        {
            return;
        }
        
        if (restingTries <= 0)
        {
            return;
        }
        puckRigidbody.velocity = Vector3.zero;
        puckGameObject.transform.position = puckStartingPos.position;
        puckGameObject.transform.rotation = Quaternion.identity;
        currentPuck.TriggerBumpAnimation();
        isPuckReady = true;
        ShowHealthBars(false);
        puckButton.interactable = false;
    }

    public void OnhittingObstacle(Obstacle obstacle)
    {
        obstacle.HittingObstacle();
    }

    public void ShowHealthBars(bool isVisible)
    {
        foreach (Obstacle obs in currentLevel.levelObstacles)
        {
            if (obs is DestroyableObstacle)
            {
               ((DestroyableObstacle)obs).ShowHealthBar(isVisible);
            }
        }
    }

    public void OnObstacleDestroy(Obstacle obstacle)
    {
        if (obstacle.gameObject.name.Contains("Chest"))
        {
            EndScreen(true);
        }
        else
        {
            obstacle.gameObject.SetActive(false);
            prediction.RemoveObstacle(obstacle);
        }
    }

    public void EndScreen(bool hasWon)
    {
        shouldPredict = false;
        prediction.CleanLevel();
        Destroy(currentLevel.gameObject);
        puckGameObject.SetActive(false);
        
        if (hasWon)
        {
            winImage.SetActive(true);
        }
        else
        {
            looseImage.SetActive(true);
        }
        
        endScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

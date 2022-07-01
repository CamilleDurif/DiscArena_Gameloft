using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public Level level;

    public void LaunchLevel()
    {
        GameManager.Instance.StartLevel(level);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 direction;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            int id = touch.fingerId;
            
            if (EventSystem.current.IsPointerOverGameObject(id) || !GameManager.Instance.isPuckReady)
            {
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    if (direction.magnitude > 80)
                    {
                        GameManager.Instance.force = new Vector3(-direction.x, 0, -direction.y).normalized;
                        GameManager.Instance.shouldPredict = true;
                        GameManager.Instance.ShowHealthBars(true);
                    }
                    else
                    {
                        GameManager.Instance.StopPrediction();
                        GameManager.Instance.shouldPredict = false;
                        GameManager.Instance.ShowHealthBars(false);
                    }
                    break;

                case TouchPhase.Ended:
                    if (direction.magnitude > 80)
                    {
                        GameManager.Instance.AddForce(new Vector3(-direction.x, 0, -direction.y).normalized);
                    }
                    GameManager.Instance.StopPrediction();
                    GameManager.Instance.shouldPredict = false;
                    direction = Vector2.zero;
                    break;
            }
        }
    }
}

using UnityEngine;

public class RobotController : MonoBehaviour
{
    private Transform nextPosition;

    void Update()
    {
        if (nextPosition != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(nextPosition.position.x, transform.position.y, nextPosition.position.z),
                3 * Time.deltaTime);
        }
    }

    public void SetNextPosition(Transform value)
    {
        nextPosition = value;
        transform.Rotate(0, 90, 0);
    }
}
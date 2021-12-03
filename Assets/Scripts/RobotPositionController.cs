using UnityEngine;

public class RobotPositionController : MonoBehaviour
{
    [SerializeField]
    Transform nextPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Robot")
            other.gameObject.GetComponent<RobotController>().SetNextPosition(nextPosition);
    }
}
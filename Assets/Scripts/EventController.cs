using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField]
    private EventType eventType;

    public EventType GetEventType()
    {
        return eventType;
    }
}
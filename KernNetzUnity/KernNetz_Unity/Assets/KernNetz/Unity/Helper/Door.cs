using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform start, end, current, doorPrefab;
    float timer;

    bool _state = false;
    public bool DoorState
    {
        get { return _state; }
        set 
        { 
            _state = value; 
            timer = 0f;
            current = _state ? end : start;
        }
    }

    void Awake()
    {
        current = end;
    }

    void Update()
    {
        timer += Time.deltaTime * 0.01f;
        if(timer > 1) timer = 1;
        doorPrefab.position = Vector3.Lerp(doorPrefab.position, current.position, timer);
    }
}

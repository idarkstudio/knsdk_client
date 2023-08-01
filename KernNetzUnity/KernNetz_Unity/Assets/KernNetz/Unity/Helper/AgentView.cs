using KernNetz;
using UnityEngine;
using FigNetCommon;
using FigNet.KernNetz;
using System.Collections;

public class AgentView : MonoBehaviour
{

    private NetworkEntity networkEntity;
    private Animator _animator;

    // animation IDs
    private int _animIDSpeed;

    private float _animationBlend;

    public WayPoints wayPoints;
    public float speed = 2f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public float PauseBetweenWayPoints = 0.0f;

    public FNShort ActiveWayPointIndex = default;
    private short CurrentWayPoint = 0;

    private Coroutine patrolling = null;

    IEnumerator Patrol()
    {
        var delay = new WaitForSeconds(PauseBetweenWayPoints); //pause when you reach the target
        while (true)
        {
            for (; CurrentWayPoint < wayPoints.waypoints.Length; CurrentWayPoint++)
            {
                Transform thisWaypoint = wayPoints.waypoints[CurrentWayPoint];
                Transform nextWaypoint;
                ActiveWayPointIndex.Value = CurrentWayPoint;  // automatically will be synced with all client
                if (CurrentWayPoint + 1 < wayPoints.waypoints.Length) nextWaypoint = wayPoints.waypoints[CurrentWayPoint + 1];
                else nextWaypoint = wayPoints.waypoints[0];
                float moveTime = (nextWaypoint.position - thisWaypoint.position).magnitude;
                transform.LookAt(nextWaypoint.position);
                for (float t = 0f; t < moveTime; t += Time.deltaTime * 3f)
                {
                    transform.position = Vector3.Lerp(thisWaypoint.position, nextWaypoint.position, t / moveTime);
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    
                    yield return 0;
                }
                _animator.SetFloat(_animIDSpeed, 0);
               
                transform.position = nextWaypoint.position;
                yield return delay;
            }
            CurrentWayPoint = 0;
            ActiveWayPointIndex.Value = CurrentWayPoint;
        }
    }


    private void Awake()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animator = GetComponent<Animator>();

        wayPoints = GameObject.FindObjectOfType<WayPoints>();
        KN.OnMasterClientUpdate += OnMasterClientUpdated;
    }

    private void OnDestroy()
    {
        KN.OnMasterClientUpdate -= OnMasterClientUpdated;
    }


    private void OnMasterClientUpdated(bool isMaster) 
    {
        if (isMaster)
        {
            StopAllCoroutines();
            patrolling = StartCoroutine(Patrol());
        }
    }

    void Start()
    {
        networkEntity = GetComponent<KernNetzView>().NetworkEntity;


        if (networkEntity.IsMine)
        {
            ActiveWayPointIndex = new FNShort();
            networkEntity.SetNetProperty<FNShort>(1, ActiveWayPointIndex, FigNet.Core.DeliveryMethod.Reliable);
        }
        else
        {
            networkEntity.GetNetProperty<FNShort>(1,FigNet.Core.DeliveryMethod.Reliable, (value) =>
            {
                ActiveWayPointIndex = value;
                ActiveWayPointIndex.OnValueChange += ActiveWayPointIndex_OnValueChange;

            });
        }

        if (networkEntity.IsMine)
        {
            StopAllCoroutines();
            patrolling = StartCoroutine(Patrol());
        }
    }
    // 
    private void ActiveWayPointIndex_OnValueChange(short obj)
    {
        CurrentWayPoint = obj;
    }

    void Update()
    {
        //if (!EN.IsMasterClient) return;

        _animationBlend = Mathf.Lerp(_animationBlend, speed, Time.deltaTime * SpeedChangeRate);

        // for non owners set animation state [where as for owner this state has been set in patrol function]
        if (networkEntity != null && !networkEntity.IsMine)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
        }

    }
}

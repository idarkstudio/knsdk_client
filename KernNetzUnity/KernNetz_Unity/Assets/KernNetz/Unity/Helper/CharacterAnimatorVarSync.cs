using KernNetz;
using UnityEngine;
using FigNetCommon;

public class CharacterAnimatorVarSync : MonoBehaviour
{

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    private NetworkEntity networkEntity;

    public FNFloat Speed = default;
    public FNFloat MotionSpeed = default;

    // for bool props
    public FNShort Jump = default;
    //public FNShort FreeFall = default;
    //public FNShort Grounded = default;

    [SerializeField]
    private Transform PlayerCameraRoot;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private Animator _animator;

    public bool IsMine = true;

    public enum NetworkProperties : byte
    {
        Speed = 1,
        MotionSpeed = 2,
        Jump = 3,
        FreeFall = 4,
        Grounded = 5
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }


    // Start is called before the first frame update
    void Start()
    {
        if (IsMine)
        {
            var vCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera");
            vCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = PlayerCameraRoot;
        }

        _animator = GetComponent<Animator>();
        AssignAnimationIDs();

        networkEntity = GetComponent<KernNetzView>().NetworkEntity;
        if (networkEntity.IsMine)
        {
            Speed = new FNFloat();
            networkEntity.SetNetProperty<FNFloat>((byte)NetworkProperties.Speed, Speed, FigNet.Core.DeliveryMethod.Unreliable);
        }
        else
        {
            networkEntity.GetNetProperty<FNFloat>((byte)NetworkProperties.Speed, FigNet.Core.DeliveryMethod.Unreliable, (value) =>
            {
                Speed = value;
                Speed.OnValueChange += Speed_OnValueChange;

            });
        }


        if (networkEntity.IsMine)
        {
            MotionSpeed = new FNFloat();
            networkEntity.SetNetProperty<FNFloat>((byte)NetworkProperties.MotionSpeed, MotionSpeed, FigNet.Core.DeliveryMethod.Unreliable);
        }
        else
        {
            networkEntity.GetNetProperty<FNFloat>((byte)NetworkProperties.MotionSpeed, FigNet.Core.DeliveryMethod.Unreliable, (value) =>
            {
                MotionSpeed = value;
                MotionSpeed.OnValueChange += MotionSpeed_OnValueChange;
            });
        }


        if (networkEntity.IsMine)
        {
            Jump = new FNShort();
            networkEntity.SetNetProperty<FNShort>((byte)NetworkProperties.Jump, Jump, FigNet.Core.DeliveryMethod.Reliable);
        }
        else
        {
            networkEntity.GetNetProperty<FNShort>((byte)NetworkProperties.Jump, FigNet.Core.DeliveryMethod.Reliable, (value) =>
            {
                Jump = value;
                Jump.OnValueChange += Jump_OnValueChange;
            });
        }
    }

    private void Speed_OnValueChange(float obj)
    {
        _animator.SetFloat(_animIDSpeed, obj);
    }

    private void MotionSpeed_OnValueChange(float obj)
    {
        _animator.SetFloat(_animIDMotionSpeed, obj);
    }

    private void FreeFall_OnValueChange(short obj)
    {
        _animator.SetBool(_animIDFreeFall, obj == 1 ? true : false);
    }

    private void Grounded_OnValueChange(short obj)
    {
        _animator.SetBool(_animIDGrounded, obj == 1 ? true : false);
    }

    private void Jump_OnValueChange(short obj)
    {
        _animator.SetBool(_animIDJump, obj == 1 ? true : false);
    }

    private void Update()
    {
        if (!IsMine)
        {
            GroundedCheck();
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        var _grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, _grounded);


        if (_grounded)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);
        }
        else
        {
            _animator.SetBool(_animIDFreeFall, true);
        }
    }

}

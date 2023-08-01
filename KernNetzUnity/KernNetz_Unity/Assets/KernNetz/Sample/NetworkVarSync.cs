using KernNetz;
using FigNetCommon;
using UnityEngine;

public class NetworkVarSync : MonoBehaviour
{
    NetworkEntity networkEntity;
	public FNFloat Animtor_Mag = default;
	public FNFloat Animtor_Mag2 = default;
	public FNFloat Animtor_Mag3 = default;
	public FNFloat Animtor_Mag4 = default;


	public FNShort Short = default;
	public FNInt Int = default;
	public FNString String = default;
	public FNVector2 Vec2 = default;
	public FNVector3 pos = default;
	public FNVector4 Vec4 = default;


	public bool IsMine = false;
	// Start is called before the first frame update
	void Start()
    {
		networkEntity = GetComponent<KernNetzView>().NetworkEntity;
		if (networkEntity.IsMine)
		{
			Animtor_Mag = new FNFloat();
			networkEntity.SetNetProperty<FNFloat>(5, Animtor_Mag, FigNet.Core.DeliveryMethod.Unreliable);
			Animtor_Mag.OnValueChange += Animtor_Mag_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNFloat>(5, FigNet.Core.DeliveryMethod.Unreliable, (anim_mag) =>
			{
				Animtor_Mag = anim_mag;
				Animtor_Mag.OnValueChange += Animtor_Mag_OnValueChange;

			});
		}
		

		if (networkEntity.IsMine)
		{
			Animtor_Mag2 = new FNFloat();
			networkEntity.SetNetProperty<FNFloat>(1, Animtor_Mag2, FigNet.Core.DeliveryMethod.Reliable);
			Animtor_Mag2.OnValueChange += Animtor_Mag2_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNFloat>(1, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				Animtor_Mag2 = anim_mag;
				Animtor_Mag2.OnValueChange += Animtor_Mag2_OnValueChange;
			});
		}

		if (networkEntity.IsMine)
		{
			Animtor_Mag3 = new FNFloat();
			networkEntity.SetNetProperty<FNFloat>(6, Animtor_Mag3, FigNet.Core.DeliveryMethod.ReliableUnordered);
			Animtor_Mag3.OnValueChange += Animtor_Mag3_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNFloat>(6, FigNet.Core.DeliveryMethod.ReliableUnordered, (anim_mag) =>
			{
				Animtor_Mag3 = anim_mag;
				Animtor_Mag3.OnValueChange += Animtor_Mag3_OnValueChange;

			});
		}

		if (networkEntity.IsMine)
		{
			Animtor_Mag4 = new FNFloat();
			networkEntity.SetNetProperty<FNFloat>(7, Animtor_Mag4, FigNet.Core.DeliveryMethod.Sequenced);
			Animtor_Mag4.OnValueChange += Animtor_Mag4_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNFloat>(7, FigNet.Core.DeliveryMethod.Sequenced, (anim_mag) =>
			{
				Animtor_Mag4 = anim_mag;
				Animtor_Mag4.OnValueChange += Animtor_Mag4_OnValueChange;

			});
		}


		if (networkEntity.IsMine)
		{
			Short = new FNShort();
			networkEntity.SetNetProperty<FNShort>(8, Short, FigNet.Core.DeliveryMethod.Reliable);
            Short.OnValueChange += Short_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNShort>(8, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				Short = anim_mag;
				Short.OnValueChange += Short_OnValueChange;

			});
		}


		if (networkEntity.IsMine)
		{
			Int = new FNInt();
			networkEntity.SetNetProperty<FNInt>(9, Int, FigNet.Core.DeliveryMethod.Reliable);
            Int.OnValueChange += Int_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNInt>(9, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				Int = anim_mag;
				Int.OnValueChange += Int_OnValueChange;
			});
		}


		if (networkEntity.IsMine)
		{
			String = new FNString();
			networkEntity.SetNetProperty<FNString>(10, String, FigNet.Core.DeliveryMethod.Reliable);
            String.OnValueChange += String_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNString>(10, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				String = anim_mag;
				String.OnValueChange += String_OnValueChange;
			});
		}


		if (networkEntity.IsMine)
		{
			Vec2 = new FNVector2();
			networkEntity.SetNetProperty<FNVector2>(11, Vec2, FigNet.Core.DeliveryMethod.Reliable);
            Vec2.OnValueChange += Vec2_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNVector2>(11, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				Vec2 = anim_mag;
				Vec2.OnValueChange += Vec2_OnValueChange;
			});
		}


		if (networkEntity.IsMine)
		{
			pos = new FNVector3();
			networkEntity.SetNetProperty<FNVector3>(12, pos, FigNet.Core.DeliveryMethod.Reliable);
            pos.OnValueChange += Pos_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNVector3>(12, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				pos = anim_mag;
				pos.OnValueChange += Pos_OnValueChange;
			});
		}

		if (networkEntity.IsMine)
		{
			Vec4 = new FNVector4();
			networkEntity.SetNetProperty<FNVector4>(13, Vec4, FigNet.Core.DeliveryMethod.Reliable);
            Vec4.OnValueChange += Vec4_OnValueChange;
		}
		else
		{
			networkEntity.GetNetProperty<FNVector4>(13, FigNet.Core.DeliveryMethod.Reliable, (anim_mag) =>
			{
				Vec4 = anim_mag;
				Vec4.OnValueChange += Vec4_OnValueChange;
			});
		}

	}

    private void Vec4_OnValueChange(FNVec4 obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On Vec4 changed {obj.X}|{obj.Y}|{obj.Z}|{obj.W}");
	}

    private void Pos_OnValueChange(FNVec3 obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On Vec3 changed {obj.X}|{obj.Y}|{obj.Z}");
	}

    private void Vec2_OnValueChange(FNVec2 obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On Vec2 changed {obj.X}|{obj.Y}");
	}

    private void String_OnValueChange(string obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On String changed {obj}");
	}

    private void Int_OnValueChange(int obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On Int changed {obj}");
	}

    private void Short_OnValueChange(short obj)
    {
		Debug.LogError($"{gameObject.name} Relaible On Short changed {obj}");
	}

    private void Animtor_Mag_OnValueChange(float obj)
	{
		Debug.LogError($"{gameObject.name} UNRELIABLE On Anim Mag changed {obj}");
	}

	private void Animtor_Mag2_OnValueChange(float obj)
	{
		Debug.LogError($"{gameObject.name} RELIABLE On Anim Mag2 changed {obj}");
	}

	private void Animtor_Mag3_OnValueChange(float obj)
	{
		Debug.LogError($"{gameObject.name} RELIABLE UNORDERED On Anim Mag2 changed {obj}");
	}

	private void Animtor_Mag4_OnValueChange(float obj)
	{
		Debug.LogError($"{gameObject.name} SEQUENCED On Anim Mag2 changed {obj}");
	}
	// Update is called once per frame
	void Update()
    {
		if (!IsMine) return;
		if (Input.GetKeyDown(KeyCode.T))
		{
			Animtor_Mag.Value = Random.Range(1, 10);
		}

		if (Input.GetKeyDown(KeyCode.Y))
		{
			Animtor_Mag2.Value = Random.Range(1, 10);
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			Animtor_Mag3.Value = Random.Range(1, 10);
		}

		if (Input.GetKeyDown(KeyCode.I))
		{
			Animtor_Mag4.Value = Random.Range(1, 10);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Short.Value = (short)Random.Range(1, 100);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Int.Value = Random.Range(1, 100);
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			String.Value = Random.Range(1, 100).ToString();
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Vec2.SetValue(new FNVec2(Random.Range(1, 100), Random.Range(1, 100)));
		}

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			pos.SetValue(new FNVec3(Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100)));
		}

		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			Vec4.SetValue(new FNVec4(Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100), Random.Range(1, 100)));
		}
	}
}

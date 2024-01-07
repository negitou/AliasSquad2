using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMove : NetworkBehaviour
{
	//移動ステータス
	private const float runSpeed = 10.0f;
	private const float walkMultiplier = 0.5f;
	private const float crouchMultiplier = 0.4f;
	private const float backMultiplier = 0.9f;
	private const float forceMultiplier = 300;
	private const int jumpPower = 10;

	//カメラオブジェクト
	[SerializeField]
	private GameObject camera;

	[SerializeField]
	private GameObject fpsPlayerObj;

	//攻撃用スクリプト 重量取得
	private PlayerAttack playerAttack;
	//物理
	private Rigidbody rigidbody;
	//速度取得
	public Vector3 CurrentVelocity { get { return rigidbody.velocity; } }
	//向き
	private Vector3 dir;
	//ジャンプフラグ
	private bool jump = false;
	//接地フラグ
	private bool onGround = false;

	//public bool GetOnGround { get { return onGround; } }

	void Reset ()
	{
		camera = transform.Find ("PlayerCamera").gameObject;
		fpsPlayerObj = camera.transform.Find ("FPSPlayerObj").gameObject;
	}

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update ()
	{
        if (IsLocalPlayer) {
            //設置判定
            if (Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y + 0.29f, transform.position.z), 0.3f, LayerMask.GetMask(new string[] { "Default" , "Object"}))) {
                onGround = true;
            } else {
                onGround = false;
            }
        }
    }


	void FixedUpdate ()
	{
        if (IsLocalPlayer)
        {
            //重力
            if (!onGround)
            {
                rigidbody.velocity += new Vector3(0, 2 * -9.81f * Time.fixedDeltaTime, 0);
            }
            //ジャンプ動作
            if (jump)
            {
                rigidbody.velocity += new Vector3(0, jumpPower, 0);
                jump = false;
            }
        }
	}

	//移動
	public void Move (float vertical, float horizontal, bool walk = false, bool crouch = false)
	{
		dir = Vector3.zero;

		dir += transform.right * horizontal + transform.forward * vertical;
		dir.Normalize ();
			
		if (vertical < 0) {
			dir -= transform.forward * vertical * (1.0f - backMultiplier);
		}
		if (!crouch) {

			if (!walk) {
				rigidbody.AddForce (forceMultiplier * (new Vector3 (runSpeed * dir.x - rigidbody.velocity.x, 0, runSpeed * dir.z - rigidbody.velocity.z)));
			} else {
				rigidbody.AddForce (forceMultiplier * (new Vector3 (runSpeed * walkMultiplier * dir.x - rigidbody.velocity.x, 0, runSpeed * walkMultiplier * dir.z - rigidbody.velocity.z)));
			}
		} else {
			rigidbody.AddForce (forceMultiplier * (new Vector3 (runSpeed * crouchMultiplier * dir.x - rigidbody.velocity.x, 0, runSpeed * crouchMultiplier * dir.z - rigidbody.velocity.z)));
		}

	}

	//ジャンプ
	public void Jump ()
	{
		if (onGround) {
			jump = true;
		}
	}

	public void Crouch ()
	{

	}

	public void Turned (float x, float y)
	{
		//振り向き
		transform.eulerAngles += new Vector3 (0, x, 0);
		//カメラ
		float camRot = -y;
		if (camera.transform.localEulerAngles.x + camRot > 89 || camera.transform.localEulerAngles.x + camRot < -89) {
			if (camera.transform.localEulerAngles.x >= 270) {
				camera.transform.localEulerAngles = new Vector3 (Mathf.Clamp (camera.transform.localEulerAngles.x + camRot - 360, -89, 89), 0, 0);
			} else {
				camera.transform.localEulerAngles = new Vector3 (Mathf.Clamp (camera.transform.localEulerAngles.x + camRot, -89, 89), 0, 0);
			}
		} else {
			camera.transform.eulerAngles += new Vector3 (camRot, 0, 0);
		}

	}
}

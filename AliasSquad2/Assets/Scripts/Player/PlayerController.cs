using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
	// コンポーネント取得用
	private Rigidbody rigidbody;

	// 移動関係
	[SerializeField]
	private float runSpeed = 15;
	[SerializeField]
	private float walkSpeed = 8;
	[SerializeField]
	private float crouchWalkSpeed = 6;
	[SerializeField]
	private float forceMultiplier = 200;
	[SerializeField]
	private float jumpPower = 10;
	private Vector3 dir = Vector3.zero;

	// 視点関係 (振り向きなど)
	[SerializeField]
	private GameObject camera;

	// 攻撃関係
	private int selectPreset;
	private int selectSlotIndex;
	private int beforeSelectSlotIndex;

	// アイテムスロット
	// 0 = Main     4 = Smoke
	// 1 = Sub      5 = Flash
	// 2 = Knife    6 = Molotov
	// 3 = Grenade  7 = C4
	private int[] itemSlot = new int[8];

	[SerializeField]private GameObject[] mainWeaponObject = new GameObject[4];
	[SerializeField]private GameObject[] subWeaponObject = new GameObject[1];
	[SerializeField]private GameObject knifeObject;
	[SerializeField]private GameObject[] nadeObject = new GameObject[4];

	enum MainWeapon
	{
		AK47,
		M4,
		AWP,
		SG,
	}

	enum SubWeapon
	{
		Pistol,
	}

	// ステータス
	[SerializeField]
	private int maxHealth;
	private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

	// フラグ
	private bool bot = false;
	private bool control = false;
	private bool death = false;

	private bool onGround = false;
	private bool jump = false;

	void Awake ()
	{

	}

	void Start ()
	{
		if (!IsLocalPlayer) {
			return;
		}
		rigidbody = GetComponent<Rigidbody>();
		selectSlotIndex = 2;
		for (int i = 0; i < itemSlot.Length; i++) {
			itemSlot [i] = 0;
		}
		itemSlot [2] = 1;
	}

	void Update ()
	{
		if (!IsLocalPlayer) {
			return;
		}

		//死んでいないとき
		if (!death) {
			//移動
			if (onGround) {
				dir = Vector3.zero;
			}
			dir += transform.right * Input.GetAxis ("Horizontal") + transform.forward * Input.GetAxis ("Vertical");
			dir.Normalize ();

			//ジャンプ
			if (Input.GetKeyDown (KeyCode.Space) && onGround) {
				jump = true;
			}

			//設置判定
			if (Physics.CheckSphere (new Vector3 (transform.position.x, transform.position.y + 0.29f, transform.position.z), 0.3f, LayerMask.GetMask ("Default"))) {
				onGround = true;
			} else {
				onGround = false;
			}

			//振り向き
			transform.eulerAngles += new Vector3 (0, Input.GetAxis ("Mouse X"), 0);
			//カメラ
			float camRot = -Input.GetAxis ("Mouse Y");
			if (camera.transform.localEulerAngles.x + camRot > 89 || camera.transform.localEulerAngles.x + camRot < -89) {
				if (camera.transform.localEulerAngles.x >= 270) {
					camera.transform.localEulerAngles = new Vector3 (Mathf.Clamp (camera.transform.localEulerAngles.x + camRot - 360, -89, 89), 0, 0);
				} else {
					camera.transform.localEulerAngles = new Vector3 (Mathf.Clamp (camera.transform.localEulerAngles.x + camRot, -89, 89), 0, 0);
				}
			} else {
				camera.transform.eulerAngles += new Vector3 (camRot, 0, 0);
			}

            //武器選択
            //ホイール
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                beforeSelectSlotIndex = selectSlotIndex;
                do
                {
                    selectSlotIndex++;
                    if (selectSlotIndex > 7)
                    {
                        selectSlotIndex -= 8;
                    }
                } while (itemSlot[selectSlotIndex] == 0);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                beforeSelectSlotIndex = selectSlotIndex;
                do
                {
                    selectSlotIndex--;
                    if (selectSlotIndex < 0)
                    {
                        selectSlotIndex += 8;
                    }
                } while (itemSlot[selectSlotIndex] == 0);
            }
            //数値キー
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (itemSlot[0] != 0)
                {
                    beforeSelectSlotIndex = selectSlotIndex;
                    selectSlotIndex = 0;
                }
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                if (itemSlot[1] != 0)
                {
                    beforeSelectSlotIndex = selectSlotIndex;
                    selectSlotIndex = 1;
                }
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                if (itemSlot[2] != 0)
                {
                    beforeSelectSlotIndex = selectSlotIndex;
                    selectSlotIndex = 2;
                }
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                if (itemSlot[3] != 0)
                {
                    beforeSelectSlotIndex = selectSlotIndex;
                    if (selectSlotIndex >= 3 && selectSlotIndex <= 6)
                    {

                        if (itemSlot[3] != 0)
                        {

                        }
                    }
                }
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                if (itemSlot[7] != 0)
                {
                    beforeSelectSlotIndex = selectSlotIndex;
                    selectSlotIndex = 7;
                }
            }

            switch (selectSlotIndex) {
			case 0:
				//mainWeaponObject[itemSlot [selectSlotIndex]].SetActive = true;
				break;
			case 1:
				switch (itemSlot [selectSlotIndex]) {
				case (int)SubWeapon.Pistol:

					break;
				default:
					break;
				}
				break;
			case 2:
				if(itemSlot [selectSlotIndex] == 1){

				}
				break;
			case 3:
				if(itemSlot [selectSlotIndex] >= 1){
					
				}
				break;
			case 4:
				if(itemSlot [selectSlotIndex] >= 1){

				}
				break;
			case 5:
				if(itemSlot [selectSlotIndex] >= 1){

				}
				break;
			case 6:
				if(itemSlot [selectSlotIndex] >= 1){

				}
				break;
			case 7:
				if(itemSlot [selectSlotIndex] == 1){

				}
				break;
			default:
				
				break;
			}
		}
		Debug.Log(selectSlotIndex);
	}

	void FixedUpdate ()
	{
		if (!IsLocalPlayer) {
			return;
		}
		rigidbody.AddForce (forceMultiplier * (new Vector3 (runSpeed * dir.x - rigidbody.velocity.x, 0, runSpeed * dir.z - rigidbody.velocity.z)));

		if (!onGround) {
			rigidbody.velocity += new Vector3 (0, 2 * -9.81f * Time.fixedDeltaTime, 0);
		}

		if (jump) {
			rigidbody.velocity += new Vector3 (0, jumpPower, 0);

			jump = false;
		}

	}



}

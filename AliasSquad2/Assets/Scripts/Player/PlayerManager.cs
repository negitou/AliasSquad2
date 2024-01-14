using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerManager : NetworkBehaviour {

    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

	public string team;

    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;

	private float vertical,horizontal;

    [SerializeField] private SkinnedMeshRenderer playerMesh;
    [SerializeField] private Material materialAT;
    [SerializeField] private Material materialFT;

    private GameObject scoreboard;

    C4bomb c4bomb;
    private bool diffuse = false;
    private float diffuseTime;

    private void Reset()
    {
        playerStatus = GetComponent<PlayerStatus>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMesh = transform.Find("TPSPlayerObj").GetChild(0).transform.Find("solder_mesh").gameObject.GetComponent<SkinnedMeshRenderer>();
    }

    void Start()
    {
        if (IsServer)
        {
            TeamManager.teamManager.SetGameObject(gameObject);
            TeamChange(team);
        }

        if (!IsLocalPlayer)
        {
            transform.Find("RecoilObj").transform.Find("PlayerCamera").gameObject.GetComponent<Camera>().enabled = false;
            transform.Find("RecoilObj").transform.Find("PlayerCamera").gameObject.GetComponent<AudioListener>().enabled = false;
            transform.Find("RecoilObj").transform.Find("PlayerCamera").Find("Camera").GetComponent<Camera>().enabled = false;
            transform.Find("RecoilObj").transform.Find("PlayerCamera").Find("FPSPlayerObj").gameObject.SetActive(false);
            TeamChange(team);

        }

        if (IsLocalPlayer)
        {
            scoreboard = GameObject.Find("HUD_Alias").transform.Find("ScoreBoard").gameObject;

            // カーソルをウィンドウから出さない
            Cursor.lockState = CursorLockMode.Locked;
            // カーソルを表示しない
            Cursor.visible = false;

            playerMesh.enabled = false;

            //SetNameServerRpc(AccountManager.accountManager.AccountName);
        }
    }

    [ServerRpc]
    void SetNameServerRpc(string str)
    {
        playerName.Value = str;
    }


    [ServerRpc]
    void C4DiffuseServerRpc()
    {
        c4bomb.SetC4State(2);
    }


    void Update () {
        if (IsLocalPlayer)
        {
			if(Input.GetKeyDown(KeyCode.Escape)){
				// カーソルをウィンドウから出さない
				Cursor.lockState = CursorLockMode.Locked;
				// カーソルを表示しない
				Cursor.visible = false;
			}

            // 移動入力
            // W S
            vertical = Input.GetAxis("Vertical");
            // A D
            horizontal = Input.GetAxis("Horizontal");
            // ジャンプ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerMove.Jump();
            }
            //振り向き入力
            playerMove.Turned(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //持ち替え入力



			Ray ray = new Ray(transform.Find("RecoilObj").Find("PlayerCamera").position, transform.Find("RecoilObj").Find("PlayerCamera").forward * 2);
            RaycastHit hit;
            if (Input.GetKey(KeyCode.E))
            {
                Physics.Raycast(ray, out hit, 3);
                if (hit.collider.tag == "C4")
                {
                    c4bomb = C4bomb.c4bomb;
                    switch (team)
                    {
                        //C4解除 AT
                        case "AT":
                            if (c4bomb.c4State.Value == 1)
                            {
                                diffuse = true;
                                c4bomb.SetC4State(2);
                            }
                            break;

                        //C4拾う FT
                        case "FT":
                            if (c4bomb.c4State.Value == 0)
                            {
                                c4bomb.GetComponent<NetworkObject>().Despawn(true);
                                playerAttack.C4TakeOrDrop(gameObject);
                            }
                            break;

                        default:
                            break;
                    }

                }

                if (diffuse)
                {
                    diffuseTime += Time.deltaTime;
                    if (diffuseTime >= 5f)
                    {
                        c4bomb.SetC4State(3);
                    }
                }
            }
            else if (diffuse)
            {
                diffuse = false;
                diffuseTime = 0;
                c4bomb.SetC4State(2);
            }

            //スコアボード
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreboard.SetActive(true);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                scoreboard.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            playerMove.Move(vertical, horizontal, Input.GetKey(KeyCode.LeftShift));
        }
    }

    public void TeamSelect(int num = 0)
    {
        switch (num)
        {
            case 0:
                if (team == "AT")
                {
                    team = "FT";
                }
                else
                {
                    team = "AT";
                }
                break;
            case 1:
                team = "AT";

                break;
            case 2:
                team = "FT";

                break;
        }
    }

    private void TeamChange(string team)
    {
        if (team == "AT")
        {
            Debug.Log("AT");
            playerMesh.gameObject.GetComponent<Renderer>().material = materialAT;
        }
        else if (team == "FT")
        {
            Debug.Log("FT");
            playerMesh.gameObject.GetComponent<Renderer>().material = materialFT;
        }
    }
}

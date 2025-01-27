using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

public class RefereeScript : NetworkBehaviour
{
    public EnemyScript targetEnemy; 

    public List<EnemyScript> enemyList;

    [SerializeField]
    EnemyGenerator ennemyGeneratorAccess;

    private bool isGameOver;
    bool areAllEnemiesDead;
    [SyncVar]
    public bool canTransferTurnToPlayer;

    [SyncVar]
    public bool isServersTurn;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    public int chosenEnemyId;
    int waveCount = 0;

    [SyncVar]
    int targetPlayerId = 1;

    //[SerializeField]
    //ChooseNewCardScript chooseNewCardAccess;

    public delegate void PreNewWaveAction();
    public static event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public static event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public static event TurnStartAction turnStartEvent;

    public GameObject[] card;
    public GameObject[] button;
    public GameObject[] mainCamera;
    public GameObject[] playerHealth;

    public List<PlayerScript> playerList;

    public static RefereeScript instance;

    public int hostId;
    public int clientId;

    [SyncVar]
    int[] randomNumbers = new int[4];


    private void Awake()
    {
        instance = this;
    }     

    public void CallEndTurnForBothPlayers()
    {
        for(int i = 0; i < 2; i++)
        {
            playerList[i].EndTurnPlayerScript();
        }
    }
    
    private void Start()
    {
        RandomizeChooseCardsSetUp();
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;
        TurnScript.restartGameEvent += RefereeReset;
        ennemyGeneratorAccess.RandomNumber();
        ennemyGeneratorAccess.GenerateEnemies(1);
        newWaveEvent += RandomizeChooseCardsSetUp;
        //enemyList.Add(targetEnemy);
        isGameOver = false;
        //restartGameButton.SetActive(false);
        //winImage.SetActive(false);
        //lostImage.SetActive(false);
        // turnStartEvent();
        if (card != null)
        {
            card = GameObject.FindGameObjectsWithTag("Cards");

            DeactivateCards(card);
        }
        if (button != null)
        {
            button = GameObject.FindGameObjectsWithTag("EndTurnButton");
            DeactivateButton(button);
        }
        if (mainCamera != null)
        {
            mainCamera = GameObject.FindGameObjectsWithTag("MainCamera");
            DeactivateCamera(mainCamera);
        }
        if (playerHealth != null)
        {
            playerHealth = GameObject.FindGameObjectsWithTag("Health");
            DeactivateHealth(playerHealth);
        }

    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }
    }

    private void DeactivateHealth(GameObject[] health)
    {
        if (isServer)
        {
            health[1].SetActive(false);
        }
        else
        {
            health[0].SetActive(false);
        }
    }

    private void DeactivateCamera(GameObject[] camera)
    {
        if (isServer)
        {
            camera[1].SetActive(false);
        }
        else
        {
            camera[0].SetActive(false);
        }
    }

    private void DeactivateButton(GameObject[] button)
    {
        if (isServer)
        {
            button[1].SetActive(false);
        }
        else
        {
            button[0].SetActive(false);
        }
    }

    private void DeactivateCards(GameObject[] cards)
    {
        if (isServer)
        {
            for (int i = 7; i < cards.Length; i++)
            {
                card[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i < 7; i++)
            {
                card[i].SetActive(false);
            }
        } 
    }

    public void ResetChosenEnemy()
    {
        foreach(EnemyScript enemy in enemyList)
        {
            enemy.ChangeSelectedStatus(false);
        }
        chosenEnemyId = 0;
        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void ChooseNewEnemy(int inputDirection)
    {
        enemyList[chosenEnemyId].ChangeSelectedStatus(false);
        if (chosenEnemyId + inputDirection < enemyList.Count && chosenEnemyId + inputDirection >= 0)
        {
            chosenEnemyId += inputDirection;
        }
        else if(chosenEnemyId + inputDirection >= enemyList.Count)
        {
            chosenEnemyId = 0;
        }
        else if (chosenEnemyId + inputDirection < 0)
        {
            chosenEnemyId = enemyList.Count - 1;
        }


        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void EndGame(bool didPlayerWin)
    {
        TurnScript.instance.SetPlayerTurnBool(false);
        Debug.Log("game end");
        isGameOver = true;
        //string winnerName;
        if(didPlayerWin)
        {
            //winnerName = "The player";
            winImage.SetActive(true);
        }
        else
        {
            //winnerName = "The enemy";
            lostImage.SetActive(true);
        }
        //UiScript.UpdateGameOverText($"Game over! {winnerName} is victorious!");
        restartGameButton.SetActive(true);
    }

    public void StartNextWave(bool shouldStartEvents)
    {
        //Debug.Log("new wave!");
        areAllEnemiesDead = false;
        foreach (EnemyScript enemy in enemyList)
        {
           // enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        //ennemyGeneratorAccess.GenerateEnemies(Random.Range(1, 3));
       // ResetChosenEnemy();
        
        //if (shouldStartEvents)
        //{
          //  CallNewWaveEvent();   
        //}
    }
    public void CallPreNewWaveEvent()
    {
        if (isServer)
        {
           // for(int i = 0; i < 2; i++)
           // {
               //if(playerList[i].isHost)
             //  {
                    if (waveCount < 2)
                    {
                        if (preNewWaveEvent != null)
                        {
                            //Debug.Log("pre new wave event called");
                            preNewWaveEvent();
                            waveCount++;
                        }
                    }
                    else
                    {
                        EndGame(true);
                    }
            //   }
           // }
            
        }
        
       
    }
    public void CallNewWaveEvent()
    {
        if (newWaveEvent != null)
        {
            newWaveEvent();
        }
    }

    public void CallStartTurnEvent()
    {
        canTransferTurnToPlayer = true;
        if (turnStartEvent != null)
        {
            turnStartEvent();
        }
        else
        {
            Debug.Log("turn start event fail");
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }
    public void RefereeReset()
    {
        isGameOver = false;
        /*foreach (EnemyScript enemy in enemyList)
        {
            enemy.ResetEnemy();
        } */
        StartNextWave(false);
        PlayerStatScript.instance.ResetPlayer();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartForeachEnemyCoroutine()
    {
        if(!isGameOver)
        {
            StartCoroutine(ForeachEnemyTurnCoroutine());

            Debug.Log("zzoz");//ar galima sitoj vietoj padaryti kad visa logika eitu tik per turn script puse?
        }
    }

    public void RandomNumberSetUp(int maximumValue)
    {
        for(int i = 0; i < 4; i++)
        {
            randomNumbers[i] = Random.Range(0, maximumValue);
        }
    }

    public int GetRandomNumber(int inputIndex)
    {
        return randomNumbers[inputIndex];
    }
    public void NewWaveCheck()
    {
        Debug.Log("New Wave check activated");
        areAllEnemiesDead = true;
        foreach (EnemyScript enemy in enemyList)
        {
            if (enemy.enemyHealth > 0)
            {
                areAllEnemiesDead = false;
            }
        }
        if (areAllEnemiesDead)
        {  

            canTransferTurnToPlayer = false;
            if (playerList[0].isThisPlayersTurn)
            {
                playerList[0].BeginPreNewWaveCall();
            }
            else
            {
                playerList[1].BeginPreNewWaveCall();
            }   
            //CallPreNewWaveEvent();
        }
    }

    private void RandomizeChooseCardsSetUp()
    {
        if (isServer)
        {
            DatabasePlayer databasePlayerAccess = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
            if (databasePlayerAccess != null)
            {
                RandomNumberSetUp(databasePlayerAccess.cardList.Count);
            }
            else
            {
                Debug.Log("databasePlayerAccess was null!");
            }
        }
    }

    [ClientRpc]
    public void RpcDealDamageToPlayer(int inputDamage)
    {
        DealDamageLogic(inputDamage);
    }
    [Command(requiresAuthority = false)]
    public void CmdDealDamageToPlayer(int inputDamage)
    {
        DealDamageLogic(inputDamage);
    }

    public void DealDamageLogic(int inputDamage)
    {
        if (playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().TakeDamageAndCheckIfDead(inputDamage))
        {
            TurnScript.instance.ShouldStartPlayerTurn(false);
            EndGame(false);
        }
        if (targetPlayerId + 1 > 1)
        {
            targetPlayerId = 0;
        }
        else
        {
            targetPlayerId++;
        }
    }
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        if (!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                int enemyDamage = enemy.GenerateAttack();
                if(isClientOnly)
                {
                    CmdDealDamageToPlayer(enemyDamage);
                }
                else
                {
                    RpcDealDamageToPlayer(enemyDamage);
                }
                
                UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                yield return new WaitForSeconds(0.75f);
            }
        }
        if(canTransferTurnToPlayer)
        {
            CallStartTurnEvent(); 
        }
    }
}

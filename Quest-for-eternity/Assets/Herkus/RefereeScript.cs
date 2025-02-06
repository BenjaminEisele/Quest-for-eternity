using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;


public class RefereeScript : NetworkBehaviour
{
    public List<EnemyScript> enemyList;
    public List<EnemyScript> killedEnemyList;


    public EnemyGenerator enemyGeneratorAccess;



    private bool isGameOver;
    private bool areAllEnemiesDead;
    [SyncVar]
    public bool canTransferTurnToPlayer;

    [SyncVar]
    public bool isServersTurn;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [HideInInspector]
    public int chosenEnemyId;
    [SyncVar]
    public int waveCount = 0;

    [SyncVar]
    public int targetPlayerId = 1;

    public delegate void PreNewWaveAction();
    public event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public event TurnStartAction turnStartEvent;

    private GameObject[] card;
    private GameObject[] button;
    private GameObject[] mainCamera;
    private GameObject[] playerHealth;
    private GameObject[] playerScripts;

    public List<PlayerScript> playerList;

    public static RefereeScript instance;

    public bool singlePlayerMode;

    [SyncVar]
    int[] randomNumbers = new int[4];
    [SyncVar]
    public int randomEnemyCount = 0;
    public readonly SyncList<int> displayCardIdList = new SyncList<int>();

    Coroutine myCoroutine = null;

    //[SerializeField]
    //TurnScript turnScriptAccess;

    private void Awake()
    {
        instance = this;        
    }

    private void Start()
    {

        if (playerScripts == null)
        {
            playerScripts = GameObject.FindGameObjectsWithTag("PlayerScriptTag");
            SetPlayerList(playerScripts);
        }
        if(playerList.Count == 2)
        {
            singlePlayerMode = false;
            if (card == null)
            {
                card = GameObject.FindGameObjectsWithTag("Cards");
                DeactivateCards(card);
            }
            if (button == null)
            {
                button = GameObject.FindGameObjectsWithTag("EndTurnButton");
                DeactivateButton(button);
            }
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectsWithTag("MainCamera");
                DeactivateCamera(mainCamera);
            }
            if (playerHealth == null)
            {
                playerHealth = GameObject.FindGameObjectsWithTag("Health");
                DeactivateHealth(playerHealth);
            }
        }
        else
        {
            singlePlayerMode = true;
            playerList[0].EndTurnSubscription();
            targetPlayerId = 0;
        }
        
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;
        //turnScriptAccess.restartGameEvent += RefereeReset;       
               
        isGameOver = false;
        
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            SwitchPlayerAttackIdNest();
        }

    }

    private void SetPlayerList(GameObject[] Scripts)
    {
        foreach (GameObject Script in Scripts)
        {
            playerList.Add(Script.GetComponent<PlayerScript>());
        }
        RandomNumbersSetUpRoot();
        enemyGeneratorAccess.GenerateEnemies(1, false);
    }
    public void RandomNumbersSetUpRoot()
    {
        DatabasePlayer databasePlayerAccess = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
        if (isServer)
        {
            if (databasePlayerAccess != null)
            {
                RandomNumberGeneration(databasePlayerAccess.cardList.Count);
            }
        }
    }
    public void RandomNumberGeneration(int maximumValue)
    {
        displayCardIdList.Clear();
        if (isServer)
        {
            randomEnemyCount = Random.Range(2, 3);
            enemyGeneratorAccess.RandomNumber(randomEnemyCount);
            for (int i = 0; i < 4; i++)
            {
                displayCardIdList.Add(Random.Range(0, maximumValue));
            }
        }
    }

    public int GetRandomNumber(int inputIndex)
    {
        return displayCardIdList[inputIndex];
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
    public void CallStartTurnEvent()
    {
        if (turnStartEvent != null)
        {
            turnStartEvent();
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }
    public void CallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        if(singlePlayerMode)
        {
            playerList[0].EndTurnPlayerScript();
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                playerList[i].EndTurnPlayerScript();
            }
        }
        
    }

    [Command(requiresAuthority = false)]
    public void CmdCallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        CallEndTurnForBothPlayers();
    }
    public void ResetChosenEnemy()
    {
        foreach(EnemyScript enemy in enemyList)
        {
            enemy.ChangeSelectedStatus(false);
        }
        chosenEnemyId = 0;
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
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    public void NewWaveCheck()
    {
        Debug.Log("checking new wave");
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
            if(!singlePlayerMode)
            {
                canTransferTurnToPlayer = false;
            }
            
            if (playerList[0].isThisPlayersTurn)
            {
                playerList[0].BeginPreNewWaveCall();
            }
            else
            {
                playerList[1].BeginPreNewWaveCall();
            }
        }
    }
    public void CallPreNewWaveEvent()
    {
        if (isServer)
        {
            if (waveCount < 3)
            {
                if (preNewWaveEvent != null)
                {
                    preNewWaveEvent();
                    waveCount++;
                }
            }
            else
            {
                EndGame(true);
            }
        }
    }
    private void EndGame(bool didPlayerWin)
    {
        TurnScript.instance.SetPlayerTurnBool(false);
        isGameOver = true;
        if(didPlayerWin)
        {
            winImage.SetActive(true);
        }
        else
        {
            lostImage.SetActive(true);
        }
        restartGameButton.SetActive(true);
    }
    public void CallNewWaveEvent()
    {
        if (newWaveEvent != null)
        {
            newWaveEvent();
        }
    }
    public void StartNextWaveInitalize()
    {
        if(isClientOnly)
        {
            CmdStartNextWave();
        }
        else
        {
            RpcStartNextWave(true);
        }
    }
    [ClientRpc]
    public void RpcStartNextWave(bool shouldStartEvents)
    {
        StartNextWaveLogic(shouldStartEvents);
    }
    [Command(requiresAuthority = false)]
    private void CmdStartNextWave()
    {
        RpcStartNextWave(true);
    }
    private void StartNextWaveLogic(bool shouldStartEvents)
    {
        areAllEnemiesDead = false;
        foreach (EnemyScript enemy in killedEnemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        if(waveCount == 3)
        {
            enemyGeneratorAccess.GenerateEnemies(1, false);
        }
        else
        {
            enemyGeneratorAccess.GenerateEnemies(randomEnemyCount, false);
        }
        if (shouldStartEvents)
        {
            CallNewWaveEvent();   
        }
    }
    
    public void RefereeReset()
    {
        isGameOver = false;
        StartNextWaveInitalize();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartForeachEnemyCoroutine()
    {
        if (!isGameOver)
        {
            if(myCoroutine == null)
            {
                myCoroutine = StartCoroutine(ForeachEnemyTurnCoroutine());
            }   
        }
    }
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        if (!areAllEnemiesDead)
        {
            int debugCounter = 0;
            int loopCount = enemyList.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if(enemyList[i].canAttack)
                {   
                    //enemyList[0].specialAttackCounter++;
                    int enemyDamage = enemyList[i].GenerateAttack();
                    if (isClientOnly)
                    {
                        CmdDealDamageToPlayer(enemyDamage);
                    }
                    else
                    {
                        RpcDealDamageToPlayer(enemyDamage);
                    }
                    UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                }
                else
                {
                    enemyList[i].canAttack = true;
                }
                yield return new WaitForSeconds(0.75f);
                debugCounter++;
            }
            SwitchPlayerAttackIdNest();
        }
        if (canTransferTurnToPlayer)
        {
            CallStartTurnEvent();
        }
        myCoroutine = null;
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
    }
    
    private void SwitchPlayerAttackIdNest()
    {
        if (isServer)
        {
            SwitchPlayerAttackId();
        }
        else
        {
            CmdSwitchPlayerAttackId();
        }
    }

    private void SwitchPlayerAttackId()
    {
        if(!singlePlayerMode)
        {
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
       
    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchPlayerAttackId()
    {
        if (!singlePlayerMode)
        {
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
    }

    public void SpecialAttackCounterNest(bool shouldSet)
    {
        if (isServer)
        {
            RpcAttackCounter(shouldSet);
        }
        else
        {
            CmdAttackCounter(shouldSet);
        }
    }

    [ClientRpc]
    private void RpcAttackCounter(bool shouldSet)
    {
        if (isClientOnly)
        {
            if(shouldSet)
            {
                enemyList[0].specialAttackCounter = 0;
            }
            else
            {
                enemyList[0].specialAttackCounter++;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAttackCounter(bool shouldSet)
    {
        if (shouldSet)
        {
            enemyList[0].specialAttackCounter = 0;
        }
        else
        {
            enemyList[0].specialAttackCounter++;
        }
    }

    public void EnemyGenerationNest()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
        if (isServer)
        {
            RpcGenerateEnemy();
        }
        else
        {
            CmdGenerateEnemy();
        }
    }

    [ClientRpc]
    private void RpcGenerateEnemy()
    {
        if (isClientOnly)
        {
            enemyGeneratorAccess.GenerateEnemies(1, true);
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdGenerateEnemy()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
    }

    public void HealEnemyRefereeScript()
    {
        if (isServer)
        {
            playerList[0].HealEnemyPlayerScript();
        }
        else
        {
            playerList[1].HealEnemyPlayerScript();
        }
    }
}

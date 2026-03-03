using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    #region Data
    PlayerController pc;
    EnemyController aic;
    CameraHandler cameraHandler;

    [Header("TMP")]
    [SerializeField] List<EntityBehaviour> characters=new List<EntityBehaviour>();
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;
    List<EntityBehaviour> enemyCharacters = new List<EntityBehaviour>();
    List<EntityBehaviour> playerCharacters = new List<EntityBehaviour>();
    EntityBehaviour currentActiveCharacter;

    int index;
    #endregion


    #region Mono
    protected override void Awake()
    {
    }
    private void Start()    
    {
        Time.timeScale = 1.0f;
        cameraHandler = Camera.main.GetComponent<CameraHandler>();
        InitialiseCharacters();
        Debug.Log("pc is "+ pc.name);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            string path = Application.persistentDataPath + "/Screenshot.png";
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Screenshot is "+path);
        }
    }

    #endregion


    #region Methods
    public void RegisterPlayerController(PlayerController controller)
    {
        pc = controller;
    }

    public void RegisterAIController(EnemyController controller)
    {
        aic = controller;
    }
    private void InitialiseCharacters()
    {
        for(int i=0; i < characters.Count;i++)
        {
            Debug.Log($"target layer {characters[i].gameObject.layer}, my layer is {playerLayer.value}");
            if (IsGameObjectInLayerMask(characters[i].gameObject,playerLayer))
            {
                playerCharacters.Add(characters[i]);
            }
            else
            {
                enemyCharacters.Add(characters[i]);
            }
        }
        CreateQueueTurn();
    }
    bool IsGameObjectInLayerMask(GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = (1 << obj.layer);

        return (layerMask & objLayerMask) != 0;
    }
    private void CreateQueueTurn()
    {
        index = 0;
        for (int i = 0; i < characters.Count - 1; i++)
        {
            int maxIndex = i;
            for (int j = i + 1; j < characters.Count; j++)
            {
                if (characters[j].CharacterStats.speed > characters[maxIndex].CharacterStats.speed)
                {
                    maxIndex = j;
                }
            }
            // Swap characters[i] with characters[maxIndex]
            EntityBehaviour temp = characters[i];
            characters[i] = characters[maxIndex];
            characters[maxIndex] = temp;
        }
        EventManager.newTurnList?.Invoke(characters);
        StartTurn();
    }
    public void StartTurn()
    {
        if(pc.ActivePawn != null)
        {
            pc.Unselect();
        }
        if(aic.ActivePawn!=null)
        {
            aic.Unselect();
        }

        if (index >= characters.Count)
        {
            CreateQueueTurn();
            return;
        }
        currentActiveCharacter = characters[index];
        cameraHandler.SetTarget(currentActiveCharacter.gameObject);
        if(playerCharacters.Contains(currentActiveCharacter))
            pc.Select(currentActiveCharacter);
        else
            aic.Select(currentActiveCharacter);
        index++;
            
    }
    public void CharacterDied(EntityBehaviour character)
    {
        Debug.Log(character);
        characters.Remove(character);
        if(playerCharacters.Contains(character))
            playerCharacters.Remove(character);
        else
            enemyCharacters.Remove(character);
        if(playerCharacters.Count==0)
        {
            StartCoroutine(GameLost());
        }
        if(enemyCharacters.Count==0) 
        {
            StartCoroutine(GameWon());
        }

    }
    private IEnumerator GameWon()
    {
        yield return new WaitForSeconds(1); 
        EventManager.GameWon?.Invoke();
    }
    private IEnumerator GameLost()
    {
        yield return new WaitForSeconds(1);
        EventManager.GameOver?.Invoke();
    }
    public void Restart()
    {
        Scene scene= SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying=false;
#endif
    }
    #endregion
}

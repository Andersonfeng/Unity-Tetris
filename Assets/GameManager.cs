using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("方块列表")] public List<GameObject> cubeList;
    [Header("方块生成位置")] public Vector2 generatePoint;
    [Header("下一个方块显示位置")] public Vector2 nextCubePoint;
    [Header("游戏面板")] public GameObject gameBoard;
    [Header("游戏面板文本")] public Text gameBoardMessage;
    [Header("时间")] public Text timeText;
    [Header("分数")] public Text scoreText;
    [Header("等级")] public Text levelText;
    [Header("背景颜色")] public SpriteRenderer backgroundcolor;


    //多久跌落一格
    public float fallDuration = 1f;
    public static GameManager Instance;

    //二维数组 存放静止的方块位置
    private static Transform[,] staticCube = new Transform[width, height];
    private float originalFallDuration = 1.5f;
    private static int width = 10;
    private static int height = 20;
    private bool _pause;
    private bool _gameOver;
    private float _score;
    private int _level;
    private float _time;

    //下一个方块
    private GameObject nextCube;

    //删除行数
    private int _deleteline;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        originalFallDuration = fallDuration;
        GenerateRamdomCube();
        DropNextCute();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (_gameOver)
        {
            Time.timeScale = 0;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            ShowGameBoard("暂停");

        _time += Time.deltaTime;
        timeText.text = ((int) _time).ToString();
        scoreText.text = _score.ToString();

        levelText.text = _level.ToString();

        if (Input.GetKeyDown(KeyCode.R))
            RandomBackgroundColor();
    }

    public void ShowGameBoard(string message)
    {
        gameBoardMessage.text = message;
        gameBoard.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        if (_gameOver)
            Restart();
        gameBoard.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /**
     * 检查是否有可删除行
     */
    private void CheckDelete()
    {
        for (var i = height - 1; i >= 0; i--)
            if (staticCube[0, i] != null)
            {
                CheckLine(i);
            }

        if (_deleteline > 0)
        {
            _score += _deleteline * 10 + (_deleteline - 1) * 10;
            _deleteline = 0;
            SoundManager.PlaySoundByName(SoundManager.SoundName.Delete);
            CheckLevelUp();
        }
    }

    /**
     * 检查一行是否可以删
     */
    private void CheckLine(int lineNumber)
    {
        for (var i = 0; i < width; i++)
        {
            if (staticCube[i, lineNumber] == null)
                return;
        }

        DeleteLine(lineNumber);
    }

    /**
     * 删除一行
     */
    private void DeleteLine(int lineNumber)
    {
        for (var i = 0; i < width; i++)
        {
            Destroy(staticCube[i, lineNumber].gameObject);
            staticCube[i, lineNumber] = null;
        }

        _deleteline++;

        RandomBackgroundColor();
        DownRow(lineNumber);
    }

    /**
    * 下降一行
    * lineNumber行以上的所有方块下降一行
    */
    private void DownRow(int lineNumber)
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = lineNumber + 1; j < height; j++)
            {
                if (staticCube[i, j] != null)
                {
                    staticCube[i, j - 1] = staticCube[i, j];
                    staticCube[i, j] = null;
                    staticCube[i, j - 1].position += Vector3.down;
                }
            }
        }
    }

    /**
     * 固定方块
     */
    public static void Fasten(Transform transform)
    {
        SoundManager.PlaySoundByName(SoundManager.SoundName.Ground);
        foreach (Transform child in transform)
        {
            var x = Mathf.RoundToInt(child.position.x);
            var y = Mathf.RoundToInt(child.position.y);
            if (y >= height)
            {
                Instance._gameOver = true;
                GameOver();
                return;
            }

            staticCube[x, y] = child;
        }

        transform.DetachChildren();
        Destroy(transform.gameObject);

        Instance.CheckDelete();
    }

    /**
     * 游戏结束
     */
    private static void GameOver()
    {
        Instance.ShowGameBoard("木了~");
        Instance._gameOver = true;
    }

    /**
     * 生成随机方块
     */
    public static void GenerateRamdomCube()
    {
        var randomCube = Instance.cubeList[Random.Range(0, Instance.cubeList.Count - 1)];
        // var randomCube = Instance.cubeList[0];
        Instance.nextCube = Instantiate(randomCube, Instance.nextCubePoint,
            Quaternion.identity);
        Instance.nextCube.GetComponent<TetrisController>().enabled = false;
    }

    /**
     * 将下一个方块丢到场地
     */
    public static void DropNextCute()
    {
        if (Instance.nextCube != null)
        {
            Instance.nextCube.transform.position = Instance.generatePoint;
            Instance.nextCube.GetComponent<TetrisController>().enabled = true;
        }

        GenerateRamdomCube();
    }

    /**
     * 检查该位置是否已有方块
     */
    public static bool CheckValidPosition(Transform transform)
    {
        foreach (Transform child in transform)
        {
            var x = Mathf.RoundToInt(child.position.x);
            var y = Mathf.RoundToInt(child.position.y);

            if (y < height && staticCube[x, y] != null)
                return false;
        }

        return true;
    }

    /**
     * 每100分 升1级
     * 每升1级方块下落时间减少0.05s
     */
    private void CheckLevelUp()
    {
        var currentLevel = _level;
        _level = Mathf.RoundToInt(_score / 100);
        if (!currentLevel.Equals(_level))
            SoundManager.PlaySoundByName(SoundManager.SoundName.LeveUp);

        fallDuration = Mathf.Clamp(originalFallDuration - _level * 0.1f, 0.1f, 10f);
    }

    /**
     * 随机背景颜色
     */
    private void RandomBackgroundColor()
    {
        var r = Random.Range(0f, 1f);
        var g = Random.Range(0f, 1f);
        var b = Random.Range(0f, 1f);
        backgroundcolor.color = new Color(r, g, b);
    }
}
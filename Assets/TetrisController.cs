using UnityEngine;

public class TetrisController : MonoBehaviour
{
    public KeyCode rotate = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode down = KeyCode.S;
    public KeyCode fallToBottom = KeyCode.Space;

    //场地宽
    public float width = 10;

    //场地高
    public float height = 20;


    //方块自动跌落时长
    private float _currentFallDuration;

    //按下按键间隔
    private float _pressKeyDuration = 0.05f;

    //按键计时器
    private float _pressKeyTimer;

    //方块跌落计时器
    private float _timer;

    private void Start()
    {
        _currentFallDuration = GameManager.Instance.fallDuration;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Fall();
            _timer = _currentFallDuration;
        }

        if (Input.GetKey(down))
        {
            _pressKeyTimer += Time.deltaTime;
            if (_pressKeyTimer > _pressKeyDuration)
            {
                Fall();
                _pressKeyTimer = 0;
            }
        }

        if (Input.GetKeyDown(left))
        {
            Left();
            SoundManager.PlaySoundByName(SoundManager.SoundName.Move);
        }

        if (Input.GetKeyDown(right))
        {
            Right();
            SoundManager.PlaySoundByName(SoundManager.SoundName.Move);
        }

        if (Input.GetKeyDown(rotate))
        {
            Rotate();
            SoundManager.PlaySoundByName(SoundManager.SoundName.Rotate);
        }

        if (Input.GetKeyDown(fallToBottom))
            FallToBottom();
    }

    private void Rotate()
    {
        foreach (Transform child in transform)
        {
            var x = Mathf.RoundToInt(child.position.x);
            var y = Mathf.RoundToInt(child.position.y);
        }

        transform.Rotate(new Vector3(0, 0, 90));
        if (!ValidateMove())
            transform.Rotate(new Vector3(0, 0, -90));

        foreach (Transform child in transform)
        {
            var x = Mathf.RoundToInt(child.position.x);
            var y = Mathf.RoundToInt(child.position.y);
        }
    }

    private void Right()
    {
        transform.position += Vector3.right;
        if (!ValidateMove())
            transform.position -= Vector3.right;
    }

    private void Left()
    {
        transform.position += Vector3.left;
        if (!ValidateMove())
            transform.position -= Vector3.left;
    }

    /**
     * 下落
     */
    private void Fall()
    {
        transform.position += Vector3.down;
        if (!ValidateMove())
        {
            transform.position -= Vector3.down;
            //当已不能下落时,禁止方块移动,并将各方块坐标记录到二维数组
            GameManager.Fasten(transform);
            enabled = false;
            GameManager.DropNextCute();
        }
    }

    /**
     * 落到最底部
     */
    private void FallToBottom()
    {
        while (ValidateMove())
        {
            transform.position += Vector3.down;
        }

        transform.position -= Vector3.down;

        GameManager.Fasten(transform);
        enabled = false;
        GameManager.DropNextCute();
    }

    /**
     * 判断是否可以移动
     */
    private bool ValidateMove()
    {
        foreach (Transform child in transform)
        {
            var x = Mathf.RoundToInt(child.position.x);
            var y = Mathf.RoundToInt(child.position.y);
            if (x >= width
                || x < 0
                || y < 0
            )
                return false;
        }

        return GameManager.CheckValidPosition(transform);
    }
}
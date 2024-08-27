using UnityEngine;

// TicTacToeを参考に
// やること
// 碁石を置けるようにする

/// <summary>
/// フェーズ管理
/// </summary>
public enum Turn
{
    /// <summary>
    /// 黒のターン(先）
    /// </summary>
    Black,

    /// <summary>
    /// 白のターン(後）
    /// </summary>
    White
}

/// <summary>
/// オセロ全体の管理。
/// </summary>
public class Othello : MonoBehaviour
{
    private const int Rows = 8; // 行数
    private const int Columns = 8; // 列数
    private Cell[,] _cells; // セルを保持する二次元配列

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // マス目の隙間

    void Start()
    {
        _cells = new Cell[Rows, Columns];

        // 盤面の初期化
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var cell = Instantiate(_cellPrefab, transform);
                cell.transform.position = new(c * _space, 0, -r * _space);
                cell.name = $"Cell({r}, {c})";
                cell.State = CellState.None;
                _cells[r, c] = cell;
            }
        }
        InitialPosition();
    }

    /// <summary>
    /// オセロの初期配置
    /// </summary>
    private void InitialPosition()
    {
        _cells[3, 3].State = CellState.White;
        _cells[3, 4].State = CellState.Black;
        _cells[4, 3].State = CellState.Black;
        _cells[4, 4].State = CellState.White;
    }
}

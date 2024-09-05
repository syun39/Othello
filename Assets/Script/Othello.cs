using UnityEngine;
using System.Collections.Generic;


// TicTacToeを参考に
// やること
// 置けるところを制限
// 持ち時間計算、勝敗判定 AI アニメーションを使った演出の実装（裏返しアニメーション 持ち時間表示
// StoneStateを使ってないけどよいのか？

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
    private Tile[,] _tileSelect; // 選択状態を保持する二次元配列

    private Turn _currentTurn = Turn.Black; // 現在のターン

    private int _selectedRow; // 選択されている行
    private int _selectedColumn; // 選択されている列

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // マス目の隙間

    // 8方向の移動の配列
    private int[,] _directions = new int[,]
    {
        // 行列
        { -1,0 }, // 上
        { 1,0 }, // 下
        { 0,-1 }, // 左
        { 0,1 }, // 右
        { -1,-1 }, // 左上
        { -1,1 }, // 右上
        { 1,-1 }, // 左下
        { 1,1 } // 右下
    };

    void Start()
    {
        // 初期化
        _cells = new Cell[Rows, Columns];
        _tileSelect = new Tile[Rows, Columns];

        // 盤面の初期化
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var cell = Instantiate(_cellPrefab, transform);
                cell.transform.position = new(c * _space, 0, -r * _space);
                cell.name = $"Cell({r}, {c})";
                cell.State = CellState.None; // 初期状態は None

                // 配列に格納
                _cells[r, c] = cell;
                _tileSelect[r, c] = cell.Tile;
            }
        }
        InitialPosition();
    }

    private void Update()
    {
        MoveCell();

        // セルの更新
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                _tileSelect[r, c].State = TileState.None;
            }
        }

        // 選択されたタイルの状態を Selected に
        _tileSelect[_selectedRow, _selectedColumn].State = TileState.Selected;

        if (Input.GetKeyDown(KeyCode.Return)) // エンターキーを押したら
        {
            // 碁石が置かれてなかったら
            if (_cells[_selectedRow, _selectedColumn].State == CellState.None)
            {
                // ターンに応じた石を置く
                SetStone(_selectedRow, _selectedColumn, _currentTurn == Turn.Black ? CellState.Black : CellState.White);

                // 現在のターンが黒なら白のターンに変更そうでなければ黒のターンに変更
                _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;
            }
        }
    }

    /// <summary>
    /// 石を置く
    /// </summary>
    /// <param name="row">石を置く行</param>
    /// <param name="column">石を置く列</param>
    /// <param name="state">置く石の状態</param>
    private void SetStone(int row, int column, CellState state)
    {
        _cells[row, column].State = state; // 石を置く

        // ひっくり返せる石があるかチェック
        ChangeStone(row, column, state == CellState.Black ? CellState.White : CellState.Black);
    }

    /// <summary>
    /// 石をひっくり返す処理
    /// </summary>
    /// <param name="rows">石を置いた行</param>
    /// <param name="col">石を置いた列</param>
    /// <param name="enemyColor">敵の石の色</param>
    /// <returns>ひっくり返した石の数</returns>
    private int ChangeStone(int rows, int col, CellState enemyColor)
    {
        bool isChangeStone = false; // 石をひっくり返せるかどうか
        List<(int rows, int col)> stones = new List<(int rows, int col)>(); // ひっくり返す石のリスト(タプル)
        int count = 0; // ひっくり返した石の数

        for (int i = 0; i < _directions.GetLength(0); i++)
        {
            int moverow = rows; // 現在の行の位置
            int movecolumn = col; // 現在の列の位置

            int dirrow = _directions[i, 1]; // 行の移動量
            int dircol = _directions[i, 0]; // 列の移動量
            isChangeStone = false;
            stones.Clear();

            // 方向に沿って探索
            while (true)
            {
                moverow += dirrow; // 行を移動
                movecolumn += dircol; // 列を移動

                // 境界を超えたら終了
                if (moverow < 0 || moverow >= Rows || movecolumn < 0 || movecolumn >= Columns)
                {
                    break;
                }

                // 現在のセルに敵の石があれば
                if (_cells[moverow, movecolumn].State == enemyColor)
                {
                    stones.Add((moverow, movecolumn)); // ひっくり返す候補としてリストに追加
                }
                // 現在のセルに自分の色の石がある場合
                else if (_cells[moverow, movecolumn].State == (enemyColor == CellState.Black ? CellState.White : CellState.Black))
                {
                    // ひっくり返すことが可能
                    isChangeStone = true;
                    break;
                }
                else
                {
                    // 石が置かれていない、自分の石があれば終了
                    break;
                }
            }

            // ひっくり返すことができる場合
            if (isChangeStone)
            {
                count += stones.Count; // ひっくり返せる石の数をカウント

                foreach (var (changerow,changecolumn) in stones)
                {
                    // リストにある全ての石をひっくり返す
                    _cells[changerow,changecolumn].State = (enemyColor == CellState.Black ? CellState.White : CellState.Black);
                }
            }
        }
        return count;
    }

    /// <summary>
    /// セルの移動と境界チェック
    /// </summary>
    private void MoveCell()
    {
        // セルの移動
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { _selectedColumn--; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { _selectedColumn++; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { _selectedRow--; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { _selectedRow++; }

        // 境界チェック
        if (_selectedColumn < 0) { _selectedColumn = 0; }
        if (_selectedColumn >= Columns) { _selectedColumn = Columns - 1; }
        if (_selectedRow < 0) { _selectedRow = 0; }
        if (_selectedRow >= Rows) { _selectedRow = Rows - 1; }
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

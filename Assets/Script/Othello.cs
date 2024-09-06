using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    [SerializeField] float _turnTime = 30f; // ターンの持ち時間
    [SerializeField] float _remainingTime = 1.1f; // 残り時間

    [SerializeField] private Text _timerText; // タイマー表示用（UI）

    [SerializeField] private Text _turnText; // 現在のターン表示用（UI）

    private bool _gameEnd = false; // ゲームが終了してるかどうか



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
        InitialPosition(); // 初期配置
        PlaceableTile(); // 置けるタイルの色を変える

        _remainingTime = _turnTime; // ターンタイマーをリセット
    }



    private void Update()
    {
        if (_gameEnd)
        {
            return; // ゲームが終了していたら入力を受け付けない
        }

        MoveCell(); // セルの移動

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

        PlaceableTile(); // タイルの色更新 

        _remainingTime -= Time.deltaTime; // 残り時間を更新

        // 時間が０以下なら
        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            Debug.Log("負け");
            _gameEnd = true; // ゲーム終了
            return;
        }

        // タイマー表示の更新
        if (_timerText != null)
        {
            _timerText.text = $"残り: {_remainingTime:F0}s";
        }

        // 現在の選択場所のタイツの状態が Placeable なら
        if (_tileSelect[_selectedRow, _selectedColumn].State == TileState.Placeable)
        {
            // タイルの状態を PlaceableSelected へ
            _tileSelect[_selectedRow, _selectedColumn].State = TileState.PlaceableSelected;
        }

        if (Input.GetKeyDown(KeyCode.Return)) // エンターキーを押したら
        {
            // タイルの状態が PlaceableSelected だったら
            if (_tileSelect[_selectedRow, _selectedColumn].State == TileState.PlaceableSelected)
            {
                // 碁石が置かれていないなら
                if (_cells[_selectedRow, _selectedColumn].State == CellState.None)
                {
                    // ターンに応じた石を置く
                    SetStone(_selectedRow, _selectedColumn, _currentTurn == Turn.Black ? CellState.Black : CellState.White);

                    // 現在のターンが黒なら白のターンに変更そうでなければ黒のターンに変更
                    _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;

                    _remainingTime = _turnTime; // ターンタイマーをリセット
                }
            }
        }

        // 現在のターンを表示
        if (_turnText != null)
        {
            _turnText.text = _currentTurn == Turn.Black ? "現在のターン: 黒" : "現在のターン: 白";
        }

        CheckGameEnd(); // ゲーム終了かチェック

        // パス判定
        if (!_gameEnd)
        {
            // 現在のプレイヤーのターンで石を置ける場所があるかを確認
            if (!CanPlaceStoneAnywhere(_currentTurn == Turn.Black ? CellState.Black : CellState.White))
            {
                Debug.Log($"{_currentTurn} パス");
                // ターンを変更し、ターンタイマーをリセット
                _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;
                _remainingTime = _turnTime; // ターンタイマーをリセット
            }
        }
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
            int moveRow = rows; // 現在の行の位置
            int moveColumn = col; // 現在の列の位置

            int dirrow = _directions[i, 1]; // 行の移動量
            int dircol = _directions[i, 0]; // 列の移動量
            isChangeStone = false; // ひっくり返すことができるか
            stones.Clear();

            // 方向に沿って探索
            while (true)
            {
                moveRow += dirrow; // 行を移動
                moveColumn += dircol; // 列を移動

                // 境界を超えたら終了
                if (moveRow < 0 || moveRow >= Rows || moveColumn < 0 || moveColumn >= Columns)
                {
                    break;
                }

                // 現在のセルに敵の石があれば
                if (_cells[moveRow, moveColumn].State == enemyColor)
                {
                    stones.Add((moveRow, moveColumn)); // ひっくり返す候補としてリストに追加
                }
                // 現在のセルに自分の色の石がある場合
                else if (_cells[moveRow, moveColumn].State == (enemyColor == CellState.Black ? CellState.White : CellState.Black))
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

                foreach (var (changerow, changecolumn) in stones)
                {
                    // リストにある全ての石をひっくり返す
                    _cells[changerow, changecolumn].State = (enemyColor == CellState.Black ? CellState.White : CellState.Black);
                }
            }
        }
        return count;
    }

    /// <summary>
    /// 石が置けるかどうか
    /// </summary>
    /// <param name="row">チェックする行番号。</param>
    /// <param name="col">チェックする列番号。</param>
    /// <param name="state">現在のプレイヤーの石の色（黒または白）。</param>
    /// <returns>石が置ける場合は true を返し、置けない場合は false を返す。</returns>
    private bool CanPlaceStone(int row, int col, CellState state)
    {
        // 石が置かれている場合
        if (_cells[row, col].State != CellState.None)
        {
            return false;
        }

        // 敵の石の色
        CellState enemyColor = (state == CellState.Black) ? CellState.White : CellState.Black;

        // 8方向を確認
        for (int i = 0; i < _directions.GetLength(0); i++)
        {
            // 移動
            int moveRow = row + _directions[i, 1];
            int moveColumn = col + _directions[i, 0];

            bool hasEnemyStone = false; // 敵の石がその方向にあるか

            // 石の状態を探索
            while (moveRow >= 0 && moveRow < Rows && moveColumn >= 0 && moveColumn < Columns)
            {
                // 敵の石が見つかったら
                if (_cells[moveRow, moveColumn].State == enemyColor)
                {
                    hasEnemyStone = true;
                }

                // 自分の石が見つかったら
                else if (_cells[moveRow, moveColumn].State == state)
                {
                    if (hasEnemyStone) // 間に敵の石がある場合
                    {
                        return true; // 石を置ける
                    }
                    break; // なかったら終了
                }
                else
                {
                    break;
                }

                // 消すと無限リープになる
                // さらに同じ方向に移動
                moveRow += _directions[i, 1];
                moveColumn += _directions[i, 0];
            }
        }
        return false; // どの方向にも石が置けない場合
    }

    /// <summary>
    /// 置ける場所のタイルの色を変える
    /// </summary>
    private void PlaceableTile()
    {
        // 全セルをチェック
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                // 現在のプレイヤーに応じて石が置けるか
                if (CanPlaceStone(r, c, _currentTurn == Turn.Black ? CellState.Black : CellState.White))
                {
                    // 石が置けるならタイルを Placeable に
                    _tileSelect[r, c].State = TileState.Placeable;
                }
            }
        }
    }

    /// <summary>
    /// ゲームの終了、勝敗判定
    /// </summary>
    private void CheckGameEnd()
    {
        bool hasAvailableMoves = false; // 置ける場所があるかどうかを確認するフラグ

        // どちらのプレイヤーも置ける場所がない場合
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                // 黒または白の石が置けるか
                if (CanPlaceStone(r, c, CellState.Black) || CanPlaceStone(r, c, CellState.White))
                {
                    hasAvailableMoves = true;
                    break;
                }
            }

            if (hasAvailableMoves) break;
        }

        // 置ける場所がない場合
        if (!hasAvailableMoves)
        {
            int blackCount; // 黒の石の数をカウント
            int whiteCount; // 白の石の数をカウント

            // 盤上にある黒、白の石の数を数える
            CountStones(out blackCount, out whiteCount);

            Debug.Log($"ゲーム終了! 黒の石: {blackCount}, 白の石: {whiteCount}");

            // 勝敗判定
            if (blackCount > whiteCount)
            {
                Debug.Log("黒の勝利!");
            }
            else if (whiteCount > blackCount)
            {
                Debug.Log("白の勝利!");
            }
            else
            {
                Debug.Log("引き分け!");
            }

            _gameEnd = true; // ゲーム終了
        }
    }

    /// <summary>
    /// 黒か白の石が置ける場所があるかどうか
    /// </summary>
    /// <param name="state"></param>
    /// <returns>どこかに石を置ける場所がある場合は true、ない場合は false</returns>
    private bool CanPlaceStoneAnywhere(CellState state)
    {
        // 全セルをチェック
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (CanPlaceStone(r, c, state))
                {
                    return true; // どこかに石を置ける場所がある
                }
            }
        }
        return false; // どこにも石を置けない
    }

    /// <summary>
    /// 全セルをチェックして、石の数をカウントする
    /// </summary>
    /// <param name="blackCount">黒の石の数</param>
    /// <param name="whiteCount">白の石の数</param>
    private void CountStones(out int blackCount, out int whiteCount)
    {
        blackCount = 0;
        whiteCount = 0;

        // 全セルをチェック
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_cells[r, c].State == CellState.Black)
                {
                    blackCount++;
                }
                else if (_cells[r, c].State == CellState.White)
                {
                    whiteCount++;
                }
            }
        }
    }
}

using UnityEngine;

/// <summary>
/// セルの状態。
/// </summary>
public enum CellState
{
    /// <summary>
    /// 何も置かれていない
    /// </summary>
    None,

    /// <summary>
    /// 黒石。
    /// </summary>
    Black,

    /// <summary>
    /// 白石。
    /// </summary>
    White
}

/// <summary>
/// オセロのセル。
/// </summary>
public class Cell : MonoBehaviour
{
    [SerializeField]
    private Tile _tile = null; // タイル

    [SerializeField]
    private Stone _stone = null; // 石

    [SerializeField]
    private CellState _state = CellState.None;

    /// <summary>
    /// セルの状態。
    /// </summary>
    public CellState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged();
        }
    }

    private void OnValidate()
    {
        OnStateChanged();
    }

    /// <summary>
    /// 状態が更新されたときに呼び出される。
    /// </summary>
    private void OnStateChanged()
    {
        _stone.gameObject.SetActive(State != CellState.None);
        _stone.State = State switch
        {
            CellState.Black => StoneState.Black,
            CellState.White => StoneState.White,
            _ => StoneState.Black,
        };
    }
}
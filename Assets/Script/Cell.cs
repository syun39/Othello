using UnityEngine;

/// <summary>
/// �Z���̏�ԁB
/// </summary>
public enum CellState
{
    /// <summary>
    /// �����u����Ă��Ȃ�
    /// </summary>
    None,

    /// <summary>
    /// ���΁B
    /// </summary>
    Black,

    /// <summary>
    /// ���΁B
    /// </summary>
    White
}

/// <summary>
/// �I�Z���̃Z���B
/// </summary>
public class Cell : MonoBehaviour
{
    [SerializeField]
    private Tile _tile = null; // �^�C��

    [SerializeField]
    private Stone _stone = null; // ��

    [SerializeField]
    private CellState _state = CellState.None;

    /// <summary>
    /// �Z���̏�ԁB
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
    /// ��Ԃ��X�V���ꂽ�Ƃ��ɌĂяo�����B
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
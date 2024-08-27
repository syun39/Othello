using UnityEngine;

// TicTacToe���Q�l��
// ��邱��
// ��΂�u����悤�ɂ���

/// <summary>
/// �t�F�[�Y�Ǘ�
/// </summary>
public enum Turn
{
    /// <summary>
    /// ���̃^�[��(��j
    /// </summary>
    Black,

    /// <summary>
    /// ���̃^�[��(��j
    /// </summary>
    White
}

/// <summary>
/// �I�Z���S�̂̊Ǘ��B
/// </summary>
public class Othello : MonoBehaviour
{
    private const int Rows = 8; // �s��
    private const int Columns = 8; // ��
    private Cell[,] _cells; // �Z����ێ�����񎟌��z��

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // �}�X�ڂ̌���

    void Start()
    {
        _cells = new Cell[Rows, Columns];

        // �Ֆʂ̏�����
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
    /// �I�Z���̏����z�u
    /// </summary>
    private void InitialPosition()
    {
        _cells[3, 3].State = CellState.White;
        _cells[3, 4].State = CellState.Black;
        _cells[4, 3].State = CellState.Black;
        _cells[4, 4].State = CellState.White;
    }
}

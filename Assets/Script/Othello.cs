using UnityEngine;
using System.Collections.Generic;


// TicTacToe���Q�l��
// ��邱��
// �u����Ƃ���𐧌�
// �������Ԍv�Z�A���s���� AI �A�j���[�V�������g�������o�̎����i���Ԃ��A�j���[�V���� �������ԕ\��
// StoneState���g���ĂȂ����ǂ悢�̂��H

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
    private Tile[,] _tileSelect; // �I����Ԃ�ێ�����񎟌��z��

    private Turn _currentTurn = Turn.Black; // ���݂̃^�[��

    private int _selectedRow; // �I������Ă���s
    private int _selectedColumn; // �I������Ă����

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // �}�X�ڂ̌���

    // 8�����̈ړ��̔z��
    private int[,] _directions = new int[,]
    {
        // �s��
        { -1,0 }, // ��
        { 1,0 }, // ��
        { 0,-1 }, // ��
        { 0,1 }, // �E
        { -1,-1 }, // ����
        { -1,1 }, // �E��
        { 1,-1 }, // ����
        { 1,1 } // �E��
    };

    void Start()
    {
        // ������
        _cells = new Cell[Rows, Columns];
        _tileSelect = new Tile[Rows, Columns];

        // �Ֆʂ̏�����
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var cell = Instantiate(_cellPrefab, transform);
                cell.transform.position = new(c * _space, 0, -r * _space);
                cell.name = $"Cell({r}, {c})";
                cell.State = CellState.None; // ������Ԃ� None

                // �z��Ɋi�[
                _cells[r, c] = cell;
                _tileSelect[r, c] = cell.Tile;
            }
        }
        InitialPosition();
    }

    private void Update()
    {
        MoveCell();

        // �Z���̍X�V
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                _tileSelect[r, c].State = TileState.None;
            }
        }

        // �I�����ꂽ�^�C���̏�Ԃ� Selected ��
        _tileSelect[_selectedRow, _selectedColumn].State = TileState.Selected;

        if (Input.GetKeyDown(KeyCode.Return)) // �G���^�[�L�[����������
        {
            // ��΂��u����ĂȂ�������
            if (_cells[_selectedRow, _selectedColumn].State == CellState.None)
            {
                // �^�[���ɉ������΂�u��
                SetStone(_selectedRow, _selectedColumn, _currentTurn == Turn.Black ? CellState.Black : CellState.White);

                // ���݂̃^�[�������Ȃ甒�̃^�[���ɕύX�����łȂ���΍��̃^�[���ɕύX
                _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;
            }
        }
    }

    /// <summary>
    /// �΂�u��
    /// </summary>
    /// <param name="row">�΂�u���s</param>
    /// <param name="column">�΂�u����</param>
    /// <param name="state">�u���΂̏��</param>
    private void SetStone(int row, int column, CellState state)
    {
        _cells[row, column].State = state; // �΂�u��

        // �Ђ�����Ԃ���΂����邩�`�F�b�N
        ChangeStone(row, column, state == CellState.Black ? CellState.White : CellState.Black);
    }

    /// <summary>
    /// �΂��Ђ�����Ԃ�����
    /// </summary>
    /// <param name="rows">�΂�u�����s</param>
    /// <param name="col">�΂�u������</param>
    /// <param name="enemyColor">�G�̐΂̐F</param>
    /// <returns>�Ђ�����Ԃ����΂̐�</returns>
    private int ChangeStone(int rows, int col, CellState enemyColor)
    {
        bool isChangeStone = false; // �΂��Ђ�����Ԃ��邩�ǂ���
        List<(int rows, int col)> stones = new List<(int rows, int col)>(); // �Ђ�����Ԃ��΂̃��X�g(�^�v��)
        int count = 0; // �Ђ�����Ԃ����΂̐�

        for (int i = 0; i < _directions.GetLength(0); i++)
        {
            int moverow = rows; // ���݂̍s�̈ʒu
            int movecolumn = col; // ���݂̗�̈ʒu

            int dirrow = _directions[i, 1]; // �s�̈ړ���
            int dircol = _directions[i, 0]; // ��̈ړ���
            isChangeStone = false;
            stones.Clear();

            // �����ɉ����ĒT��
            while (true)
            {
                moverow += dirrow; // �s���ړ�
                movecolumn += dircol; // ����ړ�

                // ���E�𒴂�����I��
                if (moverow < 0 || moverow >= Rows || movecolumn < 0 || movecolumn >= Columns)
                {
                    break;
                }

                // ���݂̃Z���ɓG�̐΂������
                if (_cells[moverow, movecolumn].State == enemyColor)
                {
                    stones.Add((moverow, movecolumn)); // �Ђ�����Ԃ����Ƃ��ă��X�g�ɒǉ�
                }
                // ���݂̃Z���Ɏ����̐F�̐΂�����ꍇ
                else if (_cells[moverow, movecolumn].State == (enemyColor == CellState.Black ? CellState.White : CellState.Black))
                {
                    // �Ђ�����Ԃ����Ƃ��\
                    isChangeStone = true;
                    break;
                }
                else
                {
                    // �΂��u����Ă��Ȃ��A�����̐΂�����ΏI��
                    break;
                }
            }

            // �Ђ�����Ԃ����Ƃ��ł���ꍇ
            if (isChangeStone)
            {
                count += stones.Count; // �Ђ�����Ԃ���΂̐����J�E���g

                foreach (var (changerow,changecolumn) in stones)
                {
                    // ���X�g�ɂ���S�Ă̐΂��Ђ�����Ԃ�
                    _cells[changerow,changecolumn].State = (enemyColor == CellState.Black ? CellState.White : CellState.Black);
                }
            }
        }
        return count;
    }

    /// <summary>
    /// �Z���̈ړ��Ƌ��E�`�F�b�N
    /// </summary>
    private void MoveCell()
    {
        // �Z���̈ړ�
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { _selectedColumn--; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { _selectedColumn++; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { _selectedRow--; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { _selectedRow++; }

        // ���E�`�F�b�N
        if (_selectedColumn < 0) { _selectedColumn = 0; }
        if (_selectedColumn >= Columns) { _selectedColumn = Columns - 1; }
        if (_selectedRow < 0) { _selectedRow = 0; }
        if (_selectedRow >= Rows) { _selectedRow = Rows - 1; }
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

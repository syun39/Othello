using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    [SerializeField] float _turnTime = 30f; // �^�[���̎�������
    [SerializeField] float _remainingTime = 1.1f; // �c�莞��

    [SerializeField] private Text _timerText; // �^�C�}�[�\���p�iUI�j

    [SerializeField] private Text _turnText; // ���݂̃^�[���\���p�iUI�j

    private bool _gameEnd = false; // �Q�[�����I�����Ă邩�ǂ���



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
        InitialPosition(); // �����z�u
        PlaceableTile(); // �u����^�C���̐F��ς���

        _remainingTime = _turnTime; // �^�[���^�C�}�[�����Z�b�g
    }



    private void Update()
    {
        if (_gameEnd)
        {
            return; // �Q�[�����I�����Ă�������͂��󂯕t���Ȃ�
        }

        MoveCell(); // �Z���̈ړ�

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

        PlaceableTile(); // �^�C���̐F�X�V 

        _remainingTime -= Time.deltaTime; // �c�莞�Ԃ��X�V

        // ���Ԃ��O�ȉ��Ȃ�
        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            Debug.Log("����");
            _gameEnd = true; // �Q�[���I��
            return;
        }

        // �^�C�}�[�\���̍X�V
        if (_timerText != null)
        {
            _timerText.text = $"�c��: {_remainingTime:F0}s";
        }

        // ���݂̑I���ꏊ�̃^�C�c�̏�Ԃ� Placeable �Ȃ�
        if (_tileSelect[_selectedRow, _selectedColumn].State == TileState.Placeable)
        {
            // �^�C���̏�Ԃ� PlaceableSelected ��
            _tileSelect[_selectedRow, _selectedColumn].State = TileState.PlaceableSelected;
        }

        if (Input.GetKeyDown(KeyCode.Return)) // �G���^�[�L�[����������
        {
            // �^�C���̏�Ԃ� PlaceableSelected ��������
            if (_tileSelect[_selectedRow, _selectedColumn].State == TileState.PlaceableSelected)
            {
                // ��΂��u����Ă��Ȃ��Ȃ�
                if (_cells[_selectedRow, _selectedColumn].State == CellState.None)
                {
                    // �^�[���ɉ������΂�u��
                    SetStone(_selectedRow, _selectedColumn, _currentTurn == Turn.Black ? CellState.Black : CellState.White);

                    // ���݂̃^�[�������Ȃ甒�̃^�[���ɕύX�����łȂ���΍��̃^�[���ɕύX
                    _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;

                    _remainingTime = _turnTime; // �^�[���^�C�}�[�����Z�b�g
                }
            }
        }

        // ���݂̃^�[����\��
        if (_turnText != null)
        {
            _turnText.text = _currentTurn == Turn.Black ? "���݂̃^�[��: ��" : "���݂̃^�[��: ��";
        }

        CheckGameEnd(); // �Q�[���I�����`�F�b�N

        // �p�X����
        if (!_gameEnd)
        {
            // ���݂̃v���C���[�̃^�[���Ő΂�u����ꏊ�����邩���m�F
            if (!CanPlaceStoneAnywhere(_currentTurn == Turn.Black ? CellState.Black : CellState.White))
            {
                Debug.Log($"{_currentTurn} �p�X");
                // �^�[����ύX���A�^�[���^�C�}�[�����Z�b�g
                _currentTurn = (_currentTurn == Turn.Black) ? Turn.White : Turn.Black;
                _remainingTime = _turnTime; // �^�[���^�C�}�[�����Z�b�g
            }
        }
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
            int moveRow = rows; // ���݂̍s�̈ʒu
            int moveColumn = col; // ���݂̗�̈ʒu

            int dirrow = _directions[i, 1]; // �s�̈ړ���
            int dircol = _directions[i, 0]; // ��̈ړ���
            isChangeStone = false; // �Ђ�����Ԃ����Ƃ��ł��邩
            stones.Clear();

            // �����ɉ����ĒT��
            while (true)
            {
                moveRow += dirrow; // �s���ړ�
                moveColumn += dircol; // ����ړ�

                // ���E�𒴂�����I��
                if (moveRow < 0 || moveRow >= Rows || moveColumn < 0 || moveColumn >= Columns)
                {
                    break;
                }

                // ���݂̃Z���ɓG�̐΂������
                if (_cells[moveRow, moveColumn].State == enemyColor)
                {
                    stones.Add((moveRow, moveColumn)); // �Ђ�����Ԃ����Ƃ��ă��X�g�ɒǉ�
                }
                // ���݂̃Z���Ɏ����̐F�̐΂�����ꍇ
                else if (_cells[moveRow, moveColumn].State == (enemyColor == CellState.Black ? CellState.White : CellState.Black))
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

                foreach (var (changerow, changecolumn) in stones)
                {
                    // ���X�g�ɂ���S�Ă̐΂��Ђ�����Ԃ�
                    _cells[changerow, changecolumn].State = (enemyColor == CellState.Black ? CellState.White : CellState.Black);
                }
            }
        }
        return count;
    }

    /// <summary>
    /// �΂��u���邩�ǂ���
    /// </summary>
    /// <param name="row">�`�F�b�N����s�ԍ��B</param>
    /// <param name="col">�`�F�b�N�����ԍ��B</param>
    /// <param name="state">���݂̃v���C���[�̐΂̐F�i���܂��͔��j�B</param>
    /// <returns>�΂��u����ꍇ�� true ��Ԃ��A�u���Ȃ��ꍇ�� false ��Ԃ��B</returns>
    private bool CanPlaceStone(int row, int col, CellState state)
    {
        // �΂��u����Ă���ꍇ
        if (_cells[row, col].State != CellState.None)
        {
            return false;
        }

        // �G�̐΂̐F
        CellState enemyColor = (state == CellState.Black) ? CellState.White : CellState.Black;

        // 8�������m�F
        for (int i = 0; i < _directions.GetLength(0); i++)
        {
            // �ړ�
            int moveRow = row + _directions[i, 1];
            int moveColumn = col + _directions[i, 0];

            bool hasEnemyStone = false; // �G�̐΂����̕����ɂ��邩

            // �΂̏�Ԃ�T��
            while (moveRow >= 0 && moveRow < Rows && moveColumn >= 0 && moveColumn < Columns)
            {
                // �G�̐΂�����������
                if (_cells[moveRow, moveColumn].State == enemyColor)
                {
                    hasEnemyStone = true;
                }

                // �����̐΂�����������
                else if (_cells[moveRow, moveColumn].State == state)
                {
                    if (hasEnemyStone) // �ԂɓG�̐΂�����ꍇ
                    {
                        return true; // �΂�u����
                    }
                    break; // �Ȃ�������I��
                }
                else
                {
                    break;
                }

                // �����Ɩ������[�v�ɂȂ�
                // ����ɓ��������Ɉړ�
                moveRow += _directions[i, 1];
                moveColumn += _directions[i, 0];
            }
        }
        return false; // �ǂ̕����ɂ��΂��u���Ȃ��ꍇ
    }

    /// <summary>
    /// �u����ꏊ�̃^�C���̐F��ς���
    /// </summary>
    private void PlaceableTile()
    {
        // �S�Z�����`�F�b�N
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                // ���݂̃v���C���[�ɉ����Đ΂��u���邩
                if (CanPlaceStone(r, c, _currentTurn == Turn.Black ? CellState.Black : CellState.White))
                {
                    // �΂��u����Ȃ�^�C���� Placeable ��
                    _tileSelect[r, c].State = TileState.Placeable;
                }
            }
        }
    }

    /// <summary>
    /// �Q�[���̏I���A���s����
    /// </summary>
    private void CheckGameEnd()
    {
        bool hasAvailableMoves = false; // �u����ꏊ�����邩�ǂ������m�F����t���O

        // �ǂ���̃v���C���[���u����ꏊ���Ȃ��ꍇ
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                // ���܂��͔��̐΂��u���邩
                if (CanPlaceStone(r, c, CellState.Black) || CanPlaceStone(r, c, CellState.White))
                {
                    hasAvailableMoves = true;
                    break;
                }
            }

            if (hasAvailableMoves) break;
        }

        // �u����ꏊ���Ȃ��ꍇ
        if (!hasAvailableMoves)
        {
            int blackCount; // ���̐΂̐����J�E���g
            int whiteCount; // ���̐΂̐����J�E���g

            // �Տ�ɂ��鍕�A���̐΂̐��𐔂���
            CountStones(out blackCount, out whiteCount);

            Debug.Log($"�Q�[���I��! ���̐�: {blackCount}, ���̐�: {whiteCount}");

            // ���s����
            if (blackCount > whiteCount)
            {
                Debug.Log("���̏���!");
            }
            else if (whiteCount > blackCount)
            {
                Debug.Log("���̏���!");
            }
            else
            {
                Debug.Log("��������!");
            }

            _gameEnd = true; // �Q�[���I��
        }
    }

    /// <summary>
    /// �������̐΂��u����ꏊ�����邩�ǂ���
    /// </summary>
    /// <param name="state"></param>
    /// <returns>�ǂ����ɐ΂�u����ꏊ������ꍇ�� true�A�Ȃ��ꍇ�� false</returns>
    private bool CanPlaceStoneAnywhere(CellState state)
    {
        // �S�Z�����`�F�b�N
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (CanPlaceStone(r, c, state))
                {
                    return true; // �ǂ����ɐ΂�u����ꏊ������
                }
            }
        }
        return false; // �ǂ��ɂ��΂�u���Ȃ�
    }

    /// <summary>
    /// �S�Z�����`�F�b�N���āA�΂̐����J�E���g����
    /// </summary>
    /// <param name="blackCount">���̐΂̐�</param>
    /// <param name="whiteCount">���̐΂̐�</param>
    private void CountStones(out int blackCount, out int whiteCount)
    {
        blackCount = 0;
        whiteCount = 0;

        // �S�Z�����`�F�b�N
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

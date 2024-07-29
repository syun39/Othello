using UnityEngine;

/// <summary>
/// �I�Z���S�̂̊Ǘ��B
/// </summary>
public class Reversi : MonoBehaviour
{
    private const int Rows = 8; // �s��
    private const int Columns = 8; // ��

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // �}�X�ڂ̌���

    void Start()
    {
        // �Ֆʂ̏�����
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var cell = Instantiate(_cellPrefab, transform);
                cell.transform.position = new(c * _space, 0, -r * _space);
                cell.name = $"Cell({r}, {c})";
            }
        }
    }
}

using UnityEngine;

/// <summary>
/// オセロ全体の管理。
/// </summary>
public class Reversi : MonoBehaviour
{
    private const int Rows = 8; // 行数
    private const int Columns = 8; // 列数

    [SerializeField] Cell _cellPrefab = null;
    [SerializeField] float _space = 1.1f; // マス目の隙間

    void Start()
    {
        // 盤面の初期化
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

using UnityEngine;

/// <summary>
/// タイルの状態。
/// </summary>
public enum TileState
{
    /// <summary>
    /// 何もないデフォルトの状態。
    /// </summary>
    None,

    /// <summary>
    /// 選択されている。
    /// </summary>
    Selected,

    /// <summary>
    /// ここに石を置ける。
    /// </summary>
    Placeable
}


/// <summary>
/// 石を置く地面。
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField]
    private Material _noneMaterial = null; // デフォルト

    [SerializeField]
    private Material _selectedMaterial = null; // 選択中

    [SerializeField]
    private Material _placeableMaterial = null; // 石を置ける

    [SerializeField]
    private TileState _state;

    /// <summary>
    /// タイルの状態。
    /// </summary>
    public TileState State
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
        if (TryGetComponent<Renderer>(out var renderer))
        {
            // 状態に合わせてマテリアルを切り替える。
            renderer.material = State switch
            {
                TileState.None => _noneMaterial,
                TileState.Selected => _selectedMaterial,
                TileState.Placeable => _placeableMaterial,
                _ => null
            };
        }
    }
}

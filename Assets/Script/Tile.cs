using UnityEngine;

/// <summary>
/// �^�C���̏�ԁB
/// </summary>
public enum TileState
{
    /// <summary>
    /// �����Ȃ��f�t�H���g�̏�ԁB
    /// </summary>
    None,

    /// <summary>
    /// �I������Ă���B
    /// </summary>
    Selected,

    /// <summary>
    /// �����ɐ΂�u����B
    /// </summary>
    Placeable
}


/// <summary>
/// �΂�u���n�ʁB
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField]
    private Material _noneMaterial = null; // �f�t�H���g

    [SerializeField]
    private Material _selectedMaterial = null; // �I��

    [SerializeField]
    private Material _placeableMaterial = null; // �΂�u����

    [SerializeField]
    private TileState _state;

    /// <summary>
    /// �^�C���̏�ԁB
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
    /// ��Ԃ��X�V���ꂽ�Ƃ��ɌĂяo�����B
    /// </summary>
    private void OnStateChanged()
    {
        if (TryGetComponent<Renderer>(out var renderer))
        {
            // ��Ԃɍ��킹�ă}�e���A����؂�ւ���B
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

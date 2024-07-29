using UnityEngine;

/// <summary>
/// �΂̏�ԁB
/// </summary>
public enum StoneState
{
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
/// �I�Z���̐΁B
/// </summary>
public class Stone : MonoBehaviour
{
    [SerializeField]
    private StoneState _state;

    /// <summary>
    /// ���݂̐΂̏�ԁB��̖ʂ̐F�B
    /// </summary>
    public StoneState State
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
        // �΂̏�Ԃɍ��킹�ăI�u�W�F�N�g����]������
        var r = _state == StoneState.Black ? 0 : 180;
        transform.rotation = Quaternion.Euler(r, 0, 0);
    }
}

using UnityEngine;

/// <summary>
/// 石の状態。
/// </summary>
public enum StoneState
{
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
/// オセロの石。
/// </summary>
public class Stone : MonoBehaviour
{
    [SerializeField]
    private StoneState _state;

    /// <summary>
    /// 現在の石の状態。上の面の色。
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
    /// 状態が更新されたときに呼び出される。
    /// </summary>
    private void OnStateChanged()
    {
        // 石の状態に合わせてオブジェクトを回転させる
        var r = _state == StoneState.Black ? 0 : 180;
        transform.rotation = Quaternion.Euler(r, 0, 0);
    }
}

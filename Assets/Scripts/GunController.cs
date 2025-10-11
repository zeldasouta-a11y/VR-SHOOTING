using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GunController : MonoBehaviour
{
    /// <summary>
    /// 銃弾のプレハブ。
    /// 発砲した際に、このオブジェクトを弾として実体化する。
    /// </summary>
    [SerializeField]
    private GameObject m_bulletPrefab = null;

    /// <summary>
    /// 銃口の位置。
    /// 銃弾を実体化する時の位置や向きの設定などに使用する。
    /// </summary>
    [SerializeField]
    private Transform m_muzzlePos = null;

    /// <summary>
    /// VRコントローラーのトリガーが握られた時に呼び出す。
    /// </summary>
    public void Activate()
	{
        ShootAmmo();
    }

    /// <summary>
    /// 銃弾を生成する。
    /// </summary>
    public void ShootAmmo()
	{
        //弾のプレハブか銃口位置が設定されていなければ処理を行わず帰る。ついでに煽る。
        if(m_bulletPrefab == null ||
            m_muzzlePos == null)
		{
            Debug.Log(" Inspector の設定が間違ってるでww m9(^Д^)ﾌﾟｷﾞｬｰ ");
            return;
		}

        //弾を生成する。
        GameObject bulletObj = Instantiate(m_bulletPrefab);

        //弾の位置を、銃口の位置と同一にする。
        bulletObj.transform.position = m_muzzlePos.position;

        //弾の向きを、銃口の向きと同一にする。
        bulletObj.transform.rotation = m_muzzlePos.rotation;

    }
}
#if UNITY_EDITOR

// エディタ上で試射できるボタン（ビルドには含まれない）
[CustomEditor(typeof(GunController))]
public class GunControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var gun = (GunController)target;

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Shot (Editor)"))
        {
            gun.ShootAmmo();
        }
    }
}
#endif
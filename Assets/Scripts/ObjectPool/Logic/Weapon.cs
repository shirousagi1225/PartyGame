using Example;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public PlayerRef playerRef;
    [SerializeField] private WeaponName weaponName;
    //[SerializeField] private ActionAniType actionAniType;

    [Header("射線檢測設置")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform[] referencePoints;  //射線發射參照點

    private GameManager gameManager = GameManager.Instance;
    private Dictionary<int,Vector3> oldPointsDict= new Dictionary<int,Vector3>();  //存放上一幀位置訊息
    private RaycastHit[] raycastHits;
    private PlayerNetworkData playerNetworkData;

    public override void Spawned()
    {
        /*foreach (var playerNetworkData in gameManager.playerDict.Values)
        {
            if (playerNetworkData.actionAniType != actionAniType)
                continue;
            else
            {
                if (gameManager.gameNetworkData.playerDict.TryGet(playerNetworkData.playerRef, out NetworkObject player))
                {
                    transform.SetParent(player.transform.GetChild(0));
                    //transform.position = player.transform.GetChild(0).position;
                    transform.localScale = Vector3.one;
                    Debug.Log(playerNetworkData.playerRef + "：" + player);
                }

                oldPointsDict.Clear();

                if (oldPointsDict.Count == 0)
                {
                    raycastHits = new RaycastHit[gameManager.playerDict.Count];

                    for (int i = 0; i < referencePoints.Length; i++)
                        oldPointsDict.Add(referencePoints[i].GetHashCode(), referencePoints[i].position);
                }

                return;
            }
        }*/

        if (gameManager.Runner.GameMode == GameMode.Host)
        {
            playerNetworkData = null;
            oldPointsDict.Clear();

            if (oldPointsDict.Count == 0)
                raycastHits = new RaycastHit[gameManager.playerDict.Count];
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (playerRef.IsValid && gameManager.Runner.GameMode == GameMode.Host)
        {
            if (playerNetworkData == null)
            {
                gameManager.playerDict.TryGetValue(playerRef, out playerNetworkData);
                //Debug.Log(playerRef + "：" + playerNetworkData);
            }

            if (playerNetworkData.isAttack)
            {
                if (oldPointsDict.Count == 0)
                {
                    if(weaponName==WeaponName.Fist)
                        raycastHits = new RaycastHit[gameManager.playerDict.Count];

                    for (int i = 0; i < referencePoints.Length; i++)
                        oldPointsDict.Add(referencePoints[i].GetHashCode(), referencePoints[i].position);
                }

                var newStartPoint = startPoint.position;
                var newEndPoint = endPoint.position;

                Debug.DrawLine(newStartPoint, newEndPoint, Color.red, 1f);

                RayDetect(referencePoints);
            }
            else
                if (oldPointsDict.Count != 0 && weaponName == WeaponName.Fist)
                    oldPointsDict.Clear();
        }
    }

    //射線檢測
    private void RayDetect(Transform[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            var nowPos = points[i];
            oldPointsDict.TryGetValue(nowPos.GetHashCode(),out Vector3 oldPos);
            Debug.DrawRay(oldPos, nowPos.position-oldPos,Color.blue,1f);

            Ray ray=new Ray(oldPos, nowPos.position - oldPos);
            Physics.RaycastNonAlloc(ray,raycastHits,Vector3.Distance(oldPos, nowPos.position),layerMask,QueryTriggerInteraction.Ignore);

            foreach(var hit in raycastHits)
            {
                if (hit.collider == null) continue;

                Debug.Log("Hit Player："+hit.collider.gameObject);
            }

            raycastHits.Initialize();

            if (nowPos.position != oldPos)
                oldPointsDict[nowPos.GetHashCode()] = nowPos.position;
        }
    }

    private void OnGUI()
    {
        var labelStyle=new GUIStyle();
        labelStyle.fontSize = 32;
        labelStyle.normal.textColor = Color.white;
        int height = 40;
        GUIContent[] contents = new GUIContent[]
        {
            new GUIContent($"frameCount:{Time.frameCount }"),
        };

        for (int i = 0; i < contents.Length; i++)
            GUI.Label(new Rect(0, height * i, 180, 80), contents[i], labelStyle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.position + colCenter, colHalfExtents);
    }
}

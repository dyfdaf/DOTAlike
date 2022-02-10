 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace QFSW.MOP2
{
    public class PlayerControl : MonoBehaviour
    {
        public GameObject Arrow;
        public ObjectPool arrowsPool = null;

        NavMeshAgent agent;
        LineRenderer line;
        GameObject _arrow;
        bool isMoving;
        public float interval = 1;    // 箭头之间的间距
        bool isClicked;

        List<GameObject> arrows;
        // Start is called before the first frame update
        void Start()
        {
            arrowsPool.Initialize();

            agent = GetComponent<NavMeshAgent>();
            line = GetComponent<LineRenderer>();

            arrows = new List<GameObject>();
        }

        void DrawArrows()
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                // 画箭头
                float distance = Vector3.Distance(agent.path.corners[i], agent.path.corners[i + 1]);
                //        Debug.Log(distance);
                // 计算要画几个箭头
                int num = (int)(distance / interval);

                for (int j = 0; j < num; j++)
                {
                    Vector3 rot = (agent.path.corners[i + 1] - agent.path.corners[i]).normalized;

                    // 用于判断是1、2象限还是3、4象限
                    // Vector3 quadrant = rot + Vector3.right;


                    float angle = Vector3.Dot(rot, Vector3.right.normalized) / rot.sqrMagnitude / Vector3.right.normalized.sqrMagnitude;

                    if (rot.x > 0 && rot.z > 0)
                    {
                        angle = -Mathf.Acos(angle) * 180 / Mathf.PI;
                    }
                    else if (rot.x < 0 && rot.z > 0)
                    {
                        angle = -Mathf.Acos(angle) * 180 / Mathf.PI;
                    }
                    else if (rot.x < 0 && rot.z < 0)
                    {
                        angle = Mathf.Acos(angle) * 180 / Mathf.PI;
                    }
                    else
                    {
                        angle = Mathf.Acos(angle) * 180 / Mathf.PI;
                    }
                    rot.x = 90;
                    rot.y = angle;
                    rot.z = 0;
                    //Quaternion direction = Quaternion.LookRotation(rot);
                    Quaternion direction = Quaternion.Euler(rot);
                    //        _arrow = GameObject.Instantiate(Arrow, Vector3.Lerp(agent.path.corners[i], agent.path.corners[i + 1], (float)j / (float)num), direction);
                    _arrow = arrowsPool.GetObject(Vector3.Lerp(agent.path.corners[i], agent.path.corners[i + 1], (float)j / (float)num), direction);
          //          Debug.Log(_arrow);
                    arrows.Add(_arrow);
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            // 点击移动到指定点
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isHit = Physics.Raycast(ray, out hit, 100);
                if (isHit && hit.transform.tag == "Ground")
                {
                    agent.SetDestination(hit.point);
                }


                isMoving = true;
                isClicked = true;

                return;

                //if (arrows.Count > 0)
                //{
                //    for (int i = 0; i < arrows.Count; i++)
                //    {
                //        DestroyImmediate(arrows[0]);
                //        //             Destroy(arrows[i]);
                //        //                arrows.Remove(arrows[i]);
                //    }
                //    arrows.Clear();
                //    Debug.Log(arrows.Count);
                //}


            }

            if (isClicked)
            {
                Debug.Log(agent.hasPath);
                arrowsPool.ReleaseAll();
                arrows.Clear();


                DrawArrows();
                isClicked = false;
                return;
            }




            // 如果没在移动了
            if (agent.path.corners.Length == 1)
            {
                isMoving = false;
                //if (arrows.Count > 0)
                //{
                //    for (int i = 0; i < arrows.Count; i++)
                //    {
                //        //         Destroy(arrows[i]);
                //        //     arrows.Remove(arrows[i]);
                //    }
                //    arrows.Clear();
                //}

                arrowsPool.ReleaseAll();
                arrows.Clear();
            }



            // 随人物移动删除箭头
            if (isMoving && arrows.Count > 0)
            {
                for (int i = 0; i < arrows.Count; i++)
                {
                    //        Debug.Log(Mathf.Abs(Vector3.Distance(arrows[i].transform.position, transform.position)));
                    if (Mathf.Abs(Vector3.Distance(arrows[i].transform.position, transform.position)) < 0.2f)
                    {
                        arrowsPool.Release(arrows[i]);
                //        Destroy(arrows[i]);
                        arrows.Remove(arrows[i]);
                        break;
                    }
                }

            }


            // 根据行进路线画线和箭头
            //if (agent.path.corners.Length > 1)
            //{
            //    line.positionCount = agent.path.corners.Length;
            //    line.SetPositions(agent.path.corners);

            //}
            //else
            //{
            //    if (arrows.Count > 0)
            //    {
            //        for (int i=0; i < arrows.Count; i++)
            //        {
            //            Destroy(arrows[i]);
            //            arrows.Remove(arrows[i]);
            //        }
            //    }
            //}
        }

    }
}
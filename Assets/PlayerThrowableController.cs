using UnityEngine;

public class PlayerThrowableController : MonoBehaviour
{
    public GameObject grenadePrefab;
    public GameObject smokePrefab;
    public GameObject fireBombPrefab;

    public Transform throwPoint; // 投掷点，比如手的位置
    public float throwForce = 10f;

    private GameObject currentThrowable; // 当前手里拿的道具
    private int currentType = 0; // 0: 无, 1: 手雷, 2: 烟雾, 3: 燃烧

    // 新增：引用预测线脚本
    public TrajectoryLine trajectoryLine;

    void Start()
    {
        // 初始化手里拿个手雷
        SwitchThrowable(1);
    }

    void Update()
    {
        // 切换道具 (1, 2, 3 键)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchThrowable(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchThrowable(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchThrowable(3);

        // 准备投掷 (鼠标右键按住)
        if (currentThrowable != null && Input.GetMouseButton(1))
        {
            // 显示预测线
            Vector3 vel = CalculateVelocity(throwPoint.position, Camera.main.transform.forward, throwForce);
            trajectoryLine.ShowTrajectory(throwPoint.position, vel);
        }

        // 取消投掷或松开鼠标
        if (Input.GetMouseButtonUp(1))
        {
            trajectoryLine.HideTrajectory();
        }

        // 投掷 (鼠标左键)
        if (currentThrowable != null && Input.GetMouseButtonDown(0))
        {
            Throw();
        }
    }

    void SwitchThrowable(int type)
    {
        // 销毁手里旧的
        if (currentThrowable != null) Destroy(currentThrowable);

        currentType = type;
        GameObject prefabToSpawn = null;

        if (type == 1) prefabToSpawn = grenadePrefab;
        else if (type == 2) prefabToSpawn = smokePrefab;
        else if (type == 3) prefabToSpawn = fireBombPrefab;

        if (prefabToSpawn != null)
        {
            // 在手里生成一个新的道具模型
            currentThrowable = Instantiate(prefabToSpawn, throwPoint.position, Quaternion.identity);
            // 让它成为手的子物体，跟着手走
            currentThrowable.transform.SetParent(throwPoint);
            // 关闭物理，让它不掉落
            Rigidbody rb = currentThrowable.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
        }
    }

    void Throw()
    {
        if (currentThrowable == null) return;

        trajectoryLine.HideTrajectory();

        // 1. 解除父子关系，变成独立物体
        currentThrowable.transform.SetParent(null);

        // 2. 开启物理
        Rigidbody rb = currentThrowable.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // 3. 施加力
        Vector3 throwDir = Camera.main.transform.forward; // 朝摄像机方向扔
        rb.AddForce(throwDir * throwForce, ForceMode.Impulse);

        // 4. 投掷后手里空了（或者你可以选择立刻生成下一个）
        currentThrowable = null;
    }

    // 简单的物理公式反推初速度，用于画线
    Vector3 CalculateVelocity(Vector3 start, Vector3 direction, float force)
    {
        return direction.normalized * force;
    }
}
using System.Collections;
using UnityEngine;

public class WheelEffects : MonoBehaviour
{
    [SerializeField]private Transform SkidTrailPrefab;      //滑痕
    private static Transform skidTrailsDetachedParent;      //世界坐标，做为滑痕的parent
    [SerializeField] private ParticleSystem skidParticles;  //烟雾粒子效果
    [SerializeField] private AudioSource m_AudioSource;     //音源
    [SerializeField] private float slipLimit = 0.35f;       //标记为滑动的距离
        
    private bool skidding;      //是否在滑动中
        
    private Transform m_SkidTrail;
    private WheelCollider m_WheelCollider;
        
    private void Start()
    {
        skidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

        if ( skidParticles == null )
        {
            Debug.LogWarning( " no particle system found on car to generate smoke particles", gameObject );
        }
        else
        {
            EndSkidTrail();
            skidParticles.Stop();
        }

        m_WheelCollider = GetComponent<WheelCollider>();

        if ( skidTrailsDetachedParent == null )
        {
            skidTrailsDetachedParent = new GameObject( "Skid Trails - Detached" ).transform;
        }
    }

    private void FixedUpdate()
    {
        DetectSkid();
    }

    //检测打滑
    private void DetectSkid()
    {
        WheelHit hit;
        m_WheelCollider.GetGroundHit( out hit );
        if ( Mathf.Abs( hit.sidewaysSlip ) >= slipLimit && m_WheelCollider.attachedRigidbody.velocity.magnitude > 3f)
        {
            PlayAudio();
            EmitSmokeAndTrail();
        }
        else
        {
            //没有打滑时停止效果
            StopAudio();
            EndSkidTrail();
        }
    }

    //产生烟雾和车痕
    private void EmitSmokeAndTrail()
    {
        skidParticles.transform.position = transform.position - transform.up * m_WheelCollider.radius;
        skidParticles.Emit( 1 );    //发射粒子效果
            
        //如果没有标记打滑，开始生成滑痕
        if ( !skidding )
        {
            StartCoroutine( StartSkidTrail() );
        }
    }
        
    //开始播放音效
    private void PlayAudio()
    {
        //播放打滑音效
        if( !m_AudioSource.isPlaying )
        {
            m_AudioSource.time = Random.Range( 0, m_AudioSource.clip.length );
            m_AudioSource.Play();
        }
    }

    //停止播放音效
    private void StopAudio()
    {
        m_AudioSource.Stop();
    }

    //创建滑痕
    public IEnumerator StartSkidTrail()
    {
        skidding = true;    //标记为正在打滑
        m_SkidTrail = Instantiate( SkidTrailPrefab );   //实例化一条滑痕

        while ( m_SkidTrail == null )
        {
            yield return null;
        }

        m_SkidTrail.parent = transform;     
        m_SkidTrail.localPosition = -Vector3.up * m_WheelCollider.radius; //绑定滑痕到轮胎与地面接触位置
    }

    //结束创建滑痕，将它的parent transform设置为一个世界静止的transform，用于固定位置显示滑痕
    private void EndSkidTrail()
    {
        if ( !skidding )
        {
            return;
        }

        skidding = false;   //标记打滑结束

        m_SkidTrail.parent = skidTrailsDetachedParent;

        Destroy( m_SkidTrail.gameObject, 10 );  //在10秒后销毁此滑痕
    }
}

using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
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
            
            //如果没有标记打滑
            if ( !skidding )
            {
                StartCoroutine( StartSkidTrail() );
            }
        }
        
        //开始播放音效
        private void PlayAudio()
        {
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
            skidding = true;
            m_SkidTrail = Instantiate( SkidTrailPrefab );

            while ( m_SkidTrail == null )
            {
                yield return null;
            }

            m_SkidTrail.parent = transform;     
            m_SkidTrail.localPosition = -Vector3.up * m_WheelCollider.radius; //绑定滑痕到轮胎与地面接触位置
        }

        //结束创建滑痕
        private void EndSkidTrail()
        {
            if ( !skidding )
            {
                return;
            }

            skidding = false;
            m_SkidTrail.parent = skidTrailsDetachedParent;
            Destroy( m_SkidTrail.gameObject, 10 );
        }
    }
}
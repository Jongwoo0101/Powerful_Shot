using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LivingEntity, IDamageable
{
    public LayerMask whatISTarget; //추적 대상 레이어
    private LivingEntity targetEntity; //추적 대상
    private NavMeshAgent pathFinder; //경로 계산 AI 에이전트

    private Animator enemyAnimator; //애니메이터 컴포넌트
    private Renderer enemyRenderer; //렌더러 컴포넌트

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;
    NavMeshAgent nav;

    //추적할 대상이 존재하는지 알려주는 함수
    private bool hasTarget
    {
        get
        {
            if(targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        //초기화
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
    }

    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth;
        health = newHealth;

        damage = newDamage;

        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;
    }

    private void Start()
    {
        //게임 오브젝트 활성화와 동시에 ai의 추적 루틴 시
        nav.enabled = true;
        StartCoroutine(UpdatePath());
        
    }

    private void Update()
    {
        enemyAnimator.SetBool("HasTarget", hasTarget);
    }

    private IEnumerator UpdatePath() {
        while (!dead) {
            if (hasTarget) {
                //추적대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            } else {
                //추적 대상 없음 : AI 이동 중지
                pathFinder.isStopped = true;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatISTarget);
                for(int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if(livingEntity != null && !livingEntity.dead) {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            //0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
    }
    

    public override void Die()
    {
        base.Die();

        //다른 AI를 방해하지 않도록 자신의 모든 콜라이더를 비활성화
        Collider[]enemyColliders = GetComponents<Collider>();
        for(int i=0; i<enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        pathFinder.isStopped = true;
        pathFinder.enabled = false;

        enemyAnimator.SetTrigger("Die");

    }


    private void OnTriggerStay(Collider other)
    {
        if(!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            //상대방의 LivingEntity 타입 가져오기 시도
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            //상대방의 livingentity가 자신의 추적 대상이라면 공격 실행
            if(attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                //공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
                enemyAnimator.SetTrigger("HasTarget");
            }
        }
    }
}

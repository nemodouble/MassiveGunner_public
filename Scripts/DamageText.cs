using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace com.nemodouble.massiveGunner.Scripts
{
    public class DamageText : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0, 0.5f, 0);
        
        private TextMeshPro _text;
        
        private float _damage = 0;
        private const float MaxDeadTime = 0.5f;
        private float _deadTime = MaxDeadTime;
        private IDamageable _owner;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        private void OnEnable()
        {
            StartCoroutine(FadeOut());
        }

        private void OnDisable()
        {
            _damage = 0;
            _deadTime = MaxDeadTime;
            if (_owner == null) return;
            _owner.DamageTextDead();
            _owner = null;
        }

        public void AddDamage(float damage, IDamageable enemy)
        {
            _damage += damage;
            _deadTime = MaxDeadTime;
            _owner = enemy;
            transform.position = _owner.GetGameObject().transform.position + offset;

            _text.text = GameManger.Instance.ToKmbNumber(_damage);
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
        }

        private IEnumerator FadeOut()
        {
            while (_deadTime >= 0)
            {
                _deadTime -= 0.1f;
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _deadTime / MaxDeadTime);
                yield return new WaitForSeconds(0.1f);
            }
            GameManger.Instance.damageTextPool.ReturnObject(gameObject);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using ARLuft.Data;

namespace ARLuft
{
    public class ParticlesManager : MonoBehaviour
    {
        [SerializeField] private GameObject particlesParent;
        [SerializeField] private List<SingleParticleController> particleTypesList = new();
        [SerializeField] private float speed = 0.5f;

        [Header("PM - Particles' settings")]
        [SerializeField] private float minPM2Size = 0.01f;
        [SerializeField] private float maxPM2Size = 0.1f;
        [SerializeField] private float scaleFor10PM = 1.2f;

        private GameObject _template;
        private bool _particlesActive;

        //Only for nitrogen oxides
        private GameObject _noTemplate;
        private GameObject _no2Template;


        //Helpers for creation
        private Vector3 RandomVector(float min, float max)
        {
            var x = Random.Range(min, max);
            var y = Random.Range(min, max);
            var z = Random.Range(min, max);
            return new Vector3(x, y, z);
        }
        private void SetVelocity(Rigidbody rb)
        {
            rb.velocity = RandomVector(-0.05f, 0.05f);
            rb.AddForce(RandomVector(-10, 10) * speed, ForceMode.Acceleration);
            rb.AddTorque(RandomVector(0, 180), ForceMode.Acceleration);
        }
        private void SetParticleTemplate(ParticleType type)
        {
            foreach (SingleParticleController x in particleTypesList)
            {
                if (x.Type == type)
                {
                    _template = x.gameObject;
                    break;
                }
            }
        }
        private void SetNOXTemplates()
        {
            foreach (SingleParticleController x in particleTypesList)
            {
                if (x.Type == ParticleType.StickstoffMonoxid)
                    _noTemplate = x.gameObject;

                if (x.Type == ParticleType.StickstoffDioxid)
                    _no2Template = x.gameObject;

                if (_noTemplate && _no2Template)
                    break;
            }
        }
        private void ChangeStickoxideTemplate(bool mono)
        {
            if (mono)
                _template = _noTemplate;
            else
                _template = _no2Template;
        }
        private void ScalePMParticles(ParticleType type)
        {
            _template.transform.localScale = Random.Range(minPM2Size, maxPM2Size) * Vector3.one;
            if (type == ParticleType.Feinstaub10)
                _template.transform.localScale *= scaleFor10PM;
        }

        //Main methods
        public void SpawnParticles()
        {
            //Setup
            GameObject tempGo;
            bool nOxides = false;
            bool nMonoxideActive = false;

            transform.position = Camera.main.transform.position;
            ParticleType type = ParticleTypesDescription.GetValueOrDefault(
                    DataManager.Instance.SelectedPollutantForAR.core);
            SetParticleTemplate(type);

            //Stickoxide = nitrogen oxides -> include nitrogen monoxide and dioxide
            if (type == ParticleType.Stickoxide)
            {
                nOxides = true;
                SetNOXTemplates();
            }

            for (int i = 0; i < DataManager.Instance.SelectedPollutantForAR.value; i++)
            {
                //Change template in case of nitrogen oxides (from mono to dioxide)
                if (nOxides)
                {
                    ChangeStickoxideTemplate(nMonoxideActive);
                    nMonoxideActive = !nMonoxideActive;
                }

                //Random scaling of pm particles
                if (type == ParticleType.Feinstaub2 || type == ParticleType.Feinstaub10)
                    ScalePMParticles(type);

                //Instanciation
                tempGo = Instantiate(_template, particlesParent.transform);
                tempGo.transform.position = Camera.main.transform.position;
                tempGo.SetActive(true);

                SetVelocity(tempGo.GetComponent<Rigidbody>());
            }
            _particlesActive = true;
        }
        public void PlayParticles()
        {
            if (_particlesActive) return;

            Transform tempGO;
            Rigidbody tempRB;
            for (int i = 0; i < particlesParent.transform.childCount; i++)
            {
                tempGO = particlesParent.transform.GetChild(i);
                tempRB = tempGO.GetComponent<Rigidbody>();
                tempRB.constraints = RigidbodyConstraints.None;
                SetVelocity(tempRB);
            }
            _particlesActive = true;
        }
        public void StopParticles()
        {
            if (!_particlesActive) return;

            Transform tempGO;
            Rigidbody tempRB;
            for (int i = 0; i < particlesParent.transform.childCount; i++)
            {
                tempGO = particlesParent.transform.GetChild(i);
                tempRB = tempGO.GetComponent<Rigidbody>();
                tempRB.constraints = RigidbodyConstraints.FreezeAll;
            }

            _particlesActive = false;
        }
        public void RestartParticles()
        {
            _particlesActive = false;
            DestroyParticles();
            SpawnParticles();
        }

        private void DestroyParticles()
        {
            Transform tempGO;
            for (int i = 0; i < particlesParent.transform.childCount; i++)
            {
                tempGO = particlesParent.transform.GetChild(i);
                Destroy(tempGO.gameObject);
            }
        }

        public readonly Dictionary<string, ParticleType> ParticleTypesDescription = new()
        {
            { "chb", ParticleType.Benzol },
            { "co", ParticleType.KohlenstoffMonoxid },
            { "pm10", ParticleType.Feinstaub10 },
            { "pm2", ParticleType.Feinstaub2 },
            { "o3", ParticleType.Ozon },
            { "no2", ParticleType.StickstoffDioxid },
            { "no", ParticleType.StickstoffMonoxid },
            { "nox", ParticleType.Stickoxide },
            { "cht", ParticleType.Toluol }
        };
    }

    public enum ParticleType
    {
        Benzol,
        Feinstaub10,
        Feinstaub2,
        KohlenstoffMonoxid,
        Ozon,
        StickstoffDioxid,
        StickstoffMonoxid,
        Stickoxide,
        Toluol
    }
}
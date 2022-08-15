using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class PessoaExternaAppService : AppServiceBase<PESSOA_EXTERNA>, IPessoaExternaAppService
    {
        private readonly IPessoaExternaService _baseService;

        public PessoaExternaAppService(IPessoaExternaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public PESSOA_EXTERNA GetItemById(Int32 id)
        {
            PESSOA_EXTERNA item = _baseService.GetItemById(id);
            return item;
        }

        public PESSOA_EXTERNA GetByEmail(String email, Int32 idAss)
        {
            PESSOA_EXTERNA item = _baseService.GetByEmail(email, idAss);
            return item;
        }

        public List<PESSOA_EXTERNA> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idAss)
        {
            return _baseService.GetAllItensAdm(idAss);
        }

        public PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA conta, Int32 idAss)
        {
            PESSOA_EXTERNA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CARGO_USUARIO> GetAllCargos(Int32 idAss)
        {
            return _baseService.GetAllCargos(idAss);
        }

        public Int32 ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss, out List<PESSOA_EXTERNA> objeto)
        {
            try
            {
                objeto = new List<PESSOA_EXTERNA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(cargo, nome, cpf, email, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(PESSOA_EXTERNA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PEEX_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PEEX_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPEEX",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<PESSOA_EXTERNA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PESSOA_EXTERNA item, PESSOA_EXTERNA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditPEEX",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<PESSOA_EXTERNA>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<PESSOA_EXTERNA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PESSOA_EXTERNA item, PESSOA_EXTERNA itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PESSOA_EXTERNA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PEEX_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelePEEX",
                    LOG_TX_TEXTO = "Nome: " + item.PEEX_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PESSOA_EXTERNA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PEEX_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPEEX",
                    LOG_TX_TEXTO = "Nome: " + item.PEEX_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

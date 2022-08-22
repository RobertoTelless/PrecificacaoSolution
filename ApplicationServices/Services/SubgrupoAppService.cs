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
    public class SubgrupoAppService : AppServiceBase<SUBGRUPO_PLANO_CONTA>, ISubgrupoAppService
    {
        private readonly ISubgrupoService _baseService;

        public SubgrupoAppService(ISubgrupoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItens(Int32 idAss)
        {
            List<SUBGRUPO_PLANO_CONTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<SUBGRUPO_PLANO_CONTA> GetAllItensAdm(Int32 idAss)
        {
            List<SUBGRUPO_PLANO_CONTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public SUBGRUPO_PLANO_CONTA GetItemById(Int32 id)
        {
            SUBGRUPO_PLANO_CONTA item = _baseService.GetItemById(id);
            return item;
        }

        public List<GRUPO_PLANO_CONTA> GetAllGrupos(Int32 idAss)
        {
            List<GRUPO_PLANO_CONTA> lista = _baseService.GetAllGrupos(idAss);
            return lista;
        }

        public SUBGRUPO_PLANO_CONTA CheckExist(SUBGRUPO_PLANO_CONTA obj, Int32 idAss)
        {
            SUBGRUPO_PLANO_CONTA item = _baseService.CheckExist(obj, idAss);
            return item;
        }


        public Int32 ValidateCreate(SUBGRUPO_PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.SGCC_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddSUBG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<SUBGRUPO_PLANO_CONTA>(item)
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

        public Int32 ValidateEdit(SUBGRUPO_PLANO_CONTA item, SUBGRUPO_PLANO_CONTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditSUBG",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<SUBGRUPO_PLANO_CONTA>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<SUBGRUPO_PLANO_CONTA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(SUBGRUPO_PLANO_CONTA item, SUBGRUPO_PLANO_CONTA itemAntes)
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

        public Int32 ValidateDelete(SUBGRUPO_PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.PLANO_CONTA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.SGCC_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DeleSUBG",
                    LOG_TX_TEXTO = "SubGrupo: " + item.SGCC_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(SUBGRUPO_PLANO_CONTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SGCC_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatSUBG",
                    LOG_TX_TEXTO = "SubGrupo: " + item.SGCC_NM_NOME
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

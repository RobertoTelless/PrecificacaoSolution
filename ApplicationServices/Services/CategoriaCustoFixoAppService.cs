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
    public class CategoriaCustoFixoAppService : AppServiceBase<CATEGORIA_CUSTO_FIXO>, ICategoriaCustoFixoAppService
    {
        private readonly ICategoriaCustoFixoService _baseService;

        public CategoriaCustoFixoAppService(ICategoriaCustoFixoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllItens(Int32 idAss)
        {
            List<CATEGORIA_CUSTO_FIXO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllItensAdm(Int32 idAss)
        {
            List<CATEGORIA_CUSTO_FIXO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CATEGORIA_CUSTO_FIXO GetItemById(Int32 id)
        {
            CATEGORIA_CUSTO_FIXO item = _baseService.GetItemById(id);
            return item;
        }

        public CATEGORIA_CUSTO_FIXO CheckExist(CATEGORIA_CUSTO_FIXO conta, Int32 idAss)
        {
            CATEGORIA_CUSTO_FIXO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CATEGORIA_CUSTO_FIXO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.CACF_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCACF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CATEGORIA_CUSTO_FIXO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateEdit(CATEGORIA_CUSTO_FIXO item, CATEGORIA_CUSTO_FIXO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCACF",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CATEGORIA_CUSTO_FIXO>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CATEGORIA_CUSTO_FIXO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CATEGORIA_CUSTO_FIXO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CUSTO_FIXO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CACF_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCACF",
                    LOG_TX_TEXTO = item.CACF_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CATEGORIA_CUSTO_FIXO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CACF_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCACF",
                    LOG_TX_TEXTO = item.CACF_NM_NOME
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

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
    public class TipoEmbalagemAppService : AppServiceBase<TIPO_EMBALAGEM>, ITipoEmbalagemAppService
    {
        private readonly ITipoEmbalagemService _baseService;

        public TipoEmbalagemAppService(ITipoEmbalagemService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public TIPO_EMBALAGEM CheckExist(TIPO_EMBALAGEM conta, Int32 idAss)
        {
            TIPO_EMBALAGEM item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_EMBALAGEM> GetAllItens(Int32 idAss)
        {
            List<TIPO_EMBALAGEM> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_EMBALAGEM> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_EMBALAGEM> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_EMBALAGEM GetItemById(Int32 id)
        {
            TIPO_EMBALAGEM item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(TIPO_EMBALAGEM item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.TIEM_NR_ESTOQUE = item.TIEM_ESTOQUE_INICIAL;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTIEM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<TIPO_EMBALAGEM>(item)
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

        public Int32 ValidateEdit(TIPO_EMBALAGEM item, TIPO_EMBALAGEM itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTIEM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<TIPO_EMBALAGEM>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<TIPO_EMBALAGEM>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_EMBALAGEM item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PRODUTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIEM_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIEM",
                    LOG_TX_TEXTO = "Tipo: " + item.TIEM_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_EMBALAGEM item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIEM_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIEM",
                    LOG_TX_TEXTO = "Tipo: " + item.TIEM_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

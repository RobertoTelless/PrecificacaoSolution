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
    public class TemplatePropostaAppService : AppServiceBase<TEMPLATE_PROPOSTA>, ITemplatePropostaAppService
    {
        private readonly ITemplatePropostaService _baseService;

        public TemplatePropostaAppService(ITemplatePropostaService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_PROPOSTA> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_PROPOSTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_PROPOSTA CheckExist(TEMPLATE_PROPOSTA conta, Int32 idAss)
        {
            TEMPLATE_PROPOSTA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_PROPOSTA> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_PROPOSTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_PROPOSTA GetItemById(Int32 id)
        {
            TEMPLATE_PROPOSTA item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_PROPOSTA GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_PROPOSTA item = _baseService.GetByCode(sigla, idAss);
            return item;
        }

        public Int32 ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss, out List<TEMPLATE_PROPOSTA> objeto)
        {
            try
            {
                objeto = new List<TEMPLATE_PROPOSTA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(sigla, nome, conteudo, idAss);
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

        public Int32 ValidateCreate(TEMPLATE_PROPOSTA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia pr??via
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.TEPR_IN_ATIVO = 1;
                item.TEPR_TX_TEXTO = item.TEPR_TX_CABECALHO +  item.TEPR_TX_TEXTO + item.TEPR_TX_RODAPE;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddTEPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<TEMPLATE_PROPOSTA>(item)
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

        public Int32 ValidateEdit(TEMPLATE_PROPOSTA item, TEMPLATE_PROPOSTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EditTEEM",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = item.TEPR_SG_SIGLA + "|" + item.TEPR_NM_NOME,
                    LOG_TX_TEXTO_ANTES = String.Empty
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TEMPLATE_PROPOSTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CRM_PEDIDO_VENDA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TEPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTEPR",
                    LOG_TX_TEXTO = "Template: " + item.TEPR_NM_NOME
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TEMPLATE_PROPOSTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TEPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatTEPR",
                    LOG_TX_TEXTO = Serialization.SerializeJSON<TEMPLATE_PROPOSTA>(item)
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

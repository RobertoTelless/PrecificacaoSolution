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
using iText.Signatures;

namespace ApplicationServices.Services
{
    public class CustoFixoAppService : AppServiceBase<CUSTO_FIXO>, ICustoFixoAppService
    {
        private readonly ICustoFixoService _baseService;
        private readonly IConfiguracaoService _confService;
        private readonly IContaBancariaService _cbService;
        private readonly IFormaPagRecAppService _fpService;
        private readonly IContaPagarService _cpService;

        public CustoFixoAppService(ICustoFixoService baseService, IConfiguracaoService confService, IContaBancariaService cbService, IFormaPagRecAppService fpService, IContaPagarService cpService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
            _cbService = cbService;
            _fpService = fpService;
            _cpService = cpService;
        }

        public List<CUSTO_FIXO> GetAllItens(Int32 idAss)
        {
            List<CUSTO_FIXO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CUSTO_FIXO> GetAllItensAdm(Int32 idAss)
        {
            List<CUSTO_FIXO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CUSTO_FIXO GetItemById(Int32 id)
        {
            CUSTO_FIXO item = _baseService.GetItemById(id);
            return item;
        }

        public CUSTO_FIXO CheckExist(CUSTO_FIXO conta, Int32 idAss)
        {
            CUSTO_FIXO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CATEGORIA_CUSTO_FIXO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_CUSTO_FIXO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<PERIODICIDADE_TAREFA> GetAllPeriodicidades(Int32 idAss)
        {
            List<PERIODICIDADE_TAREFA> lista = _baseService.GetAllPeriodicidades(idAss);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, String nome, DateTime? dataInicio, DateTime? dataFinal, Int32 idAss, out List<CUSTO_FIXO> objeto)
        {
            try
            {
                objeto = new List<CUSTO_FIXO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, nome, dataInicio, dataFinal, idAss);
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

        public Int32 ValidateCreate(CUSTO_FIXO item, USUARIO usuario, out Int32 conta)
        {
            try
            {
                // Inicializa
                conta = 0;                
                
                // Verifica Existencia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Criticas
                if (item.CUFX_NR_DIA_VENCIMENTO < 1 || item.CUFX_NR_DIA_VENCIMENTO > 31)
                {
                    return 2;
                }
                if (item.CUFX_DT_INICIO == null || item.CUFX_DT_INICIO == DateTime.MinValue)
                {
                    return 3;
                }
                if (item.CUFX_DT_TERMINO == null || item.CUFX_DT_TERMINO == DateTime.MinValue)
                {
                    return 4;
                }
                if (item.CUFX_NR_DIA_VENCIMENTO < 1 || item.CUFX_NR_DIA_VENCIMENTO > 30)
                {
                    return 5;
                }

                // Completa objeto
                item.CUFX_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCUFX",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CUSTO_FIXO>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);

                // Recupera conta bancaria padrão e forma de pagamento
                CONTA_BANCO cb = _cbService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.COBA_IN_CONTA_PADRAO == 1).FirstOrDefault();
                FORMA_PAGTO_RECTO fp = _fpService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.COBA_CD_ID == cb.COBA_CD_ID & p.FOPA_IN_TIPO_FORMA == 1).FirstOrDefault();

                // Calcula valor dos lançamentos
                DateTime inicio = item.CUFX_DT_INICIO.Value.Date;
                Int32 intervalo = item.PERIODICIDADE_TAREFA.PETA_NR_DIAS.Value;
                Decimal valor = 0;

                // Gerar contas a pagar
                DateTime proxima = inicio;
                Int32 sai = 0;
                Int32 num = 1;
                DateTime? dataAjustada = null;
                while (sai == 0)
                {
                    // Calcula data de vencimento
                    proxima = inicio.AddDays(intervalo);
                    Int32 mes = proxima.Month;
                    if ((proxima.Month == inicio.Month + 1) || (inicio.Month == 12 & proxima.Month == 1)
                    {
                        dataAjustada = Convert.ToDateTime(item.CUFX_NR_DIA_VENCIMENTO.ToString() + "/" + proxima.Month.ToString() + "/" + proxima.Year.ToString());
                    }
                    else
                    {
                        if (proxima.Month == 3 & item.CUFX_NR_DIA_VENCIMENTO > 28)
                        {
                            dataAjustada = Convert.ToDateTime("28/" + (proxima.Month - 1).ToString() + "/" + proxima.Year.ToString());
                        }
                    }

                    // Grava CP
                    if (proxima <= item.CUFX_DT_TERMINO)
                    {
                        // Monta conta a pagar
                        CONTA_PAGAR cp = new CONTA_PAGAR();
                        cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        cp.USUA_CD_ID = usuario.USUA_CD_ID;
                        cp.FORN_CD_ID = item.FORN_CD_ID;
                        cp.FOPR_CD_ID = fp.FOPR_CD_ID;
                        cp.PETA_CD_ID = item.PETA_CD_ID;
                        cp.CUFX_CD_ID = item.CUFX_CD_ID;
                        cp.CECU_CD_ID = item.CECU_CD_ID;
                        cp.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
                        cp.CAPA_VL_VALOR = item.CUFX_VL_VALOR;
                        cp.CAPA_DS_DESCRICAO = "Custo Fixo: " + item.CUFX_NM_NOME;
                        cp.CAPA_IN_TIPO_LANCAMENTO = 1;
                        cp.CAPA_IN_LIQUIDADA = 0;
                        cp.CAPA_IN_ATIVO = 1;
                        cp.CAPA_DT_VENCIMENTO = dataAjustada;
                        cp.CAPA_VL_VALOR_PAGO = 0;
                        cp.CAPA_IN_PARCELADA = 0;
                        cp.CAPA_IN_PARCELAS = 0;
                        cp.CAPA_NR_DOCUMENTO = "FX" + item.CACF_CD_ID.ToString() + num.ToString();
                        cp.CAPA_DT_COMPETENCIA = proxima;
                        cp.CAPA_VL_DESCONTO = 0;
                        cp.CAPA_VL_JUROS = 0;
                        cp.CAPA_VL_TAXAS = 0;
                        cp.CAPA_VL_SALDO = item.CUFX_VL_VALOR;
                        cp.CAPA_IN_PAGA_PARCIAL = 0;
                        cp.CAPA_IN_LIQUIDA_NORMAL = 0;
                        cp.CAPA_IN_LIQUIDA_PARCELA = 0;
                        Int32 volta1 = _cpService.Create(cp);
                        num++;

                        // Recalcula data inicial
                        if (proxima.Month == inicio.Month + 1)
                        {
                            inicio = proxima;
                        }
                        else if (proxima.Month != inicio.Month + 1 & (inicio.Month == 12 & proxima.Month == 1)
                        {
                            inicio = proxima;
                        }
                        else if (proxima.Month != inicio.Month & (inicio.Month == 1 & proxima.Month == 3)
                        {
                            inicio = Convert.ToDateTime("28/" + (proxima.Month - 1).ToString() + "/" + proxima.Year.ToString()); 
                        }
                        else
                        {
                            inicio = proxima;
                        }
                   }
                    else
                    {
                        sai = 1;
                    }
                }
                conta = num;
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CUSTO_FIXO item, CUSTO_FIXO itemAntes, USUARIO usuario)
        {
            try
            {
                if (itemAntes.ASSINANTE != null)
                {
                    itemAntes.ASSINANTE = null;
                }
                if (itemAntes.CATEGORIA_CUSTO_FIXO != null)
                {
                    itemAntes.CATEGORIA_CUSTO_FIXO = null;
                }
                if (itemAntes.EMPRESA != null)
                {
                    itemAntes.EMPRESA = null;
                }

                // Criticas
                if (item.CUFX_NR_DIA_VENCIMENTO < 1 || item.CUFX_NR_DIA_VENCIMENTO > 31)
                {
                    return 2;
                }
                if (item.CUFX_DT_INICIO == null || item.CUFX_DT_INICIO == DateTime.MinValue)
                {
                    return 3;
                }
                if (item.CUFX_DT_TERMINO == null || item.CUFX_DT_TERMINO == DateTime.MinValue)
                {
                    return 4;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCUFX",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CUSTO_FIXO>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CUSTO_FIXO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CUSTO_FIXO item, CUSTO_FIXO itemAntes)
        {
            try
            {
                // Criticas
                if (item.CUFX_NR_DIA_VENCIMENTO < 1 || item.CUFX_NR_DIA_VENCIMENTO > 31)
                {
                    return 2;
                }
                if (item.CUFX_DT_INICIO == null || item.CUFX_DT_INICIO == DateTime.MinValue)
                {
                    return 3;
                }
                if (item.CUFX_DT_TERMINO == null || item.CUFX_DT_TERMINO == DateTime.MinValue)
                {
                    return 4;
                }

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CUSTO_FIXO item, USUARIO usuario, out Int32 conta)
        {
            try
            {
                // Inicializa
                conta = 0;
                
                // Verifica integridade referencial

                // Acerta campos
                item.CUFX_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCUFX",
                    LOG_TX_TEXTO = "Nome: " + item.CUFX_NM_NOME
                };

                // Persiste
                 Int32 volta = _baseService.Edit(item, log);

                // Excluir contas a pagar
                List<CONTA_PAGAR> cps = _cpService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.CUFX_CD_ID == item.CUFX_CD_ID).ToList();
                foreach (CONTA_PAGAR cp in cps)
                {
                    // Processa exclusão
                    cp.CAPA_IN_ATIVO = 0;
                    Int32 volta2 = _cpService.Edit(cp);
                    conta++;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CUSTO_FIXO item, USUARIO usuario, out Int32 conta)
        {
            try
            {
                // Inicializa
                conta = 0;

                // Verifica integridade referencial

                // Acerta campos
                item.CUFX_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatCUFX",
                    LOG_TX_TEXTO = "Nome: " + item.CUFX_NM_NOME
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);

                // Reativar contas a pagar
                List<CONTA_PAGAR> cps = _cpService.GetAllItens(usuario.ASSI_CD_ID).Where(p => p.CUFX_CD_ID == item.CUFX_CD_ID).ToList();
                foreach (CONTA_PAGAR cp in cps)
                {
                    // Processa exclusão
                    cp.CAPA_IN_ATIVO = 1;
                    Int32 volta2 = _cpService.Edit(cp);
                    conta++;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

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
using EntitiesServices.DTO;

namespace ApplicationServices.Services
{
    public class ContaReceberAppService : AppServiceBase<CONTA_RECEBER>, IContaReceberAppService
    {
        private readonly IContaReceberService _baseService;
        private readonly INotificacaoService _notiService;
        private readonly IUsuarioService _usuService;
        private readonly IContaBancariaService _cbService;
        private readonly IPeriodicidadeService _perService;
        private readonly IContaReceberRateioService _ratService;
        private readonly IFormaPagRecAppService _fopaService;

        public ContaReceberAppService(IContaReceberService baseService, INotificacaoService notiService, IUsuarioService usuService, IContaBancariaService cbService, IPeriodicidadeService perService, IContaReceberRateioService ratService, IFormaPagRecAppService fopaService) : base(baseService)
        {
            _baseService = baseService;
            _notiService = notiService;
            _usuService = usuService;
            _cbService = cbService;
            _perService = perService;
            _ratService = ratService;
            _fopaService = fopaService;
        }

        public CONTA_RECEBER_ANEXO GetAnexoById(Int32 id)
        {
            CONTA_RECEBER_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public CONTA_RECEBER_PARCELA GetParcelaById(Int32 id)
        {
            CONTA_RECEBER_PARCELA lista = _baseService.GetParcelaById(id);
            return lista;
        }

        public CONTA_RECEBER GetItemById(Int32 id)
        {
            CONTA_RECEBER item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss)
        {
            return _baseService.GetItensAtrasoCliente(idAss);
        }

        public List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetRecebimentosMes(mes, idAss);
        }

        public List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetAReceberMes(mes, idAss);
        }

        public List<CONTA_RECEBER> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss)
        {
            return _baseService.GetAllItensAdm(idAss);
        }

        public List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss)
        {
            return _baseService.GetVencimentoAtual(idAss);
        }

        public Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetTotalRecebimentosMes(mes, idAss);
        }

        public Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss)
        {
            return _baseService.GetTotalAReceberMes(mes, idAss);
        }
        
        public Int32 ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss, out List<CONTA_RECEBER> objeto)
        {
            try
            {
                objeto = new List<CONTA_RECEBER>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(cliId, ccId, dtLanc, data, dataFinal, descricao, aberto, conta, idAss);
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

        public Int32 ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss, out List<CONTA_RECEBER> objeto)
        {
            try
            {
                objeto = new List<CONTA_RECEBER>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterRecebimentoMes(clieId, ccId, desc, emissao, venc, liqui, idAss);
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

        public Int32 ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss, out List<CONTA_RECEBER> objeto)
        {
            try
            {
                objeto = new List<CONTA_RECEBER>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterAReceberMes(clieId, ccId, desc, emissao, venc, idAss);
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

        public Int32 ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss, out List<CONTA_RECEBER> objeto)
        {
            try
            {
                objeto = new List<CONTA_RECEBER>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterCRAtrasos(clieId, ccId, desc, emissao, venc, idAss);
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

        public Int32 ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss, out List<CONTA_RECEBER> objeto)
        {
            try
            {
                objeto = new List<CONTA_RECEBER>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterAtrasos(nome, cidade, uf, idAss);
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

        public Int32 ValidateCreate(CONTA_RECEBER item, Int32 recorrente, DateTime? data, USUARIO usuario)
        {
            try
            {
                // Verifica existencia pr??via

                // Completa objeto
                item.CARE_IN_ATIVO = 1;
                item.CARE_VL_DESCONTO = 0;
                item.CARE_VL_JUROS = 0;
                item.CARE_VL_PARCELADO = item.CARE_VL_VALOR;
                item.CARE_VL_PARCIAL = 0;
                item.CARE_VL_SALDO = item.CARE_VL_VALOR;
                item.CARE_VL_TAXAS = 0;
                item.CARE_VL_VALOR_LIQUIDADO = 0;
                item.CARE_IN_PARCELADA = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CONTA_RECEBER>(item)
                };

                if (item.CARE_DT_INICIO_PARCELA != null)
                {
                    // Checa data
                    if (item.CARE_DT_INICIO_PARCELA < DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Num.Parcelas
                    if (item.CARE_IN_PARCELAS < 2)
                    {
                        return 2;
                    }

                    // Verifica se ?? recorrente
                    if (recorrente > 0)
                    {
                        return 3;
                    }

                    // Acerta objeto
                    item.CARE_IN_PARCELADA = 1;
                }

                // Pagamento recorrente
                if (recorrente > 0)
                {
                    if (item.CARE_DT_INICIO_PARCELA != null)
                    {
                        return 3;
                    }
                    if (data == null)
                    {
                        return 4;
                    }
                }

                // Gera lan??amentos
                Int32 volta = 0;
                if (recorrente > 0)
                {
                    // Gera Notifica????o
                    NOTIFICACAO noti = new NOTIFICACAO();
                    noti.NOTC_DT_EMISSAO = DateTime.Now;
                    noti.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti.NOTC_IN_NIVEL = 1;
                    noti.NOTC_IN_VISTA = 0;
                    noti.NOTC_NM_TITULO = "Contas a Receber - Cria????o do Lan??amento Recorrente";
                    noti.NOTC_IN_ATIVO = 1;
                    noti.NOTC_TX_NOTIFICACAO = "O lan??amento recorrente " + item.CARE_NR_DOCUMENTO + " foi criado em " + DateTime.Today.Date.ToLongDateString() + " com " + recorrente.ToString() + " ocorr??ncias,  sob sua responsabilidade.";
                    noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti.CANO_CD_ID = 1;

                    // Persiste notifica????o
                    Int32 volta3 = _notiService.Create(noti);

                    // Gera ocorrencias
                    DateTime dataParcela = data.Value;
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= recorrente; i++)
                    {
                        CONTA_RECEBER cp = new CONTA_RECEBER();
                        cp.CARE_DS_DESCRICAO = item.CARE_DS_DESCRICAO + " - Ocorr??ncia " + i.ToString();
                        cp.CARE_DT_COMPETENCIA = item.CARE_DT_COMPETENCIA;
                        cp.CARE_DT_INICIO_PARCELA = item.CARE_DT_INICIO_PARCELA;
                        cp.CARE_DT_LANCAMENTO = item.CARE_DT_LANCAMENTO;
                        cp.CARE_DT_DATA_LIQUIDACAO = item.CARE_DT_DATA_LIQUIDACAO;
                        cp.CARE_DT_VENCIMENTO = dataParcela;
                        cp.CARE_IN_ABERTOS = item.CARE_IN_ABERTOS;
                        cp.CARE_IN_LIQUIDADA = item.CARE_IN_LIQUIDADA;
                        cp.CARE_IN_PAGA_PARCIAL = item.CARE_IN_PAGA_PARCIAL;
                        cp.CARE_IN_PARCELAS = item.CARE_IN_PARCELAS;
                        cp.CARE_IN_TIPO_LANCAMENTO = item.CARE_IN_TIPO_LANCAMENTO;
                        cp.CARE_NM_FAVORECIDO = item.CARE_NM_FAVORECIDO;
                        cp.CARE_NM_FORMA_PAGAMENTO = item.CARE_NM_FORMA_PAGAMENTO;
                        cp.CARE_NR_DOCUMENTO = item.CARE_NR_DOCUMENTO;
                        cp.CARE_TX_OBSERVACOES = item.CARE_TX_OBSERVACOES;
                        cp.CECU_CD_ID = item.CECU_CD_ID;
                        cp.COBA_CD_ID = item.COBA_CD_ID;
                        cp.FOPR_CD_ID = item.FOPR_CD_ID;
                        cp.CLIE_CD_ID = item.CLIE_CD_ID;
                        cp.PETA_CD_ID = item.PETA_CD_ID;
                        cp.USUA_CD_ID = item.USUA_CD_ID;
                        cp.CARE_VL_VALOR = item.CARE_VL_VALOR;
                        cp.CARE_IN_ATIVO = 1;
                        cp.CARE_VL_DESCONTO = 0;
                        cp.CARE_VL_JUROS = 0;
                        cp.CARE_VL_PARCELADO = item.CARE_VL_VALOR;
                        cp.CARE_VL_PARCIAL = 0;
                        cp.CARE_VL_SALDO = item.CARE_VL_SALDO;
                        cp.CARE_VL_TAXAS = 0;
                        cp.CARE_VL_VALOR_LIQUIDADO = 0;
                        cp.CARE_IN_PARCELADA = 0;
                        cp.ASSI_CD_ID = usuario.ASSI_CD_ID;
                        cp.FILI_CD_ID = item.FILI_CD_ID;
                        volta = _baseService.Create(cp);
                        dataParcela = dataParcela.AddDays(30);
                    }
                }
                else
                {
                    // Gera Notifica????o
                    NOTIFICACAO noti2 = new NOTIFICACAO();
                    noti2.NOTC_DT_EMISSAO = DateTime.Now;
                    noti2.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti2.NOTC_IN_NIVEL = 1;
                    noti2.NOTC_IN_VISTA = 0;
                    noti2.NOTC_NM_TITULO = "Contas a Receber - Cria????o do Lan??amento";
                    noti2.NOTC_IN_ATIVO = 1;
                    noti2.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_NR_DOCUMENTO + " foi criado em " + DateTime.Today.Date.ToLongDateString() + " sob sua responsabilidade.";
                    noti2.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti2.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti2.CANO_CD_ID = 1;

                    // Persiste Lancamento CR
                    volta = _baseService.Create(item);

                    // Persiste notifica????o
                    Int32 volta2 = _notiService.Create(noti2);

                    // Cria Parcelas
                    if (item.CARE_IN_PARCELADA == 1)
                    {
                        CONTA_RECEBER rec = _baseService.GetItemById(item.CARE_CD_ID);
                        DateTime dataParcela = rec.CARE_DT_INICIO_PARCELA.Value;
                        PERIODICIDADE_TAREFA period = _perService.GetItemById(item.PETA_CD_ID.Value);
                        if (dataParcela.Date <= DateTime.Today.Date)
                        {
                            dataParcela = dataParcela.AddMonths(1);
                        }

                        for (int i = 1; i <= rec.CARE_IN_PARCELAS; i++)
                        {
                            CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                            parc.CARE_CD_ID = item.CARE_CD_ID;
                            parc.CRPA_DT_QUITACAO = null;
                            parc.CRPA_DT_VENCIMENTO = dataParcela;
                            parc.CRPA_IN_ATIVO = 1;
                            parc.CRPA_IN_QUITADA = 0;
                            parc.CRPA_NR_PARCELA = i.ToString() + "/" + item.CARE_IN_PARCELAS.Value.ToString();
                            parc.CRPA_VL_RECEBIDO = 0;
                            parc.CRPA_VL_VALOR = item.CARE_VL_PARCELADO / item.CARE_IN_PARCELAS;
                            parc.CRPA_IN_PARCELA = i;
                            parc.CRPA_DS_DESCRICAO = rec.CARE_DS_DESCRICAO;
                            rec.CONTA_RECEBER_PARCELA.Add(parc);
                            dataParcela = dataParcela.AddDays(period.PETA_NR_DIAS.Value);
                        }
                        volta = _baseService.Edit(rec);
                    }
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONTA_RECEBER item, CONTA_RECEBER itemAntes, USUARIO usuario, DTO_CR dto)
        {
            try
            {
                //if (item.CENTRO_CUSTO != null)
                //{
                //    item.CENTRO_CUSTO = null;
                //}
                //if (item.CLIENTE != null)
                //{
                //    item.CLIENTE = null;
                //}
                //if (item.ASSINANTE != null)
                //{
                //    item.ASSINANTE = null;
                //}
                //if (item.CONTA_BANCO != null)
                //{
                //    item.CONTA_BANCO = null;
                //}
                //if (item.FILIAL != null)
                //{
                //    item.FILIAL = null;
                //}
                //if (item.FORMA_PAGAMENTO != null)
                //{
                //    item.FORMA_PAGAMENTO = null;
                //}
                //if (item.PEDIDO_VENDA != null)
                //{
                //    item.PEDIDO_VENDA = null;
                //}
                //if (item.PERIODICIDADE != null)
                //{
                //    item.PERIODICIDADE = null;
                //}
                //if (item.USUARIO != null)
                //{
                //    item.USUARIO = null;
                //}

                //  ****** Se for Liquida????o
                if (item.CARE_IN_LIQUIDA_NORMAL == 1)
                {
                    // Verifica se ?? parcelada
                    if (item.CARE_IN_PARCELADA == 1)
                    {
                        return 8;
                    }                  
                    
                    // Checa data
                    if (item.CARE_DT_DATA_LIQUIDACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica Valor
                    Decimal soma = 0;
                    if (item.CARE_IN_PAGA_PARCIAL == 0)
                    {
                        soma = item.CARE_VL_VALOR + item.CARE_VL_TAXAS.Value + item.CARE_VL_JUROS.Value - item.CARE_VL_DESCONTO.Value;
                    }
                    else
                    {
                        soma = item.CARE_VL_SALDO.Value + item.CARE_VL_TAXAS.Value + item.CARE_VL_JUROS.Value - item.CARE_VL_DESCONTO.Value;
                    }
                    if (soma != item.CARE_VL_VALOR_RECEBIDO)
                    {
                        return 2;
                    }

                    // Acerta objeto
                    item.CARE_IN_ATIVO = 1;
                    item.CARE_IN_LIQUIDADA = 1;
                    item.CARE_VL_SALDO = 0;
                    if (item.CARE_IN_PAGA_PARCIAL == 1)
                    {
                        item.CARE_VL_VALOR_RECEBIDO += item.CARE_VL_PARCIAL;
                    }

                    // Monta lan??amento banc??rio
                    FORMA_PAGTO_RECTO forma = _fopaService.GetItemById((Int32)item.FOPR_CD_ID);
                    CONTA_BANCO conta = _cbService.GetItemById((Int32)forma.COBA_CD_ID);
                    conta.COBA_VL_SALDO_ATUAL += item.CARE_VL_VALOR_RECEBIDO.Value;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = conta.COBA_CD_ID;
                    lanc.CBLA_DS_DESCRICAO = item.CARE_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CARE_DT_DATA_LIQUIDACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 1;
                    lanc.CBLA_NR_NUMERO = item.CARE_NR_DOCUMENTO;
                    if (item.CARE_IN_PAGA_PARCIAL == 0)
                    {
                        lanc.CBLA_VL_VALOR = item.CARE_VL_VALOR_RECEBIDO.Value;
                    }
                    else
                    {
                        item.CARE_VL_VALOR_LIQUIDADO += itemAntes.CARE_VL_VALOR_LIQUIDADO;
                        lanc.CBLA_VL_VALOR = item.CARE_VL_SALDO.Value;
                    }
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Gera Notifica????o
                    NOTIFICACAO noti1 = new NOTIFICACAO();
                    noti1.NOTC_DT_EMISSAO = DateTime.Now;
                    noti1.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti1.NOTC_IN_NIVEL = 1;
                    noti1.NOTC_IN_VISTA = 0;
                    noti1.NOTC_NM_TITULO = "Contas a Receber - Liquida????o do Lan??amento";
                    noti1.NOTC_IN_ATIVO = 1;
                    noti1.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " foi luiquidado em " + DateTime.Today.Date.ToLongDateString();
                    noti1.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti1.CANO_CD_ID = 1;

                    // Envia notifica????o
                    Int32 volta = _notiService.Create(noti1);

                    // Persiste lancamento Bancario
                    volta = _cbService.Edit(conta);

                    // Persiste Lancamento CR
                    volta = _baseService.Edit(item);

                    return 0;
                }

                //  ****** Se for Pagamento parcial
                if (item.CARE_IN_PARCIAL == 1)
                {
                    // Verifica se ?? parcelada
                    if (item.CARE_IN_PARCELADA == 1)
                    {
                        return 9;
                    }

                    // Checa data
                    if (item.CARE_DT_DATA_LIQUIDACAO > DateTime.Now.Date)
                    {
                        return 1;
                    }

                    // Verifica se ?? parcelada
                    if (item.CARE_IN_PARCELADA == 1)
                    {
                        return 3;
                    }

                    // Checa valor
                    if (item.CARE_VL_PARCIAL > item.CARE_VL_VALOR)
                    {
                        return 4;
                    }
                    if (item.CARE_VL_PARCIAL == item.CARE_VL_VALOR)
                    {
                        return 5;
                    }

                    // Acerta objeto
                    item.CARE_IN_ATIVO = 1;
                    item.CARE_IN_PAGA_PARCIAL = 1;
                    item.CARE_VL_SALDO = item.CARE_VL_VALOR - item.CARE_VL_PARCIAL.Value;
                    item.CARE_VL_VALOR_RECEBIDO = item.CARE_VL_PARCIAL.Value;

                    // Monta lan??amento banc??rio
                    FORMA_PAGTO_RECTO forma = _fopaService.GetItemById(item.FOPR_CD_ID.Value);
                    CONTA_BANCO conta = _cbService.GetItemById(forma.COBA_CD_ID);
                    conta.COBA_VL_SALDO_ATUAL += item.CARE_VL_PARCIAL.Value;
                    CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                    lanc.COBA_CD_ID = conta.COBA_CD_ID;
                    lanc.CBLA_DS_DESCRICAO = "Pagamento Parcial - " + item.CARE_DS_DESCRICAO;
                    lanc.CBLA_DT_LANCAMENTO = item.CARE_DT_DATA_LIQUIDACAO.Value;
                    lanc.CBLA_IN_ATIVO = 1;
                    lanc.CBLA_IN_ORIGEM = 0;
                    lanc.CBLA_IN_TIPO = 1;
                    lanc.CBLA_NR_NUMERO = item.CARE_NR_DOCUMENTO;
                    lanc.CBLA_VL_VALOR = item.CARE_VL_PARCIAL.Value;
                    conta.CONTA_BANCO_LANCAMENTO.Add(lanc);

                    // Gera Notifica????o
                    NOTIFICACAO noti1 = new NOTIFICACAO();
                    noti1.NOTC_DT_EMISSAO = DateTime.Now;
                    noti1.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti1.NOTC_IN_NIVEL = 1;
                    noti1.NOTC_IN_VISTA = 0;
                    noti1.NOTC_NM_TITULO = "Contas a Receber - Pagamento Parcial";
                    noti1.NOTC_IN_ATIVO = 1;
                    noti1.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " foi parcialmente liquidado em " + DateTime.Today.Date.ToLongDateString();
                    noti1.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti1.CANO_CD_ID = 1;

                    // Envia notifica????o
                    Int32 volta1 = _notiService.Create(noti1);

                    // Persiste lancamento Bancario
                    volta1 = _cbService.Edit(conta);

                    // Persiste Lancamento CR
                    volta1 = _baseService.Edit(item);

                    // Grava parcela
                    CONTA_RECEBER rec = _baseService.GetItemById(item.CARE_CD_ID);
                    DateTime dataParcela = DateTime.Today.Date;
                    CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                    parc.CARE_CD_ID = item.CARE_CD_ID;
                    parc.CRPA_DT_QUITACAO = DateTime.Today.Date;
                    parc.CRPA_DT_VENCIMENTO = dataParcela;
                    parc.CRPA_IN_ATIVO = 1;
                    parc.CRPA_IN_QUITADA = 1;
                    parc.CRPA_NR_PARCELA = "1/1";
                    parc.CRPA_VL_RECEBIDO = rec.CARE_VL_PARCIAL;
                    parc.CRPA_VL_VALOR = rec.CARE_VL_PARCIAL;
                    parc.CRPA_IN_PARCELA = 1;
                    parc.CRPA_DS_DESCRICAO = rec.CARE_DS_DESCRICAO;
                    rec.CONTA_RECEBER_PARCELA.Add(parc);
                    volta1 = _baseService.Edit(rec);
                    return 0;
                }

                //  ****** Se for Parcelamento
                if (dto.Parcelamento == 1)
                {
                    // Veriica se ?? parcial
                    if (item.CARE_IN_PAGA_PARCIAL == 1)
                    {
                        return 10;
                    }
                    
                    // Checa data
                    if (item.CARE_DT_INICIO_PARCELA < DateTime.Now.Date)
                    {
                        return 6;
                    }

                    // Verifica Num.Parcelas
                    if (item.CARE_IN_PARCELAS < 2)
                    {
                        return 7;
                    }

                    // Acerta objeto
                    item.CARE_IN_ATIVO = 1;
                    item.CARE_IN_PARCELADA = 1;

                    // Gera Notifica????o
                    NOTIFICACAO noti1 = new NOTIFICACAO();
                    noti1.NOTC_DT_EMISSAO = DateTime.Now;
                    noti1.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                    noti1.NOTC_IN_NIVEL = 1;
                    noti1.NOTC_IN_VISTA = 0;
                    noti1.NOTC_NM_TITULO = "Contas a Receber - Parcelamento";
                    noti1.NOTC_IN_ATIVO = 1;
                    noti1.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " foi parcelado em " + DateTime.Today.Date.ToLongDateString();
                    noti1.USUA_CD_ID = item.USUA_CD_ID.Value;
                    noti1.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    noti1.CANO_CD_ID = 1;

                    // Envia notifica????o
                    Int32 volta3 = _notiService.Create(noti1);

                    // Persiste Lancamento CR
                    volta3 =  _baseService.Edit(item);

                    // Cria Parcelas
                    CONTA_RECEBER rec = _baseService.GetItemById(item.CARE_CD_ID);
                    DateTime dataParcela = rec.CARE_DT_INICIO_PARCELA.Value;
                    PERIODICIDADE_TAREFA period = _perService.GetItemById(item.PETA_CD_ID.Value);
                    if (dataParcela.Date <= DateTime.Today.Date)
                    {
                        dataParcela = dataParcela.AddMonths(1);
                    }

                    for (int i = 1; i <= rec.CARE_IN_PARCELAS; i++)
                    {
                        CONTA_RECEBER_PARCELA parc = new CONTA_RECEBER_PARCELA();
                        parc.CARE_CD_ID = item.CARE_CD_ID;
                        parc.CRPA_DT_QUITACAO = null;
                        parc.CRPA_DT_VENCIMENTO = dataParcela;
                        parc.CRPA_IN_ATIVO = 1;
                        parc.CRPA_IN_QUITADA = 0;
                        parc.CRPA_NR_PARCELA = i.ToString() + "/" + item.CARE_IN_PARCELAS.Value.ToString();
                        parc.CRPA_VL_RECEBIDO = 0;
                        parc.CRPA_VL_VALOR = item.CARE_VL_PARCELADO / item.CARE_IN_PARCELAS;
                        parc.CRPA_IN_PARCELA = i;
                        parc.CRPA_DS_DESCRICAO = rec.CARE_DS_DESCRICAO;
                        rec.CONTA_RECEBER_PARCELA.Add(parc);
                        dataParcela = dataParcela.AddDays(period.PETA_NR_DIAS.Value);
                    }
                    volta3 = _baseService.Edit(rec);
                    return 0;
                }

                // ***** Se for Altera????o comum
                // Acerta objeto
                item.CARE_IN_ATIVO = 1;

                // Gera Notifica????o
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.NOTC_DT_EMISSAO = DateTime.Now;
                noti.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTC_IN_NIVEL = 1;
                noti.NOTC_IN_VISTA = 0;
                noti.NOTC_NM_TITULO = "Contas a Receber - Altera????o do Lan??amento";
                noti.NOTC_IN_ATIVO = 1;
                noti.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " foi alterado em " + DateTime.Today.Date.ToLongDateString();
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.CANO_CD_ID = 1;

                // Persiste notifica????o
                Int32 volta2 = _notiService.Create(noti);

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditSimples(CONTA_RECEBER item)
        {
           // Acerta objeto
            item.CARE_IN_ATIVO = 1;

            Int32 volta = _baseService.Edit(item);
            return 0;
        }

        public Int32 ValidateEditReceber(CONTA_RECEBER item, CONTA_RECEBER itemAntes, USUARIO usuario)
        {
            try
            {
                // Acerto do objeto
                item.CARE_IN_LIQUIDADA = 1;

                // gera lancamento conta bancaria
                CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                lanc.CBLA_DS_DESCRICAO = item.CARE_DS_DESCRICAO;
                lanc.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
                lanc.CBLA_IN_ATIVO = 1;
                lanc.CBLA_IN_ORIGEM = 2;
                lanc.CBLA_IN_TIPO = 1;
                lanc.CBLA_NR_NUMERO = item.CARE_NR_DOCUMENTO;
                lanc.CBLA_VL_VALOR = item.CARE_VL_VALOR_LIQUIDADO.Value;
                lanc.COBA_CD_ID = item.FORMA_PAGTO_RECTO.COBA_CD_ID;
                item.CONTA_BANCO.CONTA_BANCO_LANCAMENTO.Add(lanc);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CONTA_RECEBER>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CONTA_RECEBER>(itemAntes)
                };

                // Gera Notifica????o
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.NOTC_DT_EMISSAO = DateTime.Now;
                noti.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTC_IN_NIVEL = 1;
                noti.NOTC_IN_VISTA = 0;
                noti.NOTC_NM_TITULO = "Contas a Receber - Recebimento";
                noti.NOTC_IN_ATIVO = 1;
                noti.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " com vencimento em " + item.CARE_DT_VENCIMENTO.Value.ToLongDateString() + " de " + item.CARE_NM_FAVORECIDO + " foi liquidado em " + item.CARE_DT_DATA_LIQUIDACAO.Value.ToLongDateString() + " sendo recebido R$" + item.CARE_VL_VALOR_LIQUIDADO.Value.ToString() + " - Valor original a recceber R$" + item.CARE_VL_VALOR.ToString();
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.CANO_CD_ID = 1;

                // Persiste notifica????o
                Int32 volta = _notiService.Create(noti);

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditReceberNormal(CONTA_RECEBER item, CONTA_RECEBER itemAntes, USUARIO usuario)
        {
            try
            {
                // Acerto do objeto
                item.CARE_IN_LIQUIDADA = 1;

                // gera lancamento conta bancaria
                CONTA_BANCO_LANCAMENTO lanc = new CONTA_BANCO_LANCAMENTO();
                lanc.CBLA_DS_DESCRICAO = item.CARE_DS_DESCRICAO;
                lanc.CBLA_DT_LANCAMENTO = DateTime.Today.Date;
                lanc.CBLA_IN_ATIVO = 1;
                lanc.CBLA_IN_ORIGEM = 2;
                lanc.CBLA_IN_TIPO = 1;
                lanc.CBLA_NR_NUMERO = item.CARE_CD_ID.ToString();
                lanc.CBLA_VL_VALOR = item.CARE_VL_VALOR_LIQUIDADO.Value;
                lanc.COBA_CD_ID = item.FORMA_PAGTO_RECTO.COBA_CD_ID;
                item.CONTA_BANCO.CONTA_BANCO_LANCAMENTO.Add(lanc);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_LOG = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditCARE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_TEXTO = Serialization.SerializeJSON<CONTA_RECEBER>(item),
                    LOG_TX_TEXTO_ANTES = Serialization.SerializeJSON<CONTA_RECEBER>(itemAntes)
                };

                // Gera Notifica????o
                NOTIFICACAO noti = new NOTIFICACAO();
                noti.NOTC_DT_EMISSAO = DateTime.Now;
                noti.NOTC_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
                noti.NOTC_IN_NIVEL = 1;
                noti.NOTC_IN_VISTA = 0;
                noti.NOTC_NM_TITULO = "Contas a Receber - Recebimento";
                noti.NOTC_IN_ATIVO = 1;
                noti.NOTC_TX_NOTIFICACAO = "O lan??amento " + item.CARE_CD_ID.ToString() + " com vencimento em " + item.CARE_DT_VENCIMENTO.Value.ToLongDateString() + " de " + item.CARE_NM_FAVORECIDO + " foi liquidado em " + item.CARE_DT_DATA_LIQUIDACAO.Value.ToLongDateString() + " sendo recebido R$" + item.CARE_VL_VALOR_LIQUIDADO.Value.ToString() + " - Valor original a recceber R$" + item.CARE_VL_VALOR.ToString();
                noti.USUA_CD_ID = item.USUA_CD_ID.Value;
                noti.ASSI_CD_ID = usuario.ASSI_CD_ID;
                noti.CANO_CD_ID = 1;

                // Persiste notifica????o
                Int32 volta = _notiService.Create(noti);

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONTA_RECEBER item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CARE_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(CONTA_RECEBER item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CARE_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 IncluirRateioCC(CONTA_RECEBER item, Int32? cc, Int32? perc, USUARIO usuario)
        {
            try
            {
                // Verifica soma
                List<CONTA_RECEBER_RATEIO> lista = item.CONTA_RECEBER_RATEIO.ToList();
                Int32 soma = lista.Sum(p => p.CRRA_NR_PERCENTUAL.Value);
                Int32 total = soma + perc.Value;

                if (total > 100)
                {
                    return 1;
                }

                // Monta rateio
                CONTA_RECEBER_RATEIO rat = new CONTA_RECEBER_RATEIO();
                rat.CARE_CD_ID = item.CARE_CD_ID;
                rat.CECU_CD_ID = cc.Value;
                rat.CRRA_IN_ATIVO = 1;
                rat.CRRA_NR_PERCENTUAL = perc.Value;

                // Persiste
                return _ratService.Create(rat);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using PrecificacaoPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading.Tasks;

namespace ERP_Condominios_Solution.Controllers
{
    public class ContaPagarController : Controller
    {
        private readonly IBancoAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly IContaBancariaAppService contaApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IFornecedorAppService forApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly IContaPagarParcelaAppService ppApp;
        private readonly IUsuarioAppService usuApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IPeriodicidadeAppService perApp;
        private readonly IFormaPagRecAppService fpApp;
        private readonly IContaPagarRateioAppService ratApp;
        private readonly IFornecedorAppService fornApp;
        private readonly IContaReceberAppService recApp;

        private String msg;
        private Exception exception;
        String extensao = String.Empty;
        BANCO objetoBanco = new BANCO();
        BANCO objetoBancoAntes = new BANCO();
        List<BANCO> listaMasterBanco = new List<BANCO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        CONTA_BANCO objConta = new CONTA_BANCO();
        CONTA_BANCO objContaAntes = new CONTA_BANCO();
        List<CONTA_BANCO> listaMasterConta = new List<CONTA_BANCO>();
        CONTA_PAGAR objetoCP = new CONTA_PAGAR();
        CONTA_PAGAR objetoCPAntes = new CONTA_PAGAR();
        List<CONTA_PAGAR> listaCPMaster = new List<CONTA_PAGAR>();
        CONTA_PAGAR_PARCELA objetoCPP = new CONTA_PAGAR_PARCELA();
        CONTA_PAGAR_PARCELA objetoCPPAntes = new CONTA_PAGAR_PARCELA();
        List<CONTA_PAGAR_PARCELA> listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();

        public ContaPagarController(IBancoAppService baseApps, ILogAppService logApps, IContaBancariaAppService contaApps, IContaPagarAppService cpApps, IFornecedorAppService forApps, IContaPagarParcelaAppService ppApps, IContaBancariaAppService cbApps, IUsuarioAppService usuApps, ICentroCustoAppService ccApps, IPeriodicidadeAppService perApps, IFormaPagRecAppService fpApps, IContaPagarRateioAppService ratApps, IFornecedorAppService fornApps, IContaReceberAppService recApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            contaApp = contaApps;
            forApp = forApps;
            cpApp = cpApps;
            cbApp = cbApps;
            ppApp = ppApps;
            usuApp = usuApps;
            ccApp = ccApps;
            perApp = perApps;
            fpApp = fpApps;
            ratApp = ratApps;
            fornApp = fornApps;
            recApp = recApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult IncluirFornecedor()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["VoltaCP"] = 1;
            Session["VoltaFornecedor"] = 10;
            return RedirectToAction("IncluirFornecedor", "Fornecedor");

        }

        [HttpPost]
        public void LiquidarCPClick()
        {
            Session["LiquidaCP"] = 1;
        }

        [HttpGet]
        public ActionResult EditarContaBanco(Int32 id)
        {
            CONTA_PAGAR cp = cpApp.GetItemById(id);
            FORMA_PAGTO_RECTO forma = fpApp.GetItemById(cp.FOPR_CD_ID.Value);

            Session["FiltroLancamento"] = new CONTA_BANCO_LANCAMENTO
            {
                CBLA_DT_LANCAMENTO = cp.CAPA_DT_LIQUIDACAO.Value
            };

            Session["voltaLiquidacao"] = 1;
            Session["idContaPagar"] = id;

            return RedirectToAction("EditarConta", "Banco", new { id = forma.COBA_CD_ID });
        }

        [HttpPost]
        public JsonResult VerificaSaldo(Int32 valor)
        {
            CONTA_PAGAR item = (CONTA_PAGAR)Session["ContaPagar"];
            FORMA_PAGTO_RECTO forma = fpApp.GetItemById(item.FOPR_CD_ID.Value);
            if (valor < forma.CONTA_BANCO.COBA_VL_SALDO_ATUAL)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult GetRateio()
        {
            CONTA_PAGAR item = cpApp.GetItemById((Int32)Session["IdVolta"]);
            List<Hashtable> result = new List<Hashtable>();

            if (item.CONTA_PAGAR_RATEIO != null && item.CONTA_PAGAR_RATEIO.Count > 0)
            {
                List<Int32> lstCC = item.CONTA_PAGAR_RATEIO.Select(x => x.CECU_CD_ID).ToList<Int32>();

                foreach (var i in lstCC)
                {
                    Hashtable id = new Hashtable();
                    id.Add("id", i);
                    result.Add(id);
                }
            }
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaCP()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCP"] == null)
            {
                listaCPMaster = cpApp.GetAllItens(idAss);
                Session["ListaCP"] = listaCPMaster;
            }

            listaCPMaster = (List<CONTA_PAGAR>)Session["ListaCP"];
            ViewBag.Listas = ((List<CONTA_PAGAR>)Session["ListaCP"]).OrderByDescending(x => x.CAPA_DT_LANCAMENTO).ToList<CONTA_PAGAR>();
            ViewBag.Title = "Contas a Pagar";
            Session["Fornecedores"] = forApp.GetAllItens(idAss);
            ViewBag.Forn = new SelectList(((List<FORNECEDOR>)Session["Fornecedores"]).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllItens(idAss).OrderBy(x => x.CECU_NM_NOME).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            List<SelectListItem> tipoFiltro = new List<SelectListItem>();
            tipoFiltro.Add(new SelectListItem() { Text = "Somente em Aberto", Value = "1" });
            tipoFiltro.Add(new SelectListItem() { Text = "Somente Fechados", Value = "2" });
            ViewBag.Filtro = new SelectList(tipoFiltro, "Value", "Text");
            List<SelectListItem> atrasado = new List<SelectListItem>();
            atrasado.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            atrasado.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Atrasado = new SelectList(atrasado, "Value", "Text");
            ViewBag.Contas = new SelectList(contaApp.GetAllItens(idAss).OrderBy(p => p.COBA_NM_NOME_EXIBE), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            if ((Int32)Session["ErroSoma"] == 2)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["ErroSoma"] == 3)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0083", CultureInfo.CurrentCulture));
            }
            Session["ErroSoma"] = 0;
            Session["VoltaCP"] = 0;

            // Indicadores
            List<CONTA_PAGAR> pag = listaCPMaster;

            List<CONTA_PAGAR> lp1 = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_IN_PARCELADA == 0).ToList();
            Decimal pago = lp1.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year & p.CAPA_IN_PARCELADA == 0).Sum(p => p.CAPA_VL_VALOR_PAGO).Value;
            pago += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.Pago = pago;

            Decimal sumPagar = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumPagar += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.APagar = sumPagar;

            Decimal sumAtrasoCP = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumAtrasoCP += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).Sum(p => p.CPPA_VL_VALOR).Value;
            ViewBag.Atraso = sumAtrasoCP;

            List<CONTA_PAGAR> lp = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_IN_PARCELADA == 0).ToList();
            Int32 pagos = lp.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year & p.CAPA_IN_PARCELADA == 0).ToList().Count;
            pagos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).ToList().Count;

            Int32 atrasos = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Count();
            atrasos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).ToList().Count;

            Int32 pendentes = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).ToList().Count;
            pendentes += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).ToList().Count;
            
            ViewBag.CPS = listaCPMaster.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            Session["ContasBancarias"] = contaApp.GetAllItens(idAss);
            Session["VoltaPop"] = 0;
            if ((Int32)Session["IdCP"] != 0)
            {
                ViewBag.CodigoCP = (Int32)Session["IdCP"];
            }

            if ((Int32)Session["VoltaCP"] != 0)
            {
                ViewBag.Volta = (Int32)Session["VoltaCP"];
                Session["VoltaCP"] = 0;
            }

            if (Session["MensCP"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCP"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0194", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0195", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoCP = new CONTA_PAGAR();
            if (Session["FiltroCP"] != null)
            {
                objetoCP = (CONTA_PAGAR)Session["FiltroCP"];
            }
            else
            {
                objetoCP.CAPA_DT_LANCAMENTO = null;
                objetoCP.CAPA_DT_VENCIMENTO = null;
                objetoCP.CAPA_DT_LIQUIDACAO = null;
            }
            return View(objetoCP);
        }

        public ActionResult RetirarFiltroCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaCP"] = null;
            Session["FiltroCP"] = null;
            return RedirectToAction("MontarTelaCP");
        }

        public ActionResult MostrarTudoCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaCPMaster = cpApp.GetAllItensAdm(idAss);
            Session["FiltroCP"] = null;
            Session["ListaCP"] = listaCPMaster;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpPost]
        public ActionResult FiltrarCP(CONTA_PAGAR item, Int32? CAPA_ATRASO)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<CONTA_PAGAR> listaObj = new List<CONTA_PAGAR>();
                Session["FiltroCP"] = item;
                Int32 volta = cpApp.ExecuteFilter(item.FORN_CD_ID, item.CECU_CD_ID, item.CAPA_DT_LANCAMENTO, item.CAPA_DS_DESCRICAO, item.CAPA_IN_ABERTOS, item.CAPA_DT_VENCIMENTO, item.CAPA_DT_FINAL, item.CAPA_DT_LIQUIDACAO, CAPA_ATRASO, item.FORMA_PAGTO_RECTO.COBA_CD_ID, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    //return RedirectToAction("MontarTelaCP");
                }

                // Sucesso
                listaCPMaster = listaObj;
                Session["ListaCP"] = listaObj;
                return RedirectToAction("MontarTelaCP");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaCP");
            }
        }

        public ActionResult VoltarBaseCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaCP"] == 10)
            {
                return RedirectToAction("MontarCentralMensagens", "BaseAdmin");
            }
            if ((Int32)Session["VoltaPop"] == 1)
            {
                return RedirectToAction("MontarTelaPedidoCompra", "Compra");
            }
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult VerPagamentosMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCP"]).Where(p => p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null).ToList();
            lista = lista.Where(p => p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            ViewBag.ListaCP = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CAPA_VL_VALOR_PAGO);
            Session["ListaCPPagamentoMes"] = lista;
            return View();
        }

        [HttpGet]
        public ActionResult VerAPagarMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            List<CONTA_PAGAR> pag = cpApp.GetAllItens(idAss);
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCP"]).Where(p => p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CAPA_IN_PARCELADA == 0).ToList();
            List<CONTA_PAGAR_PARCELA> lista1 = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).ToList();

            Decimal sumPagar = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumPagar += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.APagar = sumPagar;

            ViewBag.ListaCP = lista;
            ViewBag.ListaParc = lista1;
            ViewBag.LR = lista.Count + lista1.Count;
            ViewBag.Valor = sumPagar;
            Session["ListaCPPagarMes"] = lista;
            Session["ListaCPParcPagarMes"] = lista1;
            return View();
        }

        [HttpGet]
        public ActionResult VerLancamentosAtrasoCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCP"]).Where(p => p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            ViewBag.ListaCP = lista;
            ViewBag.LR = lista.Count;
            ViewBag.Valor = lista.Sum(x => x.CAPA_VL_VALOR);
            Session["ListaCPAtraso"] = lista;
            return View();
        }

        [HttpGet]
        public ActionResult VerCP(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CONTA_PAGAR item = cpApp.GetItemById(id);
            Session["ContaPagar"] = item;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            Session["IdVolta"] = id;
            Session["IdCPVolta"] = 1;
            return View(vm);
        }

        [HttpPost]
        public void UploadFileToSession(IEnumerable<HttpPostedFileBase> files, String profile)
        {
            List<FileQueue> queue = new List<FileQueue>();

            foreach (var file in files)
            {
                FileQueue f = new FileQueue();
                f.Name = Path.GetFileName(file.FileName);
                f.ContentType = Path.GetExtension(file.FileName);

                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                f.Contents = ms.ToArray();

                if (profile != null)
                {
                    if (file.FileName.Equals(profile))
                    {
                        f.Profile = 1;
                    }
                }

                queue.Add(f);
            }
            Session["FileQueueCP"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueueLancamentoCP(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCP"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCP"] = 5;
                return RedirectToAction("VoltarAnexoCP");
            }

            CONTA_PAGAR item = cpApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 50)
            {
                Session["MensCP"] = 6;
                return RedirectToAction("VoltarAnexoCP");
            }
            String caminho = "/Imagens/1/ContaPagar/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_PAGAR_ANEXO foto = new CONTA_PAGAR_ANEXO();
            foto.CPAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CPAN_DT_ANEXO = DateTime.Today;
            foto.CPAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CPAN_IN_TIPO = tipo;
            foto.CPAN_NM_TITULO = fileName;
            foto.CAPA_CD_ID = item.CAPA_CD_ID;

            item.CONTA_PAGAR_ANEXO.Add(foto);
            objetoCPAntes = item;
            Int32 volta = cpApp.ValidateEdit(item, item, usu, 0, 0);
            if ((Int32)Session["IdCPVolta"] == 1)
            {
                return RedirectToAction("VoltarAnexoVerCP");
            }
            return RedirectToAction("VoltarAnexoCP");
        }

        [HttpPost]
        public ActionResult UploadFileLancamentoCP(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdCP"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensCP"] = 5;
                return RedirectToAction("VoltarAnexoCP");
            }

            CONTA_PAGAR item = cpApp.GetItemById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensCP"] = 6;
                return RedirectToAction("VoltarAnexoCP");
            }
            String caminho = "/Imagens/1/ContaPagar/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            CONTA_PAGAR_ANEXO foto = new CONTA_PAGAR_ANEXO();
            foto.CPAN_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.CPAN_DT_ANEXO = DateTime.Today;
            foto.CPAN_IN_ATIVO = 1;
            Int32 tipo = 3;
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                tipo = 1;
            }
            if (extensao.ToUpper() == ".MP4" || extensao.ToUpper() == ".AVI" || extensao.ToUpper() == ".MPEG")
            {
                tipo = 2;
            }
            if (extensao.ToUpper() == ".PDF")
            {
                tipo = 3;
            }
            foto.CPAN_IN_TIPO = tipo;
            foto.CPAN_NM_TITULO = fileName;
            foto.CAPA_CD_ID = item.CAPA_CD_ID;

            item.CONTA_PAGAR_ANEXO.Add(foto);
            objetoCPAntes = item;
            Int32 volta = cpApp.ValidateEdit(item, item, usu, 0, 0);
            if ((Int32)Session["IdCPVolta"] == 1)
            {
                return RedirectToAction("VoltarAnexoVerCP");
            }
            return RedirectToAction("VoltarAnexoCP");
        }

        [HttpGet]
        public ActionResult VerAnexoLancamentoCP(Int32 id)
        {

            // Prepara view
            CONTA_PAGAR_ANEXO item = cpApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadLancamentoCP(Int32 id)
        {
            CONTA_PAGAR_ANEXO item = cpApp.GetAnexoById(id);
            String arquivo = item.CPAN_AQ_ARQUIVO;
            Int32 pos = arquivo.LastIndexOf("/") + 1;
            String nomeDownload = arquivo.Substring(pos);
            String contentType = string.Empty;
            if (arquivo.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }
            else if (arquivo.Contains(".jpg"))
            {
                contentType = "image/jpg";
            }
            else if (arquivo.Contains(".png"))
            {
                contentType = "image/png";
            }
            return File(arquivo, contentType, nomeDownload);
        }

        [HttpGet]
        public ActionResult ExcluirCP(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCP"] = 2;
                    return RedirectToAction("MontarTelaCP");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = (CONTA_PAGAR)Session["ContaPagar"];
            item.CAPA_IN_ATIVO = 0;
            Int32 volta = cpApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCP"] = 4;
                return RedirectToAction("MontarTelaCP");
            }
            listaCPMaster = new List<CONTA_PAGAR>();
            Session["ListaCP"] = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult ReativarCP(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensCP"] = 2;
                    return RedirectToAction("MontarTelaCP");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_PAGAR item = cpApp.GetItemById(id);
            objetoCPAntes = (CONTA_PAGAR)Session["ContaPagar"];
            item.CAPA_IN_ATIVO = 1;
            Int32 volta = cpApp.ValidateReativar(item, usuario);
            listaCPMaster = new List<CONTA_PAGAR>();
            Session["ListaCP"] = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult VerParcelaCP(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara view
            CONTA_PAGAR_PARCELA item = cpApp.GetParcelaById(id);
            return View(item);
        }

        public ActionResult VoltarAnexoVerCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("VerCP", new { id = (Int32)Session["IdCP"] });
        }

        public ActionResult VoltarAnexoCP()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarCP", new { id = (Int32)Session["IdCP"] });
        }

        [HttpGet]
        public ActionResult IncluirCP()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCP"] = 2;
                    return RedirectToAction("MontarTelaCP");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
           
            // Prepara listas
            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(contaApp.GetAllItens(idAss).OrderBy(p => p.COBA_NM_NOME_EXIBE), "COBA_CD_ID", "COBA_NM_NOME_EXIBE_OLD");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME_FORMA");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens().OrderBy(p => p.PETA_NM_NOME), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> tipoPag = new List<SelectListItem>();
            tipoPag.Add(new SelectListItem() { Text = "Pagamento Recorrente", Value = "1" });
            tipoPag.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoPag, "Value", "Text");
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");

            if (Session["MensCP"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCP"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 12)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 13)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 14)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0200", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            CONTA_PAGAR item = new CONTA_PAGAR();
            ContaPagarViewModel vm = new ContaPagarViewModel();
            if ((Int32)Session["VoltaCompra"] != 0)
            {
                vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>((CONTA_PAGAR)Session["ContaPagar"]);
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                FORNECEDOR forn = fornApp.GetItemById(vm.FORN_CD_ID.Value);
                Session["FornTela"] = forn.FORN_NM_NOME;
                vm.FORN_CD_ID = forn.FORN_CD_ID;
                vm.CAPA_NM_FORNECEDOR = forn.FORN_NM_NOME;
                Session["ContaPagar"] = null;
            }
            else
            {
                vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
                vm.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
                vm.CAPA_IN_ATIVO = 1;
                vm.CAPA_DT_COMPETENCIA = DateTime.Today.Date;
                vm.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
                vm.CAPA_IN_LIQUIDADA = 0;
                vm.CAPA_IN_PAGA_PARCIAL = 0;
                vm.CAPA_IN_PARCELADA = 0;
                vm.CAPA_IN_PARCELAS = 0;
                vm.CAPA_VL_SALDO = 0;
                vm.USUA_CD_ID = usuario.USUA_CD_ID;
                vm.CAPA_IN_RECORRENTE = 0;
                vm.CAPA_DT_INICIO_RECORRENCIA = null;
                vm.CAPA_IN_CHEQUE = 0;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCP(ContaPagarViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            var result = new Hashtable();
            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(contaApp.GetAllItens(idAss).OrderBy(p => p.COBA_NM_NOME_EXIBE), "COBA_CD_ID", "COBA_NM_NOME_EXIBE_OLD");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME_FORMA");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens().OrderBy(p => p.PETA_NM_NOME), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> tipoPag = new List<SelectListItem>();
            tipoPag.Add(new SelectListItem() { Text = "Pagamento Recorrente", Value = "1" });
            tipoPag.Add(new SelectListItem() { Text = "Parcelamento", Value = "2" });
            ViewBag.Pagamento = new SelectList(tipoPag, "Value", "Text");
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    Int32 recorrencia = vm.CAPA_IN_RECORRENTE;
                    DateTime? data = vm.CAPA_DT_INICIO_RECORRENCIA;
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    FORMA_PAGTO_RECTO forma = fpApp.GetItemById(item.FOPR_CD_ID.Value);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreate(item, recorrencia, data, usuarioLogado);
                    
                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCP"] = 10;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensCP"] = 11;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCP"] = 12;
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensCP"] = 13;
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        Session["MensCP"] = 14;
                        return View(vm);
                    }

                    // Acerta numero
                    if (item.CAPA_NR_DOCUMENTO == null)
                    {
                        item.CAPA_NR_DOCUMENTO = item.CAPA_CD_ID.ToString();
                    }
                    
                    // Cria pastas
                    String caminho = "/Imagens/1/ContaPagar/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    Session["IdVolta"] = item.CAPA_CD_ID;
                    Session["IdCP"] = item.CAPA_CD_ID;
                    Session["VoltaCP"] = item.CAPA_CD_ID;
                    if (Session["FileQueueCP"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCP"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueLancamentoCP(file);
                        }

                        Session["FileQueueCP"] = null;
                    }

                    listaCPMaster = new List<CONTA_PAGAR>();
                    Session["ListaCP"] = null;

                    if ((Int32)Session["VoltaCompra"] == 1)
                    {
                        Session["VoltaCompra"] = 0;
                        return RedirectToAction("MontarTelaPedidoCompra", "Compra");
                    }
                    return RedirectToAction("MontarTelaCP");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCP(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCP"] = 2;
                    return RedirectToAction("MontarTelaCP");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            CONTA_PAGAR item = cpApp.GetItemById(id);
            FORMA_PAGTO_RECTO forma = fpApp.GetItemById(item.FOPR_CD_ID.Value);

            if (forma.CONTA_BANCO.COBA_VL_SALDO_ATUAL > item.CAPA_VL_VALOR)
            {
                ViewBag.TemSaldo = 1;
            }
            else
            {
                ViewBag.TemSaldo = 0;
            }

            // Prepara view
            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(contaApp.GetAllItens(idAss).OrderBy(p => p.COBA_NM_NOME_EXIBE), "COBA_CD_ID", "COBA_NM_NOME_EXIBE_OLD");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME_FORMA");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Liquida = 0;
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");
            Session["LiquidaCP"] = 0;

            if (item.CONTA_PAGAR_PARCELA.Count(x => x.CPPA_IN_QUITADA == 1) > 0)
            {
                ViewBag.Rateio = 0;
            }
            else
            {
                ViewBag.Rateio = 1;
            }

            objetoCPAntes = item;
            Session["ContaPagar"] = item;
            Session["IdVolta"] = id;
            Session["IdCPVolta"] = 2;
            Session["IdCP"] = id;
            Session["IdContaBanco"] = forma.COBA_CD_ID;
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>(item);
            vm.CAPA_VL_PARCELADO = vm.CAPA_VL_VALOR;
            vm.CAPA_DT_LIQUIDACAO = DateTime.Now;
            if (vm.CAPA_IN_PAGA_PARCIAL == 1)
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_SALDO;
            }
            else
            {
                vm.CAPA_VL_VALOR_PAGO = vm.CAPA_VL_VALOR;
            }

            if (Session["MensCP"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCP"] == 20)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0201", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 21)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0202", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 22)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0203", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 23)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0204", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 24)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0205", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 25)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 26)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 27)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0206", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 28)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0203", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 29)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0207", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 32)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0208", CultureInfo.CurrentCulture));
                }
            }
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarCP(ContaPagarViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(contaApp.GetAllItens(idAss).OrderBy(p => p.COBA_NM_NOME_EXIBE), "COBA_CD_ID", "COBA_NM_NOME_EXIBE_OLD");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME_FORMA");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PERI_CD_ID", "PERI_NM_NOME");
            ViewBag.Liquida = 0;
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    FORMA_PAGTO_RECTO forma = fpApp.GetItemById(item.FOPR_CD_ID.Value);
                    Session["eParcela"] = 0;
                    Int32 liq = (Int32)Session["LiquidaCP"];
                    Int32 parc = (Int32)Session["eParcela"];
                    item.CAPA_IN_LIQUIDA_NORMAL = liq;
                    item.CAPA_IN_LIQUIDA_PARCELA = parc;
                    Int32 volta = cpApp.ValidateEdit(item, objetoCPAntes, usuarioLogado, liq, parc);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCP"] = 20;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensCP"] = 21;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCP"] = 22;
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensCP"] = 23;
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        Session["MensCP"] = 24;
                        return View(vm);
                    }
                    if (volta == 6)
                    {
                        Session["MensCP"] = 25;
                        return View(vm);
                    }
                    if (volta == 7)
                    {
                        Session["MensCP"] = 26;
                        return View(vm);
                    }
                    if (volta == 8)
                    {
                        Session["MensCP"] = 27;
                        return View(vm);
                    }
                    if (volta == 9)
                    {
                        Session["MensCP"] = 28;
                        return View(vm);
                    }
                    if (volta == 10)
                    {
                        Session["MensCP"] = 29;
                        return View(vm);
                    }

                    Session["VoltaCP"] = item.CAPA_CD_ID;

                    // Sucesso
                    Session["IdCP"] = item.CAPA_CD_ID;
                    listaCPMaster = new List<CONTA_PAGAR>();
                    Session["ListaCP"] = null;
                    if (Session["FiltroCP"] != null)
                    {
                        FiltrarCP((CONTA_PAGAR)Session["FiltroCP"], null);
                    }

                    if (vm.CAPA_VL_PARCELADO != null && vm.CAPA_IN_PARCELAS != null && vm.CAPA_DT_INICIO_PARCELAS != null && vm.PETA_CD_ID != null)
                    {
                        return RedirectToAction("EditarCP", new { id = vm.CAPA_CD_ID });
                    }

                    return RedirectToAction("MontarTelaCP");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult DuplicarCP(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Monta novo lançamento
            CONTA_PAGAR item = cpApp.GetItemById(id);
            CONTA_PAGAR novo = new CONTA_PAGAR();
            novo.CAPA_DS_DESCRICAO = "Lançamento Duplicado - " + item.CAPA_DS_DESCRICAO;
            novo.CAPA_DT_COMPETENCIA = item.CAPA_DT_COMPETENCIA;
            novo.CAPA_DT_LANCAMENTO = DateTime.Today.Date;
            novo.CAPA_DT_VENCIMENTO = DateTime.Today.Date.AddDays(30);
            novo.CAPA_IN_ATIVO = 1;
            novo.CAPA_IN_LIQUIDADA = 0;
            novo.CAPA_IN_PAGA_PARCIAL = 0;
            novo.CAPA_IN_PARCELADA = 0;
            novo.CAPA_IN_PARCELAS = 0;
            novo.CAPA_IN_TIPO_LANCAMENTO = 0;
            novo.CAPA_NM_FAVORECIDO = item.CAPA_NM_FAVORECIDO;
            novo.CAPA_NR_DOCUMENTO = item.CAPA_NR_DOCUMENTO;
            novo.CAPA_TX_OBSERVACOES = item.CAPA_TX_OBSERVACOES;
            novo.CAPA_VL_DESCONTO = 0;
            novo.CAPA_VL_JUROS = 0;
            novo.CAPA_VL_PARCELADO = 0;
            novo.CAPA_VL_PARCIAL = 0;
            novo.CAPA_VL_SALDO = 0;
            novo.CAPA_VL_TAXAS = 0;
            novo.CAPA_VL_VALOR = item.CAPA_VL_VALOR;
            novo.CAPA_VL_VALOR_PAGO = 0;
            novo.CECU_CD_ID = item.CECU_CD_ID;
            novo.FORN_CD_ID = item.FORN_CD_ID;
            novo.COBA_CD_ID = item.COBA_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;
            novo.FOPR_CD_ID = item.FOPR_CD_ID;
            novo.PETA_CD_ID = item.PETA_CD_ID;
            novo.USUA_CD_ID = item.USUA_CD_ID;
            novo.CAPA_IN_CHEQUE = item.CAPA_IN_CHEQUE;
            novo.CAPA_NR_CHEQUE = item.CAPA_NR_CHEQUE;
            novo.COBA_CD_ID_1 = item.COBA_CD_ID_1;

            // Grava
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            Int32 volta = cpApp.ValidateCreate(novo, 0, null, usuarioLogado);

            // Cria pastas
            String caminho = "/Imagens/1/ContaPagar/" + novo.CAPA_CD_ID.ToString() + "/Anexos/";
            Directory.CreateDirectory(Server.MapPath(caminho));

            // Sucesso
            listaCPMaster = new List<CONTA_PAGAR>();
            Session["ListaCP"] = null;
            return RedirectToAction("MontarTelaCP");
        }

        [HttpGet]
        public ActionResult LiquidarParcelaCP(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Contas = new SelectList(cpApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            CONTA_PAGAR_PARCELA item = ppApp.GetItemById(id);
            CONTA_PAGAR cp = cpApp.GetById(item.CAPA_CD_ID);
            if (item.CPPA_VL_VALOR_PAGO < cp.FORMA_PAGTO_RECTO.CONTA_BANCO.COBA_VL_SALDO_ATUAL)
            {
                ViewBag.TemSaldo = 1;
            }
            else
            {
                ViewBag.TemSaldo = 0;
            }

            if (Session["MensCP"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCP"] == 30)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0209", CultureInfo.CurrentCulture));
                }
            }

            objetoCPPAntes = item;
            Session["ContaPagarParcela"] = item;
            Session["IdVoltaCPP"] = id;
            ContaPagarParcelaViewModel vm = Mapper.Map<CONTA_PAGAR_PARCELA, ContaPagarParcelaViewModel>(item);
            vm.CPPA_VL_DESCONTO = 0;
            vm.CPPA_VL_JUROS = 0;
            vm.CPPA_VL_TAXAS = 0;
            vm.CPPA_VL_VALOR_PAGO = vm.CPPA_VL_VALOR;
            vm.CPPA_DT_QUITACAO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        public ActionResult LiquidarParcelaCP(ContaPagarParcelaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            ViewBag.Contas = new SelectList(cpApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CONTA_PAGAR_PARCELA item = Mapper.Map<ContaPagarParcelaViewModel, CONTA_PAGAR_PARCELA>(vm);
                    Int32 volta = ppApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCP"] = 30;
                        return View(vm);
                    }

                    // Acerta saldo
                    CONTA_PAGAR rec = cpApp.GetItemById((Int32)Session["IdVolta"]);
                    CONTA_PAGAR cpEdit = new CONTA_PAGAR
                    {
                        //Atributos
                        CAPA_CD_ID = rec.CAPA_CD_ID,
                        USUA_CD_ID = rec.USUA_CD_ID,
                        COBA_CD_ID = rec.COBA_CD_ID,
                        FORN_CD_ID = rec.FORN_CD_ID,
                        FOPR_CD_ID = rec.FOPR_CD_ID,
                        PETA_CD_ID = rec.PETA_CD_ID,
                        CECU_CD_ID = rec.CECU_CD_ID,
                        CAPA_DT_LANCAMENTO = rec.CAPA_DT_LANCAMENTO,
                        CAPA_VL_VALOR = rec.CAPA_VL_VALOR,
                        CAPA_DS_DESCRICAO = rec.CAPA_DS_DESCRICAO,
                        CAPA_IN_TIPO_LANCAMENTO = rec.CAPA_IN_TIPO_LANCAMENTO,
                        CAPA_NM_FAVORECIDO = rec.CAPA_NM_FAVORECIDO,
                        CAPA_NM_FORMA_PAGAMENTO = rec.CAPA_NM_FORMA_PAGAMENTO,
                        CAPA_IN_LIQUIDADA = rec.CAPA_IN_LIQUIDADA,
                        CAPA_IN_ATIVO = rec.CAPA_IN_ATIVO,
                        CAPA_DT_VENCIMENTO = rec.CAPA_DT_VENCIMENTO,
                        CAPA_VL_VALOR_PAGO = rec.CAPA_VL_VALOR_PAGO,
                        CAPA_DT_LIQUIDACAO = rec.CAPA_DT_LIQUIDACAO,
                        CAPA_NR_ATRASO = rec.CAPA_NR_ATRASO,
                        CAPA_TX_OBSERVACOES = rec.CAPA_TX_OBSERVACOES,
                        CAPA_IN_PARCELADA = rec.CAPA_IN_PARCELADA,
                        CAPA_IN_PARCELAS = rec.CAPA_IN_PARCELAS,
                        CAPA_DT_INICIO_PARCELAS = rec.CAPA_DT_INICIO_PARCELAS,
                        CAPA_VL_PARCELADO = rec.CAPA_VL_PARCELADO,
                        CAPA_NR_DOCUMENTO = rec.CAPA_NR_DOCUMENTO,
                        CAPA_DT_COMPETENCIA = rec.CAPA_DT_COMPETENCIA,
                        CAPA_VL_DESCONTO = rec.CAPA_VL_DESCONTO,
                        CAPA_VL_JUROS = rec.CAPA_VL_JUROS,
                        CAPA_VL_TAXAS = rec.CAPA_VL_TAXAS,
                        CAPA_VL_SALDO = rec.CAPA_VL_SALDO,
                        CAPA_IN_PAGA_PARCIAL = rec.CAPA_IN_PAGA_PARCIAL,
                        CAPA_VL_PARCIAL = rec.CAPA_VL_PARCIAL,
                        CAPA_IN_ABERTOS = rec.CAPA_IN_ABERTOS,
                        CAPA_IN_FECHADOS = rec.CAPA_IN_FECHADOS,
                        ASSI_CD_ID = rec.ASSI_CD_ID,
                        PECO_CD_ID = rec.PECO_CD_ID,
                        COBA_CD_ID_1 = rec.COBA_CD_ID_1,
                        CAPA_IN_CHEQUE = rec.CAPA_IN_CHEQUE,
                        CAPA_NR_CHEQUE = rec.CAPA_NR_CHEQUE,
                        CAPA_IN_TIPO_DOC = rec.CAPA_IN_TIPO_DOC,

                        //Coleções
                        CONTA_PAGAR_ANEXO = rec.CONTA_PAGAR_ANEXO,
                        CONTA_PAGAR_PARCELA = rec.CONTA_PAGAR_PARCELA,
                        CONTA_PAGAR_RATEIO = rec.CONTA_PAGAR_RATEIO,
                    };

                    cpEdit.CAPA_VL_SALDO = cpEdit.CAPA_VL_SALDO - item.CPPA_VL_VALOR_PAGO;

                    // Verifica se liquidou todas
                    List<CONTA_PAGAR_PARCELA> lista = cpEdit.CONTA_PAGAR_PARCELA.Where(p => p.CPPA_IN_QUITADA == 0).ToList<CONTA_PAGAR_PARCELA>();
                    if (lista.Count == 0)
                    {
                        cpEdit.CAPA_IN_LIQUIDADA = 1;
                        cpEdit.CAPA_DT_LIQUIDACAO = DateTime.Today.Date;
                        cpEdit.CAPA_VL_VALOR_PAGO = cpEdit.CONTA_PAGAR_PARCELA.Sum(p => p.CPPA_VL_VALOR_PAGO);
                        cpEdit.CAPA_VL_SALDO = 0;
                    }
                    else if (cpEdit.CONTA_PAGAR_PARCELA.Count(p => p.CPPA_IN_QUITADA == 1) > 0)
                    {
                        cpEdit.CAPA_VL_VALOR_PAGO = cpEdit.CONTA_PAGAR_PARCELA.Where(x => x.CPPA_IN_QUITADA == 1).Sum(p => p.CPPA_VL_VALOR_PAGO);
                    }
                    Session["eParcela"] = 1;
                    cpEdit.CAPA_IN_LIQUIDA_NORMAL = 0;
                    cpEdit.CAPA_IN_LIQUIDA_PARCELA = 1;
                    volta = cpApp.ValidateEdit(cpEdit, rec, usuarioLogado, 0, 1);

                    // Sucesso
                    listaCPPMaster = new List<CONTA_PAGAR_PARCELA>();
                    Session["ListaCPP"] = null;

                    listaCPMaster = new List<CONTA_PAGAR>();
                    Session["ListaCP"] = null;
                    return RedirectToAction("VoltarBaseCP");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult IncluirRateioCC(ContaPagarViewModel vm)
        {

            try
            {
                // Executa a operação
                Int32? cc = vm.CECU_CD_RATEIO;
                Int32? perc = vm.CAPA_VL_PERCENTUAL;
                USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                CONTA_PAGAR item = cpApp.GetItemById(vm.CAPA_CD_ID);
                Int32 volta = cpApp.IncluirRateioCC(item, cc, perc, usuarioLogado);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensCP"] = 32;
                    return RedirectToAction("VoltarAnexoCP");
                }

                // Sucesso
                Session["IdVoltaTrab"] = 3;
                return RedirectToAction("VoltarAnexoCP");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("VoltarAnexoCP");
            }
        }

        [HttpGet]
        public ActionResult ExcluirRateio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            CONTA_PAGAR cp = (CONTA_PAGAR)Session["ContaPagar"];
            CONTA_PAGAR_RATEIO rl = ratApp.GetItemById(id);
            Int32 volta = ratApp.ValidateDelete(rl);
            Session["IdVoltaTrab"] = 3;
            return RedirectToAction("VoltarAnexoCP");
        }

        [HttpGet]
        public ActionResult ReativarRateio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
            CONTA_PAGAR cp = (CONTA_PAGAR)Session["ContaPagar"];
            CONTA_PAGAR_RATEIO rl = ratApp.GetItemById(id);
            Int32 volta = ratApp.ValidateReativar(rl, usuarioLogado);
            Session["IdVoltaTrab"] = 3;
            return RedirectToAction("VoltarAnexoCP");
        }

        [HttpGet]
        public ActionResult IncluirCPExpressa()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS" || usuario.PERFIL.PERF_SG_SIGLA == "VEN")
                {
                    Session["MensCP"] = 2;
                    return RedirectToAction("MontarTelaCP");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(cpApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");

            if (Session["MensCP"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensCP"] == 10)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0196", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 11)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0197", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 12)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0198", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 13)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0199", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCP"] == 14)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0200", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            CONTA_PAGAR item = (CONTA_PAGAR)Session["ContaPagar"];
            ContaPagarViewModel vm = Mapper.Map<CONTA_PAGAR, ContaPagarViewModel>((CONTA_PAGAR)Session["ContaPagar"]);
            FORNECEDOR forn = forApp.GetItemById(item.FORN_CD_ID.Value);
            vm.NomeFornecedor = forn.FORN_NM_NOME;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirCPExpressa(ContaPagarViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            var result = new Hashtable();
            ViewBag.Forn = new SelectList(fornApp.GetAllItens(idAss).OrderBy(x => x.FORN_NM_NOME).ToList<FORNECEDOR>(), "FORN_CD_ID", "FORN_NM_NOME");
            ViewBag.CC = new SelectList(ccApp.GetAllDespesas(idAss).OrderBy(x => x.CECU_NM_EXIBE).ToList<PLANO_CONTA>(), "CECU_CD_ID", "CECU_NM_EXIBE");
            ViewBag.Usuarios = new SelectList(usuApp.GetAllItens(idAss), "USUA_CD_ID", "USUA_NM_NOME");
            ViewBag.Contas = new SelectList(cpApp.GetAllItens(idAss), "COBA_CD_ID", "COBA_NM_NOME_EXIBE");
            ViewBag.Formas = new SelectList(fpApp.GetAllItens(idAss).Where(p => p.FOPA_IN_TIPO_FORMA == 1 || p.FOPA_IN_TIPO_FORMA == 3).OrderBy(x => x.FOPR_NM_NOME_FORMA).ToList<FORMA_PAGTO_RECTO>(), "FOPR_CD_ID", "FOPR_NM_NOME");
            ViewBag.Periodicidade = new SelectList(perApp.GetAllItens(), "PETA_CD_ID", "PETA_NM_NOME");
            List<SelectListItem> tipoDoc = new List<SelectListItem>();
            tipoDoc.Add(new SelectListItem() { Text = "Boleto", Value = "1" });
            tipoDoc.Add(new SelectListItem() { Text = "Nota", Value = "2" });
            tipoDoc.Add(new SelectListItem() { Text = "Recibo", Value = "3" });
            tipoDoc.Add(new SelectListItem() { Text = "Fatura", Value = "4" });
            tipoDoc.Add(new SelectListItem() { Text = "Crediário", Value = "5" });
            ViewBag.TipoDoc = new SelectList(tipoDoc, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CONTA_PAGAR item = Mapper.Map<ContaPagarViewModel, CONTA_PAGAR>(vm);
                    FORMA_PAGTO_RECTO forma = fpApp.GetItemById(item.FOPR_CD_ID.Value);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = cpApp.ValidateCreateExpressa(item, usuarioLogado);
                    
                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCP"] = 10;
                        return View(vm);
                    }
                    if (volta == 2)
                    {
                        Session["MensCP"] = 11;
                        return View(vm);
                    }
                    if (volta == 3)
                    {
                        Session["MensCP"] = 12;
                        return View(vm);
                    }
                    if (volta == 4)
                    {
                        Session["MensCP"] = 13;
                        return View(vm);
                    }
                    if (volta == 5)
                    {
                        Session["MensCP"] = 14;
                        return View(vm);
                    }

                    // Acerta numero
                    if (item.CAPA_NR_DOCUMENTO == null)
                    {
                        item.CAPA_NR_DOCUMENTO = item.CAPA_CD_ID.ToString();
                    }
                    
                    // Cria pastas
                    String caminho = "/Imagens/1/ContaPagar/" + item.CAPA_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    Session["IdVolta"] = item.CAPA_CD_ID;
                    if (Session["FileQueueCP"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueCP"];

                        foreach (var file in fq)
                        {
                            UploadFileQueueLancamentoCP(file);
                        }

                        Session["FileQueueCP"] = null;
                    }
                    return RedirectToAction("MontarTelaMovimentacaoAvulsa", "Estoque");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult GerarRelatorioLista()
        {
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "ContaPagarLista" + "_" + data + ".pdf";
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCP"]);
            CONTA_PAGAR filtro = (CONTA_PAGAR)Session["filtroCP"];
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Contas a Pagar - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(7);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Itens de Conta a Pagar selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 7;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Fornecedor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Saldo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_PAGAR item in lista)
            {
                if (item.FORNECEDOR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO_CONTA.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CAPA_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CAPA_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CAPA_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CAPA_VL_SALDO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.FORN_CD_ID != null)
                {
                    parametros += "Fornecedor: " + fornApp.GetItemById(filtro.FORN_CD_ID.Value);
                    ja = 1;
                }
                if (filtro.CAPA_DT_LANCAMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data da Emissão: " + filtro.CAPA_DT_LANCAMENTO.Value.ToShortDateString();
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data da Emissão: " + filtro.CAPA_DT_LANCAMENTO.Value.ToShortDateString();
                    }
                }
                if (filtro.CAPA_DT_VENCIMENTO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Inicial: " + filtro.CAPA_DT_VENCIMENTO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Inicial: " + filtro.CAPA_DT_VENCIMENTO;
                    }
                }
                if (Session["vencFinal"] != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Data de Vencimento Final: " + filtro.CAPA_DT_FINAL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Data de Vencimento Final: " + filtro.CAPA_DT_FINAL;
                    }
                }
                if (filtro.CAPA_DS_DESCRICAO != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Descrição (Histórico): " + filtro.CAPA_DS_DESCRICAO;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Descrição (Histórico): " + filtro.CAPA_DS_DESCRICAO;
                    }
                }
                if (filtro.CAPA_IN_ABERTOS != null)
                {
                    String af = filtro.CAPA_IN_ABERTOS == 1 ? "Abertos" : "Fechados";
                    if (ja == 0)
                    {
                        parametros += "Abertos/Fechados: " + af;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Abertos/Fechados: " + af;
                    }
                }
                if (ja == 0)
                {
                    parametros = "Nenhum filtro definido.";
                }
            }
            else
            {
                parametros = "Nenhum filtro definido.";
            }
            Chunk chunk = new Chunk(parametros, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("MontarTelaCP");
        }

        public ActionResult GerarRelatorioListaPagMes()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "PagamentoMesLista" + "_" + data + ".pdf";
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCPPagamentoMes"]).Where(p => p.CAPA_IN_LIQUIDADA == 1 && p.CAPA_DT_LIQUIDACAO != null && p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month && p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year).ToList();
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Pagamentos do Mês - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fornecedor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Liquidada", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_PAGAR item in lista)
            {
                if (item.FORNECEDOR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO_CONTA.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CAPA_VL_VALOR_PAGO.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CAPA_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CAPA_IN_LIQUIDADA == 1)
                {
                    cell = new PdfPCell(new Paragraph("Sim", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("Não", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerPagamentosMes");
        }

        public ActionResult GerarRelatorioListaAPagMes()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "APagarMesLista" + "_" + data + ".pdf";
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("A Pagar no Mês - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCPPagarMes"]).Where(p => p.CAPA_IN_LIQUIDADA == 0 && p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month && p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year).ToList();
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fornecedor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_PAGAR item in lista)
            {
                if (item.FORNECEDOR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO_CONTA.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CAPA_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CAPA_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CAPA_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Grid
            List<CONTA_PAGAR_PARCELA> lista1 = ((List<CONTA_PAGAR_PARCELA>)Session["ListaCPParcPagarMes"]).ToList();
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fornecedor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Parcela", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_PAGAR_PARCELA item in lista1)
            {
                if (item.CONTA_PAGAR.FORNECEDOR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CONTA_PAGAR.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_PAGAR.PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CONTA_PAGAR.PLANO_CONTA.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_PAGAR.CAPA_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CONTA_PAGAR.CAPA_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CPPA_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CPPA_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CPPA_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CPPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CPPA_NR_PARCELA.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line4 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line4);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerAPagarMes");
        }

        public ActionResult GerarRelatorioListaLancAtraso()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "LancAtrasoLista" + "_" + data + ".pdf";

            List<CONTA_PAGAR> lista = ((List<CONTA_PAGAR>)Session["ListaCPAtraso"]).Where(p => p.CAPA_NR_ATRASO > 0 && p.CAPA_DT_VENCIMENTO < DateTime.Today.Date).ToList();
            Font meuFont = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont1 = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            Font meuFont2 = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Cria documento
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            // Linha horizontal
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            // Cabeçalho
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Images/Precificacao_Favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Lançamentos em Atraso - Listagem", meuFont2))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            cell.Border = 0;
            cell.Colspan = 4;
            table.AddCell(cell);
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);
            line1 = new Paragraph("  ");
            pdfDoc.Add(line1);

            // Grid
            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Fornecedor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Plano de Contas", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Conta Bancária", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Emissão", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Valor", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Descrição", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Vencimento", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Atraso", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (CONTA_PAGAR item in lista)
            {
                if (item.FORNECEDOR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.FORNECEDOR.FORN_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.PLANO_CONTA != null)
                {
                    cell = new PdfPCell(new Paragraph(item.PLANO_CONTA.CECU_NM_NOME, meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CONTA_BANCO != null)
                {
                    cell = new PdfPCell(new Paragraph((item.CONTA_BANCO.BANCO == null ? "" : item.CONTA_BANCO.BANCO.BANC_NM_NOME + ".") + (item.CONTA_BANCO.COBA_NM_AGENCIA == null ? "" : item.CONTA_BANCO.COBA_NM_AGENCIA + ".") + (item.CONTA_BANCO.COBA_NR_CONTA == null ? "" : item.CONTA_BANCO.COBA_NR_CONTA), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_DT_LANCAMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_LANCAMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Paragraph(item.CAPA_VL_VALOR.ToString(), meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CAPA_DS_DESCRICAO, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.CAPA_DT_VENCIMENTO != null)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_DT_VENCIMENTO.Value.ToShortDateString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                if (item.CAPA_NR_ATRASO > 0)
                {
                    cell = new PdfPCell(new Paragraph(item.CAPA_NR_ATRASO.Value.ToString(), meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(new Paragraph("-", meuFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(cell);
                }
            }
            pdfDoc.Add(table);

            // Linha Horizontal
            Paragraph line3 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line3);

            // Finaliza
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + nomeRel);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("VerLancamentosAtrasoCP");
        }

    }
}
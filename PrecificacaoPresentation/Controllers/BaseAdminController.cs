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
using EntitiesServices.Work_Classes;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using System.Text;
using System.Net;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class BaseAdminController : Controller
    {
        private readonly IUsuarioAppService baseApp;
        private readonly INoticiaAppService notiApp;
        private readonly ILogAppService logApp;
        private readonly ITarefaAppService tarApp;
        private readonly INotificacaoAppService notfApp;
        private readonly IUsuarioAppService usuApp;
        private readonly IAgendaAppService ageApp;
        private readonly IConfiguracaoAppService confApp;
        private readonly IVideoAppService vidApp;
        private readonly IPessoaExternaAppService pesApp;
        private readonly IMaquinaAppService maqApp;
        private readonly IFormaPagRecAppService forApp;
        private readonly IBancoAppService banApp;
        private readonly IContaBancariaAppService cbApp;
        private readonly ICentroCustoAppService ccApp;
        private readonly IFornecedorAppService fornApp;
        private readonly IClienteAppService cliApp;
        private readonly IProdutoAppService proApp;
        private readonly IContaPagarAppService cpApp;
        private readonly IContaReceberAppService crApp;
        private readonly ITemplatePropostaAppService tpApp;
        private readonly IAssinanteAppService assiApp;
        private readonly IPlanoAppService planApp;
        private readonly ICRMAppService crmApp;
        //private readonly IMensagemAppService menApp;
        private readonly ITemplateAppService temApp;

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, IVideoAppService vidApps, IPessoaExternaAppService pesApps, IMaquinaAppService maqApps, IFormaPagRecAppService forApps, IBancoAppService banApps, IContaBancariaAppService cbApps, ICentroCustoAppService ccApps, IFornecedorAppService fornApps, IClienteAppService cliApps, IProdutoAppService proApps, IContaPagarAppService cpApps, IContaReceberAppService crApps, ITemplatePropostaAppService tpApps, IAssinanteAppService assiApps, IPlanoAppService planApps, ICRMAppService crmApps, ITemplateAppService temApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notApps;
            tarApp = tarApps;
            notfApp = notfApps;
            usuApp = usuApps;
            ageApp = ageApps;
            confApp = confApps;
            vidApp = vidApps;
            pesApp = pesApps;   
            maqApp = maqApps;   
            forApp = forApps;
            banApp = banApps;
            cbApp = cbApps;
            ccApp = ccApps;
            fornApp = fornApps;
            cliApp = cliApps;
            proApp = proApps;
            cpApp = cpApps;
            crApp = crApps;
            tpApp = tpApps;
            assiApp = assiApps; 
            planApps = planApps;
            crmApp = crmApps;
            temApp = temApps;
        }

        public ActionResult CarregarAdmin()
        {
            Int32? idAss = (Int32)Session["IdAssinante"];
            ViewBag.Usuarios = baseApp.GetAllUsuarios(idAss.Value).Count;
            ViewBag.Logs = logApp.GetAllItens(idAss.Value).Count;
            ViewBag.UsuariosLista = baseApp.GetAllUsuarios(idAss.Value);
            ViewBag.LogsLista = logApp.GetAllItens(idAss.Value);
            return View();

        }

        public ActionResult CarregarLandingPage()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return View();
        }

        public JsonResult GetRefreshTime()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            return Json(confApp.GetAllItems(idAss).FirstOrDefault().CONF_NR_REFRESH_DASH);
        }

        public JsonResult GetConfigNotificacoes()
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            bool hasNotf;
            var hash = new Hashtable();
            USUARIO usu = (USUARIO)Session["Usuario"];
            CONFIGURACAO conf = confApp.GetAllItems(idAss).FirstOrDefault();

            if (baseApp.GetAllItensUser(usu.USUA_CD_ID, idAss).Count > 0)
            {
                hasNotf = true;
            }
            else
            {
                hasNotf = false;
            }

            hash.Add("CONF_NM_ARQUIVO_ALARME", conf.CONF_NM_ARQUIVO_ALARME);
            hash.Add("NOTIFICACAO", hasNotf);
            return Json(hash);
        }

        public ActionResult CarregarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["Login"] == 1)
            {
                Session["Perfis"] = baseApp.GetAllPerfis();
                Session["Usuarios"] = usuApp.GetAllUsuarios(idAss);
            }

            Session["MensTarefa"] = 0;
            Session["MensNoticia"] = 0;
            Session["MensNotificacao"] = 0;
            Session["MensUsuario"] = 0;
            Session["MensLog"] = 0;
            Session["MensUsuarioAdm"] = 0;
            Session["MensAgenda"] = 0;
            Session["MensTemplate"] = 0;
            Session["MensConfiguracao"] = 0;
            Session["MensCargo"] = 0;
            Session["MensBanco"] = 0;
            Session["MensSMSError"] = 0;
            Session["MensPermissao"] = 0;
            Session["MensBanco"] = 0;
            Session["MensConta"] = 0;
            Session["MensVideo"] = 0;
            Session["MensEstoque"] = 0;
            Session["VoltaNotificacao"] = 3;
            Session["VoltaNoticia"] = 1;
            Session["ErroSoma"] = 0;
            Session["IdCP"] = 0;
            Session["MensVencimentoCR"] = 0;
            Session["VoltaCompra"] = 0;

            USUARIO usu = new USUARIO();
            UsuarioViewModel vm = new UsuarioViewModel();
            List<NOTIFICACAO> noti = new List<NOTIFICACAO>();

            ObjectCache cache = MemoryCache.Default;
            USUARIO usuContent = cache["usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID] as USUARIO;
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            //if (usuContent == null)
            //{
            //    usu = usuApp.GetItemById(((USUARIO)Session["UserCredentials"]).USUA_CD_ID);
            //    vm = Mapper.Map<USUARIO, UsuarioViewModel>(usu);
            //    noti = notfApp.GetAllItens(idAss);
            //    DateTime expiration = DateTime.Now.AddDays(15);
            //    cache.Set("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, usu, expiration);
            //    cache.Set("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, vm, expiration);
            //    cache.Set("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID, noti, expiration);
            //}

            //usu = cache.Get("usuario" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as USUARIO;
            //vm = cache.Get("vm" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as UsuarioViewModel;
            //noti = cache.Get("noti" + ((USUARIO)Session["UserCredentials"]).USUA_CD_ID) as List<NOTIFICACAO>;
            //ViewBag.Perfil = usu.PERFIL.PERF_SG_SIGLA;

            noti = notfApp.GetAllItensUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID);
            Session["Notificacoes"] = noti;
            Session["ListaNovas"] = noti.Where(p => p.NOTC_IN_VISTA == 0).ToList().Take(5).OrderByDescending(p => p.NOTC_DT_EMISSAO.Value).ToList();
            Session["NovasNotificacoes"] = noti.Where(p => p.NOTC_IN_VISTA == 0).Count();
            Session["Nome"] = usuario.USUA_NM_NOME;

            Session["Noticias"] = notiApp.GetAllItensValidos(idAss);
            Session["NoticiasNumero"] = ((List<NOTICIA>)Session["Noticias"]).Count;

            Session["Videos"] = vidApp.GetAllItensValidos(idAss);
            Session["VideosNumero"] = ((List<VIDEO>)Session["Videos"]).Count;

            Session["ListaPendentes"] = tarApp.GetTarefaStatus(usuario.USUA_CD_ID, 1);
            Session["TarefasPendentes"] = ((List<TAREFA>)Session["ListaPendentes"]).Count;
            Session["TarefasLista"] = tarApp.GetByUser(usuario.USUA_CD_ID);
            Session["Tarefas"] = ((List<TAREFA>)Session["TarefasLista"]).Count;

            Session["Agendas"] = ageApp.GetByUser(usuario.USUA_CD_ID, usuario.ASSI_CD_ID);
            Session["NumAgendas"] = ((List<AGENDA>)Session["Agendas"]).Count;
            Session["AgendasHoje"] = ((List<AGENDA>)Session["Agendas"]).Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).ToList();
            Session["NumAgendasHoje"] = ((List<AGENDA>)Session["AgendasHoje"]).Count;
            Session["Logs"] = usu.LOG.Count;

            String frase = String.Empty;
            String nome = usuario.USUA_NM_NOME.Substring(0, usuario.USUA_NM_NOME.IndexOf(" "));
            Session["NomeGreeting"] = nome;
            if (DateTime.Now.Hour <= 12)
            {
                frase = "Bom dia, " + nome;
            }
            else if (DateTime.Now.Hour > 12 & DateTime.Now.Hour <= 18)
            {
                frase = "Boa tarde, " + nome;
            }
            else
            {
                frase = "Boa noite, " + nome;
            }
            Session["Greeting"] = frase;
            Session["Foto"] = usuario.USUA_AQ_FOTO;

            // Mensagens
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", ERP_Condominio_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }
            Session["MensPermissao"] = 0;
            return View(vm);

        }

        public ActionResult CarregarDesenvolvimento()
        {
            return View();
        }

        public ActionResult VoltarDashboard()
        {
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarDashboardAdministracao()
        {
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        public ActionResult MontarFaleConosco()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAdministracao()
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
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Carrega valores dos cadastros
            List<PESSOA_EXTERNA> pessoa = pesApp.GetAllItens(idAss);
            Int32 pessoas = pessoa.Count;
            List<NOTICIA> noticia = notiApp.GetAllItens(idAss);
            Int32 noticias = noticia.Count;
            List<VIDEO> video = vidApp.GetAllItens(idAss);
            Int32 videos = video.Count;
            List<MAQUINA> maquina = maqApp.GetAllItens(idAss);
            Int32 maquinas = maquina.Count;
            List<FORMA_PAGTO_RECTO> forma = forApp.GetAllItens(idAss);
            Int32 formas = forma.Count;

            Session["Pessoas"] = pessoas;
            Session["Noticias1"] = noticias;
            Session["Video"] = videos;
            Session["Maquina"] = maquinas;
            Session["Formas"] = formas;

            ViewBag.Pessoas = pessoas;
            ViewBag.Noticias = noticias;
            ViewBag.Videos = videos;
            ViewBag.Maquinas = maquinas;
            ViewBag.Formas = formas;

            // Recupera listas usuarios
            List<USUARIO> listaTotal = baseApp.GetAllItens(idAss);
            List<USUARIO> bloqueados = listaTotal.Where(p => p.USUA_IN_BLOQUEADO == 1).ToList();

            Int32 numUsuarios = listaTotal.Count;
            Int32 numBloqueados = bloqueados.Count;
            Int32 numAcessos = listaTotal.Sum(p => p.USUA_NR_ACESSOS);
            Int32 numFalhas = listaTotal.Sum(p => p.USUA_NR_FALHAS);

            ViewBag.NumUsuarios = numUsuarios;
            ViewBag.NumBloqueados = numBloqueados;
            ViewBag.NumAcessos = numAcessos;
            ViewBag.NumFalhas = numFalhas;

            Session["TotalUsuarios"] = listaTotal.Count;
            Session["Bloqueados"] = numBloqueados;

            // Recupera listas log
            List<LOG> listaLog = logApp.GetAllItens(idAss);
            Int32 log = listaLog.Count;
            Int32 logDia = listaLog.Where(p => p.LOG_DT_LOG.Date == DateTime.Today.Date).ToList().Count;
            Int32 logMes = listaLog.Where(p => p.LOG_DT_LOG.Month == DateTime.Today.Month & p.LOG_DT_LOG.Year == DateTime.Today.Year).ToList().Count;
            List<LOG> listaDia = listaLog.Where(p => p.LOG_DT_LOG.Date == DateTime.Today.Date).ToList();
            List<LOG> listaMes = listaLog.Where(p => p.LOG_DT_LOG.Month == DateTime.Today.Month & p.LOG_DT_LOG.Year == DateTime.Today.Year).ToList().ToList();
            
            ViewBag.Log = log;
            ViewBag.LogDia = logDia;
            ViewBag.LogMes = logMes;

            Session["TotalLog"] = log;
            Session["LogDia"] = logDia;
            Session["LogMes"] = logMes;

            // Resumo Log Diario
            List<DateTime> datasCR = listaMes.Where(m => m.LOG_DT_LOG != null).OrderBy(m => m.LOG_DT_LOG).Select(p => p.LOG_DT_LOG.Date).Distinct().ToList();
            List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                Int32 conta = listaLog.Where(p => p.LOG_DT_LOG.Date == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                listaLogDia.Add(mod1);
            }
            listaLogDia = listaLogDia.OrderBy(p => p.DataEmissao).ToList();
            ViewBag.ListaLogDia = listaLogDia;
            ViewBag.ContaLogDia = listaLogDia.Count;
            Session["ListaDatasLog"] = datasCR;
            Session["ListaLogResumo"] = listaLogDia;

            // Resumo Log Situacao  
            List<String> opLog = listaLog.Select(p => p.LOG_NM_OPERACAO).Distinct().ToList();
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            foreach (String item in opLog)
            {
                Int32 conta1 = listaLog.Where(p => p.LOG_NM_OPERACAO == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.Nome = item;
                mod1.Valor = conta1;
                lista2.Add(mod1);
            }
            ViewBag.ListaLogOp = lista2;
            ViewBag.ContaLogOp = lista2.Count;
            Session["ListaOpLog"] = opLog;
            Session["ListaLogOp"] = lista2;
            Session["VoltaDash"] = 3;
            Session["VoltaUnidade"] = 1;
            return View(vm);
        }

        public JsonResult GetDadosGraficoDia()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogResumo"];
            List<String> dias = new List<String>();
            List<Int32> valor = new List<Int32>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRSituacao()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoLogOper()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaLogOp"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in listaCP1)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public ActionResult MontarCentralMensagens()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensCRM"] = 2;
                    return RedirectToAction("VoltarAcompanhamentoCRM");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Cria Base de mensagens
            CONFIGURACAO conf = confApp.GetItemById(idAss);
            List<MensagemWidgetViewModel> listaMensagens = new List<MensagemWidgetViewModel>();
            if (Session["ListaMensagem"] == null)
            {
                // Carrega notificações
                List<NOTIFICACAO> notificacoes = (List<NOTIFICACAO>)Session["Notificacoes"];
                List<NOTIFICACAO> naoLidas = notificacoes.Where(p => p.NOTC_IN_VISTA == 0).OrderByDescending(p => p.NOTC_DT_EMISSAO).ToList();
                foreach (NOTIFICACAO item in naoLidas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.NOTC_DT_EMISSAO;
                    mens.Descrição = item.NOTC_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.NOTC_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 1;
                    mens.Categoria = item.CATEGORIA_NOTIFICACAO.CANO_NM_NOME;
                    mens.NomeCliente = item.NOTC_IN_VISTA == 1 ? "Lida" : "Não Lida";
                    listaMensagens.Add(mens);
                }

                // Carrega Agenda
                List<AGENDA> agendas = (List<AGENDA>)Session["Agendas"];
                List<AGENDA> hoje = agendas.Where(p => p.AGEN_DT_DATA == DateTime.Today.Date).OrderByDescending(p => p.AGEN_DT_DATA).ToList();
                foreach (AGENDA item in hoje)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.AGEN_DT_DATA;
                    mens.Descrição = item.AGEN_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.AGEN_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 2;
                    mens.Categoria = item.CATEGORIA_AGENDA.CAAG_NM_NOME;
                    mens.NomeCliente = item.AGEN_IN_STATUS == 1 ? "Ativa" : (item.AGEN_IN_STATUS == 2 ? "Suspensa" : "Encerrada");
                    listaMensagens.Add(mens);
                }

                // Carrega Tarefas
                List<TAREFA> tarefas = (List<TAREFA>)Session["ListaPendentes"];
                tarefas = tarefas.OrderByDescending(p => p.TARE_DT_ESTIMADA).ToList();
                foreach (TAREFA item in tarefas)
                {
                    MensagemWidgetViewModel mens = new MensagemWidgetViewModel();
                    mens.DataMensagem = item.TARE_DT_ESTIMADA;
                    mens.Descrição = item.TARE_NM_TITULO;
                    mens.FlagUrgencia = 1;
                    mens.IdMensagem = item.TARE_CD_ID;
                    mens.NomeUsuario = usuario.USUA_NM_NOME;
                    mens.TipoMensagem = 3;
                    mens.Categoria = item.TIPO_TAREFA.TITR_NM_NOME;
                    if (item.TARE_IN_STATUS == 1)
                    {
                        mens.NomeCliente = "Ativa";

                    }
                    else if (item.TARE_IN_STATUS == 2)
                    {
                        mens.NomeCliente = "Em Andamento";

                    }
                    else if (item.TARE_IN_STATUS == 3)
                    {
                        mens.NomeCliente = "Suspensa";

                    }
                    else if (item.TARE_IN_STATUS == 4)
                    {
                        mens.NomeCliente = "Cancelada";

                    }
                    else if (item.TARE_IN_STATUS == 5)
                    {
                        mens.NomeCliente = "Encerrada";

                    }
                    listaMensagens.Add(mens);
                }
                Session["ListaMensagem"] = listaMensagens;
                Session["ListaMensagemGeral"] = listaMensagens;
            }
            else
            {
                listaMensagens = (List<MensagemWidgetViewModel>)Session["ListaMensagem"];
            }

            // Prepara listas dos filtros
            List<SelectListItem> tipos = new List<SelectListItem>();
            tipos.Add(new SelectListItem() { Text = "Notificações", Value = "1" });
            tipos.Add(new SelectListItem() { Text = "Agenda", Value = "2" });
            tipos.Add(new SelectListItem() { Text = "Tarefas", Value = "3" });
            ViewBag.Tipos = new SelectList(tipos, "Value", "Text");
            List<SelectListItem> urg = new List<SelectListItem>();
            urg.Add(new SelectListItem() { Text = "Sim", Value = "1" });
            urg.Add(new SelectListItem() { Text = "Não", Value = "2" });
            ViewBag.Urgencia = new SelectList(urg, "Value", "Text");

            // Exibe
            ViewBag.ListaMensagem = listaMensagens;
            MensagemWidgetViewModel mod = new MensagemWidgetViewModel();
            return View(mod);

        }

        public ActionResult RetirarFiltroCentralMensagens()
        {

            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaMensagem"] = null;
            return RedirectToAction("MontarCentralMensagens");
        }

        [HttpPost]
        public ActionResult FiltrarCentralMensagens(MensagemWidgetViewModel item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            try
            {
                // Executa a operação
                List<MensagemWidgetViewModel> listaObj = (List<MensagemWidgetViewModel>)Session["ListaMensagemGeral"];
                if (item.TipoMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.TipoMensagem == item.TipoMensagem).ToList();
                }
                if (item.Descrição != null)
                {
                    listaObj = listaObj.Where(p => p.Descrição.Contains(item.Descrição)).ToList();
                }
                if (item.DataMensagem != null)
                {
                    listaObj = listaObj.Where(p => p.DataMensagem == item.DataMensagem).ToList();
                }
                if (item.FlagUrgencia != null)
                {
                    listaObj = listaObj.Where(p => p.FlagUrgencia == item.FlagUrgencia).ToList();
                }
                Session["ListaMensagem"] = listaObj;

                // Sucesso
                return RedirectToAction("MontarCentralMensagens");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarCentralMensagens");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTabelasAuxiliares()
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
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);
            return View(vm);
        }

        public ActionResult MontarTelaDashboardCadastros()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            Int32 idAss = (Int32)Session["IdAssinante"];
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Carrega valores dos cadastros
            List<BANCO> banco = banApp.GetAllItens(idAss);
            Int32 bancos = banco.Count;
            List<PLANO_CONTA> plano = ccApp.GetAllItens(idAss);
            Int32 planos = plano.Count;
            List<CONTA_BANCO> conta = cbApp.GetAllItens(idAss);
            Int32 contas = conta.Count;
            List<FORNECEDOR> forn = fornApp.GetAllItens(idAss);
            Int32 forns = forn.Count;
            List<CLIENTE> cliente = cliApp.GetAllItens(idAss);
            Int32 clientes = cliente.Count;
            List<PRODUTO> produto = proApp.GetAllItens(idAss);
            Int32 produtos = produto.Count;

            Session["Bancos"] = bancos;
            Session["Planos"] = planos;
            Session["Contas"] = contas;
            Session["Fornecedores"] = forns;
            Session["Clientes"] = clientes;
            Session["Produto"] = produtos;

            ViewBag.Bancos = bancos;
            ViewBag.Planos = planos;
            ViewBag.Contas = contas;
            ViewBag.Clientes = clientes;
            ViewBag.Fornecedores = forns;
            ViewBag.Produtos = produtos;

            Session["PlanoCredito"] = plano.Where(p => p.CECU_IN_TIPO == 1).ToList().Count;
            Session["PlanoDebito"] = plano.Where(p => p.CECU_IN_TIPO == 2).ToList().Count;
            Session["ContaCorrente"] = conta.Where(p => p.TICO_CD_ID == 1).ToList().Count;
            Session["ContaPoupanca"] = conta.Where(p => p.TICO_CD_ID == 2).ToList().Count;
            Session["ContaInvestimento"] = conta.Where(p => p.TICO_CD_ID == 3).ToList().Count;

            // Recupera fornecedores por UF
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            List<UF> ufs = fornApp.GetAllUF().ToList();
            foreach (UF item in ufs)
            {
                Int32 num = forn.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.UF_NM_NOME;
                    mod.Valor = num;
                    lista2.Add(mod);
                }
            }
            ViewBag.ListaFornUF = lista2;
            Session["ListaFornUF"] = lista2;

            // Recupera fornecedores por Cidade
            List<ModeloViewModel> lista3 = new List<ModeloViewModel>();
            List<String> cids = forn.Select(p => p.FORN_NM_CIDADE.ToUpper()).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = forn.Where(p => p.FORN_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista3.Add(mod);
            }
            ViewBag.ListaFornCidade = lista3;
            Session["ListaFornCidade"] = lista3;

            // Recupera fornecedores por Categoria
            List<ModeloViewModel> lista4 = new List<ModeloViewModel>();
            List<CATEGORIA_FORNECEDOR> cats = fornApp.GetAllTipos(idAss).ToList();
            foreach (CATEGORIA_FORNECEDOR item in cats)
            {
                Int32 num = forn.Where(p => p.CAFO_CD_ID == item.CAFO_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.CAFO_NM_NOME;
                    mod.Valor = num;
                    lista4.Add(mod);
                }
            }
            ViewBag.ListaFornCats = lista4;
            Session["ListaFornCats"] = lista4;

            // Recupera clientes por UF
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            ufs = fornApp.GetAllUF().ToList();
            foreach (UF item in ufs)
            {
                Int32 num = cliente.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.UF_NM_NOME;
                    mod.Valor = num;
                    lista5.Add(mod);
                }
            }
            ViewBag.ListaClienteUF = lista5;
            Session["ListaClienteUF"] = lista5;

            // Recupera clientes por Cidade
            List<ModeloViewModel> lista6 = new List<ModeloViewModel>();
            cids = cliente.Select(p => p.CLIE_NM_CIDADE).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = cliente.Where(p => p.CLIE_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista6.Add(mod);
            }
            ViewBag.ListaClienteCidade = lista6;
            Session["ListaClienteCidade"] = lista6;

            // Recupera clientes por Categoria
            List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
            List<CATEGORIA_CLIENTE> catc = cliApp.GetAllTipos(idAss).ToList();
            foreach (CATEGORIA_CLIENTE item in catc)
            {
                Int32 num = cliente.Where(p => p.CACL_CD_ID == item.CACL_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.CACL_NM_NOME;
                    mod.Valor = num;
                    lista7.Add(mod);
                }
            }
            ViewBag.ListaClienteCats = lista7;
            Session["ListaClienteCats"] = lista7;

            return View(vm);
        }

        public JsonResult GetDadosGraficoPlanoTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["PlanoCredito"];
            Int32 q2 = (Int32)Session["PlanoDebito"];

            desc.Add("Contas de Receitas");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Contas de Despesas");
            quant.Add(q2);
            cor.Add("#FFAE00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoContaTipo()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["ContaCorrente"];
            Int32 q2 = (Int32)Session["ContaPoupanca"];
            Int32 q3 = (Int32)Session["ContaInvestimento"];


            desc.Add("Conta Corrente");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Conta Poupança");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Conta Investimento");
            quant.Add(q2);
            cor.Add("#FFAB69");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosFornecedorUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaFornUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosFornecedorCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaFornCidade"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosFornecedorUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaFornUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosFornecedorCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaFornCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosFornecedorCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaFornCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosClienteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosClienteCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosClienteCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#359E18");
            cor.Add("#FFAE00");
            cor.Add("#FF7F00");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#359E18");
                }
                else if (i == 2)
                {
                    cor.Add("#FFAE00");
                }
                else if (i == 3)
                {
                    cor.Add("#FF7F00");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosClienteUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosClienteCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaClienteCidade"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardFinanceiro()
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
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
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);

            // Recupera listas CP
            List<CONTA_PAGAR> pag = cpApp.GetAllItens(idAss);

            List<CONTA_PAGAR> lp1 = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_IN_PARCELADA == 0).ToList();
            if (lp1.Count > 0)
            {
                Decimal pago = lp1.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year & p.CAPA_IN_PARCELADA == 0).Sum(p => p.CAPA_VL_VALOR_PAGO).Value;
                pago += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).Sum(p => p.CPPA_VL_VALOR);
                ViewBag.Pago = pago;
            }
            else
            {
                ViewBag.Pago = 0;
            }

            Decimal sumPagar = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumPagar += (Decimal)pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).Sum(p => p.CPPA_VL_VALOR);
            ViewBag.APagar = sumPagar;

            Decimal sumAtrasoCP = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Sum(p => p.CAPA_VL_VALOR).Value;
            sumAtrasoCP += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).Sum(p => p.CPPA_VL_VALOR).Value;
            ViewBag.Atraso = sumAtrasoCP;

            Int32 pagos = 0;
            List<CONTA_PAGAR> lp = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_IN_PARCELADA == 0).ToList();
            if (lp.Count > 0)
            {
                pagos = lp.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CAPA_DT_LIQUIDACAO != null & p.CAPA_DT_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year & p.CAPA_IN_PARCELADA == 0).ToList().Count;
                pagos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_QUITACAO.Value.Month == DateTime.Now.Month & x.CPPA_DT_QUITACAO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 1).ToList().Count;
            }
            else
            {
                pagos = 0;
            }

            Int32 atrasos = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_NR_ATRASO > 0 & p.CAPA_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).Count();
            atrasos += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_NR_ATRASO > 0 & x.CPPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).ToList().Count;

            Int32 pendentes = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CAPA_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CAPA_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_PAGAR_PARCELA == null || p.CONTA_PAGAR_PARCELA.Count == 0)).ToList().Count;
            pendentes += pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 0 & p.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_VL_VALOR != null & x.CPPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CPPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CPPA_IN_QUITADA == 0).ToList().Count;

            Session["TotalCP"] = pag.Count;
            Session["APagarMes"] = pendentes;
            Session["Atraso"] = atrasos;
            Session["PagoMes"] = pagos;

            // Resumo Mes Pagamentos
            List<DateTime> datasCP = pag.Where(m => m.CAPA_IN_ATIVO == 1 & m.CAPA_IN_LIQUIDADA == 1 & (m.CONTA_PAGAR_PARCELA == null || m.CONTA_PAGAR_PARCELA.Count == 0)).Select(p => p.CAPA_DT_LIQUIDACAO.Value.Date).Distinct().ToList();
            List<DateTime> datasParc = pag.Where(m => m.CAPA_IN_ATIVO == 1 & m.CONTA_PAGAR_PARCELA != null).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_IN_QUITADA == 1).Select(p => p.CPPA_DT_QUITACAO.Value.Date).Distinct().ToList();
            List<DateTime> datas = datasCP.Concat(datasParc).Distinct().ToList();

            List<ModeloViewModel> lista = new List<ModeloViewModel>();
            List<CONTA_PAGAR> lista5 = pag.Where(p => p.CAPA_IN_ATIVO == 1 & p.CAPA_IN_LIQUIDADA == 1).ToList();
            foreach (DateTime item in datasCP)
            {
                List<CONTA_PAGAR> lista10 = lista5.Where(p => p.CAPA_DT_LIQUIDACAO.Value.Date == item.Date).ToList();
                Decimal conta = lista10.Sum(p => p.CAPA_VL_VALOR_PAGO).Value;

                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta;
                lista.Add(mod1);
            }
            ViewBag.ListaPagDia = lista;
            ViewBag.ContaPagDia = lista.Count;
            Session["ListaDatas"] = datasCP;
            Session["ListaPagResumo"] = lista;

            List<ModeloViewModel> listaX = new List<ModeloViewModel>();
            List<CONTA_PAGAR_PARCELA> lista6 = pag.Where(p => p.CAPA_IN_ATIVO == 1).SelectMany(p => p.CONTA_PAGAR_PARCELA).Where(x => x.CPPA_IN_QUITADA == 1).ToList();
            foreach (DateTime item in datasParc)
            {
                List<CONTA_PAGAR_PARCELA> lista10 = lista6.Where(p => p.CPPA_DT_QUITACAO.Value.Date == item.Date).ToList();
                Decimal conta = lista10.Sum(p => p.CPPA_VL_VALOR_PAGO).Value;

                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta;
                listaX.Add(mod1);
            }
            ViewBag.ListaPagDiaParc = listaX;
            ViewBag.ContaPagDiaParg = listaX.Count;
            Session["ListaDatasParc"] = datasParc;
            Session["ListaPagResumoParc"] = listaX;

            // Resumo CP Situacao  
            List<ModeloViewModel> lista1 = new List<ModeloViewModel>();
            ModeloViewModel mod = new ModeloViewModel();
            mod.Data = "Liquidados";
            mod.Valor = pagos;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Em Atraso";
            mod.Valor = atrasos;
            lista1.Add(mod);
            mod = new ModeloViewModel();
            mod.Data = "Pendentes";
            mod.Valor = pendentes;
            lista1.Add(mod);
            ViewBag.ListaCPSituacao = lista1;
            Session["ListaCPSituacao"] = lista1;
            Session["VoltaDash"] = 3;

            // Recupera listas CR
            List<CONTA_RECEBER> rec = crApp.GetAllItens(idAss);

            Decimal aReceberDia = (Decimal)rec.Where(x => x.CARE_IN_ATIVO == 1 & x.CARE_IN_LIQUIDADA == 0 & x.CARE_DT_VENCIMENTO.Value.Date == DateTime.Now.Date & (x.CONTA_RECEBER_PARCELA == null || x.CONTA_RECEBER_PARCELA.Count == 0)).Sum(x => x.CARE_VL_SALDO);
            aReceberDia += (Decimal)rec.Where(x => x.CARE_IN_ATIVO == 1 & x.CARE_IN_LIQUIDADA == 0 & x.CARE_DT_VENCIMENTO.Value.Day == DateTime.Now.Day & x.CONTA_RECEBER_PARCELA != null).SelectMany(x => x.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Date == DateTime.Now.Date & x.CRPA_IN_QUITADA == 0).Sum(x => x.CRPA_VL_VALOR);
            ViewBag.CRS = aReceberDia;

            List<CONTA_RECEBER> lr1 = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO != null & p.CARE_IN_PARCELADA == 0).ToList();
            if (lr1.Count > 0)
            {
                Decimal recebido = lr1.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_DATA_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year).Sum(p => p.CARE_VL_VALOR_RECEBIDO).Value;
                recebido += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 1).Sum(p => p.CRPA_VL_VALOR);
                ViewBag.Recebido = recebido;
            }
            else
            {
                ViewBag.Recebido = 0;
            }

            List<CONTA_RECEBER> lr2 = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO != null & p.CARE_IN_PARCELADA == 0).ToList();
            if (lr2.Count > 0)
            {
                Decimal recebidoDia = lr1.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_DATA_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year & p.CARE_DT_DATA_LIQUIDACAO.Value.Day == DateTime.Today.Date.Day).Sum(p => p.CARE_VL_VALOR_RECEBIDO).Value;
                recebidoDia += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CARE_DT_VENCIMENTO.Value.Day == DateTime.Today.Date.Day & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 1).Sum(p => p.CRPA_VL_VALOR);
                ViewBag.RecebidoDia = recebidoDia;
            }
            else
            {
                ViewBag.RecebidoDia = 0;
            }

            Decimal sumReceber = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumReceber += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 0).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AReceber = sumReceber;

            Decimal sumAtraso = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_NR_ATRASO > 0 & p.CARE_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Sum(p => p.CARE_VL_VALOR);
            sumAtraso += (Decimal)rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_NR_ATRASO > 0 & x.CRPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).Sum(p => p.CRPA_VL_VALOR);
            ViewBag.AtrasoCR = sumAtraso;

            Int32 recebidas = 0;
            List<CONTA_RECEBER> lr = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO != null).ToList();
            if (lr.Count > 0)
            {
                recebidas = lr.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_DATA_LIQUIDACAO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_DATA_LIQUIDACAO.Value.Year == DateTime.Today.Date.Year).ToList().Count;
                recebidas += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 1 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 1).ToList().Count;
            }
            else
            {
                recebidas = 0;
            }

            Int32 atrasosCR = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_NR_ATRASO > 0 & p.CARE_DT_VENCIMENTO < DateTime.Today.Date & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).Count();
            atrasosCR += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_NR_ATRASO > 0 & x.CRPA_DT_VENCIMENTO.Value.Date < DateTime.Now.Date).ToList().Count;

            Int32 pendentesCR = rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & (p.CONTA_RECEBER_PARCELA == null || p.CONTA_RECEBER_PARCELA.Count == 0)).ToList().Count;
            pendentesCR += rec.Where(p => p.CARE_IN_ATIVO == 1 & p.CARE_IN_LIQUIDADA == 0 & p.CARE_DT_VENCIMENTO.Value.Month == DateTime.Today.Date.Month & p.CARE_DT_VENCIMENTO.Value.Year == DateTime.Today.Date.Year & p.CONTA_RECEBER_PARCELA != null).SelectMany(p => p.CONTA_RECEBER_PARCELA).Where(x => x.CRPA_VL_VALOR != null & x.CRPA_DT_VENCIMENTO.Value.Month == DateTime.Now.Month & x.CRPA_DT_VENCIMENTO.Value.Year == DateTime.Now.Year & x.CRPA_IN_QUITADA == 0).ToList().Count;

            Session["TotalCR"] = rec.Count;
            Session["Recebido"] = recebidas;
            Session["AReceber"] = pendentesCR;
            Session["AtrasoCR"] = atrasosCR;

            // Resumo Mes Recebimentos
            List<DateTime> datasCR = rec.Where(m => m.CARE_DT_DATA_LIQUIDACAO != null).Select(p => p.CARE_DT_DATA_LIQUIDACAO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaCR = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                CONTA_RECEBER cr = rec.Where(p => p.CARE_DT_DATA_LIQUIDACAO == item).FirstOrDefault();
                Decimal? conta = rec.Where(p => p.CARE_DT_DATA_LIQUIDACAO == item).Sum(p => p.CARE_VL_VALOR_RECEBIDO);
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.ValorDec = conta.Value;
                listaCR.Add(mod1);
            }
            ViewBag.ListaRecDia = listaCR;
            ViewBag.ContaRecDia = listaCR.Count;
            Session["ListaDatasCR"] = datasCR;
            Session["ListaRecResumo"] = listaCR;

            // Resumo CR Situacao  
            List<ModeloViewModel> lista2 = new List<ModeloViewModel>();
            ModeloViewModel mod2 = new ModeloViewModel();
            mod2.Data = "Recebidas";
            mod2.Valor = recebidas;
            lista2.Add(mod2);
            mod2 = new ModeloViewModel();
            mod2.Data = "Em Atraso";
            mod2.Valor = atrasosCR;
            lista2.Add(mod2);
            mod2 = new ModeloViewModel();
            mod2.Data = "Pendentes";
            mod2.Valor = pendentesCR;
            lista2.Add(mod2);
            ViewBag.ListaCRSituacao = lista2;
            Session["ListaCRSituacao"] = lista2;
            Session["VoltaDash"] = 3;
            Session["ListaForma"] = null;
            return View(vm);
        }

        public JsonResult GetDadosGraficoCPSituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["APagarMes"];
            Int32 q2 = (Int32)Session["Atraso"];
            Int32 q3 = (Int32)Session["PagoMes"];

            desc.Add("A Pagar (Mês)");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Em Atraso");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("Pago (Mês)");
            quant.Add(q3);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCP()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaPagResumo"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.ValorDec);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCRESituacao()
        {
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();

            Int32 q1 = (Int32)Session["Recebido"];
            Int32 q2 = (Int32)Session["AtrasoCR"];
            Int32 q3 = (Int32)Session["AReceber"];

            desc.Add("Recebido (Mês)");
            quant.Add(q1);
            cor.Add("#359E18");
            desc.Add("Em Atraso");
            quant.Add(q2);
            cor.Add("#FFAE00");
            desc.Add("A Receber (Mês)");
            quant.Add(q3);
            cor.Add("#FF7F00");

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosGraficoCR()
        {
            List<ModeloViewModel> listaCP1 = (List<ModeloViewModel>)Session["ListaRecResumo"];
            List<String> dias = new List<String>();
            List<Decimal> valor = new List<Decimal>();
            dias.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in listaCP1)
            {
                dias.Add(item.DataEmissao.ToShortDateString());
                valor.Add(item.ValorDec);
            }

            Hashtable result = new Hashtable();
            result.Add("dias", dias);
            result.Add("valores", valor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaDashboardAssinantes()
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
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Recupera listas 
            List<ASSINANTE> listaTotal = assiApp.GetAllItens();
            List<ASSINANTE> bloqueados = listaTotal.Where(p => p.ASSI_IN_BLOQUEADO == 1).ToList();
            List<PLANO> planos = planApp.GetAllItens();

            Int32 numAssinantes = listaTotal.Count;
            Int32 numBloqueados = bloqueados.Count;
            ViewBag.NumAssinantes = numAssinantes;
            ViewBag.NumBloqueados = numBloqueados;
            ViewBag.Planos = planos.Count;

            List<ASSINANTE> listaDia = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Date == DateTime.Today.Date).ToList();
            List<ASSINANTE> listaMes = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Month == DateTime.Today.Month & p.ASSI_DT_INICIO.Value.Year == DateTime.Today.Year).ToList();

            // Recupera vencimentos
            List<ASSINANTE_PLANO> planosAss = assiApp.GetAllAssPlanos();
            List<ASSINANTE_PLANO> planosVencidos = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
            List<ASSINANTE_PLANO> planosVencer30 = planosAss.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
            Int32 vencidos = planosVencidos.Count;
            Int32 vencer30 = planosVencer30.Count;

            Session["PlanosVencidos"] = planosVencidos;
            Session["PlanosVencer30"] = planosVencer30;
            ViewBag.Vencidos = vencidos;
            ViewBag.Vencer30 = vencer30;
            ViewBag.Planos = planos.Count;

            // Recupera assinantes por UF
            List<ModeloViewModel> lista5 = new List<ModeloViewModel>();
            List<UF> ufs = cliApp.GetAllUF().ToList();
            foreach (UF item in ufs)
            {
                Int32 num = listaTotal.Where(p => p.UF_CD_ID == item.UF_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.UF_NM_NOME;
                    mod.Valor = num;
                    lista5.Add(mod);
                }
            }
            ViewBag.ListaAssUF = lista5;
            Session["ListaAssUF"] = lista5;

            // Recupera assinantes por Cidade
            List<ModeloViewModel> lista6 = new List<ModeloViewModel>();
            List<String> cids = listaTotal.Select(p => p.ASSI_NM_CIDADE).Distinct().ToList();
            foreach (String item in cids)
            {
                Int32 num = listaTotal.Where(p => p.ASSI_NM_CIDADE == item).ToList().Count;
                ModeloViewModel mod = new ModeloViewModel();
                mod.Nome = item;
                mod.Valor = num;
                lista6.Add(mod);
            }
            ViewBag.ListaAssCidade = lista6;
            Session["ListaAssCidade"] = lista6;

            // Recupera assinantes por tipo
            List<ModeloViewModel> lista7 = new List<ModeloViewModel>();
            List<TIPO_PESSOA> catc = cliApp.GetAllTiposPessoa().ToList();
            foreach (TIPO_PESSOA item in catc)
            {
                Int32 num = listaTotal.Where(p => p.TIPE_CD_ID == item.TIPE_CD_ID).ToList().Count;
                if (num > 0)
                {
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = item.TIPE_NM_NOME;
                    mod.Valor = num;
                    lista7.Add(mod);
                }
            }
            ViewBag.ListaAssCats = lista7;
            Session["ListaAssCats"] = lista7;

            // Recupera assinantes por data de início
            List<DateTime> datasCR = listaMes.Where(m => m.ASSI_DT_INICIO.Value != null).OrderBy(m => m.ASSI_DT_INICIO.Value).Select(p => p.ASSI_DT_INICIO.Value.Date).Distinct().ToList();
            List<ModeloViewModel> listaLogDia = new List<ModeloViewModel>();
            foreach (DateTime item in datasCR)
            {
                Int32 conta = listaTotal.Where(p => p.ASSI_DT_INICIO.Value.Date == item).ToList().Count;
                ModeloViewModel mod1 = new ModeloViewModel();
                mod1.DataEmissao = item;
                mod1.Valor = conta;
                listaLogDia.Add(mod1);
            }
            listaLogDia = listaLogDia.OrderBy(p => p.DataEmissao).ToList();
            ViewBag.ListaLogDia = listaLogDia;
            ViewBag.ContaLogDia = listaLogDia.Count;
            Session["ListaDatasLog"] = datasCR;
            Session["ListaLogResumo"] = listaLogDia;

            // Assinantes em atraso
            List<ASSINANTE_PAGAMENTO> pags = assiApp.GetAllPagamentos().ToList();
            pags = pags.Where(p => p.ASPA_NR_ATRASO > 0 & p.ASPA_IN_PAGO == 0).ToList();
            List<Int32> assi = pags.Select(p => p.ASSI_CD_ID).Distinct().ToList();
            Int32 numAtrasos = pags.Count;
            ViewBag.NumAtrasos = numAtrasos;
            Int32 numAssiAtrasos = assi.Count;
            ViewBag.NumAssiAtrasos = numAssiAtrasos;
            
            List<ModeloViewModel> lista8 = new List<ModeloViewModel>();
            foreach (Int32 item in assi)
            {
                ASSINANTE ass = listaTotal.Where(p => p.ASSI_CD_ID == item).FirstOrDefault();
                if (ass != null)
                {
                    String nome = ass.ASSI_NM_NOME;
                    Int32 num = pags.Where(p => p.ASSI_CD_ID == item).Count();
                    Decimal? valor = pags.Where(p => p.ASSI_CD_ID == item).Sum(p => p.ASPA_VL_VALOR);
                    ModeloViewModel mod = new ModeloViewModel();
                    mod.Nome = nome;
                    mod.Valor = num;
                    mod.ValorDec = valor.Value;
                    lista8.Add(mod);
                }
            }
            ViewBag.ListaAssAtraso = lista8;
            Session["ListaAssAtraso"] = lista8;

            // Exibe
            UsuarioViewModel vm = Mapper.Map<USUARIO, UsuarioViewModel>(usuario);
            Session["TotalAssinantes"] = listaTotal.Count;
            Session["Bloqueados"] = numBloqueados;
            Session["VoltaDash"] = 3;
            Session["VoltaAssinante"] = 2;
            return View(vm);
        }

        public JsonResult GetDadosUsuario()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaUsuario"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteUFLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssUF"];
            List<String> uf = new List<String>();
            List<Int32> valor = new List<Int32>();
            uf.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                uf.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("ufs", uf);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteCidadeLista()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCidade"];
            List<String> cidade = new List<String>();
            List<Int32> valor = new List<Int32>();
            cidade.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                cidade.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", cidade);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteAtraso()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssAtraso"];
            List<String> nome = new List<String>();
            List<Int32> valor = new List<Int32>();
            nome.Add(" ");
            valor.Add(0);

            foreach (ModeloViewModel item in lista)
            {
                nome.Add(item.Nome);
                valor.Add(item.Valor);
            }

            Hashtable result = new Hashtable();
            result.Add("cids", nome);
            result.Add("valores", valor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteCategoria()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCats"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteUF()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssUF"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        public JsonResult GetDadosAssinanteCidade()
        {
            List<ModeloViewModel> lista = (List<ModeloViewModel>)Session["ListaAssCidade"];
            List<String> desc = new List<String>();
            List<Int32> quant = new List<Int32>();
            List<String> cor = new List<String>();
            cor.Add("#cd9d6d");
            cor.Add("#cdc36d");
            cor.Add("#a0cfff");
            Int32 i = 1;

            foreach (ModeloViewModel item in lista)
            {
                desc.Add(item.Nome);
                quant.Add(item.Valor);
                if (i == 1)
                {
                    cor.Add("#cd9d6d");
                }
                else if (i == 2)
                {
                    cor.Add("#cdc36d");
                }
                else if (i == 3)
                {
                    cor.Add("#a0cfff");
                }
                i++;
                if (i > 3)
                {
                    i = 1;
                }
            }

            Hashtable result = new Hashtable();
            result.Add("labels", desc);
            result.Add("valores", quant);
            result.Add("cores", cor);
            return Json(result);
        }

        [HttpGet]
        public ActionResult MontarTelaCentralAssinante()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE assi = assiApp.GetItemById(idAss);
            ViewBag.UF = new SelectList(assiApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");

            // Recupera consumo
            Int32 numUsu = usuApp.GetAllItens(idAss).Count;
            Int32 numCli = cliApp.GetAllItens(idAss).Count;
            Int32 numProc = crmApp.GetAllItens(idAss).Count;
            Int32 numAcoes = crmApp.GetAllAcoes(idAss).Count;
            Int32 numProps = crmApp.GetAllPedidos(idAss).Count;
            //Int32 numEmail = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 1).ToList().Count;
            //Int32 numSMS = menApp.GetAllItens(idAss).Where(p => p.MENS_IN_TIPO == 2).ToList().Count;

            ViewBag.NumUsu = numUsu;
            ViewBag.NumCli = numCli;
            ViewBag.NumProc = numProc;
            ViewBag.NumAcoes = numAcoes;
            ViewBag.NumProps = numProps;
            //ViewBag.NumEmail = numEmail;
            //ViewBag.NumSMS = numSMS;

            // Recupera vencimentos
            List<ASSINANTE_PLANO> planos = assi.ASSINANTE_PLANO.ToList();
            List<ASSINANTE_PLANO> planosVencidos = planos.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date).ToList();
            List<ASSINANTE_PLANO> planosVencer30 = planos.Where(p => p.ASPL_DT_VALIDADE < DateTime.Today.Date.AddDays(30)).ToList();
            Int32 vencidos = planosVencidos.Count;
            Int32 vencer30 = planosVencer30.Count;

            Session["PlanosVencidos"] = planosVencidos;
            Session["PlanosVencer30"] = planosVencer30;
            ViewBag.Vencidos = vencidos;
            ViewBag.Vencer30 = vencer30;
            ViewBag.Planos = planos.Count;
            ViewBag.Usuarios = assi.USUARIO.Where(p => p.USUA_IN_ATIVO == 1).Count();

            // Recupera parcelas em atraso
            List<ASSINANTE_PAGAMENTO> pags = assiApp.GetAllPagamentos();
            pags = pags.Where(p => p.ASPA_IN_PAGO == 0 & p.ASPA_NR_ATRASO > 0 & p.ASSI_CD_ID == idAss).ToList();
            Int32 atraso = pags.Count;
            ViewBag.Atrasos = pags.Count;

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 5)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 6)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 21)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0204", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 22)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0205", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 23)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0206", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 99)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0216", CultureInfo.CurrentCulture));
                    Session["Ativa"] = null;
                }
            }

            // Indicadores

            // Sessões
            Session["MensAssinante"] = null;
            Session["VoltaAssinante"] = 1;
            Session["Assinante"] = assi;
            Session["IdAssinante"] = idAss;
            AssinanteViewModel vm = Mapper.Map<ASSINANTE, AssinanteViewModel>(assi);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MontarTelaCentralAssinante(AssinanteViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.UF = new SelectList(assiApp.GetAllUF(), "UF_CD_ID", "UF_SG_SIGLA");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    ASSINANTE item = Mapper.Map<AssinanteViewModel, ASSINANTE>(vm);
                    Int32 volta = assiApp.ValidateEdit(item, item, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    return RedirectToAction("MontarTelaCentralAssinante");
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

        public ActionResult VerComparativoPlanos()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["VoltaCompPlano"] = 2;
            return RedirectToAction("VerComparativoPlanos", "Plano");
        }

        [HttpGet]
        public ActionResult ExcluirAssinantePlanoProprio(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Cancela plano
            ASSINANTE_PLANO item = assiApp.GetPlanoById(id);
            item.ASPL_IN_ATIVO = 0;
            Int32 volta = assiApp.ValidateEditPlano(item);

            // Desativa pagamentos
            ASSINANTE assi = assiApp.GetItemById(item.ASSI_CD_ID);
            PLANO plan = assiApp.GetPlanoBaseById(item.PLAN_CD_ID);
            List<ASSINANTE_PAGAMENTO> pags = assi.ASSINANTE_PAGAMENTO.Where(p => p.PLAN_CD_ID == item.PLAN_CD_ID).ToList();
            pags.ForEach(p => p.ASPA_IN_ATIVO = 0);
            Int32 volta1 = assiApp.ValidateEdit(assi, assi, usuario);

            // Envia e-mail para CRMSys
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);
            String texto = temApp.GetByCode("ASSPLANCAN").TEMP_TX_CORPO;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = usuario.USUA_NM_NOME;
            mens.ID = assi.ASSI_CD_ID;
            mens.MODELO = conf.CONF_EM_CRMSYS;
            mens.MENS_NM_CAMPANHA = assi.ASSI_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            mens.MENS_NM_LINK = null;
            mens.LINK = "1";
            texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
            texto = texto.Replace("{data}", DateTime.Today.Date.ToShortDateString());
            texto = texto.Replace("{plano1}", plan.PLAN_NM_NOME);
            texto = texto.Replace("{data1}", DateTime.Today.Date.ToShortDateString());
            mens.MENS_TX_TEXTO = texto;
            Int32 volta2 = ProcessaEnvioEMailAssinante(mens, usuario);
            Session["MensAssinante"] = 99;
            Session["TipoAssunto"] = 1;
            return RedirectToAction("MontarTelaCentralAssinante");
        }

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailAssinante(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            ASSINANTE cont = (ASSINANTE)Session["Assinante"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezada <b>Administração Precificação</b>";

            // Prepara rodape
            String rod = String.Empty;
            if (vm.MENS_NM_RODAPE == null)
            {
                rod = "Atenciosamente <b>" + usuario.ASSINANTE.ASSI_NM_NOME + "</b>";
            }

            // Prepara corpo do e-mail
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);

            // Link          
            if (!String.IsNullOrEmpty(vm.MENS_NM_LINK))
            {
                if (!vm.MENS_NM_LINK.Contains("www."))
                {
                    vm.MENS_NM_LINK = "www." + vm.MENS_NM_LINK;
                }
                if (!vm.MENS_NM_LINK.Contains("http://"))
                {
                    vm.MENS_NM_LINK = "http://" + vm.MENS_NM_LINK;
                }
                if (vm.LINK == "1")
                {
                    str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para acessar o CRMSys</a>");
                }
                else
                {
                    str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
                }
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            if ((Int32)Session["TipoAssunto"] == 1)
            {
                mensagem.ASSUNTO = "Cancelamento de Plano de Assinatura";
            }
            else
            {
                mensagem.ASSUNTO = "Solicitação de Plano de Assinatura";
            }
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = vm.MODELO;
            mensagem.EMAIL_CC_DESTINO = vm.MENS_NM_CAMPANHA;
            mensagem.EMAIL_EMISSOR = conf.CONF_NM_EMAIL_EMISSOO;
            mensagem.ENABLE_SSL = true;
            mensagem.NOME_EMISSOR = usuario.ASSINANTE.ASSI_NM_NOME;
            mensagem.PORTA = conf.CONF_NM_PORTA_SMTP;
            mensagem.PRIORIDADE = System.Net.Mail.MailPriority.High;
            mensagem.SENHA_EMISSOR = conf.CONF_NM_SENHA_EMISSOR;
            mensagem.SMTP = conf.CONF_NM_HOST_SMTP;
            mensagem.IS_HTML = true;
            mensagem.NETWORK_CREDENTIAL = net;

            // Envia mensagem
            try
            {
                Int32 voltaMail = CommunicationPackage.SendEmail(mensagem);
            }
            catch (Exception ex)
            {
                String erro = ex.Message;
            }
            return 0;
        }

        [HttpGet]
        public ActionResult SolicitarIncluirAssinantePlano()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];

            // Prepara Listas
            ViewBag.Planos = new SelectList(assiApp.GetAllPlanos().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Parcelado", Value = "1" });
            preco.Add(new SelectListItem() { Text = "À Vista", Value = "2" });
            ViewBag.Precos = new SelectList(preco, "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Boleto Bancário", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Cartão Crédito", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Mensagens
            if (Session["MensAssinante"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensAssinante"] == 31)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0207", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 32)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0208", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 33)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0209", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 34)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0210", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensAssinante"] == 35)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0217", CultureInfo.CurrentCulture));
                }
            }

            // Prepara view
            ASSINANTE_PLANO item = new ASSINANTE_PLANO();
            AssinantePlanoViewModel vm = Mapper.Map<ASSINANTE_PLANO, AssinantePlanoViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.ASPL_IN_ATIVO = 1;
            vm.ASPL_DT_INICIO = DateTime.Today.Date;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SolicitarIncluirAssinantePlano(AssinantePlanoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            ASSINANTE assi = assiApp.GetItemById(idAss);

            ViewBag.Planos = new SelectList(assiApp.GetAllPlanos().OrderBy(p => p.PLAN_NM_NOME), "PLAN_CD_ID", "PLAN_NM_NOME");
            List<SelectListItem> preco = new List<SelectListItem>();
            preco.Add(new SelectListItem() { Text = "Parcelado", Value = "1" });
            preco.Add(new SelectListItem() { Text = "À Vista", Value = "2" });
            ViewBag.Precos = new SelectList(preco, "Value", "Text");
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Boleto Bancário", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Cartão Crédito", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica existencia
                    List<ASSINANTE_PLANO> plans = assi.ASSINANTE_PLANO.ToList();
                    Int32 planEsc = plans.Where(p => p.PLAN_CD_ID == vm.PLAN_CD_ID & p.ASPL_IN_ATIVO == 1 & p.ASSI_CD_ID == idAss).ToList().Count();
                    if (planEsc > 0)
                    {
                        Session["MensAssinante"] = 34;
                        return RedirectToAction("SolicitarIncluirAssinantePlano");
                    }
                    planEsc = plans.Where(p => p.PLAN_CD_ID == vm.PLAN_CD_ID & p.ASPL_IN_ATIVO == 2 & p.ASSI_CD_ID == idAss).ToList().Count();
                    if (planEsc > 0)
                    {
                        Session["MensAssinante"] = 35;
                        return RedirectToAction("SolicitarIncluirAssinantePlano");
                    }

                    // Executa a operação
                    ASSINANTE_PLANO item = Mapper.Map<AssinantePlanoViewModel, ASSINANTE_PLANO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PLANO plan = assiApp.GetPlanoBaseById(item.PLAN_CD_ID);
                    item.ASPL_DT_VALIDADE = item.ASPL_DT_INICIO.Value.AddMonths(plan.PLAN_IN_DURACAO.Value);
                    item.ASPL_IN_ATIVO = 2;
                    Int32 volta = assiApp.ValidateCreatePlano(item);

                    // Envia e-mail para CRMSys
                    CONFIGURACAO conf = confApp.GetItemById(usuarioLogado.ASSI_CD_ID);
                    String texto = temApp.GetByCode("ASSSOLPLAN").TEMP_TX_CORPO;
                    MensagemViewModel mens = new MensagemViewModel();
                    mens.NOME = usuario.USUA_NM_NOME;
                    mens.ID = assi.ASSI_CD_ID;
                    mens.MODELO = conf.CONF_EM_CRMSYS;
                    mens.MENS_NM_CAMPANHA = assi.ASSI_NM_EMAIL;
                    mens.MENS_DT_CRIACAO = DateTime.Today.Date;
                    mens.MENS_IN_TIPO = 1;
                    mens.MENS_NM_LINK = null;
                    mens.LINK = "1";
                    texto = texto.Replace("{plano}", plan.PLAN_NM_NOME);
                    texto = texto.Replace("{data}", DateTime.Today.Date.ToShortDateString());
                    texto = texto.Replace("{plano1}", plan.PLAN_NM_NOME);
                    texto = texto.Replace("{data1}", DateTime.Today.Date.ToShortDateString());
                    texto = texto.Replace("{pagto}", vm.ASPL_IN_PRECO == 1 ? "Parcelado" : "À Vista");
                    texto = texto.Replace("{preco}", vm.ASPL_IN_PRECO == 1 ? "R$ " + CrossCutting.Formatters.DecimalFormatter(plan.PLAN_VL_PRECO.Value) : "R$ " + CrossCutting.Formatters.DecimalFormatter(plan.PLAN_VL_PROMOCAO.Value));
                    texto = texto.Replace("{forma}", vm.ASPL_IN_PAGTO == 1 ? "Boleto Bancário" : "Cartão Crédito");
                    mens.MENS_TX_TEXTO = texto;
                    Session["TipoAssunto"] = 2;
                    Int32 volta1 = ProcessaEnvioEMailAssinante(mens, usuarioLogado);

                    // Finaliza
                    return RedirectToAction("MontarTelaCentralAssinante");
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

        public JsonResult GetPlanos(Int32 id)
        {
            PLANO forn = assiApp.GetPlanoBaseById(id);
            var hash = new Hashtable();
            hash.Add("nome", forn.PLAN_NM_NOME);
            hash.Add("periodicidade", forn.PLANO_PERIODICIDADE.PLPE_NM_NOME);
            hash.Add("valor", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PRECO.Value));
            hash.Add("promo", CrossCutting.Formatters.DecimalFormatter(forn.PLAN_VL_PROMOCAO.Value));
            DateTime data = DateTime.Today.Date.AddDays(Convert.ToDouble(forn.PLANO_PERIODICIDADE.PLPE_NR_DIAS));
            hash.Add("data", data.ToShortDateString());
            hash.Add("duracao", forn.PLAN_IN_DURACAO);
            return Json(hash);
        }

    }
}
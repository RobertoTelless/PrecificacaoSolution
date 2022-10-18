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

        private String msg;
        private Exception exception;
        USUARIO objeto = new USUARIO();
        USUARIO objetoAntes = new USUARIO();
        List<USUARIO> listaMaster = new List<USUARIO>();

        public BaseAdminController(IUsuarioAppService baseApps, ILogAppService logApps, INoticiaAppService notApps, ITarefaAppService tarApps, INotificacaoAppService notfApps, IUsuarioAppService usuApps, IAgendaAppService ageApps, IConfiguracaoAppService confApps, IVideoAppService vidApps, IPessoaExternaAppService pesApps, IMaquinaAppService maqApps, IFormaPagRecAppService forApps, IBancoAppService banApps, IContaBancariaAppService cbApps, ICentroCustoAppService ccApps, IFornecedorAppService fornApps, IClienteAppService cliApps, IProdutoAppService proApps)
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

        public ActionResult MontarTelaDashboardFinanceiro()
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

            Session["Bancos"] = bancos;
            Session["Planos"] = planos;
            Session["Contas"] = contas;

            ViewBag.Bancos = bancos;
            ViewBag.Planos = planos;
            ViewBag.Contas = contas;

            Session["PlanoCredito"] = plano.Where(p => p.CECU_IN_TIPO == 1).ToList().Count;
            Session["PlanoDebito"] = plano.Where(p => p.CECU_IN_TIPO == 2).ToList().Count;
            Session["ContaCorrente"] = conta.Where(p => p.TICO_CD_ID == 1).ToList().Count;
            Session["ContaPoupanca"] = conta.Where(p => p.TICO_CD_ID == 2).ToList().Count;
            Session["ContaInvestimento"] = conta.Where(p => p.TICO_CD_ID == 3).ToList().Count;

            return View(vm);
        }

    }
}
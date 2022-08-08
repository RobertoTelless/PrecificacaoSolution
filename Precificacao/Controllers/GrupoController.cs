using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution.App_Start;
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

namespace ERP_Condominios_Solution.Controllers
{
    public class GrupoController : Controller
    {
        private readonly IGrupoAppService baseApp;
        private readonly ISubgrupoAppService subApp;
        private String msg;
        private Exception exception;
        GRUPO objeto = new GRUPO();
        GRUPO objetoAntes = new GRUPO();
        List<GRUPO> listaMaster = new List<GRUPO>();
        SUBGRUPO objetoSub = new SUBGRUPO();
        SUBGRUPO objetoSubAntes = new SUBGRUPO();
        List<SUBGRUPO> listaMasterSub = new List<SUBGRUPO>();
        String extensao;

        public GrupoController(IGrupoAppService baseApps, ISubgrupoAppService subApps)
        {
            baseApp = baseApps;
            subApp = subApps;
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardCadastros", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaGrupo()
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


            // Carrega listas
            if ((List<GRUPO>)Session["ListaGrupo"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaGrupo"] = listaMaster;
            }
            ViewBag.Listas = (List<GRUPO>)Session["ListaGrupo"];
            Session["Grupo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensGrupo"] != null)
            {
                if ((Int32)Session["MensGrupoCC"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0115", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensGrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0114", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaGrupo"] = 1;
            objeto = new GRUPO();
            return View(objeto);
        }

        public ActionResult RetirarFiltroGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult MostrarTudoGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaGrupo"] = listaMaster;
            return RedirectToAction("MontarTelaGrupo");
        }

        public ActionResult VoltarBaseGrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult IncluirGrupo()
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas

            // Prepara view
            GRUPO item = new GRUPO();
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.GRUP_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirGrupo(GrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensGrupo"] = 3;
                        return RedirectToAction("MontarTelaGrupo");
                    }

                    // Sucesso
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    Session["IdGrupo"] = item.GRUP_CD_ID;
                    if ((Int32)Session["VoltaGrupo"] == 2)
                    {
                        return RedirectToAction("IncluirClasse", "Classe");
                    }
                    return RedirectToAction("MontarTelaGrupo");
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
        public ActionResult EditarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            Session["Grupo"] = item;

            // Indicadores
            
            // Mensagens
            if (Session["MensGrupo"] != null)
            {


            }

            objetoAntes = item;
            Session["IdGrupo"] = id;
            GrupoViewModel vm = Mapper.Map<GRUPO, GrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarGrupo(GrupoViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    GRUPO item = Mapper.Map<GrupoViewModel, GRUPO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<GRUPO>();
                    Session["ListaGrupo"] = null;
                    return RedirectToAction("MontarTelaGrupo");
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

        public ActionResult VoltarAnexoGrupo()
        {

            return RedirectToAction("EditarGrupo", new { id = (Int32)Session["IdGrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRUP_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensGrupo"] = 4;
                return RedirectToAction("MontarTelaGrupo");
            }
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult ReativarGrupo(Int32 id)
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
                    Session["MensGrupo"] = 2;
                    return RedirectToAction("MontarTelaGrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            GRUPO item = baseApp.GetItemById(id);
            objetoAntes = (GRUPO)Session["Grupo"];
            item.GRUP_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            Session["ListaGrupo"] = null;
            return RedirectToAction("MontarTelaGrupo");
        }

        [HttpGet]
        public ActionResult MontarTelaSubgrupo()
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
            if ((List<SUBGRUPO>)Session["ListaSubgrupo"] == null)
            {
                listaMasterSub = subApp.GetAllItens(idAss);
                Session["ListaSubgrupo"] = listaMasterSub;
            }
            ViewBag.Listas = (List<SUBGRUPO>)Session["ListaSubgrupo"];
            ViewBag.Cats = new SelectList(baseApp.GetAllItens(idAss).OrderBy(x => x.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            Session["Subgrupo"] = null;

            // Indicadores
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            if (Session["MensSubgrupo"] != null)
            {
                if ((Int32)Session["MensSubgrupo"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0116", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensSubgrupo"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0117", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaSubGrupo"] = 1;
            objetoSub = new SUBGRUPO();
            return View(objetoSub);
        }

        public ActionResult RetirarFiltroSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult MostrarTudoSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterSub= subApp.GetAllItensAdm(idAss);
            Session["ListaSubgrupo"] = listaMasterSub;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        public ActionResult VoltarBaseSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult IncluirSubgrupo()
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(baseApp.GetAllItens(idAss).OrderBy(x => x.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            
            // Prepara view
            SUBGRUPO item = new SUBGRUPO();
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            vm.ASSI_CD_ID = idAss;
            vm.SGGR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirSubgrupo(SubgrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(baseApp.GetAllItens(idAss).OrderBy(x => x.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    Int32 volta = subApp.ValidateCreate(item, usuario);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensSubgrupo"] = 3;
                        return RedirectToAction("MontarTelaSubgrupo");
                    }

                    // Sucesso
                    listaMasterSub = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    Session["IdSubgrupo"] = item.SGGR_CD_ID;
                    if ((Int32)Session["VoltaSubgrupo"] == 2)
                    {
                        return RedirectToAction("IncluirClasse", "Classe");
                    }
                    return RedirectToAction("MontarTelaSubgrupo");
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
        public ActionResult EditarSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = subApp.GetItemById(id);
            Session["Subgrupo"] = item;
            ViewBag.Cats = new SelectList(baseApp.GetAllItens(idAss).OrderBy(x => x.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");

            // Indicadores

            // Mensagens
            if (Session["MensSubgrupo"] != null)
            {


            }

            objetoSubAntes = item;
            Session["IdSubgrupo"] = id;
            SubgrupoViewModel vm = Mapper.Map<SUBGRUPO, SubgrupoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarSubgrupo(SubgrupoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(baseApp.GetAllItens(idAss).OrderBy(x => x.GRUP_NM_NOME), "GRUP_CD_ID", "GRUP_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuario = (USUARIO)Session["UserCredentials"];
                    SUBGRUPO item = Mapper.Map<SubgrupoViewModel, SUBGRUPO>(vm);
                    Int32 volta = subApp.ValidateEdit(item, objetoSubAntes, usuario);

                    // Verifica retorno

                    // Sucesso
                    listaMasterSub = new List<SUBGRUPO>();
                    Session["ListaSubgrupo"] = null;
                    return RedirectToAction("MontarTelaSubgrupo");
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

        public ActionResult VoltarAnexoSubgrupo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("EditarSubgrupo", new { id = (Int32)Session["IdSubgrupo"] });
        }

        [HttpGet]
        public ActionResult ExcluirSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = subApp.GetItemById(id);
            objetoSubAntes = (SUBGRUPO)Session["Subgrupo"];
            item.SGGR_IN_ATIVO = 0;
            Int32 volta = subApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensSubgrupo"] = 4;
                return RedirectToAction("MontarTelaSubgrupo");
            }
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

        [HttpGet]
        public ActionResult ReativarSubgrupo(Int32 id)
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
                    Session["MensSubgrupo"] = 2;
                    return RedirectToAction("MontarTelaSubgrupo");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            SUBGRUPO item = subApp.GetItemById(id);
            objetoSubAntes = (SUBGRUPO)Session["Subgrupo"];
            item.SGGR_IN_ATIVO = 1;
            Int32 volta = subApp.ValidateReativar(item, usuario);
            Session["ListaSubgrupo"] = null;
            return RedirectToAction("MontarTelaSubgrupo");
        }

    }
}
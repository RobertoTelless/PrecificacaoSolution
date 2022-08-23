﻿using System;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using EntitiesServices.WorkClasses;

namespace ERP_Condominios_Solution.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoAppService baseApp;
        private readonly ILogAppService logApp;

        private String msg;
        private Exception exception;
        VIDEO objeto = new VIDEO();
        VIDEO objetoAntes = new VIDEO();
        List<VIDEO> listaMaster = new List<VIDEO>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public VideoController(IVideoAppService baseApps, ILogAppService logApps)
        {
            baseApp = baseApps;
            logApp = logApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            VIDEO item = new VIDEO();
            VideoViewModel vm = Mapper.Map<VIDEO, VideoViewModel>(item);
            return View(vm);
        }

        public ActionResult VerVideo(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["IdVolta"] = id;
            VIDEO item = baseApp.GetItemById(id);
            item.VIDE_NR_ACESSOS = ++item.VIDE_NR_ACESSOS;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            
            VideoViewModel vm = Mapper.Map<VIDEO, VideoViewModel>(item);
            return View(vm);
        }

        public ActionResult MontarTelaUsuario()
        {
            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            usuario = (USUARIO)Session["UserCredentials"];

            if ((List<VIDEO>)Session["ListaVideo"] == null)
            {
                listaMaster = baseApp.GetAllItensValidos(idAss);
                Session["ListaVideo"] = listaMaster;
            }
            ViewBag.Listas = (List<VIDEO>)Session["ListaVideo"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Video";

            // Indicadores
            ViewBag.Videos = ((List<VIDEO>)Session["ListaVideo"]).Count;

            // Mensagem

            // Abre view
            Session["MensVideo"] = 0;
            Session["VoltaVideo"] = 1;
            objeto = new VIDEO();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaVideo"] == 1)
            {
                return RedirectToAction("CarregarBase", "BaseAdmin");
            }
            return RedirectToAction("MontarTelaVideo", "Video");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVideo"];
            return RedirectToAction("VerVideo", new { id = idNot });
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpPost]
        public ActionResult FiltrarVideo(VIDEO item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<VIDEO> listaObj = new List<VIDEO>();
                Int32 volta = baseApp.ExecuteFilter(item.VIDE_NM_TITULO, item.VIDE_NM_AUTOR, item.VIDE_DT_EMISSAO, item.VIDE_NM_DESCRICAO, item.VIDE_LK_LINK, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensVideo"] = 1;
                }

                // Sucesso
                Session["MensVideo"] = 0;
                listaMaster = listaObj;
                Session["ListaVideo"] = listaObj;
                return RedirectToAction("MontarTelaUsuario");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaUsuario");
            }
        }

        public ActionResult RetirarFiltroVideo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaVideo"] = null;
            return RedirectToAction("MontarTelaUsuario");
        }

        public ActionResult MostrarTudoVideo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItens(idAss);
            Session["ListaVideo"] = listaMaster;
            return RedirectToAction("MontarTelaUsuario");
        }

       [HttpGet]
        public ActionResult MontarTelaVideo()
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

            // Carrega listas
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((List<VIDEO>)Session["ListaVideo"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaVideo"] = listaMaster;
            }
            ViewBag.Listas = (List<VIDEO>)Session["ListaVideo"];
            ViewBag.Title = "Videos";

            // Indicadores
            ViewBag.Videos = ((List<VIDEO>)Session["ListaVideo"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Mensagem
            if ((Int32)Session["MensPermissao"] == 2)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
            }

            // Abre view
            objeto = new VIDEO();
            Session["VoltaVideo"] = 1;
            Session["MensVideo"] = 0;
            return View(objeto);
        }

        public ActionResult RetirarFiltroVideoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaVideo"] = null;
            return RedirectToAction("MontarTelaVideo");
        }

        public ActionResult MostrarTudoVideoGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaVideo"] = listaMaster;
            return RedirectToAction("MontarTelaVideo");
        }

        [HttpPost]
        public ActionResult FiltrarVideoGeral(VIDEO item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<VIDEO> listaObj = new List<VIDEO>();
                Int32 volta = baseApp.ExecuteFilter(item.VIDE_NM_TITULO, item.VIDE_NM_AUTOR, item.VIDE_DT_EMISSAO, item.VIDE_NM_DESCRICAO, item.VIDE_LK_LINK, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensVideo"] = 1;
                }

                // Sucesso
                Session["MensVideo"] = 0;
                listaMaster = listaObj;
                Session["ListaVideo"]  = listaObj;
                return RedirectToAction("MontarTelaVideo");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaVideo");
            }
        }

        public ActionResult VoltarBaseVideo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaVideo");
        }

        [HttpGet]
        public ActionResult IncluirVideo()
        {
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
                    return RedirectToAction("MontarTelaVideo", "Video");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Prepara view
            VIDEO item = new VIDEO();
            VideoViewModel vm = Mapper.Map<VIDEO, VideoViewModel>(item);
            vm.ASSI_CD_ID = (Int32)Session["IdAssinante"];
            vm.VIDE_DT_EMISSAO = DateTime.Today.Date;
            vm.VIDE_IN_ATIVO = 1;
            vm.VIDE_DT_VALIDADE = DateTime.Today.Date.AddDays(30);
            vm.VIDE_NR_ACESSOS = 0;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirVideo(VideoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    Int32 idAss = (Int32)Session["IdAssinante"];
                    VIDEO item = Mapper.Map<VideoViewModel, VIDEO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno

                    // Carrega foto e processa alteracao
                    if (item.VIDE_AQ_FOTO == null)
                    {
                        item.VIDE_AQ_FOTO = "~/Images/p_big2.jpg";
                        volta = baseApp.ValidateEdit(item, item, usuarioLogado);

                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + idAss.ToString() + "/Videos/" + item.VIDE_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    Session["IdVideo"] = item.VIDE_CD_ID;
                    if (Session["FileQueueVideo"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueueVideo"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueueVideo(file);
                            }
                            else
                            {
                                UploadFotoQueueVideo(file);
                            }
                        }

                        Session["FileQueueVideo"] = null;
                    }

                    // Sucesso
                    listaMaster = new List<VIDEO>();
                    Session["ListaVideo"] = null;
                    Session["VoltaVideo"] = 1;
                    Session["IdVideoVolta"] = item.VIDE_CD_ID;
                    Session["Video"] = item;
                    Session["MensVideo"] = 0;
                    return RedirectToAction("MontarTelaVideo");
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
        public ActionResult EditarVideo(Int32 id)
        {
            // Prepara view
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
                    return RedirectToAction("MontarTelaVideo", "Video");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }

            // Mensagens
            if ((Int32)Session["MensVideo"] == 10)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
            }
            if ((Int32)Session["MensVideo"] == 11)
            {
                ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
            }

            VIDEO item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["Video"] = item;
            Session["IdVideo"] = id;
            VideoViewModel vm = Mapper.Map<VIDEO, VideoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarVideo(VideoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    VIDEO item = Mapper.Map<VideoViewModel, VIDEO>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMaster = new List<VIDEO>();
                    Session["ListaVideo"] = null;
                    Session["MensVideo"] = 0;
                    return RedirectToAction("MontarTelaVideo");
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
        public ActionResult ExcluirVideo(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VIDEO item = baseApp.GetItemById(id);
            objetoAntes = (VIDEO)Session["Video"];
            item.VIDE_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            listaMaster = new List<VIDEO>();
            Session["ListaVideo"] = null;
            Session["FiltroVideo"] = null;
            return RedirectToAction("MontarTelaVideo");
        }

        [HttpGet]
        public ActionResult ReativarVideo(Int32 id)
        {
            // Valida acesso
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            VIDEO item = baseApp.GetItemById(id);
            objetoAntes = (VIDEO)Session["Video"];
            item.VIDE_IN_ATIVO = 1;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            listaMaster = new List<VIDEO>();
            Session["ListaVideo"] = null;
            Session["FiltroVideo"] = null;
            return RedirectToAction("MontarTelaVideo");
        }

        [HttpPost]
        public ActionResult UploadFotoVideo(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if (file == null)
            {
                Session["MensVideo"] = 10;
                return RedirectToAction("VoltarAnexoVideo");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Int32 idNot = (Int32)Session["IdVideo"];

            VIDEO item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensVideo"] = 11;
                return RedirectToAction("VoltarAnexoVideo");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Videos/" + item.VIDE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            item.VIDE_AQ_FOTO = "~" + caminho + fileName;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, objetoAntes);
            return RedirectToAction("VoltarAnexoVideo");
        }

        public ActionResult VoltarAnexoVideo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdVolta"];
            return RedirectToAction("EditarNoticia", new { id = idNot });
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
            Session["FileQueueVideo"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFotoQueueVideo(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdVideo"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensVideo"] = 10;
                return RedirectToAction("VoltarAnexoVideo");
            }

            VIDEO item = baseApp.GetById(idUsu);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensVideo"] = 11;
                return RedirectToAction("VoltarAnexoVideo");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/Videos/" + item.VIDE_CD_ID.ToString() + "/Fotos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Checa extensão
            if (extensao.ToUpper() == ".JPG" || extensao.ToUpper() == ".GIF" || extensao.ToUpper() == ".PNG" || extensao.ToUpper() == ".JPEG")
            {
                // Salva arquivo
                System.IO.File.WriteAllBytes(path, file.Contents);

                // Gravar registro
                item.VIDE_AQ_FOTO = "~" + caminho + fileName;
                objeto = item;
                Int32 volta = baseApp.ValidateEdit(item, objeto);
            }
            return RedirectToAction("VoltarAnexoVideo");
        }

        [HttpPost]
        public ActionResult UploadFileQueueVideo(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("VoltarAnexoUsuario");
        }
    }
}
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
using System.Text;
using System.Net;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class PessoaExternaController : Controller
    {
        private readonly IPessoaExternaAppService baseApp;
        private readonly ILogAppService logApp;
        private readonly INotificacaoAppService notiApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        PESSOA_EXTERNA objeto = new PESSOA_EXTERNA();
        PESSOA_EXTERNA objetoAntes = new PESSOA_EXTERNA();
        List<PESSOA_EXTERNA> listaMaster = new List<PESSOA_EXTERNA>();
        LOG objLog = new LOG();
        LOG objLogAntes = new LOG();
        List<LOG> listaMasterLog = new List<LOG>();
        String extensao;

        public PessoaExternaController(IPessoaExternaAppService baseApps, ILogAppService logApps, INotificacaoAppService notiApps, IConfiguracaoAppService confApps)
        {
            baseApp = baseApps;
            logApp = logApps;
            notiApp = notiApps;
            confApp = confApps;
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

        [HttpGet]
        public ActionResult MontarTelaPessoaExterna()
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
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");

            // Carrega listas
            if ((List<PESSOA_EXTERNA>)Session["ListaPessoaExterna"] == null)
            {
                listaMaster = baseApp.GetAllItens(idAss);
                Session["ListaPessoaExterna"] = listaMaster;
                Session["FiltroPessoaExterna"] = null;
            }
            List<PESSOA_EXTERNA> listaUsu = (List<PESSOA_EXTERNA>)Session["ListaPessoaExterna"];
            ViewBag.Listas = listaUsu;
            ViewBag.Usuarios = listaUsu.Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            ViewBag.Title = "Pessoa Externa";

            // Mensagens
            if (Session["MensPessoaExterna"] != null)
            {
                if ((Int32)Session["MensPessoaExterna"] == 2)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 3)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0156", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 4)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0157", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 5)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0110", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 6)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0111", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 7)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0097", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 9)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0045", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensPessoaExterna"] == 50)
                {
                    ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0051", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["MensPessoaExterna"] = 0;
            objeto = new PESSOA_EXTERNA();
            return View(objeto);
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarBase()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaPessoaExterna");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao");
        }

        [HttpPost]
        public ActionResult FiltrarPessoaExterna(PESSOA_EXTERNA item)
        {
            try
            {
                // Executa a operação
                if ((String)Session["Ativa"] == null)
                {
                    return RedirectToAction("Login", "ControleAcesso");
                }
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<PESSOA_EXTERNA> listaObj = new List<PESSOA_EXTERNA>();
                Int32 volta = baseApp.ExecuteFilter(item.CARG_CD_ID, item.PEEX_NM_NOME, item.PEEX_NR_CPF, item.PEES_EM_EMAIL, idAss, out listaObj);
                Session["FiltroPessoaExterna"] = item;

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensPessoaExterna"] = 1;
                }

                // Sucesso
                Session["MensPessoaExterna"] = 0;
                listaMaster = listaObj;
                Session["ListaPessoaExterna"] = listaObj;
                return RedirectToAction("MontarTelaPessoaExterna");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaUsuario");
            }
        }

        public ActionResult RetirarFiltroPessoaExterna()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            Session["ListaPessoaExterna"] = null;
            return RedirectToAction("MontarTelaPessoaExterna");
        }

        public ActionResult MostrarTudoPessoaExterna()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            USUARIO usuario = (USUARIO)Session["UserCredentials"];
            listaMaster = baseApp.GetAllItensAdm(idAss);
            Session["ListaPessoaExterna"] = listaMaster;
            return RedirectToAction("MontarTelaPessoaExterna");
        }

        [HttpGet]
        public ActionResult VerAnexoPessoaExterna(Int32 id)
        {
            // Prepara view
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            PESSOA_EXTERNA_ANEXO item = baseApp.GetAnexoById(id);
            return View(item);
        }

        public FileResult DownloadPessoaExterna(Int32 id)
        {
            PESSOA_EXTERNA_ANEXO item = baseApp.GetAnexoById(id);
            String arquivo = item.PEAX_AQ_ARQUIVO;
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

        public ActionResult VoltarAnexoPessoaExterna()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idUsu = (Int32)Session["IdPessoaExterna"];
            return RedirectToAction("EditarPessoaExterna", new { id = idUsu });
        }

        [HttpGet]
        public ActionResult VerPessoaExterna(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["IdUsuario"] = id;
            PESSOA_EXTERNA item = baseApp.GetItemById(id);
            PessoaExternaViewModel vm = Mapper.Map<PESSOA_EXTERNA, PessoaExternaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult IncluirPessoaExterna()
        {
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPessoaExterna"] = 2;
                    return RedirectToAction("MontarTelaPessoaExterna");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss).OrderBy(p => p.CARG_NM_NOME), "CARG_CD_ID", "CARG_NM_NOME");

            // Prepara view
            PESSOA_EXTERNA item = new PESSOA_EXTERNA();
            PessoaExternaViewModel vm = Mapper.Map<PESSOA_EXTERNA, PessoaExternaViewModel>(item);
            vm.PEEX_DT_CADASTRO = DateTime.Today.Date;
            vm.PEEX_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirPessoaExterna(PessoaExternaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    PESSOA_EXTERNA item = Mapper.Map<PessoaExternaViewModel, PESSOA_EXTERNA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = baseApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPessoaExterna"] = 3;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }
                    if (volta == 2)
                    {
                        Session["MensPessoaExterna"] = 4;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }
                    if (volta == 3)
                    {
                        Session["MensPessoaExterna"] = 5;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }
                    if (volta == 4 )
                    {
                        Session["MensPessoaExterna"] = 6;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }
                    if (volta == 5)
                    {
                        Session["MensPessoaExterna"] = 7;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }

                    // Cria pastas
                    String caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/PessoaExterna/" + item.PEEX_CD_ID.ToString() + "/Fotos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));
                    caminho = "/Imagens/" + item.ASSI_CD_ID.ToString() + "/PessoaExterna/" + item.PEEX_CD_ID.ToString() + "/Anexos/";
                    Directory.CreateDirectory(Server.MapPath(caminho));

                    // Sucesso
                    listaMaster = new List<PESSOA_EXTERNA>();
                    Session["ListaPessoaExterna"] = null;
                    Session["IdPessoaExterna"] = item.PEEX_CD_ID;

                    if (Session["FileQueuePessoaExterna"] != null)
                    {
                        List<FileQueue> fq = (List<FileQueue>)Session["FileQueuePessoaExterna"];

                        foreach (var file in fq)
                        {
                            if (file.Profile == null)
                            {
                                UploadFileQueuePessoaExterna(file);
                            }
                        }

                        Session["FileQueuePessoaExterna"] = null;
                    }
                    return RedirectToAction("MontarTelaPessoaExterna");
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
        public ActionResult EditarPessoaExterna(Int32 id)
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
                    Session["MensPessoaExterna"] = 2;
                    return RedirectToAction("MontarTelaPessoaExterna");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            ViewBag.UsuarioLogado = usuario;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            PESSOA_EXTERNA item = baseApp.GetItemById(id);
            objetoAntes = item;
            Session["PessoaExterna"] = item;
            Session["IdPessoaExterna"] = id;
            PessoaExternaViewModel vm = Mapper.Map<PESSOA_EXTERNA, PessoaExternaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarUsuario(PessoaExternaViewModel vm)
        {
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfis = new SelectList((List<PERFIL>)Session["Perfis"], "PERF_CD_ID", "PERF_NM_NOME");
            ViewBag.Cargos = new SelectList(baseApp.GetAllCargos(idAss), "CARG_CD_ID", "CARG_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    PESSOA_EXTERNA item = Mapper.Map<PessoaExternaViewModel, PESSOA_EXTERNA>(vm);
                    Int32 volta = baseApp.ValidateEdit(item, objetoAntes, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensPessoaExterna"] = 4;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }
                    if (volta == 2)
                    {
                        Session["MensPessoaExterna"] = 5;
                        return RedirectToAction("MontarTelaPessoaExterna");
                    }

                    // Mensagens
                    if (Session["MensPessoaExterna"] != null)
                    {
                        if ((Int32)Session["MensPessoaExterna"] == 10)
                        {
                            ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0019", CultureInfo.CurrentCulture));
                        }
                        if ((Int32)Session["MensPessoaExterna"] == 11)
                        {
                            ModelState.AddModelError("", PlatMensagens_Resources.ResourceManager.GetString("M0024", CultureInfo.CurrentCulture));
                        }
                    }

                    // Sucesso
                    listaMaster = new List<PESSOA_EXTERNA>();
                    Session["ListaPessoaExterna"] = null;
                    Session["MensPessoaExterna"] = 0;
                    return RedirectToAction("MontarTelaPessoaExterna");
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
        public ActionResult ExcluirPessoaExterna(Int32 id)
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
                    Session["MensPessoaExterna"] = 2;
                    return RedirectToAction("MontarTelaPessoaExterna");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PESSOA_EXTERNA item = baseApp.GetItemById(id);
            objetoAntes = item;
            item.PEEX_IN_ATIVO = 0;
            Int32 volta = baseApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensPessoaExterna"] = 4;
                return RedirectToAction("MontarTelaPessoaExterna");
            }
            listaMaster = new List<PESSOA_EXTERNA>();
            Session["ListaPessoaExterna"] = null;
            return RedirectToAction("MontarTelaPessoaExterna");
        }

        [HttpGet]
        public ActionResult ReativarPessoaExterna(Int32 id)
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
                    Session["MensPessoaExterna"] = 2;
                    return RedirectToAction("MontarTelaPessoaExterna");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            PESSOA_EXTERNA item = baseApp.GetItemById(id);
            item.PEEX_IN_ATIVO = 1;
            objetoAntes = item;
            Int32 volta = baseApp.ValidateReativar(item, usuario);
            listaMaster = new List<PESSOA_EXTERNA>();
            Session["ListaPessoaExterna"] = null;
            return RedirectToAction("MontarTelaPessoaExterna");
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
            Session["FileQueuePessoaExterna"] = queue;
        }

        [HttpPost]
        public ActionResult UploadFileQueuePessoaExterna(FileQueue file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPessoaExterna"] = 10;
                return RedirectToAction("VoltarAnexoPessoaExterna");
            }

            PESSOA_EXTERNA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = file.Name;
            if (fileName.Length > 250)
            {
                Session["MensPessoaExterna"] = 11;
                return RedirectToAction("VoltarAnexoPessoaExterna");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/PessoaExterna/" + item.PEEX_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            System.IO.File.WriteAllBytes(path, file.Contents);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PESSOA_EXTERNA_ANEXO foto = new PESSOA_EXTERNA_ANEXO();
            foto.PEAX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PEAX_DT_ANEXO = DateTime.Today;
            foto.PEAX_IN_ATIVO = 1;
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
            foto.PEAX_IN_TIPO = tipo;
            foto.PEAX_NM_TITULO = fileName;
            foto.PEEX_CD_ID = item.PEEX_CD_ID;

            item.PESSOA_EXTERNA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoPessoaExterna");
        }

       [HttpPost]
        public ActionResult UploadFilePessoaExterna(HttpPostedFileBase file)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idNot = (Int32)Session["IdUsuario"];
            Int32 idAss = (Int32)Session["IdAssinante"];

            if (file == null)
            {
                Session["MensPessoaExterna"] = 10;
                return RedirectToAction("VoltarAnexoPessoaExterna");
            }

            PESSOA_EXTERNA item = baseApp.GetById(idNot);
            USUARIO usu = (USUARIO)Session["UserCredentials"];
            var fileName = Path.GetFileName(file.FileName);
            if (fileName.Length > 250)
            {
                Session["MensPessoaExterna"] = 11;
                return RedirectToAction("VoltarAnexoPessoaExterna");
            }
            String caminho = "/Imagens/" + idAss.ToString() + "/PessoaExterna/" + item.PEEX_CD_ID.ToString() + "/Anexos/";
            String path = Path.Combine(Server.MapPath(caminho), fileName);
            System.IO.Directory.CreateDirectory(Server.MapPath(caminho));
            file.SaveAs(path);

            //Recupera tipo de arquivo
            extensao = Path.GetExtension(fileName);
            String a = extensao;

            // Gravar registro
            PESSOA_EXTERNA_ANEXO foto = new PESSOA_EXTERNA_ANEXO();
            foto.PEAX_AQ_ARQUIVO = "~" + caminho + fileName;
            foto.PEAX_DT_ANEXO = DateTime.Today;
            foto.PEAX_IN_ATIVO = 1;
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
            foto.PEAX_IN_TIPO = tipo;
            foto.PEAX_NM_TITULO = fileName;
            foto.PEEX_CD_ID = item.PEEX_CD_ID;

            item.PESSOA_EXTERNA_ANEXO.Add(foto);
            objetoAntes = item;
            Int32 volta = baseApp.ValidateEdit(item, item);
            return RedirectToAction("VoltarAnexoPessoaExterna");
        }

        public ActionResult GerarRelatorioLista()
        {            
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "PessoaExternaLista" + "_" + data + ".pdf";
            List<PESSOA_EXTERNA> lista = (List<PESSOA_EXTERNA>)Session["ListaPessoaExterna"];
            PESSOA_EXTERNA filtro = (PESSOA_EXTERNA)Session["FiltroPessoaExterna"];
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

            cell = new PdfPCell(new Paragraph("Pessoa Externa - Listagem", meuFont2))
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
            table = new PdfPTable(new float[] { 120f, 100f, 60f, 50f, 50f});
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Pessoas Externas selecionadas pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cargo", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("CPF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Celular", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (PESSOA_EXTERNA item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.PEEX_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PEES_EM_EMAIL, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.CARGO_USUARIO.CARG_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PEEX_NR_CPF, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.PEEX_NR_CELULAR, meuFont))
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
                if (filtro.PEEX_NM_NOME != null)
                {
                    parametros += "Nome: " + filtro.PEEX_NM_NOME;
                    ja = 1;
                }
                if (filtro.PEES_EM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.PEES_EM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.PEES_EM_EMAIL;
                    }
                }
                if (filtro.PEEX_NR_CPF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "CPF: " + filtro.PEEX_NR_CPF;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.PEEX_NR_CPF;
                    }
                }
                if (filtro.CARG_CD_ID > 0)
                {
                    if (ja == 0)
                    {
                        parametros += "Cargo: " + filtro.CARGO_USUARIO.CARG_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e cargo: " + filtro.CARGO_USUARIO.CARG_NM_NOME;
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
            return RedirectToAction("MontarTelaPessoaExterna");
        }
    }
}
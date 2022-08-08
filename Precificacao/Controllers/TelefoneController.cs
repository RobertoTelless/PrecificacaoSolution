﻿using System;
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
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Canducci.Zip;
using CrossCutting;

namespace ERP_Condominios_Solution.Controllers
{
    public class TelefoneController : Controller
    {
        private readonly ITelefoneAppService fornApp;
        private readonly ILogAppService logApp;
        private readonly ICategoriaTelefoneAppService cfApp;
        private readonly IConfiguracaoAppService confApp;

        private String msg;
        private Exception exception;
        TELEFONE objetoForn = new TELEFONE();
        TELEFONE objetoFornAntes = new TELEFONE();
        List<TELEFONE> listaMasterForn = new List<TELEFONE>();
        String extensao;

        public TelefoneController(ITelefoneAppService fornApps, ILogAppService logApps, ICategoriaTelefoneAppService cfApps, IConfiguracaoAppService confApps)
        {
            fornApp = fornApps;
            logApp = logApps;
            cfApp = cfApps;
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

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaUnidade"] == 1)
            {
                return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 2)
            {
                return RedirectToAction("CarregarPortaria", "BaseAdmin");
            }
            if ((Int32)Session["VoltaUnidade"] == 3)
            {
                return RedirectToAction("CarregarSindico", "BaseAdmin");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult EnviarSmsTelefone(Int32 id, String mensagem)
        {
            try
            {
                TELEFONE forn = fornApp.GetById(id);
                Int32 idAss = (Int32)Session["IdAssinante"];

                // Verifica existencia prévia
                if (forn == null)
                {
                    Session["MensSMSForn"] = 1;
                    return RedirectToAction("MontarTelaTelefone");
                }

                // Criticas
                if (forn.TELE_NR_CELULAR == null)
                {
                    Session["MensSMSForn"] = 2;
                    return RedirectToAction("MontarTelaTelefone");
                }

                // Monta token
                CONFIGURACAO conf = confApp.GetItemById(idAss);
                String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                String token = Convert.ToBase64String(textBytes);
                String auth = "Basic " + token;

                // Monta routing
                String routing = "1";

                // Monta texto
                String texto = mensagem;
                String resposta = String.Empty;

                // inicia processo
                String listaDest = "55" + Regex.Replace(forn.TELE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERP Condominios\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }

                // Saída
                    Session["MensSMSForn"] = 200;
                return RedirectToAction("MontarTelaTelefone");
            }
            catch (Exception ex)
            {
                Session["MensSMSForn"] = 3;
                Session["MensSMSFornErro"] = ex.Message;
                return RedirectToAction("MontarTelaTelefone");
            }
        }

        [HttpGet]
        public ActionResult MontarTelaTelefone()
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaTelefone"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTelefone"] = listaMasterForn;
            }
            ViewBag.Listas = (List<TELEFONE>)Session["ListaTelefone"];
            ViewBag.Title = "Telefones";
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirTel"] = 0;

            // Indicadores
            ViewBag.Telefones = ((List<TELEFONE>)Session["ListaTelefone"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            if (Session["MensTelefone"] != null)
            {
                if ((Int32)Session["MensTelefone"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTelefone"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0028", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new TELEFONE();
            objetoForn.TELE_IN_ATIVO = 1;
            Session["MensTelefone"] = 0;
            Session["VoltaTelefone"] = 1;
            return View(objetoForn);
        }

        public ActionResult RetirarFiltroTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Session["ListaTelefone"] = null;
            Session["FiltroTelefone"] = null;
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        public ActionResult MostrarTudoTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterForn = fornApp.GetAllItensAdm(idAss);
            Session["FiltroTelefone"] = null;
            Session["ListaTelefone"] = listaMasterForn;
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpPost]
        public ActionResult FiltrarTelefone(TELEFONE item)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            try
            {
                // Executa a operação
                Int32 idAss = (Int32)Session["IdAssinante"];
                List<TELEFONE> listaObj = new List<TELEFONE>();
                Session["FiltroTelefone"] = item;
                Int32 volta = fornApp.ExecuteFilter(item.CATE_CD_ID, item.TELE_NM_NOME, item.TELE_NR_TELEFONE, item.TELE_NM_CIDADE, item.UF_CD_ID, item.TELE_NR_CELULAR, item.TELE_NM_EMAIL, idAss, out listaObj);

                // Verifica retorno
                if (volta == 1)
                {
                    Session["MensTelefone"] = 1;
                }

                // Sucesso
                listaMasterForn = listaObj;
                Session["ListaTelefone"] = listaObj;
                if ((Int32)Session["VoltaTelefone"] == 2)
                {
                    return RedirectToAction("VerCardsTelefone");
                }
                return RedirectToAction("MontarTelaTelefone");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return RedirectToAction("MontarTelaTelefone");
            }
        }

        public ActionResult VoltarBaseTelefone()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            if ((Int32)Session["VoltaTelefone"] == 2)
            {
                return RedirectToAction("VerCardsTelefone");
            }
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpGet]
        public ActionResult IncluirTelefone()
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["VoltaProp"] = 4;

            // Prepara view
            TELEFONE item = new TELEFONE();
            TelefoneViewModel vm = Mapper.Map<TELEFONE, TelefoneViewModel>(item);
            vm.TELE_IN_ATIVO = 1;
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            return View(vm);
        }

        [HttpPost]
        public ActionResult IncluirTelefone(TelefoneViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TELEFONE item = Mapper.Map<TelefoneViewModel, TELEFONE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = fornApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTelefone"] = 3;
                        return RedirectToAction("MontarTelaTelefone", "Telefone");
                    }

                    // Sucesso
                    listaMasterForn = new List<TELEFONE>();
                    Session["ListaTelefone"] = null;
                    Session["IncluirTele"] = 1;
                    Session["Telefones"] = fornApp.GetAllItens(idAss);
                    Session["IdVolta"] = item.TELE_CD_ID;
                    if ((Int32)Session["VoltaTelefone"] == 2)
                    {
                        return RedirectToAction("IncluirTelefone");
                    }
                    return RedirectToAction("MontarTelaTelefone");
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
        public ActionResult EditarTelefone(Int32 id)
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
                if (usuario.PERFIL.PERF_SG_SIGLA == "FUN")
                {
                    Session["MensTelefone"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");

            TELEFONE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            ViewBag.Compras = 12;
            Session["Telefone"] = item;
            Session["IdVolta"] = id;
            Session["IdTelefone"] = id;
            Session["VoltaCEP"] = 1;
            TelefoneViewModel vm = Mapper.Map<TELEFONE, TelefoneViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditarTelefone(TelefoneViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TELEFONE item = Mapper.Map<TelefoneViewModel, TELEFONE>(vm);
                    Int32 volta = fornApp.ValidateEdit(item, objetoFornAntes, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterForn = new List<TELEFONE>();
                    Session["ListaTelefone"] = null;
                    Session["IncluirTele"] = 0;
                    if ((Int32)Session["VoltaTelefone"] == 2)
                    {
                        return RedirectToAction("VerCardsTelefone");
                    }
                    return RedirectToAction("MontarTelaTelefone");
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
        public ActionResult VerTelefone(Int32 id)
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara view
            TELEFONE item = fornApp.GetItemById(id);
            objetoFornAntes = item;
            Session["Telefone"] = item;
            Session["IdVolta"] = id;
            Session["IdTelefone"] = id;
            Session["VoltaCEP"] = 1;
            TelefoneViewModel vm = Mapper.Map<TELEFONE, TelefoneViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult ExcluirTelefone(Int32 id)
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
            TELEFONE item = fornApp.GetItemById(id);
            objetoFornAntes = (TELEFONE)Session["Telefone"];
            item.TELE_IN_ATIVO = 0;
            Int32 volta = fornApp.ValidateDelete(item, usuario);
            listaMasterForn = new List<TELEFONE>();
            Session["ListaTelefone"] = null;
            Session["FiltroTelefone"] = null;
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpGet]
        public ActionResult ReativarTelefone(Int32 id)
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
            TELEFONE item = fornApp.GetItemById(id);
            objetoFornAntes = (TELEFONE)Session["Telefone"];
            item.TELE_IN_ATIVO = 1;
            Int32 volta = fornApp.ValidateReativar(item, usuario);
            listaMasterForn = new List<TELEFONE>();
            Session["ListaTelefone"] = null;
            Session["FiltroTelefone"] = null;
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpGet]
        public ActionResult VerCardsTelefone()
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
            }
            else
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaTelefone"] == null)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTelefone"] = listaMasterForn;
            }
            if (((List<TELEFONE>)Session["ListaTelefone"]).Count == 0)
            {
                listaMasterForn = fornApp.GetAllItens(idAss);
                Session["ListaTelefone"] = listaMasterForn;
            }
            ViewBag.Listas = (List<TELEFONE>)Session["ListaTelefone"];
            ViewBag.Title = "Telefones";
            ViewBag.Cats = new SelectList(cfApp.GetAllItens(idAss).OrderBy(x => x.CATE_NM_NOME), "CATE_CD_ID", "CATE_NM_NOME");
            ViewBag.UF = new SelectList(fornApp.GetAllUF(), "UF_CD_ID", "UF_NM_NOME");
            Session["IncluirTel"] = 0;

            // Indicadores
            ViewBag.Telefones = ((List<TELEFONE>)Session["ListaTelefone"]).Count;
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> ativo = new List<SelectListItem>();
            ativo.Add(new SelectListItem() { Text = "Ativo", Value = "1" });
            ativo.Add(new SelectListItem() { Text = "Inativo", Value = "0" });
            ViewBag.Ativos = new SelectList(ativo, "Value", "Text");

            if (Session["MensTelefone"] != null)
            {
                // Mensagem
                if ((Int32)Session["MensTelefone"] == 2)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTelefone"] == 3)
                {
                    ModelState.AddModelError("", ERP_Condominios_Resource.ResourceManager.GetString("M0028", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            objetoForn = new TELEFONE();
            objetoForn.TELE_IN_ATIVO = 1;
            Session["MensTelefone"] = 0;
            Session["VoltaTelefone"] = 2;
            return View(objetoForn);
        }

        [HttpPost]
        public JsonResult PesquisaCEP_Javascript(String cep, int tipoEnd)
        {
            // Chama servico ECT
            ZipCodeLoad zipLoad = new ZipCodeLoad();
            ZipCodeInfo end = new ZipCodeInfo();
            ZipCode zipCode = null;
            cep = CrossCutting.ValidarNumerosDocumentos.RemoveNaoNumericos(cep);
            if (ZipCode.TryParse(cep, out zipCode))
            {
                end = zipLoad.Find(zipCode);
            }

            // Atualiza
            var hash = new Hashtable();

            if (tipoEnd == 1)
            {
                hash.Add("TELE_NM_ENDERECO", end.Address + "/" + end.Complement);
                hash.Add("TELE_NM_BAIRRO", end.District);
                hash.Add("TELE_NM_CIDADE", end.City);
                hash.Add("UF_CD_ID", fornApp.GetUFbySigla(end.Uf).UF_CD_ID);
                hash.Add("TELE_NR_CEP", cep);
            }

            // Retorna
            Session["VoltaCEP"] = 2;
            return Json(hash);
        }

        public ActionResult GerarRelatorioLista(Int32? tipo)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            // Prepara geração
            String data = DateTime.Today.Date.ToShortDateString();
            data = data.Substring(0, 2) + data.Substring(3, 2) + data.Substring(6, 4);
            String nomeRel = "TelefoneLista" + "_" + data + ".pdf";
            List<TELEFONE> lista = new List<TELEFONE>();
            String titulo = String.Empty;
            titulo = "Telefones - Listagem";
            lista = (List<TELEFONE>)Session["ListaTelefone"];

            TELEFONE filtro = (TELEFONE)Session["FiltroTelefone"];
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
            Image image = Image.GetInstance(Server.MapPath("~/Images/ecm_favicon.png"));
            image.ScaleAbsolute(50, 50);
            cell.AddElement(image);
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph(titulo, meuFont2))
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
            table = new PdfPTable(new float[] { 70f, 150f, 60f, 60f, 150f, 50f, 50f, 20f });
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 1f;
            table.SpacingAfter = 1f;

            cell = new PdfPCell(new Paragraph("Telefones selecionados pelos parametros de filtro abaixo", meuFont1))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.Colspan = 8;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Paragraph("Categoria", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Nome", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Telefone", meuFont))
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
            cell = new PdfPCell(new Paragraph("E-Mail", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("WhatsApp", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("Cidade", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
            cell = new PdfPCell(new Paragraph("UF", meuFont))
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            foreach (TELEFONE item in lista)
            {
                cell = new PdfPCell(new Paragraph(item.CATEGORIA_TELEFONE.CATE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(item.TELE_NM_NOME, meuFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT
                };
                table.AddCell(cell);
                if (item.TELE_NR_TELEFONE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TELE_NR_TELEFONE, meuFont))
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
                if (item.TELE_NR_CELULAR != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TELE_NR_CELULAR, meuFont))
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
                if (item.TELE_NM_EMAIL != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TELE_NM_EMAIL, meuFont))
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
                if (item.TELE_NR_WHATSAPP != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TELE_NR_WHATSAPP, meuFont))
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
                if (item.TELE_NM_CIDADE != null)
                {
                    cell = new PdfPCell(new Paragraph(item.TELE_NM_CIDADE, meuFont))
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
                if (item.UF != null)
                {
                    cell = new PdfPCell(new Paragraph(item.UF.UF_SG_SIGLA, meuFont))
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
            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLUE, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

            // Rodapé
            Chunk chunk1 = new Chunk("Parâmetros de filtro: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            pdfDoc.Add(chunk1);

            String parametros = String.Empty;
            Int32 ja = 0;
            if (filtro != null)
            {
                if (filtro.CATE_CD_ID > 0)
                {
                    parametros += "Categoria: " + filtro.CATE_CD_ID;
                    ja = 1;
                }
                if (filtro.TELE_NM_NOME != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Nome: " + filtro.TELE_NM_NOME;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Nome: " + filtro.TELE_NM_NOME;
                    }
                }
                if (filtro.TELE_NR_TELEFONE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Telefone: " + filtro.TELE_NR_TELEFONE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e CPF: " + filtro.TELE_NR_TELEFONE;
                    }
                }
                if (filtro.TELE_NM_EMAIL != null)
                {
                    if (ja == 0)
                    {
                        parametros += "E-Mail: " + filtro.TELE_NM_EMAIL;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e E-Mail: " + filtro.TELE_NM_EMAIL;
                    }
                }
                if (filtro.TELE_NM_CIDADE != null)
                {
                    if (ja == 0)
                    {
                        parametros += "Cidade: " + filtro.TELE_NM_CIDADE;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e Cidade: " + filtro.TELE_NM_CIDADE;
                    }
                }
                if (filtro.UF != null)
                {
                    if (ja == 0)
                    {
                        parametros += "UF: " + filtro.UF.UF_SG_SIGLA;
                        ja = 1;
                    }
                    else
                    {
                        parametros += " e UF: " + filtro.UF.UF_SG_SIGLA;
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
            return RedirectToAction("MontarTelaTelefone");
        }

        [HttpGet]
        public ActionResult EnviarEMailTelefone(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            TELEFONE cont = fornApp.GetItemById(id);
            Session["Telefone"] = cont;
            ViewBag.Telefone = cont;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = cont.TELE_NM_NOME;
            mens.ID = id;
            mens.MODELO = cont.TELE_NM_EMAIL;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 1;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarEMailTelefone(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioEMailTelefone(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseTelefone");
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioEMailTelefone(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera usuario
            Int32 idAss = (Int32)Session["IdAssinante"];
            TELEFONE cont = (TELEFONE)Session["Telefone"];

            // Processa e-mail
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Prepara cabeçalho
            String cab = "Prezado Sr(a). <b>" + cont.TELE_NM_NOME + "</b>";

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = "<b>" + assi.ASSI_NM_NOME + "</b>";

            // Prepara corpo do e-mail e trata link
            String corpo = vm.MENS_TX_TEXTO + "<br /><br />";
            StringBuilder str = new StringBuilder();
            str.AppendLine(corpo);
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
                str.AppendLine("<a href='" + vm.MENS_NM_LINK + "'>Clique aqui para maiores informações</a>");
            }
            String body = str.ToString();
            String emailBody = cab + "<br /><br />" + body + "<br /><br />" + rod;

            // Monta e-mail
            NetworkCredential net = new NetworkCredential(conf.CONF_NM_EMAIL_EMISSOO, conf.CONF_NM_SENHA_EMISSOR);
            Email mensagem = new Email();
            mensagem.ASSUNTO = "Contato";
            mensagem.CORPO = emailBody;
            mensagem.DEFAULT_CREDENTIALS = false;
            mensagem.EMAIL_DESTINO = cont.TELE_NM_EMAIL;
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
        public ActionResult EnviarSMSTelefone(Int32 id)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Login", "ControleAcesso");
            }
            TELEFONE item = fornApp.GetItemById(id);
            Session["Telefone"] = item;
            ViewBag.Telefone = item;
            MensagemViewModel mens = new MensagemViewModel();
            mens.NOME = item.TELE_NM_NOME;
            mens.ID = id;
            mens.MODELO = item.TELE_NR_CELULAR;
            mens.MENS_DT_CRIACAO = DateTime.Today.Date;
            mens.MENS_IN_TIPO = 2;
            return View(mens);
        }

        [HttpPost]
        public ActionResult EnviarSMSTelefone(MensagemViewModel vm)
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
                    Int32 volta = ProcessaEnvioSMSTelefone(vm, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {

                    }

                    // Sucesso
                    return RedirectToAction("VoltarBaseTelefone");
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

        [ValidateInput(false)]
        public Int32 ProcessaEnvioSMSTelefone(MensagemViewModel vm, USUARIO usuario)
        {
            // Recupera contatos
            Int32 idAss = (Int32)Session["IdAssinante"];
            TELEFONE cont = (TELEFONE)Session["Telefone"];

            // Prepara cabeçalho
            String cab = "Prezado Sr(a)." + cont.TELE_NM_NOME;

            // Prepara rodape
            ASSINANTE assi = (ASSINANTE)Session["Assinante"];
            String rod = assi.ASSI_NM_NOME;

            // Processa SMS
            CONFIGURACAO conf = confApp.GetItemById(usuario.ASSI_CD_ID);

            // Monta token
            String text = conf.CONF_SG_LOGIN_SMS + ":" + conf.CONF_SG_SENHA_SMS;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            String token = Convert.ToBase64String(textBytes);
            String auth = "Basic " + token;

            // Prepara texto
            String texto = cab + vm.MENS_TX_SMS + rod;

            // Prepara corpo do SMS e trata link
            StringBuilder str = new StringBuilder();
            str.AppendLine(vm.MENS_TX_SMS);
            if (!String.IsNullOrEmpty(vm.LINK))
            {
                if (!vm.LINK.Contains("www."))
                {
                    vm.LINK = "www." + vm.LINK;
                }
                if (!vm.LINK.Contains("http://"))
                {
                    vm.LINK = "http://" + vm.LINK;
                }
                str.AppendLine("<a href='" + vm.LINK + "'>Clique aqui para maiores informações</a>");
                texto += "  " + vm.LINK;
            }
            String body = str.ToString();
            String smsBody = body;
            String erro = null;

            // inicia processo
            String resposta = String.Empty;

            // Monta destinatarios
            try
            {
                String listaDest = "55" + Regex.Replace(cont.TELE_NR_CELULAR, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled).ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-v2.smsfire.com.br/sms/send/bulk");
                httpWebRequest.Headers["Authorization"] = auth;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                String customId = Cryptography.GenerateRandomPassword(8);
                String data = String.Empty;
                String json = String.Empty;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json = String.Concat("{\"destinations\": [{\"to\": \"", listaDest, "\", \"text\": \"", texto, "\", \"customId\": \"" + customId + "\", \"from\": \"ERPSys\"}]}");
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    resposta = result;
                }
            }
            catch (Exception ex)
            {
                erro = ex.Message;
            }
            return 0;
        }


    }
}